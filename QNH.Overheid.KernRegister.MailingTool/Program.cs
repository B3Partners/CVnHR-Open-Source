using NLog;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace QNH.Overheid.KernRegister.MailingTool
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {

            try
            {
                Pop3Client pop = new Pop3Client();
                try
                {
                    pop.Connect(ConfigurationManager.AppSettings["host"], Int32.Parse(ConfigurationManager.AppSettings["port"]), true);
                    pop.Authenticate(ConfigurationManager.AppSettings["userName"], ConfigurationManager.AppSettings["password"]);
                }
                catch (Exception e) {
                    _logger.Debug("Kan geen verbinding met de mail server maken");
                    _logger.Error(e);
                    throw e;
                }
                int count = pop.GetMessageCount();
                if (count == 0) {
                    _logger.Debug("Mailbox is leeg");
                    return;
                }

                Message newMessage = null;
                for (int i = 0; i < count; i++) {
                    Message message = pop.GetMessage(i+1);
                    if (newMessage == null)
                    {
                        newMessage = message;
                    }
                    else {
                        if (message.Headers.DateSent > newMessage.Headers.DateSent) {
                            newMessage = message;
                        }
                    }
                }

                DateTime newDate = newMessage.Headers.DateSent.Date.Date;
                String dateSent = newDate.ToString().Split(' ', '\t')[0];

                DirectoryInfo uploadDir = new DirectoryInfo(Path.Combine(ConfigurationManager.AppSettings["uploadFolder"], "mailFiles"));
                FileInfo[] oldFiles = uploadDir.GetFiles("*.csv");

                Boolean isOld = false;

                foreach (FileInfo oldFile in oldFiles) {
                    String oldDateString = oldFile.Name.Split('_')[0];
                    DateTime oldDate = DateTime.Parse(oldDateString);
                    if (newDate <= oldDate) {
                        isOld = true;

                    }
                }
                if (isOld)
                {
                    _logger.Debug("Bijlage is niet nieuwer dan de vorige mutaties. Process wordt afegbroken");
                    return;

                }
                List <MessagePart> attachments = newMessage.FindAllAttachments();
                if (attachments.Any())
                {

                    foreach (var attachment in attachments) {
                        //check if attachment is zip file or CSV file
                        String extension = attachment.FileName.Substring(attachment.FileName.Length - 3);
                        if (extension.Equals("zip"))
                        {
                            _logger.Info("Zip bestand gevonden :"+ attachment.FileName);

                        }
                        else if (extension.Equals("csv"))
                        {
                            _logger.Info("Csv bestand gevonden :"+attachment.FileName);

                            attachment.Save(new FileInfo(Path.Combine(ConfigurationManager.AppSettings["tempFolder"], "temp_" + attachment.FileName)));

                           

                            CsvParser parser = new CsvParser(attachment, dateSent);
                            parser.StripCsv();

                            File.Delete(Path.Combine(ConfigurationManager.AppSettings["tempFolder"], "temp_" + attachment.FileName));

                            _logger.Debug("Batch proces wordt gestart...");
                            Process brmo = new Process();
                            brmo.StartInfo.FileName = ConfigurationManager.AppSettings["batchLocation"];
                            brmo.StartInfo.Arguments = "BRMO 3.0 Csv " + "mailFiles\\"+dateSent + "_"+ attachment.FileName;
                            //brmo.StartInfo.UseShellExecute = false;

                            brmo.Start();
                            _logger.Debug("Batch proces is gestart met bestand: " + dateSent + "_" + attachment.FileName);
                            brmo.WaitForExit();

                            if (brmo.ExitCode != 0)
                            {
                                _logger.Error("Batchprocess is niet gelukt. Kijk in de log van het batchprocess voor meer informatie.");
                            }
                            else {
                                _logger.Debug("Batch process is succesvol verlopen");
                            }
                            
                        }
                        else {
                            _logger.Debug("Bijlage gevonden met een extensie dat niet wordt ondersteunt. Bestandsnaam: " + attachment.FileName + " mail verzonden op :" + newMessage.Headers.Date);   
                        }

                    }

                }
                else {
                    _logger.Debug("E-mail bevat geen bijlage met datesent "+ newMessage.Headers.Date);
                    return;
                }
            }
            catch (Exception e) {
                _logger.Debug("Er is iets fout gegaan.");
                _logger.Error(e);
            }
        }
    }
}



