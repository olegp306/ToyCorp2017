//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler;
using AdvantShop.Diagnostics;
using AdvantShop.Modules;
using AdvantShop.Repository.Currencies;
using AdvantShop.Statistic;

namespace AdvantShop.Core
{
    public static class AppServiceStartAction
    {
        public static DataBaseService.PingDbState state;
        public static string errMessage;
        public static bool isAppNeedToReRun;
        public static bool isAppFistRun;
    }

    public class ApplicationService
    {
        public static void StartApplication(HttpContext current)
        {
            ServicePointManager.Expect100Continue = false;
            ApplicationUptime.SetApplicationStartTime();

            SettingsGeneral.SetAbsolutePath(current.Server.MapPath("~/"));

            // loger must init ONLY after SetAbsolutePath 
            Debug.InitLogger();

            // Set "first run" flag
            AppServiceStartAction.isAppFistRun = true;

            // Try to run DB depend code
            TryToStartDbDependServices();

            // No DB depend code here!
        }

        public static void TryToStartDbDependServices()
        {
            var appStartDbRes = AdvantShop.Core.DataBaseService.CheckDbStates();

            AppServiceStartAction.state = appStartDbRes;

            if (AppServiceStartAction.state == DataBaseService.PingDbState.NoError)
            {
                // Other db depend codes
                RunDbDependAppStartServices();
            }

        }

        public static void RunDbDependAppStartServices()
        {
            //
            // Put yours DB depens code here
            //

            //Load modules
            AttachedModules.LoadModules();

            // TaskManager
            TaskManager.TaskManagerInstance().Init();
            TaskManager.TaskManagerInstance().Start();
            TaskManager.TaskManagerInstance().ManagedTask(TaskSettings.Settings);

            // LogSessionRestart
            InternalServices.LogApplicationRestart(false, false);

            // RefreshCurrency
            CurrencyService.RefreshCurrency();
        }
    }
}