using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Service.BRMO;

namespace QNH.Overheid.KernRegister.BatchProcess.Processes
{
    public class RsgbProcesses
    {
        // TODO: retry with CSV file??

        public static void FillRsgbForZipcodes(int maxDegreeOfParallelism, 
            Logger log, 
            string HRDataserviceVersion,
            BrmoProcessTypes type, 
            List<string> items = null)
        {
            Console.WriteLine("Make sure to run this process as administrator!");

            List<string> kvkIds;
            if (type == BrmoProcessTypes.ZipCodes)
            {
                var msg = $"Searching kvkIds in zipcodes: {string.Join(" ", items)}";
                log.Debug(msg);
                Console.WriteLine(msg);
                kvkIds = ZipcodeProcesses.GetKvkIdsForZipcode(maxDegreeOfParallelism, true, items.ToArray()).ToList();
                msg = $"Found {kvkIds.Count()} kvk Ids.";
                log.Debug(msg);
                Console.WriteLine(msg);
            }
            else
                kvkIds = items;

            var hrDataserviceVersionNumberBrmo = ConfigurationManager.AppSettings["HR-DataserviceVersionNumberBrmo"];
            var service = hrDataserviceVersionNumberBrmo == "2.5"
                ? IocConfig.Container.GetInstance<IKvkSearchServiceV25>()
                : IocConfig.Container.GetInstance<IKvkSearchService>();
            var brmoSyncService = IocConfig.Container.GetInstance<IBrmoSyncService>();

            var errors = new List<Exception>();
            var errorKvkNummers = new List<string>();

            Parallel.ForEach(kvkIds, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (kvkNummer) =>
            {
                try
                {
                    // retry without bypassing cache
                    var xDoc = RawXmlCache.Get(kvkNummer, () => { service.SearchInschrijvingByKvkNummer(kvkNummer, "Batchprocess BRMO"); });
                    var status = brmoSyncService.UploadXDocumentToBrmo(xDoc);
                    brmoSyncService.Transform(kvkNummer);
                    if (status != AddInschrijvingResultStatus.BrmoInschrijvingCreated)
                    {
                        throw new Exception("Status not expected: " + status.ToString());
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Exception for kvkNummer: " + kvkNummer);
                    errors.Add(ex);
                    errorKvkNummers.Add(kvkNummer);
                    Console.Write($"\r{ex.Message}");
                }
            });

            log.Info($"Succesfully uploaded {kvkIds.Count() - errors.Count()} kvk Ids. Found {errors.Count()} errors...");
            log.Info($@"Kvknummers to retry:
{string.Join(Environment.NewLine, errorKvkNummers)}");
            Console.WriteLine();
        }

        private static readonly string[] _zipCodesDrenthe =
            new[]
            {
                7283, 7705, 7740, 7741, 7742, 7750, 7751, 7753, 7754, 7755, 7756, 7760, 7761, 7764, 7765, 7766, 7800, 7801,
                7811, 7812, 7813, 7814, 7815, 7821, 7822, 7823, 7824, 7825, 7826, 7827, 7828, 7830, 7831, 7833, 7840,
                7841, 7842, 7843, 7844, 7845, 7846, 7847, 7848, 7849, 7851, 7852, 7853, 7854, 7855, 7856, 7858, 7859,
                7860, 7861, 7863, 7864, 7871, 7872, 7873, 7874, 7875, 7876, 7877, 7880, 7881, 7884, 7885, 7887, 7889,
                7890, 7891, 7892, 7894, 7895, 7900, 7901, 7902, 7903, 7904, 7905, 7906, 7907, 7908, 7909, 7910, 7911,
                7912, 7913, 7914, 7915, 7916, 7917, 7918, 7920, 7921, 7924, 7925, 7926, 7927, 7928, 7929, 7931, 7932,
                7933, 7934, 7935, 7936, 7937, 7938, 7940, 7941, 7942, 7943, 7944, 7948, 7949, 7957, 7958, 7960, 7961,
                7963, 7964, 7965, 7966, 7970, 7971, 7973, 7974, 7975, 7980, 7981, 7983, 7984, 7985, 7986, 7990, 7991,
                8351, 8380, 8381, 8382, 8383, 8384, 8385, 8386, 8387, 8437, 8438, 8439, 9300, 9301, 9302, 9304, 9305,
                9306, 9307, 9311, 9312, 9313, 9314, 9315, 9320, 9321, 9330, 9331, 9333, 9334, 9335, 9336, 9337, 9341,
                9342, 9343, 9400, 9401, 9402, 9403, 9404, 9405, 9406, 9407, 9408, 9409, 9410, 9411, 9412, 9413, 9414,
                9415, 9416, 9417, 9418, 9419, 9420, 9421, 9422, 9423, 9430, 9431, 9432, 9433, 9434, 9435, 9436, 9437,
                9438, 9439, 9441, 9442, 9443, 9444, 9445, 9446, 9447, 9448, 9449, 9450, 9451, 9452, 9453, 9454, 9456,
                9457, 9458, 9459, 9460, 9461, 9462, 9463, 9464, 9465, 9466, 9467, 9468, 9469, 9470, 9471, 9472, 9473,
                9474, 9475, 9480, 9481, 9482, 9483, 9484, 9485, 9486, 9487, 9488, 9489, 9491, 9492, 9493, 9494, 9495,
                9496, 9497, 9511, 9512, 9514, 9515, 9520, 9521, 9523, 9524, 9525, 9526, 9527, 9528, 9530, 9531, 9533,
                9534, 9535, 9536, 9537, 9564, 9571, 9573, 9574, 9654, 9655, 9656, 9657, 9658, 9659, 9749, 9760, 9761,
                9765, 9766
            }.Select(i => i.ToString()).ToArray();
    }
}
