using QNH.Overheid.KernRegister.Business.Service.Users;
using System.Web.Mvc;
using QNH.Overheid.KernRegister.Beheer.Utilities;

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
            var userActions = _userManager.GetAllUserActions();
            if (userActions.Count < 1) // Initial startup
            {
                _userManager.AddUserToAction(ApplicationActions.CVnHR_Admin, SettingsHelper.UsernameToUseWhenEmpty);
                userActions = _userManager.GetAllUserActions();
            }

            return View(userActions);
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
    }
}
