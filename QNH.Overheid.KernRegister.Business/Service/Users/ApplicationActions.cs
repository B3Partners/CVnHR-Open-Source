using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service.Users
{
    public enum ApplicationActions
    {
        [DisplayName("Kvk data bekijken"), Description("Gebruiker mag kvk gegevens zien")]
        CVnHR_ViewKvKData,
        [DisplayName("Kvk data beheren"),Description("Gebruiker mag synchroniseren met KvK")]
        CVnHR_ManageKvKData,
        [DisplayName("Crm/Dms data doorzetten"),Description("Gebruiker mag doorzetten naar Crm/Dms")]
        CVnHR_Crm,
        [DisplayName("Brmo"), Description("Gebruiker mag doorzetten naar BRMO")]
        CVnHR_Brmo,
        [DisplayName("Debiteuren"),Description("Gebruiker mag doorzetten naar Debiteuren")]
        CVnHR_Debiteuren,
        [DisplayName("Crediteuren"), Description("Gebruiker mag doorzetten naar Crediteuren")]
        CVnHR_Crediteuren,
        [DisplayName("Administrator"), Description("Administrator functie (Gebruikersbeheer)")]
        CVnHR_Admin
    }
}
