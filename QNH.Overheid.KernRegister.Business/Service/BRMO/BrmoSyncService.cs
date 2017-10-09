using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Xml.Linq;
using QNH.Overheid.KernRegister.Business.Enums;

namespace QNH.Overheid.KernRegister.Business.Service.BRMO
{
    public class BrmoReference
    {
        public string BaseUrl { get; set; } = "http://localhost:8080/brmo-service";

        public string BrmoUrl => BaseUrl.Trim('/');
        public string UserName { get; set; } = "brmo";
        public string Password { get; set; } = "brmo";

        public string NhrServiceUrl => string.Join("/", BrmoUrl, "post", "nhr");

        public string SecurityCheckUrl => string.Join("/", BrmoUrl, "j_security_check");

        public string NhrBerichtFilterUrl
            =>
                string.Join("/", BrmoUrl,
                    "Berichten.action?getGridData=&_dc=1469469984815&page=1&start=0&limit=40&filter=%5B%7B%22operator%22%3A%22in%22%2C%22value%22%3A%5B%22STAGING_OK%22%5D%2C%22property%22%3A%22status%22%7D%2C%7B%22operator%22%3A%22like%22%2C%22value%22%3A%22nhr%22%2C%22property%22%3A%22soort%22%7D%5D")
            ;

        public string TransformUrl =>
            string.Join("/", BrmoUrl, "Transform.action?transformSelected=");
    }

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

        public void Transform(string kvkNummer)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var reply = client.DownloadString(_brmoReference.BrmoUrl);
                    var sessionCookie = client.ResponseHeaders["Set-Cookie"];
                    client.Headers["Cookie"] = sessionCookie;

                    // Login
                    var values = new NameValueCollection
                    {
                        {"j_username",_brmoReference.UserName},
                        {"j_password",_brmoReference.Password},
                        { "submit", "Login"}
                    };
                    client.UploadValues(_brmoReference.SecurityCheckUrl, values);
                    var cookie = client.ResponseHeaders["Set-Cookie"];

                    // Set Cookie
                    client.Headers["Cookie"] = cookie;

                    // Haal alle staging_ok nhr berichten op: Download alle ids
                    var jsonIds = client.DownloadString(_brmoReference.NhrBerichtFilterUrl);

                    var items = Json.Decode(jsonIds).items;
                    var ids = new List<string>();
                    foreach (var item in items)
                        ids.Add(item.id.ToString());

                    var transformUrl = _brmoReference.TransformUrl + string.Concat(ids.Select(id => "&selectedIds=" + id));
                    client.DownloadString(transformUrl);
                }
            }
            catch (WebException ex)
            {
                var response = "[unknown]";
                if (ex?.Response != null)
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                        response = reader.ReadToEnd();

                throw new WebException(
                    $"Error while action to {_brmoReference.SecurityCheckUrl}. {response}", ex);
            }

        }
    }
}
