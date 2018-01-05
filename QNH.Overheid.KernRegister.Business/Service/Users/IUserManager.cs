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

        string AddUserToAction(ApplicationActions action, string username);
        string RemoveUserFromAction(ApplicationActions action, string username);

        IList<ApplicationActions> GetUserActions(string username);

        IList<string> GetUsersForActions(ApplicationActions action);

        IDictionary<ApplicationActions, IEnumerable<string>> GetAllUserActions();
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

        public string AddUserToAction(ApplicationActions action, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }
            if (_userActions.ContainsKey(username)) {
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

    // TODO
    public class BrmoUserManager
    {
        /*
         *  Queries to use
         *  
            UPDATE GEBRUIKER_ 
            SET WACHTWOORD = '[cvnhr windows authentication]'
            WHERE GEBRUIKERSNAAM = 'corne';

            INSERT INTO GEBRUIKER_
            VALUES('corne', '[cvnhr windows authentication]');

            INSERT INTO GROEP_
            VALUES('CVnHR_Admin', 'Admin groep voor CVnHR');

            INSERT INTO GEBRUIKER_GROEPEN
            VALUES ('corne', 'CVnHR_Admin');
        */
    }
}
