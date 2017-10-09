using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Organization.Resources
{
    //TODO: WHEN GROWING, create other way to manage resources
    /// <summary>
    /// Since resources by default did not work for provinces this was written. Set CurrentOrganization to the organization requested and write your resources in this file.
    /// </summary>
    public class Default
    {
        public static Organization CurrentOrganization { get; set; }

        public static string ApplicationName => new OrganizationResource()
        {
            {Organization.Qnh, "CVnHR"},
            {Organization.Drenthe, "CVnHR"},
            {Organization.Flevoland, "Voorziening Handelsregister"}
        }[CurrentOrganization];

        public static string CompanyName => new OrganizationResource()
        {
            {Organization.Qnh, "[CompanyName]"},
            {Organization.Drenthe, "Provincie Drenthe"},
            {Organization.Flevoland, "Provincie Flevoland"}
        }[CurrentOrganization];

        public static string CrmApplication => new OrganizationResource()
        {
            {Organization.Qnh, "[CrmApplication]"},
            {Organization.Drenthe, "DocBase"},
            {Organization.Flevoland, "N-adres"}
        }[CurrentOrganization];

        public static string BrmoApplication => new OrganizationResource()
        {
            {Organization.Qnh, "[BrmoApplication]"},
            {Organization.Drenthe, "BRMO"},
            {Organization.Flevoland, "BRMO"}
        }[CurrentOrganization];

        public static string ExportTitle => new OrganizationResource()
        {
            {Organization.Qnh, "CVnHR bulkexport"},
            {Organization.Drenthe, "CVnHR bulkexport"},
            {Organization.Flevoland, "Update naar N-adres"}
        }[CurrentOrganization];

        public static string HomeButton => new OrganizationResource()
        {
            {Organization.Qnh, "Beheer CVnHR"},
            {Organization.Drenthe, "Beheer CVnHR"},
            {Organization.Flevoland, "Home"}
        }[CurrentOrganization];

        public static string HomeScreen => new OrganizationResource()
        {
            {Organization.Qnh, "Welkom bij de CVnHR applicatie van de [CompanyName]"},
            {Organization.Drenthe, "Welkom bij de CVnHR applicatie van de Provincie Drenthe"},
            {Organization.Flevoland, "Welkom bij de voorziening handelsregister van de Provincie Flevoland"}
        }[CurrentOrganization];

        public static string LogoUrl => new OrganizationResource()
        {
            {Organization.Qnh, "~/Images/qnh.jpg"},
            {Organization.Drenthe, "~/Images/Provincie_Drenthe_800px.png"},
            {Organization.Flevoland, "~/Images/logo-ProvincieFlevoland.jpg"}
        }[CurrentOrganization];

        public static string SearchApplication => new OrganizationResource()
        {
            {Organization.Qnh, "Zoeken in CVnHR"},
            {Organization.Drenthe, "Zoeken in CVnHR"},
            {Organization.Flevoland, "Zoeken organisatie"}
        }[CurrentOrganization];

        public static string SearchExternal => new OrganizationResource()
        {
            {Organization.Qnh, "Zoeken bij KVK"},
            {Organization.Drenthe, "Zoeken bij KVK"},
            {Organization.Flevoland, "Ophalen nieuwe organisatie"}
        }[CurrentOrganization];

        public static string TitleSearchApplication => new OrganizationResource()
        {
            {Organization.Qnh, "Zoek in CVnHR"},
            {Organization.Drenthe, "Zoek in CVnHR"},
            {Organization.Flevoland, "Zoeken bekende organisatie"}
        }[CurrentOrganization];

        public static string TitleSearchKvK => new OrganizationResource()
        {
            {Organization.Qnh, "Zoek in het handelsregister"},
            {Organization.Drenthe, "Zoek in het handelsregister"},
            {Organization.Flevoland, "Nieuwe organisatie zoeken bij de Kamer van Koophandel"}
        }[CurrentOrganization];

        public static string ToApplication => new OrganizationResource()
        {
            {Organization.Qnh, "Naar CVnHR"},
            {Organization.Drenthe, "Naar CVnHR"},
            {Organization.Flevoland, "Importeren bulk uit KvK"}
        }[CurrentOrganization];

        public static string ToCrm => new OrganizationResource()
        {
            {Organization.Qnh, "Naar [CrmApplication]"},
            {Organization.Drenthe, "Naar DocBase"},
            {Organization.Flevoland, "Update naar n-adres"}
        }[CurrentOrganization];

        public static string ToBrmo => new OrganizationResource()
        {
            {Organization.Qnh, "Naar [BrmoApplication]"},
            {Organization.Drenthe, "Naar Brmo"},
            {Organization.Flevoland, "Naar Brmo"}
        }[CurrentOrganization];

        public static string ToCrediteuren => new OrganizationResource()
        {
            { Organization.Qnh, "Naar [CrediteurenApplication]" },
            { Organization.Drenthe, "Naar PROBIS (Crediteuren)" },
            { Organization.Flevoland, "[huh?]" }
        }[CurrentOrganization];

        public static string ToDebiteuren => new OrganizationResource()
        {
            { Organization.Qnh, "Naar [DebiteurenApplication]" },
            { Organization.Drenthe, "Naar PROBIS (Debiteuren)" },
            { Organization.Flevoland, "[huh?]" }
        }[CurrentOrganization];

        public static string FinancialApplication => new OrganizationResource()
        {
            { Organization.Qnh, "[FinancialApplication]" },
            { Organization.Drenthe, "Probis" },
            { Organization.Flevoland, "" }
        }[CurrentOrganization];

        /// <summary>
        /// For copy/paste... TODO: create template?
        /// </summary>
        public static string Empty => new OrganizationResource()
        {
            { Organization.Qnh, "" },
            { Organization.Drenthe, "" },
            { Organization.Flevoland, "" }
        }[CurrentOrganization];
    }
}
