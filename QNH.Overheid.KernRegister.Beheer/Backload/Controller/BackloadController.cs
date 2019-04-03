using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Helper;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Backload.Controllers
{

    /// <summary>
    /// The integrated controller to handle file requests. 
    /// You can remove this code, if you have a custom controller or handler.
    /// </summary>
    public partial class BackloadController : Controller
    {

        /// <summary>
        /// The Backload file handler. 
        /// To access it in an Javascript ajax request use: <code>var url = "/{Application}/Backload/FileHandler/";</code>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post|HttpVerbs.Put|HttpVerbs.Delete|HttpVerbs.Options)]
        public async Task<ActionResult> FileHandler()
        {
            try
            {

                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();
                handler.Init(HttpContext.Request);


                // Call the execution pipeline and get the result
                IBackloadResult result = await handler.Execute();


                // Helper to create an ActionResult object from the IBackloadResult instance
                return ResultCreator.Create(result);

            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}
