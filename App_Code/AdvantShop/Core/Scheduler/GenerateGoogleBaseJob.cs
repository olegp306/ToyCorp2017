using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler;
using AdvantShop.ExportImport;
using AdvantShop.Trial;
using Quartz;

[DisallowConcurrentExecution]
public class GenerateGoogleBaseJob:IJob
{
    public void Execute(IJobExecutionContext context)
    {
        if (TrialService.IsTrialEnabled) return;

        if (!context.CanStart()) return;
        context.WriteLastRun();
        string strFileName = ExportFeed.GetModuleSetting("GoogleBase", "FileName");
        string strPhysicalTargetFolder = SettingsGeneral.AbsolutePath;
        string strPhysicalFilePath = strPhysicalTargetFolder + strFileName;
        var exportFeedModule = new ExportFeedModuleGoogleBase();
        exportFeedModule.GetExportFeedString(strPhysicalFilePath);
    }
}