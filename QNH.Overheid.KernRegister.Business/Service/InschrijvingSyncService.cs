using System;
using System.Collections.Generic;
using System.Linq;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public class InschrijvingSyncService : IInschrijvingSyncService, IDisposable
    {
        private readonly IKvkInschrijvingRepository _kvkInschrijvingRepo;

        public InschrijvingSyncService(IKvkInschrijvingRepository kvkInschrijvingRepo)
        {
            _kvkInschrijvingRepo = kvkInschrijvingRepo ?? throw new ArgumentNullException(nameof(kvkInschrijvingRepo));
        }

        /// <summary>
        /// New Item will be added if the Inschrijving in the database is different from the supplied Inschrijving. If there's
        /// an existing item with the same KvkNummer, it will be "closed" and the new item will become the current one.
        /// </summary>
        /// <param name="kvkInschrijving"></param>
        public AddInschrijvingResultStatus AddNewInschrijvingIfDataChanged(KvkInschrijving kvkInschrijving)
        {
            // Ensure unique sbiCode objects
            foreach (var activiteit in kvkInschrijving.SbiActiviteiten)
            {
                var code = activiteit.SbiCode;
                activiteit.SbiCode = _kvkInschrijvingRepo.GetSbiCode(code.Code, code.Omschrijving);
            }
            // Ensure unique sbiCodes objects per vestiging
            foreach (var vestiging in kvkInschrijving.Vestigingen)
                foreach (var activiteit in vestiging.SbiActiviteiten)
                {
                    var code = activiteit.SbiCode;
                    activiteit.SbiCode = _kvkInschrijvingRepo.GetSbiCode(code.Code, code.Omschrijving);
                }

            // Now get the latest Inschrijving from the KVK register database
            var latestInschrijvingFromDb = _kvkInschrijvingRepo.GetLatestInschrijving(kvkInschrijving.KvkNummer);
            if (latestInschrijvingFromDb == null) // Just store the new one
            {
                kvkInschrijving.IngevoegdOp = DateTime.Now.AddSeconds(1);
                kvkInschrijving.GeldigTot = DateTime.MaxValue;
                _kvkInschrijvingRepo.AddOrUpdate(kvkInschrijving);
                return AddInschrijvingResultStatus.NewInschrijvingAdded;
            }

            // If one is found, let's compare it with the one that is supplied
            if (latestInschrijvingFromDb.FunctionalEquals(kvkInschrijving))
            {
                // if they are the same, do nothing
                return AddInschrijvingResultStatus.InschrijvingAlreadyExists;
            }
            
            // Set the expiration date for the old inschrijving including all its vestigingen
            latestInschrijvingFromDb.GeldigTot = DateTime.Now;
            foreach (var vestiging in latestInschrijvingFromDb.Vestigingen)
                vestiging.GeldigTot = DateTime.Now;
            _kvkInschrijvingRepo.AddOrUpdate(latestInschrijvingFromDb);

            // Create the new inchrijving
            kvkInschrijving.IngevoegdOp = DateTime.Now.AddSeconds(1);
            kvkInschrijving.GeldigTot = DateTime.MaxValue;
            foreach (var vestiging in kvkInschrijving.Vestigingen)
            {
                vestiging.IngevoegdOp = DateTime.Now.AddSeconds(1);
                vestiging.GeldigTot = DateTime.MaxValue;
            }
            _kvkInschrijvingRepo.AddOrUpdate(kvkInschrijving);

            return AddInschrijvingResultStatus.InschrijvingUpdated;
        }

        public void Dispose()
        {
            _kvkInschrijvingRepo.Dispose();
        }
    }
}