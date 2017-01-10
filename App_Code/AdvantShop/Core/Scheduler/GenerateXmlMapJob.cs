//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.ExportImport;
using AdvantShop.Trial;
using Quartz;

namespace AdvantShop.Core.Scheduler
{
    [DisallowConcurrentExecution]
    public class GenerateXmlMapJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (TrialService.IsTrialEnabled)
                return;

            if (!context.CanStart()) return;
            context.WriteLastRun();
            string strFileName = "sitemap.xml".ToLower();
            string strPhysicalTargetFolder = SettingsGeneral.AbsolutePath;
            string strPhysicalFilePath = strPhysicalTargetFolder + strFileName;
            var temp = new ExportXmlMap(strPhysicalFilePath, strPhysicalTargetFolder);
            temp.Create();

        }
    }
}