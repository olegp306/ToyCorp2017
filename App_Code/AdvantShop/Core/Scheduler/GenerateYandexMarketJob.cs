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
    public class GenerateYandexMarketJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (TrialService.IsTrialEnabled) return;
            
            if (!context.CanStart())  return;
            context.WriteLastRun();
            string strFileName = ExportFeed.GetModuleSetting("YandexMarket", "FileName");
            string strPhysicalTargetFolder = SettingsGeneral.AbsolutePath;
            string strPhysicalFilePath = strPhysicalTargetFolder + strFileName;
            var exportFeedModule = new ExportFeedModuleYandex();
            exportFeedModule.GetExportFeedString(strPhysicalFilePath);
        }
    }
}