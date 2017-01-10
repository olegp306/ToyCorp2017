//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Helpers;


namespace ClientPages
{
    public partial class MasterPage : AdvantShopMasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            form.Action = Request.RawUrl;
            SettingsDesign.eMainPageMode currentMode = !Demo.IsDemoEnabled ||
                                                       !CommonHelper.GetCookieString("structure").IsNotEmpty()
                                                           ? SettingsDesign.MainPageMode
                                                           : (SettingsDesign.eMainPageMode)
                                                             Enum.Parse(typeof(SettingsDesign.eMainPageMode),
                                                                        CommonHelper.GetCookieString("structure"));
            if (currentMode == SettingsDesign.eMainPageMode.Default)
            {
                menuTop.Visible = true;
                searchBig.Visible = (SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.TopMenu);
                menuCatalog.Visible = true;
                menuTopMainPage.Visible = false;

                liViewCss.Text = "<link rel=\"stylesheet\" href=\"css/views/default.css\" >";
            }
            else if (currentMode == SettingsDesign.eMainPageMode.TwoColumns)
            {
                menuTop.Visible = false;
                searchBig.Visible = (SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.TopMenu);
                menuCatalog.Visible = false;
                menuTopMainPage.Visible = true;

                liViewCss.Text = "<link rel=\"stylesheet\" href=\"css/views/twocolumns.css\" >";
            }
            else if (currentMode == SettingsDesign.eMainPageMode.ThreeColumns)
            {
                menuTop.Visible = false;
                searchBig.Visible = (SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.TopMenu);
                menuCatalog.Visible = false;
                menuTopMainPage.Visible = true;

                liViewCss.Text = "<link rel=\"stylesheet\" href=\"css/views/threecolumns.css\" >";
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }
    }
}