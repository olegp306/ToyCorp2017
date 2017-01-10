using AdvantShop.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{

    public partial class AlfabankControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtShopId, txtLogin, txtPassword })
                           ? new Dictionary<string, string>
                               {
                                   {"alfabank_shopid", txtShopId.Text},
                                   {"alfabank_login", txtLogin.Text},
                                   {"alfabank_password", txtPassword.Text},
                                   {"alfabank_issandbox", chkIsSandBox.Checked.ToString()}
                               }
                           : null;
            }
            set
            {
                txtShopId.Text = value.ElementOrDefault("alfabank_shopid");
                txtLogin.Text = value.ElementOrDefault("alfabank_login");
                txtPassword.Text = value.ElementOrDefault("alfabank_password");
                chkIsSandBox.Checked = value.ElementOrDefault("alfabank_issandbox").TryParseBool();
            }
        }
    }
}