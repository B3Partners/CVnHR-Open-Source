using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service.Users
{
    public enum ApplicationActions
    {
        ViewKvKData,
        ManageKvKData,
        Crm,
        Brmo,
        Debiteuren,
        Crediteuren,
        Admin
    }

    public static class ApplicationActionDescriptions
    {
        private static Dictionary<ApplicationActions, string> Descriptions => new Dictionary<ApplicationActions, string>
        {
            { ApplicationActions.Admin, "Administrator functie (Gebruikersbeheer)" },
            { ApplicationActions.Brmo, "Gebruiker mag doorzetten naar BRMO" },
            { ApplicationActions.Crediteuren, "Gebruiker mag doorzetten naar Crediteuren" },
            { ApplicationActions.Debiteuren, "Gebruiker mag doorzetten naar Debiteuren" },
            { ApplicationActions.Crm, "Gebruiker mag doorzetten naar Crm" },
            { ApplicationActions.ManageKvKData, "Gebruiker mag synchroniseren met KvK " },
            { ApplicationActions.ViewKvKData, "Gebruiker mag kvk gegevens zien" }
        };

        public static string Get(ApplicationActions action) => Descriptions[action];
    }
}
