using QNH.Overheid.KernRegister.Business.Service.Users;
using System.Web.Mvc;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserManager _userManager;

        public UsersController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        //
        // GET: /Export/
        public ActionResult Index()
        {
            return View(_userManager.GetAllUserActions());
        }
    }
}
