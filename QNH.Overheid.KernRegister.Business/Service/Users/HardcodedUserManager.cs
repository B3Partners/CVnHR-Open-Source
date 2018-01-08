using System;
using System.Collections.Generic;
using System.Linq;

namespace QNH.Overheid.KernRegister.Business.Service.Users
{
    /// <summary>
    /// Hardcoded in memory user manager. Changes are discarded on application restart/app pool recycle
    /// Used for testing for now
    /// </summary>
    public class HardCodedUserManager : IUserManager
    {
        private Dictionary<string, IList<ApplicationActions>> _userActions = new Dictionary<string, IList<ApplicationActions>>()
        {
            //{ "corne", new[] { ApplicationActions.CVnHR_Admin, ApplicationActions.CVnHR_ManageKvKData }.ToList() },
        };

        private readonly string _userNameToUseWhenEmpty;

        public HardCodedUserManager(string userNameToUseWhenEmpty = null)
        {
            _userNameToUseWhenEmpty = userNameToUseWhenEmpty;
        }

        public string AddUserToAction(ApplicationActions action, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }
            if (_userActions.ContainsKey(username))
            {
                var actions = _userActions[username];
                if (!actions.Contains(action))
                    actions.Add(action);
                else
                    return "Action for user already exists!";
            }
            else
                _userActions.Add(username, new[] { action }.ToList());

            return "success";
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

        public IDictionary<ApplicationActions, IEnumerable<string>> GetAllUserActions()
        {
            return Enum.GetValues(typeof(ApplicationActions))
                .OfType<ApplicationActions>()
                .ToDictionary(
                    key => key,
                    value => _userActions.Where(ua => ua.Value.Contains(value)).Select(ua => ua.Key)
                );
        }

        public string RemoveUserFromAction(ApplicationActions action, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }
            if (_userActions.ContainsKey(username))
            {
                var actions = _userActions[username];
                if (actions.Contains(action))
                    actions.Remove(action);
                else
                    return "Action for user does not exist!";
            }
            else
                return "Action for user does not exist!";

            return "success";
        }
    }
}
