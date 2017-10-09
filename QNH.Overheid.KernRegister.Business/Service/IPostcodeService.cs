using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Newtonsoft.Json.Converters;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public interface IPostcodeService
    {
        /// <summary>
        /// Searches a Municipality for the postcode
        /// </summary>
        /// <param name="postcode">The postcode</param>
        /// <returns>The Municipality or NULL</returns>
        string GetMunicipalityForPostcode(string postcode);


        /// <summary>
        /// Searches a CountrySubdivision (Province) for the postcode
        /// </summary>
        /// <param name="postcode">The postcode</param>
        /// <returns>The CountrySubdivision or NULL</returns>
        string GetCountrySubdivisionForPostcode(string postcode);
    }
}
