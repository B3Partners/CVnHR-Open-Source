using QNH.Overheid.KernRegister.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QNH.Overheid.KernRegister.Business.Service.ZipCodes
{
    public class AreaService : IAreaService
    {
        private readonly IKvkInschrijvingRepository _kvkInschrijvingRepo;

        public AreaService(IKvkInschrijvingRepository kvkInschrijvingRepo)
        {
            _kvkInschrijvingRepo = kvkInschrijvingRepo ?? throw new ArgumentNullException(nameof(kvkInschrijvingRepo));
        }

        public IEnumerable<string> GetInschrijvingenWithAllVestigingenOutsideArea(IEnumerable<string> zipcodeNumbersForArea)
        {
            var inschrijvingenWithAllVestigingenOutsideArea = _kvkInschrijvingRepo.GetAllCurrentVestigingen()
                    .Where(v => v.GeldigTot > DateTime.Now && v.KvkInschrijving.GeldigTot > DateTime.Now)
                    .GroupBy(v => v.KvkInschrijving.KvkNummer)
                    .Where(v => v.All(vestiging => !zipcodeNumbersForArea.Contains(vestiging.PostcodeCijfers)))
                    .Select(v=> v.Key)
                    .Distinct()
                    .ToList();

            return inschrijvingenWithAllVestigingenOutsideArea;
        }
    }
}
