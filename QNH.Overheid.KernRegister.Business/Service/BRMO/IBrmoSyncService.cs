using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Model;

namespace QNH.Overheid.KernRegister.Business.Service.BRMO
{
    public interface IBrmoSyncService
    {
        AddInschrijvingResultStatus UploadXDocumentToBrmo(XDocument xDocument);
        void Transform(string kvkNummer);
    }
}
