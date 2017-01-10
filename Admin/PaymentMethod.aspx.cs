//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Admin.UserControls.PaymentMethods;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Core;
using AdvantShop.Payment;
using AdvantShop.Configuration;
using AdvantShop.Trial;

namespace Admin
{
    public partial class EditPaymentMethod : AdvantShopAdminPage
    {

        private int _paymentMethodId;
        protected int PaymentMethodId
        {
            get
            {
                if (_paymentMethodId != 0)
                    return _paymentMethodId;
                var intval = 0;
                int.TryParse(Request["paymentmethodid"], out intval);
                return intval;
            }

            set { _paymentMethodId = value; }
        }

        protected void Msg(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
        }

        protected void ClearMsg()
        {
            lblMessage.Visible = false;
        }

        protected static readonly Dictionary<PaymentType, string> UcIds = new Dictionary<PaymentType, string>
            {
                {PaymentType.SberBank, "ucSberBank"},
                {PaymentType.Bill, "ucBill"},
                {PaymentType.Cash, "ucCash"},
                {PaymentType.MailRu, "ucMailRu"},
                {PaymentType.WebMoney, "ucWebMoney"},
                {PaymentType.Robokassa, "ucRobokassa"},
                {PaymentType.YandexMoney, "ucYandexMoney"},
                {PaymentType.YandexKassa, "ucYandexKassa"},
                {PaymentType.AuthorizeNet, "ucAuthorizeNet"},
                {PaymentType.GoogleCheckout, "ucGoogleCheckout"},
                {PaymentType.eWAY, "uceWAY"},
                {PaymentType.Check, "ucCheck"},
                {PaymentType.PayPal, "ucPayPal"},
                {PaymentType.TwoCheckout, "ucTwoCheckout"},
                {PaymentType.Alfabank, "ucAlfabank"},
                {PaymentType.Assist, "ucAssist"},
                {PaymentType.ZPayment, "ucZPayment"},
                {PaymentType.Platron, "ucPlatron"},
                {PaymentType.Rbkmoney, "ucRbkmoney"},
                {PaymentType.CyberPlat, "ucCyberPlat"},
                {PaymentType.Moneybookers,"ucMoneybookers"},
                {PaymentType.AmazonSimplePay,"ucAmazonSimplePay"},
                {PaymentType.ChronoPay, "ucChronoPay"},
                {PaymentType.PayOnline, "ucPayOnline"},
                {PaymentType.PSIGate, "ucPSIGate"},
                {PaymentType.PayPoint, "ucPayPoint"},
                {PaymentType.SagePay, "ucSagePay"},
                {PaymentType.WorldPay, "ucWorldPay"},
                {PaymentType.OnPay, "ucOnPay"},
                {PaymentType.PickPoint, "ucPickPoint"},
                {PaymentType.CashOnDelivery, "ucCashOnDelivery"},
                {PaymentType.GiftCertificate, "ucGiftCertificate"},
                {PaymentType.MasterBank, "ucMasterBank"},
                {PaymentType.WalletOneCheckout, "ucWalletOneCheckout"},
                {PaymentType.QIWI, "ucQiwi"},
                {PaymentType.Kupivkredit, "ucKupivkredit"},
                {PaymentType.YesCredit, "ucYesCredit"},
                {PaymentType.Interkassa, "ucInterkassa"},
                {PaymentType.LiqPay, "ucLiqPay"},
                {PaymentType.BillUa, "ucBillUa"},
                {PaymentType.MoscowBank, "ucMoscowBank"},
                {PaymentType.GateLine, "ucGateLine"},
                {PaymentType.Qppi, "ucQppi"},
                {PaymentType.BitPay, "ucBitPay"},
                {PaymentType.IntellectMoney, "ucIntellectMoney"},
                {PaymentType.IntellectMoneyMainProtocol, "ucIntellectMoneyMainProtocol"},
                {PaymentType.Avangard, "ucAvangard"},
                {PaymentType.Dibs, "ucDibs"},
                {PaymentType.RsbCredit, "ucRsbCredit"},
                {PaymentType.DirectCredit, "ucDirectCredit"},
                {PaymentType.PayAnyWay, "ucPayAnyWay"},
                {PaymentType.PayPalExpressCheckout, "ucPayPalExpressCheckout"},
                {PaymentType.Interkassa2, "ucInterkassa2"},
                {PaymentType.MoneXy, "ucMoneXy"},
                {PaymentType.NetPay, "ucNetPay"},
                {PaymentType.AlfabankUa, "ucAlfabankUa"}
            };
        
        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resources.Resource.Admin_PaymentMethod_Header));

            ClearMsg();
            if (!IsPostBack)
                LoadMethods();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            ddlType.DataSource = AdvantshopConfigService.GetDropdownPayments();
            ddlType.DataBind();
        }

        protected void LoadMethods()
        {
            var methods = PaymentService.GetAllPaymentMethods(false).ToList();
            if (methods.Count > 0)
            {
                if (PaymentMethodId == 0)
                    PaymentMethodId = methods.First().PaymentMethodId;
                rptTabs.DataSource = methods;
                rptTabs.DataBind();

            }
            else
                pnEmpty.Visible = true;

            ShowMethod(PaymentMethodId);
        }

        protected void ShowMethod(int methodId)
        {
            var method = PaymentService.GetPaymentMethod(methodId);
            foreach (var ucId in UcIds)
            {
                var uc = (MasterControl)pnMethods.FindControl(ucId.Value);
                if (method == null)
                {
                    uc.Visible = false;
                    continue;
                }
                if (ucId.Key == method.Type)
                    uc.Method = method;
                uc.Visible = ucId.Key == method.Type;
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            var type = (PaymentType)int.Parse(ddlType.SelectedValue);
            var method = PaymentMethod.Create(type);
            method.Name = txtName.Text;
            method.Description = txtDescription.Text;
            if (!string.IsNullOrEmpty(txtSortOrder.Text))
                method.SortOrder = int.Parse(txtSortOrder.Text);
            method.Enabled = type == PaymentType.Cash;
            //Some dirty magic
            if (method.Parameters.ContainsKey(AssistTemplate.CurrencyValue))
            {
                var parameters = method.Parameters;
                parameters[AssistTemplate.CurrencyValue] = "1";
                method.Parameters = parameters;
            }
            //End of dirty magic
            TrialService.TrackEvent(TrialEvents.AddPaymentMethod, method.Type.ToString());
            var id = PaymentService.AddPaymentMethod(method);
            if (id != 0)
                Response.Redirect("~/Admin/PaymentMethod.aspx?PaymentMethodID=" + id);
        }

        protected void PaymentMethod_Saved(object sender, MasterControl.SavedEventArgs args)
        {
            LoadMethods();
            Msg(string.Format(Resources.Resource.Admin_PaymentMethod_Saved, args.Name));
        }

        protected void PaymentMethod_Error(object arg1, MasterControl.ErrorEventArgs arg2)
        {
            Msg(arg2.Message);
        }
    }
}