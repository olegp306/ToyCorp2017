//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Net;
using System.Xml;
using AdvantShop.Configuration;
using Quartz;

namespace AdvantShop.Core.Scheduler
{
    [DisallowConcurrentExecution]
    public class JobBeAlive : IJob
    {
        private string _url;

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var jobData = (XmlNode)dataMap.Get(TaskManager.DataMap);
            if (jobData.Attributes == null || string.IsNullOrEmpty(jobData.Attributes["url"].Value))
                return;
            _url = jobData.Attributes["url"].Value;
            var resUrl = SettingsGeneral.AbsoluteUrl + _url;
            if (resUrl.Contains("http://"))
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        string response = wc.DownloadString(resUrl);
                    }
                }catch
                {
                    // empty catch: we no need to log this error
                }

            }
            //GC.Collect();
        }
    }
}