using Dapper;
using NHibernate;
using QNH.Overheid.KernRegister.Business.Model.Entities.Brmo;
using QNH.Overheid.KernRegister.Organization.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service.Users
{
    public class BrmoUserManager : IUserManager
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

        private readonly IDbConnection _connection;
        private readonly string _schemaName;
        private readonly string _userNameToUseWhenEmpty;
        private readonly string _parameterChar;

        public BrmoUserManager(
            IDbConnection connection, 
            string schemaName,
            string userNameToUseWhenEmpty = null,
            string parameterChar = ":")
        {
            _connection = connection;
            _schemaName = schemaName;
            _userNameToUseWhenEmpty = userNameToUseWhenEmpty;
            _parameterChar = parameterChar;
        }

        public string AddUserToAction(ApplicationActions action, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }

            // Create user if not exists.
            var usercount = _connection.ExecuteScalar<int>($@"SELECT COUNT(*)
                            FROM {_schemaName}.GEBRUIKER_
                            WHERE GEBRUIKERSNAAM = {_parameterChar}username", new { username });
            if (usercount == 0)
            {
                _connection.Execute($@"INSERT INTO {_schemaName}.GEBRUIKER_ (GEBRUIKERSNAAM, WACHTWOORD) 
                                        VALUES({_parameterChar}GebruikersNaam, {_parameterChar}Wachtwoord)",
                                        new { GebruikersNaam = username, Wachtwoord = "[cvnhr windows authentication]" });
            }

            // Create group if not exists.
            var groupCount = _connection.ExecuteScalar<int>($@"SELECT COUNT(*) 
                            FROM {_schemaName}.GROEP_
                            WHERE NAAM = {_parameterChar}action", new { action = action.ToString() });
            if (groupCount == 0)
            {
                _connection.Execute($@"INSERT INTO {_schemaName}.GROEP_ (NAAM, BESCHRIJVING) 
                                        VALUES({_parameterChar}Naam, {_parameterChar}Beschrijving)",
                                        new
                                        {
                                            Naam = action.ToString(),
                                            Beschrijving = action.GetDescription()
                                        });
            }

            // Get the user actions
            var userActionCount = _connection.ExecuteScalar<int>($@"
                                    SELECT count(*)
                                    FROM {_schemaName}.GEBRUIKER_GROEPEN
                                    WHERE GEBRUIKERSNAAM = {_parameterChar}username AND GROEP_ = {_parameterChar}action",
                                new { username, action = action.ToString() });

            // Insert the action for this user
            if (userActionCount == 0)
            {
                _connection.Execute($@"INSERT INTO {_schemaName}.GEBRUIKER_GROEPEN (GEBRUIKERSNAAM, GROEP_)
                                        VALUES({_parameterChar}username, {_parameterChar}action)",
                                        new { username, action = action.ToString() });
            }
            else
                return "Action for user already exists!";

            return "success";
        }

        public IDictionary<ApplicationActions, IEnumerable<string>> GetAllUserActions()
        {
            // Get the user actions
            var userActions = _connection.Query<Gebruiker_Groepen>($@"
                                SELECT GEBRUIKERSNAAM as ""GebruikersNaam"", GROEP_ as ""Groep""
                                    FROM {_schemaName}.GEBRUIKER_GROEPEN
                                    WHERE GROEP_ LIKE 'CVnHR_%'");

            return Enum.GetValues(typeof(ApplicationActions))
                .OfType<ApplicationActions>()
                .ToDictionary(
                    key => key,
                    value => userActions.Where(ua => ua.Groep.Contains(value.ToString())).Select(ua => ua.GebruikersNaam)
                );
        }

        private IEnumerable<string> GetUserActions(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                if (_userNameToUseWhenEmpty == null)
                    throw new ArgumentException("username cannot be null or whitespace.");
                else
                    username = _userNameToUseWhenEmpty;
            }

            return _connection.Query<string>(
                $@"SELECT GROEP_ as ""Groep"" 
                        FROM {_schemaName}.GEBRUIKER_GROEPEN 
                        WHERE 
                            (
                                GEBRUIKERSNAAM = {_parameterChar}username
                                OR GEBRUIKERSNAAM = '{Default.CVnHREveryone}'
                            )
                            AND GROEP_ LIKE 'CVnHR_%'",
                new { username });
        }

        public bool IsAllowedAllActions(string username, params ApplicationActions[] actions)
        {
            var userActions = GetUserActions(username);
            return actions.All(a => userActions.Contains(a.ToString()));
        }

        public bool IsAllowedAnyActions(string username, params ApplicationActions[] actions)
        {
            var userActions = GetUserActions(username);
            return actions.Any(a => userActions.Contains(a.ToString()));
        }

        public string RemoveUserFromAction(ApplicationActions action, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }
            var userActions = GetUserActions(username);
            if (userActions.Contains(action.ToString()))
            {
                _connection.Execute($@"DELETE FROM {_schemaName}.GEBRUIKER_GROEPEN 
                                    WHERE GEBRUIKERSNAAM = {_parameterChar}username AND GROEP_ = {_parameterChar}action",
                                    new { username, action = action.ToString() });
            }
            else
                return "Action for user does not exist!";

            return "success";
        }
    }
}
