using Rotativa;
using Rotativa.Options;
using System.Configuration;
using System.Web.Mvc;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    public class PdfController : Controller
    {
#warning //TODO: Why are errors not logged in ELMAH
#warning //TODO: Can we ensure the logged on user instead of the wkhtml user?

        //
        // GET: /Pdf/
        public ActionResult Index(string url, string fileName)
        {
            url = string.Concat(url, url.Contains("?") ? "&" : "?", "DisplayUsername=", Url.Encode(User.Identity.Name));
            return new UrlAsPdf(url)
                       { 
                           FileName = fileName,
                           PageSize = Size.A4,
                           CustomSwitches = ConfigurationManager.AppSettings["PdfWkHtmlToPdfCustomSwitches"] ?? "",
                       };
        }
    }
}
