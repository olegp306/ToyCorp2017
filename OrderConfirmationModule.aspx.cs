//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web.UI;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;

namespace ClientPages
{
    public partial class OrderConfirmationModule : AdvantShopClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var ocModule = AttachedModules.GetModules<IOrderConfirmation>().FirstOrDefault();
            IOrderConfirmation classInstance;
            if (ocModule != null)
            {
                classInstance = (IOrderConfirmation)Activator.CreateInstance(ocModule, null);
                if (!ModulesRepository.IsActiveModule(classInstance.ModuleStringId) || !classInstance.CheckAlive())
                {
                    Response.Redirect("~/orderconfirmation.aspx");
                    return;
                }
            }
            else
            {
                Response.Redirect("~/orderconfirmation.aspx");
                return;
            }

            SetMeta(new MetaInfo(string.Format("{0} - {1}", classInstance.PageName, SettingsMain.ShopName)),
                    string.Empty);
            liPageHead.Text = classInstance.PageName;
            Control c =
                (this).LoadControl(
                    UrlService.GetAbsoluteLink(string.Format("/Modules/{0}/{1}", classInstance.ModuleStringId,
                                                             classInstance.FileUserControlOrderConfirmation)));
            if (c != null)
                pnlContent.Controls.Add(c);
        }
    }
}