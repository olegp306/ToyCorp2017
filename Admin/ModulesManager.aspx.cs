//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Trial;
using Resources;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class ModulesManager : AdvantShopAdminPage
    {
        private const int ItemsPerPage = 6;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ModuleManager_Header));
            lTrialMode.Visible = TrialService.IsTrialEnabled;

            if (!string.IsNullOrEmpty(Request["installModule"]))
            {
                ModulesService.InstallModule(SQLDataHelper.GetString(Request["installModule"].ToLower()), Request["version"]);
                Response.Redirect(UrlService.GetAdminAbsoluteLink("modulesmanager.aspx"));
            }

            LoadData();
        }

        protected void lvModules_ItemCommand(object source, ListViewCommandEventArgs e)
        {
            var moduleVersion = ((HiddenField)e.Item.FindControl("hfLastVersion")).Value;
            var moduleIdOnRemoteServer = ((HiddenField)e.Item.FindControl("hfId")).Value;

            if (e.CommandName == "InstallLastVersion")
            {
                var message = ModulesService.GetModuleArchiveFromRemoteServer(moduleIdOnRemoteServer);

                if (message.IsNullOrEmpty())
                {
                    HttpRuntime.UnloadAppDomain();

                    Context.ApplicationInstance.CompleteRequest();
                    Response.Redirect(
                        UrlService.GetAdminAbsoluteLink("modulesmanager.aspx?installModule=" + e.CommandArgument + "&version=" +
                                                        moduleVersion), false);
                }
                else
                {
                    //вывести message
                }
            }
            
            if (e.CommandName == "Uninstall")
            {
                ModulesService.UninstallModule(SQLDataHelper.GetString(e.CommandArgument));
                HttpRuntime.UnloadAppDomain();
                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void LoadData()
        {
            var modulesBox = ModulesService.GetModules();
            paging.TotalPages = (int)Math.Ceiling((double)modulesBox.Items.Count / ItemsPerPage);
            lvModulesManager.DataSource = modulesBox.Items.Skip((paging.CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage);
            lvModulesManager.DataBind();
        }
    }
}