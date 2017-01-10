//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Permission;
using AdvantShop.SaasData;

namespace ClientPages
{
    public partial class LicCheck : System.Web.UI.Page
    {
        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (SettingsLic.ActiveLic && SaasDataService.CurrentSaasData.IsCorrect)
                Response.RedirectPermanent("~/");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            hlGo.Visible = false;
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKey.Text))
                return;
            bool res = false;
            try
            {
                res = PermissionAccsess.ActiveLic(txtKey.Text, SettingsMain.SiteUrl, SettingsMain.ShopName,
                                                  SettingsGeneral.SiteVersion, SettingsGeneral.SiteVersionDev);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
            if (res)
            {
                SettingsLic.ActiveLic = true;
                hlGo.Visible = true;
                SettingsLic.LicKey = txtKey.Text;
            }
            else
            {
                SettingsLic.ActiveLic = false;
                lblMsg.Text = @"key is wrong";
            }
        }
    }
}