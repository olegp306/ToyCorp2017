//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using Resources;
using System;

namespace ClientPages
{
    public partial class Subscribe : AdvantShopClientPage
    {
        protected void btnSubscribe_Click(object sender, EventArgs e)
        {
            try
            {
                if (SubscriptionService.IsSubscribe(txtEmail.Text))
                {
                    ShowMessage(Notify.NotifyType.Error, Resource.Client_Subscribe_EmailAlreadyReg);
                    return;
                }

                SubscriptionService.Subscribe(txtEmail.Text);
                
                MultiView1.SetActiveView(ViewEmailSend);

                txtEmail.Text = string.Empty;
                lblInfo.Visible = true;
                lblInfo.Text = Resource.Client_Subscribe_RegSuccess;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName,
                                           Resource.Client_Subscribe_NewSubscribe)), null);

            if (string.IsNullOrEmpty(Request["emailtosubscribe"]))
            {
                return;
            }

            if (!IsPostBack)
            {
                txtEmail.Text = Request["emailtosubscribe"];
                if (ValidationHelper.IsValidEmail(txtEmail.Text))
                {
                    btnSubscribe_Click(sender, e);
                }
            }
        }
    }
}