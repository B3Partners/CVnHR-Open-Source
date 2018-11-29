using Microsoft.Win32.TaskScheduler;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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


        public IEnumerable<Task> TaskProcesses
        {
            get
            {
                TaskService ts = new TaskService();
                return ts.RootFolder.GetTasks(new System.Text.RegularExpressions.Regex(@"CVnHR")).ToArray();
            }
        }
    }
}