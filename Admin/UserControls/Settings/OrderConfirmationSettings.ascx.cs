using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Payment;

namespace Admin.UserControls.Settings
{
    public partial class OrderConfirmationSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidOrderConfirmation;
        protected bool IsAdmin = CustomerContext.CurrentCustomer.IsAdmin;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            cbAmountLimitation.Checked = SettingsOrderConfirmation.AmountLimitation;

            cbProceedToPayment.Checked = SettingsOrderConfirmation.ProceedToPayment;

            txtMinimalPrice.Text = SettingsOrderConfirmation.MinimalOrderPrice.ToString("#0.00") ?? "0.00";
            txtMaximalPricecertificate.Text = SettingsOrderConfirmation.MaximalPriceCertificate.ToString("#0.00") ?? "0.00";
            txtMinimalPriceCertificate.Text = SettingsOrderConfirmation.MinimalPriceCertificate.ToString("#0.00") ?? "0.00";

            ckbEnableGiftCertificateService.Checked = SettingsOrderConfirmation.EnableGiftCertificateService;
            ckbDisplayPromoTextbox.Checked = SettingsOrderConfirmation.DisplayPromoTextbox;

            ckbBuyInOneClick.Checked = SettingsOrderConfirmation.BuyInOneClick;
            ckbGoToFinalStep.Checked = SettingsOrderConfirmation.BuyInOneClickGoToFinalStep;
            ckbBuyInOneClickInOrderConfirmation.Checked = SettingsOrderConfirmation.BuyInOneClickInOrderConfirmation;


            txtFirstText.Text = SettingsOrderConfirmation.BuyInOneClickFirstText;
            txtSecondText.Text = SettingsOrderConfirmation.BuyInOneClickFinalText;

            chkShowStatusInfo.Checked = SettingsOrderConfirmation.PrintOrder_ShowStatusInfo;
            chkShowMap.Checked = SettingsOrderConfirmation.PrintOrder_ShowMap;

            rbGoogleMap.Checked = SettingsOrderConfirmation.PrintOrder_MapType == "googlemap";
            rbYandexMap.Checked = SettingsOrderConfirmation.PrintOrder_MapType == "yandexmap";

            if (IsAdmin)
            {
                txtOrderId.Text = (OrderService.GetLastOrderId() + 1).ToString();
            }
        }

        public bool SaveData()
        {
            bool isCorrect = true;

            SettingsOrderConfirmation.BuyInOneClick = ckbBuyInOneClick.Checked;
            SettingsOrderConfirmation.BuyInOneClickGoToFinalStep = ckbGoToFinalStep.Checked;
            SettingsOrderConfirmation.BuyInOneClickFirstText = txtFirstText.Text;
            SettingsOrderConfirmation.BuyInOneClickFinalText = txtSecondText.Text;

            SettingsOrderConfirmation.BuyInOneClickInOrderConfirmation = ckbBuyInOneClickInOrderConfirmation.Checked;

            SettingsOrderConfirmation.AmountLimitation = cbAmountLimitation.Checked;
            SettingsOrderConfirmation.ProceedToPayment = cbProceedToPayment.Checked;

            SettingsOrderConfirmation.PrintOrder_ShowStatusInfo = chkShowStatusInfo.Checked;
            SettingsOrderConfirmation.PrintOrder_ShowMap = chkShowMap.Checked;
            SettingsOrderConfirmation.PrintOrder_MapType = rbGoogleMap.Checked ? "googlemap" : "yandexmap";

            if (SettingsOrderConfirmation.EnableGiftCertificateService != ckbEnableGiftCertificateService.Checked)
            {
                var method = PaymentService.GetPaymentMethodByType(PaymentType.GiftCertificate);
                if (method == null && ckbEnableGiftCertificateService.Checked)
                {
                    PaymentService.AddPaymentMethod(new PaymentGiftCertificate
                        {
                            Enabled = true,
                            Name = Resources.Resource.Client_GiftCertificate,
                            Description = Resources.Resource.Payment_GiftCertificateDescription,
                            SortOrder = 0
                        });
                }
                else if (method != null && !ckbEnableGiftCertificateService.Checked)
                {
                    PaymentService.DeletePaymentMethod(method.PaymentMethodId);
                    SettingsDesign.GiftSertificateVisibility = false;
                }
            }

            SettingsOrderConfirmation.EnableGiftCertificateService = ckbEnableGiftCertificateService.Checked;
            SettingsOrderConfirmation.DisplayPromoTextbox = ckbDisplayPromoTextbox.Checked;

        

            float price = 0;
            if (float.TryParse(txtMaximalPricecertificate.Text, out price) && price >= 0)
            {
                SettingsOrderConfirmation.MaximalPriceCertificate = price;
            }
            else
            {
                ErrMessage += Resources.Resource.Admin_CommonSettings_CertificateMaxPriceError;
                isCorrect = false;
            }

            if (float.TryParse(txtMinimalPriceCertificate.Text, out price) && price >= 0)
            {
                SettingsOrderConfirmation.MinimalPriceCertificate = price;
            }
            else
            {
                ErrMessage += Resources.Resource.Admin_CommonSettings_CertificateMinPriceError;
                isCorrect = false;
            }


            if (float.TryParse(txtMinimalPrice.Text, out price) && price >= 0)
            {
                SettingsOrderConfirmation.MinimalOrderPrice = price;
            }
            else
            {
                ErrMessage += Resources.Resource.Admin_CommonSettings_OrderMinPriceError;
                isCorrect = false;
            }

            LoadData();

            return isCorrect;
        }

        protected void btnChangeOrderNumber_Click(object sender, EventArgs e)
        {
            var newOrderId = txtOrderId.Text.TryParseInt();
            var lastOrderId = OrderService.GetLastOrderId();

            if (newOrderId <= lastOrderId)
            {
                lblOrderSaveResult.Text = Resources.Resource.Admin_CommonSettings_ChangeOrderFail;
                lblOrderSaveResult.CssClass = "error-msg-text";
                return;
            }

            try
            {
                SQLDataAccess.ExecuteNonQuery(
                    "DBCC CHECKIDENT ('Order.Order', RESEED, @OrderId);", CommandType.Text,
                    new SqlParameter("@OrderId", newOrderId));

                lblOrderSaveResult.Text = Resources.Resource.Admin_CommonSettings_ChangeOrderSuccess;
                lblOrderSaveResult.CssClass = "success-msg-text";
            }
            catch (Exception ex)
            {
                lblOrderSaveResult.Text = Resources.Resource.Admin_CommonSettings_ChangeOrderFail;
                lblOrderSaveResult.CssClass = "error-msg-text";
                Debug.LogError(ex);
            }
        }
    }
}