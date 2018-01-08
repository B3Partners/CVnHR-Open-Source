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

    public static class ApplicationActionDescriptions
    {
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static string GetDisplayName<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DisplayNameAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayNameAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the DisplayName value
                    return ((DisplayNameAttribute)attrs[0]).DisplayName;
                }
            }
            //If we have no DisplayName attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; private set; }
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
