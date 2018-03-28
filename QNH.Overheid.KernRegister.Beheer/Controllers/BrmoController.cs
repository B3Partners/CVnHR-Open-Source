using Newtonsoft.Json;
using QNH.Overheid.KernRegister.Beheer.ViewModel;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Service.Users;
using QNH.Overheid.KernRegister.Organization.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JsonFile = System.IO.File;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_SyncBrmo)]
    public class BrmoController : TaskSchedulerPartialController
    {
        public string JsonConfigPath => Path.Combine(new FileInfo(ExportTaskManager.ExecutablePath).DirectoryName, "JsonConfig");

        public string JsonConfig => Path.Combine(JsonConfigPath, "brmo-config.json");

        public BrmoController() 
            : base(Default.ApplicationName + " Batchupdate BRMO Task", "BRMO")
        {
        }

        //
        // GET: /Export/
        public ActionResult Index()
        {
            if (ExportTaskManager == null)
                throw new ArgumentException("ExportTaskManager");

            return View(new RsgbTaskSchedulerModel()
            {
                ExportTaskManager = ExportTaskManager,
                Config = GetConfig()
            });
        }

        [HttpPost]
        public void SaveConfig(BrmoConfig config)
        {
            var postCodes = config.PostCodes
                .Split(',', ';')
                .Select(p => p.Trim())
                .ToList();
            var argument = $"BRMO {config.HRDataserviceVersion} {config.BrmoProcessType.ToString()} {string.Join(" ", postCodes)}";
            UpdateTaskManagerArguments(argument);
            JsonFile.WriteAllText(JsonConfig, JsonConvert.SerializeObject(config));
        }

        private BrmoConfig GetConfig()
        {
            if (!Directory.Exists(JsonConfigPath))
            {
                Directory.CreateDirectory(JsonConfigPath); // TODO
            }

            if (!JsonFile.Exists(JsonConfig))
            {
                var config = new BrmoConfig() { PostCodes = "7283,7705,7740" };
                SaveConfig(config);
            }

            return JsonConvert.DeserializeObject<BrmoConfig>(JsonFile.ReadAllText(JsonConfig));
        }
    }

    public class BrmoConfig
    {
        public string PostCodes { get; set; }

        
        public string HRDataserviceVersion { get; set; } = "2.5";

        public BrmoProcessTypes BrmoProcessType { get; set; } = BrmoProcessTypes.ZipCodes;

        [JsonIgnore]
        public IEnumerable<string> PossibleHRDataserviceVersions { get; set; } = new[] { "2.5", "3.0" };

        [JsonIgnore]
        public IEnumerable<string> PossibleBrmoProcessTypes { get { return Enum.GetNames(typeof(BrmoProcessTypes)); } }

    }
}