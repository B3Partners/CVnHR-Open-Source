using Microsoft.Win32.TaskScheduler;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_Tasks)]
    public class TasksController : Controller
    {
        // GET: Tasks
        public ActionResult Index()
        {
            return View(this);
        }


        [HttpPost]
        public void DeleteTask(string name) {
            string executablePath;
            executablePath = ConfigurationManager.AppSettings["BatchProcessExecutablePath"];
            //determine whether path is absolute or relative
            if (!executablePath.Contains(":"))
            {
                executablePath = Path.Combine(
                        Directory.GetParent(
                            Directory.GetParent(
                                AppDomain.CurrentDomain.BaseDirectory
                                ).ToString()
                             ).ToString()
                             , executablePath);
            }
            string exePath = Path.Combine(new FileInfo(executablePath).DirectoryName,"JsonConfig");
            TaskService ts = new TaskService();
            ts.RootFolder.DeleteTask(name);
            string filePath = Path.Combine(exePath, name.Substring(name.IndexOf(' ') + 1) + "-brmo-config.json");
            if (System.IO.File.Exists(filePath)) {
                System.IO.File.Delete(filePath);
            }
        }

        public IEnumerable<Task> TaskProcesses
        {
            get
            {
                TaskService ts = new TaskService();
                IEnumerable<Task> taskCollection = ts.RootFolder.GetTasks(new System.Text.RegularExpressions.Regex(@"CVnHR")).ToArray();
                List<Task> taskList = new List<Task>();
                foreach (Task t in taskCollection) {
                    if (t.Definition.Actions.Cast<ExecAction>().Single().Arguments.Split(' ')[0].Equals("BRMO")) {
                        taskList.Add(t);
                    }
                }
                return taskList;

            }
        }
    }
}