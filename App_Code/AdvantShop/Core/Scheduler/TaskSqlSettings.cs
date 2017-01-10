//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using Quartz;

namespace AdvantShop.Core.Scheduler
{
    public enum TimeIntervalType
    {
        None,
        Days,
        Hours,
        Minutes
    }

    public class TaskSettings : List<TaskSetting>
    {
        private static TaskSettings GetDefault()
        {
            return new TaskSettings
            {
                new TaskSetting
                {
                    Enabled = false,
                    JobType = typeof (GenerateHtmlMapJob).ToString(),
                    TimeInterval = 0,
                    TimeHours = 0,
                    TimeMinutes = 0,
                    TimeType = TimeIntervalType.None
                },
                new TaskSetting
                {
                    Enabled = false,
                    JobType = typeof (GenerateXmlMapJob).ToString(),
                    TimeInterval = 0,
                    TimeHours = 0,
                    TimeMinutes = 0,
                    TimeType = TimeIntervalType.None
                },
                new TaskSetting
                {
                    Enabled = false,
                    JobType = typeof (GenerateYandexMarketJob).ToString(),
                    TimeInterval = 0,
                    TimeHours = 0,
                    TimeMinutes = 0,
                    TimeType = TimeIntervalType.None
                }
            };
        }

        public static TaskSettings Settings
        {
            get
            {
                var fromDb = SettingProvider.Items["TaskSqlSettings"];
                if (fromDb == null)
                    return GetDefault();
                var temp = JsonConvert.DeserializeObject<TaskSettings>(SQLDataHelper.GetString(fromDb));
                if (temp == null)
                    return GetDefault();
                return temp;
            }
            set { SettingProvider.Items["TaskSqlSettings"] = JsonConvert.SerializeObject(value); }
        }
    }

    public class TaskSetting
    {
        public string JobType { get; set; }
        public bool Enabled { get; set; }
        public int TimeInterval { get; set; }
        public int TimeHours { get; set; }
        public int TimeMinutes { get; set; }
        public TimeIntervalType TimeType { get; set; }
    }

    public static class JovExt
    {
        public static bool CanStart(this IJobExecutionContext obj)
        {
            var dataMap = obj.JobDetail.JobDataMap;
            var jobData = dataMap.Get(TaskManager.DataMap) as TaskSetting;
            if (jobData == null) return false;

            var file = SettingsGeneral.AbsolutePath + "App_Data/" + jobData.JobType;
            var lasttime = System.IO.File.Exists(file) ? System.IO.File.ReadAllText(file).TryParseDateTime(true) : null;
            var currentTime = DateTime.Now;
            if (!lasttime.HasValue) return true;
            if (jobData.TimeType == TimeIntervalType.Days)
            {
                return (currentTime - lasttime.Value).Days >= jobData.TimeInterval;
            }
            if (jobData.TimeType == TimeIntervalType.Hours)
            {
                return (currentTime - lasttime.Value).Hours >= jobData.TimeInterval;
            }

            if (jobData.TimeType == TimeIntervalType.Minutes)
            {
                return (currentTime - lasttime.Value).Minutes >= jobData.TimeInterval;
            }
            return true;
        }

        public static void WriteLastRun(this IJobExecutionContext obj)
        {
            var dataMap = obj.JobDetail.JobDataMap;
            var jobData = dataMap.Get(TaskManager.DataMap) as TaskSetting;
            if (jobData == null) return;

            var file = SettingsGeneral.AbsolutePath + "App_Data/" + jobData.JobType;
            System.IO.File.WriteAllText(file, DateTime.Now.ToString());
        }
    }
}