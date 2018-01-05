using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service.Users
{
    public interface IUserManager
    {
        bool IsAllowedAllActions(string username, params ApplicationActions[] actions);

        bool IsAllowedAnyActions(string username, params ApplicationActions[] actions);

        void AddUserToAction(ApplicationActions action, string username);

        IList<ApplicationActions> GetUserActions(string username);

        IList<string> GetUsersForActions(ApplicationActions action);
    }

    // Used for testing for now
    public class HardCodedUserManager : IUserManager
    {
        private Dictionary<string, IList<ApplicationActions>> _userActions = new Dictionary<string, IList<ApplicationActions>>()
        {
            { "corne", new[] { ApplicationActions.Admin, ApplicationActions.ManageKvKData }.ToList() },
        };

        private readonly string _userNameToUseWhenEmpty;

        public HardCodedUserManager(string userNameToUseWhenEmpty = null)
        {
            _userNameToUseWhenEmpty = userNameToUseWhenEmpty;
        }

        public void AddUserToAction(ApplicationActions action, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }
            if (_userActions.ContainsKey(username))
                _userActions[username].Add(action);
            else
                _userActions.Add(username, new[] { action }.ToList());
        }

        public IList<ApplicationActions> GetUserActions(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }
            return _userActions[username];
        }

        public IList<string> GetUsersForActions(ApplicationActions action)
        {
            return _userActions.Where(ua => ua.Value.Contains(action)).Select(ua => ua.Key).ToList();
        }

        public bool IsAllowedAllActions(string username, params ApplicationActions[] actions)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                if (_userNameToUseWhenEmpty == null)
                    throw new ArgumentException("username cannot be null or whitespace.");
                else
                    username = _userNameToUseWhenEmpty;
            }
            return _userActions.ContainsKey(username) && actions.All(a => _userActions[username].Contains(a));
        }

        public bool IsAllowedAnyActions(string username, params ApplicationActions[] actions)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                if (_userNameToUseWhenEmpty == null)
                    throw new ArgumentException("username cannot be null or whitespace.");
                else
                    username = _userNameToUseWhenEmpty;
            }
            return _userActions.ContainsKey(username) && actions.Any(a => _userActions[username].Contains(a));
        }
    }
}
