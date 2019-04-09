using Newtonsoft.Json;
using QNH.Overheid.KernRegister.Beheer.ViewModel;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using JsonFile = System.IO.File;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_SyncBrmo)]
    public class BrmoController :Controller
    {
        public static TaskSchedulerPartialController taskController;

        public string JsonConfigPath => Path.Combine(new FileInfo(taskController.ExportTaskManager.ExecutablePath).DirectoryName, "JsonConfig");

        //
        // GET: /Export/
        public ActionResult Index(string name)
        {   
            taskController =  new TaskSchedulerPartialController(name,"BRMO");
            if (taskController.ExportTaskManager == null)
                throw new ArgumentException("ExportTaskManager");
            return View(new RsgbTaskSchedulerModel()
            {
                ExportTaskManager = taskController.ExportTaskManager,
                Config = GetConfig(name)
            });
        }

        [HttpGet]
        public JsonResult GetCurrentState() {
            return taskController.GetCurrentState();
        }

        public FileResult DownloadLogFile(string fileName)
        {
            return taskController.DownloadLogFile(fileName);
        }

        [HttpPost]
        public JsonResult StartBatchUpdateNow() {
            return taskController.StartBatchUpdateNow();
        }

        [HttpPost]
        public JsonResult StopBatchUpdateNow()
        {
            return taskController.StopBatchUpdateNow();
        }

        [HttpPost]
        public JsonResult SetSchedule(DateTime? time, string triggerType, bool? enabled)
        {
            return taskController.SetSchedule(time, triggerType, enabled);
        }

        [HttpPost]
        public void SaveConfig(BrmoConfig config)
        {   
            var postCodes = config.PostCodes
                .Split(',', ';')
                .Select(p => p.Trim())
                .ToList();
            var argument = $"BRMO {config.HRDataserviceVersion} {config.BrmoProcessType.ToString()} {string.Join(" ", postCodes)}";
            taskController.UpdateTaskManagerArguments(argument,config.taskName);
            JsonFile.WriteAllText(Path.Combine(GetFileName(config.taskName)), JsonConvert.SerializeObject(config));
        }

        private BrmoConfig GetConfig(string name)
        {
            if (name.Split(' ')[0].Equals("CVnHR")) {
                name = name.Substring(name.IndexOf(' ') + 1);
            }
            if (!Directory.Exists(JsonConfigPath))
            {
                Directory.CreateDirectory(JsonConfigPath);
            }

            if (!JsonFile.Exists(Path.Combine(GetFileName(name))))
            {
                var config = new BrmoConfig() { PostCodes = "7283,7705,7740", taskName = name };
                SaveConfig(config);
            }

            return JsonConvert.DeserializeObject<BrmoConfig>(JsonFile.ReadAllText(GetFileName(name)));
        }

        private string GetFileName(string name)
        {
            return Path.Combine(JsonConfigPath, $"{name}-brmo-config.json");
        }
    }
}