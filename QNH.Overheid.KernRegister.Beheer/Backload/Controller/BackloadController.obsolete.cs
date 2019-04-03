using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Backload.Controllers
{

    /// <summary>
    /// The integrated controller to handle file requests. 
    /// You can remove this code, if you have a custom controller or a derived controller.
    /// </summary>
    public partial class BackloadController : Controller
    {
        /// <summary>
        /// This handler is for backward compatibility only. If you have a previous project updated to 
        /// Backload release 2 or higher, please also update your calls in JavaScript to the new handler url
        /// /Backload/Filehandler. After that you can remove this handler.
        /// </summary>
        [Obsolete("This handler is form backward compatibility only.")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete | HttpVerbs.Options)]
        public async Task<ActionResult> UploadHandler()
        {
            return await FileHandler();
        }
    }
}
