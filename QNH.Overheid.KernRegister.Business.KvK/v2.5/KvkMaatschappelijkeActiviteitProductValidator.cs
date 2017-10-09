using System.Collections.Generic;
using System.Linq;
using QNH.Overheid.KernRegister.Business.Service;

namespace QNH.Overheid.KernRegister.Business.KvK.v2._5
{
#warning //TODO: delete or completely rewrite to validate on base of KvK service documentation and messages
    public class KvkMaatschappelijkeActiviteitProductValidator
    {
        public static bool HasErrors(MaatschappelijkeActiviteitResponseType productResponse)
        {
            return GetAllRuleViolations(productResponse).Any(m => m.MessageType == ValidationMessageType.Error);
        }

        public static bool HasWarnings(MaatschappelijkeActiviteitResponseType productResponse)
        {
            return GetAllRuleViolations(productResponse).Any(m => m.MessageType == ValidationMessageType.Warning);
        }
        
        public static IEnumerable<ValidationMessage> GetAllRuleViolations(MaatschappelijkeActiviteitResponseType productResponse)
        {
            if (productResponse == null)
            {
                yield return new ValidationMessage("Het product wat terug kwam van de service is leeg", ValidationMessageType.Error);
            }
            else
            {
                if (productResponse.product == null)
                {
                    yield return new ValidationMessage("Het bericht bevat geen MaatschappelijkeActiviteit object", ValidationMessageType.Error);
                }

                if (productResponse.meldingen?.fout != null && productResponse.meldingen?.fout?.Any() == true)
                {
                    yield return new ValidationMessage("Er is een fout opgetreden. FoutBericht(en): " +
                        string.Join("; ", productResponse.meldingen.fout.Select(f => f.code + ": " + f.omschrijving + " - " + f.referentie)), ValidationMessageType.Error);
                }
            }
        }
 
        
        
    }
}