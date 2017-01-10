//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Permission;
using Quartz;

namespace AdvantShop.Core.Scheduler
{
    [DisallowConcurrentExecution]
    public class LicJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var rand = new Random().Next(20 * 60);
            Thread.Sleep(rand * 1000);
            try
            {
                SettingsLic.ActiveLic = PermissionAccsess.ActiveDailyLic(SettingsLic.LicKey, SettingsMain.SiteUrl,
                                                                         SettingsMain.ShopName,
                                                                         SettingsGeneral.SiteVersion, SettingsGeneral.SiteVersionDev, SettingsGeneral.AbsoluteUrl);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, "Error at license check at lic job");
            }
        }
    }
}