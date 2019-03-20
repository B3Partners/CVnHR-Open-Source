using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using NLog;
using QNH.Overheid.KernRegister.Organization.Resources;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    public class ScheduledTaskManager
    {
        public TaskService TaskService { get; private set; }
        public Task ScheduledTask { get; set; }

        public ExecAction ScheduledAction => ScheduledTask != null ? ScheduledTask.Definition.Actions.OfType<ExecAction>().FirstOrDefault() : null;

        public Trigger Trigger => ScheduledTask != null && ScheduledTask.Definition.Triggers.Any()
            ? ScheduledTask.Definition.Triggers.FirstOrDefault()
            : null;

        public string LastTaskResult
        {
            get
            {
                switch (ScheduledTask.LastTaskResult)
                {
                    case 0:
                        return "Succes";
                    case 1:
                        return "Nog niet gestart";
                    case 267014:
                        return "Onderbroken door gebruiker";
                    default:
                        return ScheduledTask.State == TaskState.Running ? "---" : "Error (code: " + ScheduledTask.LastTaskResult + ")";
                }
            }
        }

        public string NextRunTime => ScheduledTask.NextRunTime > DateTime.MinValue ? ScheduledTask.NextRunTime.ToString() : "---";

        public bool ExecutableExists => File.Exists(ExecutablePath);

        public IEnumerable<string> LogFiles
        {
            get
            {
                string caseTest = Argument;
                if (caseTest.Contains("BRMO"))
                {
                    caseTest = "BRMO";
                }
                var logPath = "Logs";
                switch (caseTest)
                {
                    case "B":
                        logPath = Path.Combine(logPath, "Crm");
                        break;
                    case "K":
                        logPath = Path.Combine(logPath, "Kernregistratie");
                        break;
                    case "BRMO":
                        logPath = Path.Combine(logPath, "Brmo");
                        break;
                    default:
                        logPath = Path.Combine(logPath, Argument);
                        break;
                }
                var directory = Path.Combine(Path.GetDirectoryName(ExecutablePath), logPath);
                if (Directory.Exists(directory))
                    return Directory.GetFiles(directory, "*.log");
                else
                    return Enumerable.Empty<string>();
            }
        }

        public IEnumerable<string> KvKLogFiles
        {
            get
            {
                var directory = HostingEnvironment.MapPath("~/Logs/kvk");
                if (Directory.Exists(directory))
                    return Directory.GetFiles(directory, "*.log");
                else
                    return Enumerable.Empty<string>();
            }
        }

        public bool ExecutableProcessDisabled => Convert.ToBoolean(ConfigurationManager.AppSettings["BatchProcessDisabled"]);

        public string ExecutablePath { get; private set; }
        public string TaskName { get; private set; }
        public string Argument { get; private set; }

        public ScheduledTaskManager(string taskName, string executablePath, string argument)
        {
            ExecutablePath = executablePath;
            if (ExecutableExists)
            {
                TaskName = taskName;
                Argument = argument;

                TaskService = new TaskService();

                ScheduledTask = null;

                TaskCollection tasks = TaskService.RootFolder.GetTasks(new System.Text.RegularExpressions.Regex(@"CVnHR"));

                foreach (Task t in tasks) {
                    if (t.Name.Equals(taskName)) {
                        ScheduledTask = t;
                        break;
                    }
                }

                if (ScheduledTask == null)
                {
                    ScheduledTask = TaskService.AddTask(TaskName,
                        new TimeTrigger()
                        {
                            StartBoundary = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 2, 0, 0),
                            Enabled = false
                        },
                        new ExecAction(ExecutablePath, Argument),
                        userId: "SYSTEM", // "LOCAL SERVICE",
                        logonType: TaskLogonType.ServiceAccount
                    );
                    //ScheduledTask.Definition.Principal.RunLevel = TaskRunLevel.Highest;
                    //ScheduledTask.RegisterChanges();
                }

                // Ensure the execution action path is still correct
                if (ScheduledAction.Path != ExecutablePath || ScheduledAction.Arguments != Argument)
                {
                    ScheduledAction.Path = ExecutablePath;
                    ScheduledAction.Arguments = Argument;
                    ScheduledTask.RegisterChanges();
                }

            }
        }
    }

    public class TaskSchedulerPartialController : Controller // BackloadController
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        private Dictionary<string, ScheduledTaskManager> taskManagers = new Dictionary<string, ScheduledTaskManager>();
        private string taskName, taskArgument, executablePath;


        public ScheduledTaskManager ExportTaskManager => taskManagers[taskName];

        public TaskSchedulerPartialController(string taskName, string argument)
        {
            this.taskName = taskName;
            this.taskArgument = argument;
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
            fillTaskManagers();
            if (!taskManagers.ContainsKey(taskName)) {
                taskManagers.Add(taskName, new ScheduledTaskManager(
                   taskName: taskName,
                   executablePath: executablePath,
                   argument: argument));
            }
            if (ExportTaskManager.Trigger is TimeTrigger
                && ExportTaskManager.Trigger.Enabled
                && ExportTaskManager.ScheduledTask.NextRunTime <= DateTime.MinValue
                && ExportTaskManager.ScheduledTask.State != TaskState.Running)
            {
                ExportTaskManager.Trigger.Enabled = false;
                ExportTaskManager.ScheduledTask.RegisterChanges();
            }
        }

#warning //TODO: create hub

        public JsonResult StartBatchUpdateNow()
        {
            _log.Info($"Task {ExportTaskManager.TaskName} with arguments {ExportTaskManager.Argument} started by {User?.Identity?.Name ?? "[unknown]"}");
            ExportTaskManager.ScheduledTask.Run();
            return Json(true);
        }

        public JsonResult StopBatchUpdateNow()
        {
            _log.Info($"Task {ExportTaskManager.TaskName} with arguments {ExportTaskManager.Argument} stopped by {User?.Identity?.Name ?? "[unknown]"}");
            ExportTaskManager.ScheduledTask.Stop();
            return Json(true);
        }

        public JsonResult GetCurrentState()
        {
            var scheduleStarted = ExportTaskManager.Trigger != null && ExportTaskManager.Trigger.Enabled;
            return Json(new
            {
                CurrentState = ExportTaskManager.ScheduledTask.State.ToString(),
                LastResult = ExportTaskManager.LastTaskResult,
                NextStart = scheduleStarted ? ExportTaskManager.NextRunTime : "--",
                ScheduleState = scheduleStarted ? "Aan" : "Uit"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetSchedule(DateTime? time, string triggerType, bool? enabled)
        {
            _log.Info($"Task {ExportTaskManager.TaskName} with arguments {ExportTaskManager.Argument} schedule set by {User?.Identity?.Name ?? "[unknown]"}. Time: {time}, triggerType: {triggerType}, enabled: {enabled}.");

            if ((!time.HasValue || string.IsNullOrEmpty(triggerType) || !enabled.HasValue) && enabled != false)
                return Json(new { error = "Set the time!" });

            if (enabled == true)
            {
                if (time.Value <= DateTime.Now)
                    time = DateTime.Now.AddMinutes(5);

                var type = (TaskTriggerType)Enum.Parse(typeof(TaskTriggerType), triggerType);
                if (ExportTaskManager.ScheduledTask.Definition.Triggers.Any())
                {
                    ExportTaskManager.ScheduledTask.Definition.Triggers.Clear(); //.RemoveAt(0);
                    ExportTaskManager.ScheduledTask.RegisterChanges();
                }

                switch (type)
                {
                    default:
                    case TaskTriggerType.Time:
                        ExportTaskManager.ScheduledTask.Definition.Triggers.Add(new TimeTrigger
                        {
                            StartBoundary = time.Value,
                            Enabled = enabled.Value
                        });
                        break;
                    case TaskTriggerType.Daily:
                        ExportTaskManager.ScheduledTask.Definition.Triggers.Add(new DailyTrigger()
                        {
                            StartBoundary = time.Value,
                            Enabled = enabled.Value
                        });
                        break;
                    case TaskTriggerType.Weekly:
                        var trigger = new WeeklyTrigger()
                        {
                            StartBoundary = time.Value,
                            Enabled = enabled.Value
                        };

                        switch (time.Value.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                trigger.DaysOfWeek = DaysOfTheWeek.Monday;
                                break;
                            case DayOfWeek.Tuesday:
                                trigger.DaysOfWeek = DaysOfTheWeek.Tuesday;
                                break;
                            case DayOfWeek.Wednesday:
                                trigger.DaysOfWeek = DaysOfTheWeek.Wednesday;
                                break;
                            case DayOfWeek.Thursday:
                                trigger.DaysOfWeek = DaysOfTheWeek.Thursday;
                                break;
                            case DayOfWeek.Friday:
                                trigger.DaysOfWeek = DaysOfTheWeek.Friday;
                                break;
                            case DayOfWeek.Saturday:
                                trigger.DaysOfWeek = DaysOfTheWeek.Saturday;
                                break;
                            case DayOfWeek.Sunday:
                                trigger.DaysOfWeek = DaysOfTheWeek.Sunday;
                                break;
                        }

                        ExportTaskManager.ScheduledTask.Definition.Triggers.Add(trigger);
                        break;
                    case TaskTriggerType.Monthly:
                        ExportTaskManager.ScheduledTask.Definition.Triggers.Add(new MonthlyTrigger()
                        {
                            StartBoundary = time.Value,
                            Enabled = enabled.Value,
                            DaysOfMonth = new[] { time.Value.Day }
                        });
                        break;
                }
            }
            else
                ExportTaskManager.Trigger.Enabled = enabled.Value;

            ExportTaskManager.ScheduledTask.RegisterChanges();

            return Json(new { success = true });
        }

        public FileResult DownloadLogFile(string fileName)
        {
            var file = ExportTaskManager.LogFiles.SingleOrDefault(f => f == fileName || f.EndsWith(fileName))
                ?? ExportTaskManager.KvKLogFiles.SingleOrDefault(f => f == fileName || f.EndsWith(fileName));
            if (file != null)
            {
                return File(file, "text/plain");
            }
            else
                return null;

        }

        public void UpdateTaskManagerArguments(string arguments, string newName)
        {
            newName = Default.ApplicationName +" "+ newName;
            if (!ExportTaskManager.ScheduledTask.Name.Equals(newName))
            {
                ExportTaskManager.ScheduledTask.Definition.Actions.Cast<ExecAction>().Single().Arguments = arguments;
                ExportTaskManager.ScheduledTask.RegisterChanges();
                TaskService tsNew = new TaskService();
                Task tsOld = ExportTaskManager.ScheduledTask;
                Trigger t = (Trigger)tsOld.Definition.Triggers.First().Clone();
                ExecAction action = (ExecAction)tsOld.Definition.Actions.First().Clone();
                ExportTaskManager.ScheduledTask = null;
                try
                {
                    ExportTaskManager.ScheduledTask = tsNew.AddTask(newName,
                    t,
                    action,
                    userId: "SYSTEM", // "LOCAL SERVICE",
                    logonType: TaskLogonType.ServiceAccount
                    );
                    tsNew.RootFolder.DeleteTask(taskName);
                    ExportTaskManager.ScheduledTask.RegisterChanges();
                    taskName = newName;
                    taskArgument = arguments;
                    fillTaskManagers();
                }
                catch (Exception e) {
                    ExportTaskManager.ScheduledTask = tsOld;
                    Console.WriteLine(e);
                }
            }
            else
            {
                ExportTaskManager.ScheduledTask.Definition.Actions.Cast<ExecAction>().Single().Arguments = arguments;
                ExportTaskManager.ScheduledTask.RegisterChanges();
            }
        }

        public void fillTaskManagers() {
            taskManagers.Clear();
            TaskService ts = new TaskService();
            TaskCollection taskList = ts.RootFolder.GetTasks(new System.Text.RegularExpressions.Regex(@"CVnHR"));
            foreach (Task t in taskList) {
                taskManagers.Add(t.Name, new ScheduledTaskManager(t.Name, t.Definition.Actions.Cast<ExecAction>().First().Path, t.Definition.Actions.Cast<ExecAction>().First().Arguments));

            }

        }

    }
}
