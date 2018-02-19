using QNH.Overheid.KernRegister.Business.Service.Users;
using System.Web.Mvc;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using System;
using System.Linq;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_Admin)]
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

        [AllowAnonymous]
        public ActionResult AccessDenied(string actions)
        {
            var deniedPermissions = (actions ?? "")
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => Enum.Parse(typeof(ApplicationActions), a)).Cast<ApplicationActions>();
            return View(new AccessDeniedModel
            {
                Administrators = SettingsHelper.InitialUserAdministrators.Concat(_userManager.GetAdministrators()),
                DeniedPermission = deniedPermissions
            });
        }

        [HttpPost]
        public ActionResult AddUser(ApplicationActions action, string username)
        {
            return Json(_userManager.AddUserToAction(action, username));
        }

        public ActionResult RemoveUser(ApplicationActions action, string username)
        {
            return Json(_userManager.RemoveUserFromAction(action, username));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
