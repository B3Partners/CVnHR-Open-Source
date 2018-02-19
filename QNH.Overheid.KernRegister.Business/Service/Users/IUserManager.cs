using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service.Users
{
    public interface IUserManager : IDisposable
    {
        bool IsAllowedAllActions(string username, params ApplicationActions[] actions);
        bool IsAllowedAnyActions(string username, params ApplicationActions[] actions);
        string AddUserToAction(ApplicationActions action, string username);
        string RemoveUserFromAction(ApplicationActions action, string username);
        IDictionary<ApplicationActions, IEnumerable<string>> GetAllUserActions();
        IEnumerable<string> GetAdministrators();
    }
}
