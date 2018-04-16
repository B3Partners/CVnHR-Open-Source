using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QNH.Overheid.KernRegister.Beheer.Controllers;

namespace QNH.Overheid.KernRegister.Beheer.ViewModel
{
    public class RsgbTaskSchedulerModel
    {
        public ScheduledTaskManager ExportTaskManager { get; set; }
        public BrmoConfig Config { get; internal set; }
    }
}