//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Taxes;
using AdvantShop.Diagnostics;

namespace Admin
{
    public partial class CertificatesOptions : AdvantShopAdminPage
    {
        protected List<int> GiftCertificatePaymentMethods;
        protected List<int> GiftCertificateTaxes;

        private void MsgErr(bool boolClean)
        {
            if (boolClean)
            {
                lblMessage.Visible = false;
                lblMessage.Text = string.Empty;
            }
            else
            {
                lblMessage.Visible = false;
            }
        }

        private void MsgErr(string strMessageText, bool isSucces)
        {
            const string strSuccesFormat = "<div class=\"label-box-admin good\">{0} // at {1}</div>";
            const string strFailFormat = "<div class=\"label-box-admin error\">{0} // at {1}</div>";

            lblMessage.Visible = true;

            if (isSucces)
            {
                lblMessage.Text = string.Format(strSuccesFormat, strMessageText, DateTime.Now.ToString());
            }
            else
            {
                lblMessage.Text = string.Format(strFailFormat, strMessageText, DateTime.Now.ToString());
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1} - {2}", SettingsMain.ShopName, lblHead.Text, lblSubHead.Text));
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            GiftCertificatePaymentMethods = new List<int>(GiftCertificateService.GetCertificatePaymentMethodsID());
            lvPaymentMethods.DataSource = PaymentService.GetAllPaymentMethods(true).Where(payment => payment.Type != PaymentType.GiftCertificate);
            lvPaymentMethods.DataBind();

            GiftCertificateTaxes = TaxServices.GetCertificateTaxes().Select(tax => tax.TaxId).ToList();
            lvTaxes.DataSource = TaxServices.GetTaxes();
            lvTaxes.DataBind();
        }

        protected void btnSaveClick(object sender, EventArgs e)
        {
            MsgErr(true);

            try
            {
                var PaymentMethodsList = new List<int>();
                foreach (ListViewItem item in lvPaymentMethods.Items)
                {
                    int id;
                    if (((CheckBox)item.FindControl("ckbActive")).Checked && Int32.TryParse(((HiddenField)item.FindControl("hfPaymentId")).Value, out id))
                    {
                        PaymentMethodsList.Add(id);
                    }
                }

                GiftCertificateService.SaveCertificatePaymentMethods(PaymentMethodsList);

                var TaxesList = new List<int>();
                foreach (ListViewItem item in lvTaxes.Items)
                {
                    int id;
                    if (((CheckBox)item.FindControl("ckbActive")).Checked && Int32.TryParse(((HiddenField)item.FindControl("hfTaxId")).Value, out id))
                    {
                        TaxesList.Add(id);
                    }
                }
                TaxServices.SaveCertificateTaxes(TaxesList);

                MsgErr(Resources.Resource.Admin_Certificates_Save, true);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                MsgErr("Error: " + ex.Message, false);
            }
        }
    }
}