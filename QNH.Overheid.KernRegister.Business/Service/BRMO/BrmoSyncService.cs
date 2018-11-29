using QNH.Overheid.KernRegister.Business.Enums;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace QNH.Overheid.KernRegister.Business.Service.BRMO
{
    public class BrmoSyncService : IBrmoSyncService
    {
        private BrmoReference _brmoReference;

        public BrmoSyncService(BrmoReference reference)
        {
            _brmoReference = reference;
        }

        public AddInschrijvingResultStatus UploadXDocumentToBrmo(XDocument xDocument)
        {
            if(xDocument == null)
                throw new NullReferenceException("xDocument is null...");

            using (var client = new WebClient())
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(xDocument.ToString());
                    var response = client.UploadData(_brmoReference.NhrServiceUrl, bytes);
                    if (response != null)
                    {
                        var responseString = Encoding.UTF8.GetString(response);
                        if(!responseString.StartsWith("Data successfully received and stored"))
                            throw new WebException("Expected empty response from BRMO service but instead received: " + responseString);
                    }
                    return AddInschrijvingResultStatus.BrmoInschrijvingCreated;
                }
                catch (WebException ex)
                {
                    var response = "[unknown]";
                    if(ex?.Response != null)
                        using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                            response = reader.ReadToEnd();
                    
                    throw new WebException(
                        $"Error while uploading xml to {_brmoReference.NhrServiceUrl}. {response}", ex);
                }
            }
        }
    }
}
