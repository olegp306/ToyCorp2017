//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Linq;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using AdvantShop.Statistic;
using AdvantShop.Trial;
using Resources;

namespace Admin
{
    public partial class ExportFeedProgress : AdvantShopAdminPage
    {
        public static string PhysicalAppPath { get; set; }

        public string ModuleName
        {
            get { return Request["ModuleId"] ?? ""; }
        }

        public bool Start
        {
            get { return (Request["start"] ?? string.Empty) == "yes"; }
        }

        private static string FileName;


        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ExportFeed_Yandex_aspx));

            if (!(CommonStatistic.IsRun))
            {
                TrialService.TrackEvent(TrialEvents.ExportProductsToFeed, "");
                CommonStatistic.Init();
            }
            if (!CommonStatistic.IsRun && string.IsNullOrEmpty(ModuleName))
            {
                Response.Redirect("ExportFeed.aspx?ModuleId=YandexMarket");
                return;
            }

            PageSubheader.Visible = true;
            ModuleNameLiteral.Text = ModuleName;

            OutDiv.Visible = Start;
            PhysicalAppPath = Request.PhysicalApplicationPath;

            if ((SaasDataService.IsSaasEnabled) && (!SaasDataService.CurrentSaasData.HaveExportFeeds))
            {
                mainDiv.Visible = false;
                notInTariff.Visible = true;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Start)
                ExportSelection(ModuleName, Request.Url.PathAndQuery);
        }

        public static void ExportSelection(string moduleName, string requestUrlPathAndQuery)
        {
            if (CommonStatistic.IsRun) return;
            CommonStatistic.Init();
            CommonStatistic.CurrentProcess = requestUrlPathAndQuery;
            CommonStatistic.CurrentProcessName = Resource.Admin_ExportFeed_PageSubHeader + " " + moduleName;
            CommonStatistic.StartNew(() => MakeExportFile(new[] { moduleName, PhysicalAppPath }));
        }

        private static void MakeExportFile(string[] myParams)
        {
            try
            {
                var moduleName = myParams[0];
                var applicationPath = myParams[1];

                FileName = ExportFeed.GetModuleSetting(moduleName, "FileName");
                var filePath = applicationPath + "/" + FileName;
                var directory = filePath.Substring(0, filePath.LastIndexOf('/'));


                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //if (File.Exists(filePath))
                //{
                //    File.Delete(filePath);
                //}

                ExportFeedModule exportFeedModule = null;
                switch (moduleName)
                {
                    case "YandexMarket":
                        exportFeedModule = new ExportFeedModuleYandex();
                        break;
                    case "GoogleBase":
                        exportFeedModule = new ExportFeedModuleGoogleBase();
                        break;
                    case "PriceGrabber":
                        exportFeedModule = new ExportFeedModulePriceGrabber();
                        break;
                    case "ShoppingCom":
                        exportFeedModule = new ExportFeedModuleShoppingCom();
                        break;
                    case "YahooShopping":
                        exportFeedModule = new ExportFeedModuleYahooShopping();
                        break;
                    case "Amazon":
                        exportFeedModule = new ExportFeedModuleAmazon();
                        break;
                    case "Shopzilla":
                        exportFeedModule = new ExportFeedModuleShopzilla();
                        break;
                }

                if (exportFeedModule != null)
                {
                    exportFeedModule.GetExportFeedString(filePath);
                }

                var fileInfo = new FileInfo(filePath);

                CommonStatistic.FileName = SettingsMain.SiteUrl + "/" + FileName;
                CommonStatistic.FileSize = " (" + Math.Ceiling(SQLDataHelper.GetDecimal(fileInfo.Length) / 1024) + " Kb)";
            }
            catch (Exception ex)
            {
                AdvantShop.Diagnostics.Debug.LogError(ex, "on MakeExportFile in exportFeed");
            }
            finally
            {
                CommonStatistic.IsRun = false;
            }
        }
    }
}