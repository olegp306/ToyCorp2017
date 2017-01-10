//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;

namespace AdvantShop.Core.Scheduler
{
    //pattern singleton 
    public class TaskManager
    {
        private static readonly TaskManager taskManager = new TaskManager();
        private readonly IScheduler _sched;
        public const string DataMap = "data";
        private const string TaskGroup = "TaskGroup";
        private const string WebConfigGrop = "WebConfigGrop";
        private const string ModuleGroup = "ModuleGroup";

        private readonly ConcurrentQueue<Action> _queueAction;
        private Task _currentTask;

        private TaskManager()
        {
            // _currentTask = Task.Factory.StartNew(DoJobs, TaskCreationOptions.LongRunning);
            _queueAction = new ConcurrentQueue<Action>();
            var properties = new NameValueCollection();
            properties["quartz.threadPool.threadCount"] = "1";
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory(properties);
            // get a scheduler
            _sched = schedFact.GetScheduler();
        }

        public static TaskManager TaskManagerInstance()
        {
            return taskManager;
        }

        public void Init()
        {
            var config = (List<XmlNode>)ConfigurationManager.GetSection("TasksConfig");

            foreach (XmlNode nodeItem in config)
            {
                //cheak for null
                if (nodeItem.Attributes == null || string.IsNullOrEmpty(nodeItem.Attributes["name"].Value) ||
                    string.IsNullOrEmpty(nodeItem.Attributes["type"].Value) || string.IsNullOrEmpty(nodeItem.Attributes["cronExpression"].Value)) return;

                var jobName = nodeItem.Attributes["name"].Value;
                var jobType = nodeItem.Attributes["type"].Value;
                var cronExpression = nodeItem.Attributes["cronExpression"].Value;
                var enabled = nodeItem.Attributes["enabled"].Value.TryParseBool();
                if (!enabled) continue;
                if (_sched.CheckExists(new JobKey(jobName, WebConfigGrop))) continue;

                var taskType = Type.GetType(jobType);
                if (taskType == null) continue;

                // construct job info
                var jobDetail = new JobDetailImpl(jobName, WebConfigGrop, taskType);

                // каждый класс сам обработает хmlNode для себя
                jobDetail.JobDataMap[DataMap] = nodeItem;
                //http://www.quartz-scheduler.org/documentation/quartz-1.x/tutorials/crontrigger

                var trigger = new CronTriggerImpl(jobName, WebConfigGrop, jobName, WebConfigGrop, cronExpression);

                _sched.ScheduleJob(jobDetail, trigger);
            }

            var tType = Type.GetType("AdvantShop.Core.Scheduler.LicJob");
            var jDetail = new JobDetailImpl("LicJob", WebConfigGrop, tType);
            var trig = new CronTriggerImpl("LicJob", WebConfigGrop, "LicJob", WebConfigGrop, "0 59 0 1/1 * ?");
            _sched.ScheduleJob(jDetail, trig);
        }

        public void ManagedTask(TaskSettings settings)
        {
            foreach (var setting in settings)
            {
                _sched.DeleteJob(new JobKey(setting.JobType, TaskGroup));

                if (setting.Enabled)
                {
                    if (string.IsNullOrEmpty(setting.JobType)) continue;

                    var taskType = Type.GetType(setting.JobType);
                    if (taskType == null) continue;
                    if (_sched.CheckExists(new JobKey(setting.JobType, TaskGroup))) continue;

                    var jobDetail = new JobDetailImpl(setting.JobType, TaskGroup, taskType);
                    jobDetail.JobDataMap[DataMap] = setting;
                    var cronExpression = GetCronString(setting);
                    if (string.IsNullOrEmpty(cronExpression)) continue;

                    var trigger = new CronTriggerImpl(setting.JobType, TaskGroup, setting.JobType, TaskGroup, cronExpression);
                    _sched.ScheduleJob(jobDetail, trigger);
                }
            }

            var moduleTasks = AttachedModules.GetModules<IModuleTask>();
            foreach (var moduleTask in moduleTasks)
            {
                var classInstance = (IModuleTask)Activator.CreateInstance(moduleTask, null);
                var tasksSettings = classInstance.GetTasks();

                foreach (var setting in tasksSettings)
                {
                    _sched.DeleteJob(new JobKey(setting.JobType, ModuleGroup));

                    if (setting.Enabled)
                    {
                        var type = Type.GetType(setting.JobType);
                        if (type == null) continue;

                        var cronExpression = GetCronString(setting);
                        if (string.IsNullOrEmpty(cronExpression)) continue;

                        var jobDetail = new JobDetailImpl(setting.JobType, ModuleGroup, type);
                        jobDetail.JobDataMap[DataMap] = setting;

                        var trigger = new CronTriggerImpl(setting.JobType, ModuleGroup, setting.JobType, ModuleGroup, cronExpression);
                        _sched.ScheduleJob(jobDetail, trigger);
                    }
                }
            }
        }

        public string GetTasks()
        {
            var res = (from jobGroupName in _sched.GetJobGroupNames()
                       from jobkey in _sched.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroupName))
                       select _sched.GetJobDetail(jobkey)).ToList();

            return res.Aggregate(string.Empty, (current, item) => current + (";" + item.JobType));
        }

        private string GetCronString(TaskSetting item)
        {
            //return string.Format("0 0/1 * * * ?");

            switch (item.TimeType)
            {
                case TimeIntervalType.Days:
                    return string.Format("0 {1} {0} 1/1 * ?", item.TimeHours, item.TimeMinutes);
                case TimeIntervalType.Hours:
                    return  "0 0 0/1 * * ?";
                case TimeIntervalType.Minutes:
                    return "0 0/1 * * * ?";
            }
            return string.Empty;
        }

        public void Start()
        {
            _sched.Start();
        }

        public void RemoveModuleTask(TaskSetting task)
        {
            if (_sched.CheckExists(new JobKey(task.JobType, ModuleGroup)))
            {
                _sched.DeleteJob(new JobKey(task.JobType, ModuleGroup));
            }
        }

        public void AddTask(Action action)
        {
            if (_queueAction.Contains(action)) return;
            _queueAction.Enqueue(action);

            if ((_currentTask!=null )&&( _currentTask.Status != TaskStatus.Created)&&(_currentTask.IsCompleted == false ||
                 _currentTask.Status == TaskStatus.Running ||
                 _currentTask.Status == TaskStatus.WaitingToRun ||
                 _currentTask.Status == TaskStatus.WaitingForActivation))
                return;

            _currentTask = Task.Factory.StartNew(DoJobs, TaskCreationOptions.LongRunning);
        }

        private void DoJobs()
        {
            Action item;
            while (_queueAction.TryDequeue(out item))
            {
                item();
            }
        }
    }
}