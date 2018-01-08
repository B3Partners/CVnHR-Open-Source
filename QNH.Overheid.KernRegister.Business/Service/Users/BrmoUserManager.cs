using Dapper;
using QNH.Overheid.KernRegister.Business.Model.Entities.Brmo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service.Users
{
    // TODO
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

        private IDbConnection _connection;

        public BrmoUserManager(IDbConnection connection)
        {
            _connection = connection;
        }

        public string AddUserToAction(ApplicationActions action, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username cannot be null or whitespace.");
            }

            // Create user if not exists.
            var user = _connection.Query<Gebruiker>(@"select GEBRUIKERSNAAM as GebruikersNaam from GEBRUIKER_;");
            if (user == null) {
                _connection.Execute(@"INSERT INTO GEBRUIKER_(GEBRUIKERSNAAM, WACHTWOORD) 
                                        VALUES(@GebruikersNaam, @Wachtwoord);",
                                        new { GebruikersNaam = username, Wachtwoord = "[cvnhr windows authentication]" });
            }

            // Create user if not exists.
            var group = _connection.Query<Groep>(@"select NAAM as Naam from GROEP_;");
            if (group == null)
            {
                _connection.Execute(@"INSERT INTO GROEP_(NAAM, BESCRHRIJVING) 
                                        VALUES(@Naam, @Beschrijving);",
                                        new {
                                            Naam = action.ToString(),
                                            Beschrijving = action.GetDescription()
                                        });
            }

            // Get the user actions
            var userActions = _connection.Query<Gebruiker_Groepen>(@"
                                    SELECT GEBRUIKERSNAAM ad GebruikersNaam, GROEP_ as Groep 
                                    FROM GEBRUIKER_GROEPEN
                                    WHERE GEBRUIKERSNAAM = @user, @GROEP_ = @action;", new { user, action });

            // Insert the action for this user
            if (!userActions.Any())
            {
                _connection.Execute(@"INSERT INTO GEBRUIKER_GROEPEN (GEBRUIKERSNAAM, GROEP_)
                                        VALUES(@user, @action)", new { user, action });
            }
            else
                return "Action for user already exists!";

            return "success";
        }

        public IDictionary<ApplicationActions, IEnumerable<string>> GetAllUserActions()
        {
            // Get the user actions
            var userActions = _connection.Query<Gebruiker_Groepen>(@"
                                    SELECT GEBRUIKERSNAAM ad GebruikersNaam, GROEP_ as Groep 
                                    FROM GEBRUIKER_GROEPEN
                                    WHERE GROEP_ LIKE 'CVnHR_';");

            return userActions.Select(gg => new
                {
                    Group = (ApplicationActions)Enum.Parse(typeof(ApplicationActions), gg.Groep),
                    gg.GebruikersNaam
                })
                .GroupBy(gg=> gg.Group)
                .ToDictionary(gg => gg.Key, gg=> gg.Select(group => group.GebruikersNaam));
        }

        public bool IsAllowedAllActions(string username, params ApplicationActions[] actions)
        {
            throw new NotImplementedException();
        }

        public bool IsAllowedAnyActions(string username, params ApplicationActions[] actions)
        {
            throw new NotImplementedException();
        }

        public string RemoveUserFromAction(ApplicationActions action, string username)
        {
            throw new NotImplementedException();
        }
    }
}
