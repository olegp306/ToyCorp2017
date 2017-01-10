//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;

namespace ClientPages
{
    public partial class SubscribeDeactivate : AdvantShopClientPage
    {
        protected void btnDeactivate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmailAdress.Text))
            {
                ShowMessage(Notify.NotifyType.Error, Resources.Resource.Client_SubscribeDeactivate_NoEmail);
                return;
            }

            if (!SubscriptionService.IsExistsSubscription(txtEmailAdress.Text))
            {
                ShowMessage(Notify.NotifyType.Error, Resources.Resource.Client_SubscribeDeactivate_EmailNotFound);
                return;
            }

            SubscriptionService.Unsubscribe(txtEmailAdress.Text, txtDeactivateReason.Text);

            var modules = AttachedModules.GetModules<ISendMails>().ToArray();
            if (modules.Any())
            {
                foreach (var moduleType in modules)
                {
                    var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                    moduleObject.UnsubscribeEmail(txtEmailAdress.Text);
                }
            }

            MultiView1.SetActiveView(viewMessage);
            lblInfo.Text = Resources.Resource.Client_SubscribeDeactivate_Deactivated;
            lblInfo.Visible = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(
                new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName,
                                           Resources.Resource.Client_SubscribeDeactivate_DeleteSubscribe)), null);
        }
    }
}