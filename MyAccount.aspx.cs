//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdvantShop.BonusSystem;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;
using Resources;

namespace ClientPages
{
    public partial class MyAccount : AdvantShopClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
                Response.Redirect("default.aspx");

            bonusTab.Visible = bonusContent.Visible = BonusSystem.IsActive;

            SetMeta(
                new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_MyAccount_MyAccount)),
                string.Empty);

            foreach (var type in AttachedModules.GetModules<IMyAccountControls>())
            {
                var mac = (IMyAccountControls)Activator.CreateInstance(type, null);
                for (int i = 0; i < mac.Controls.Count; i++)
                {
                    Control c =
                        (this).LoadControl(
                            UrlService.GetAbsoluteLink(string.Format("/Modules/{0}/{1}", mac.ModuleStringId,
                                mac.Controls[i].File)));
                    if (c != null)
                    {
                        var div = new Panel() {CssClass = "tab-content"};
                        div.Attributes.Add("data-tabs-content", "true");
                        div.Controls.Add(c);
                        tabscontents.Controls.Add(div);

                        liDopTabs.Text +=
                            string.Format(
                                "<div class=\"tab-header\" id=\"{0}{1}\" data-tabs-header=\"true\"><span class=\"tab-inside\">{2}</span><span class=\"right\"></span></div>",
                                mac.ModuleStringId.ToLower(), i + 1, mac.Controls[i].NameTab);
                    }
                }
            }
        }
    }
}