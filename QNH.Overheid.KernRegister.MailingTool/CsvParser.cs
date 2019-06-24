using CsvHelper;
using OpenPop.Mime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.MailingTool
{
    class CsvParser
    {
        private MessagePart attachment { get; set; }

        private String date { get; set; }

        public CsvParser(MessagePart attachment, String date) {
            this.attachment = attachment;
            this.date = date;
        }

        public void StripCsv() {

            List<string> kvkColumn = new List<string>();
            using (var reader = File.OpenText(Path.Combine(ConfigurationManager.AppSettings["tempFolder"], "temp_" + attachment.FileName))) {

                var csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ",";
                csv.Configuration.HasHeaderRecord = true;
                csv.Read();
                csv.ReadHeader();
                kvkColumn.Add("kvknummer\n");
                while (csv.Read()) {
                    string column = csv.GetField<string>("DOSSIERNR");
                    kvkColumn.Add(column);
                }

                File.WriteAllLines(Path.Combine(ConfigurationManager.AppSettings["uploadFolder"], "mailFiles\\"+date+"_" + attachment.FileName),kvkColumn);
            }

        }
    }
}
