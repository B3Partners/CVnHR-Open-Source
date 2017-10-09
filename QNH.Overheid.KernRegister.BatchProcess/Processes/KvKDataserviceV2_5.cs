using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using QNH.Overheid.KernRegister.Beheer;
using QNH.Overheid.KernRegister.Business;
using QNH.Overheid.KernRegister.Business.KvK;
using QNH.Overheid.KernRegister.Business.Integration;

namespace QNH.Overheid.KernRegister.BatchProcess.Processes
{
    public class KvKDataserviceV2_5
    {

        private static string _klantReferentie = ConfigurationManager.AppSettings["SearchServiceKlantReferentie"];

        public static void Test()
        {
            var service = IocConfig.Container.GetInstance<Dataservice>();
            var response = service.ophalenContactGegevens(new ophalenContactGegevensRequest()
            {
                ophalenContactGegevensRequest1 = new ContactGegevensRequestType()
                {
                    klantreferentie = _klantReferentie,
                    kvkNummer = "[fill out kvknumber]"
                }
            });
            Console.WriteLine($"Antwoord: {response?.ophalenContactGegevensResponse1?.product?.maatschappelijkeActiviteit?.berichtenbox?.berichtenboxnaam} {response?.ophalenContactGegevensResponse1?.product?.maatschappelijkeActiviteit?.naam}");
            Console.ReadLine();
        }
    }
}
