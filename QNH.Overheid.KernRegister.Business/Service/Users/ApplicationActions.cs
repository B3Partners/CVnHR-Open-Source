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
        [DisplayName("Kvk data bekijken"), 
            Description("Gebruiker mag KvK gegevens zien en zoeken.")]
        CVnHR_ViewKvKData,
        [DisplayName("CVnHR data bekijken"),
            Description("Gebruiker CVnHR gegevens zien en zoeken.")]
        CVnHR_ViewCVnHRData,
        [DisplayName("Kvk data toevoegen/verwijderen"),
            Description("Gebruiker mag KvK gegevens invoeren in de CVnHR.")]
        CVnHR_ManageKvKData,
        [DisplayName("Kvk data synchroniseren"),
            Description("Gebruiker mag KvK-CVnHR synchronisatie batches starten en stoppen.")]
        CVnHR_SyncKvKData,
        [DisplayName("Crm/Dms data doorzetten"),
            Description("Gebruiker mag kvk gegevens doorzetten naar Crm/Dms")]
        CVnHR_ManageCrm,
        [DisplayName("Crm/Dms data synchroniseren"),
            Description("Gebruiker mag CVnHR-Crm/Dms synchronisatie batches starten en stoppen.")]
        CVnHR_SyncCrm,
        [DisplayName("Brmo"), 
            Description(@"Gebruiker mag kvk gegevens doorzetten naar de BRMO en krijgt een link naar de BRMO in het menu. 
Heeft daarnaast nog wel BRMO inloggegevens nodig.")]
        CVnHR_Brmo,
        [DisplayName("Brmo Synchroniseren"),
            Description(@"Gebruiker mag kvk gegevens synchroniseren met de BRMO.")]
        CVnHR_SyncBrmo,
        [DisplayName("Debiteuren"),
            Description("Gebruiker mag KvK en CVnHR gegevens doorzetten naar Debiteuren in het Financiele systeem")]
        CVnHR_Debiteuren,
        [DisplayName("Crediteuren"), 
            Description("Gebruiker mag KvK en CVnHR gegevens doorzetten naar Crediteuren in het Financiele systeem")]
        CVnHR_Crediteuren,
        [DisplayName("Administrator"), 
            Description("Administrator functie (Gebruikersbeheer)")]
        CVnHR_Admin
    }
}
