using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public interface IInschrijvingSyncService
    {
        /// <summary>
        /// New Item will be added if the Inschrijving in the database is different from the supplied Inschrijving. If there's
        /// an existing item with the same KvkNummer, it will be "closed" and the new item will become the current one.
        /// </summary>
        /// <param name="kvkInschrijving"></param>
        AddInschrijvingResultStatus AddNewInschrijvingIfDataChanged(KvkInschrijving kvkInschrijving);
    }
}
