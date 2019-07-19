using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Business.Service.ZipCodes
{
    public interface IAreaService
    {
        IEnumerable<string> GetInschrijvingenWithAllVestigingenOutsideArea(IEnumerable<string> zipcodeNumbersForArea);
    }
}
