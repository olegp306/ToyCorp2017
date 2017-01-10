//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;
using AdvantShop.Taxes;
using Resources;

namespace ClientPages
{
    public partial class GiftCertificate_Page : AdvantShopClientPage
    {
        protected int OrderId = 0;
        protected string OrderNumber = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SettingsOrderConfirmation.EnableGiftCertificateService)
                Response.Redirect("~/");

            SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_GiftCertificate_Header)), string.Empty);
            liCaptcha.Visible = SettingsMain.EnableCaptcha;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txtEmailFrom.Text = CustomerContext.CurrentCustomer.RegistredUser
                                        ? CustomerContext.CurrentCustomer.EMail
                                        : string.Empty;
            }

            lvPaymentMethods.DataSource = PaymentService.GetCertificatePaymentMethods();
            lvPaymentMethods.DataBind();
        }

        private bool IsValidData()
        {
            bool boolIsValidPast =
                txtFrom.Text.IsNotEmpty()
                && txtTo.Text.IsNotEmpty()
                && ValidationHelper.IsValidEmail(txtEmail.Text)
                && ValidationHelper.IsValidEmail(txtEmailFrom.Text);

            float sum;
            if (!Single.TryParse(txtSum.Text.Trim(), out sum) || sum < SettingsOrderConfirmation.MinimalPriceCertificate ||
                sum > SettingsOrderConfirmation.MaximalPriceCertificate)
            {
                boolIsValidPast = false;
                ShowMessage(Notify.NotifyType.Error, Resource.Client_GiftCertificate_WrongSum);
            }

            int paymentMethodId;
            if (!Int32.TryParse(hfPaymentMethod.Value, out paymentMethodId))
            {
                boolIsValidPast = false;
                ShowMessage(Notify.NotifyType.Error, Resource.Client_GiftCertificate_WrongPaymentMethod);
            }

            if (SettingsMain.EnableCaptcha && !validShield.IsValid())
            {
                ShowMessage(Notify.NotifyType.Error, Resource.Client_GiftCertificate_WrongCode);
                boolIsValidPast = false;
            }
            if (!boolIsValidPast)
                validShield.TryNew();

            return boolIsValidPast;
        }

        protected int CreateCertificateOrder()
        {
            var certificate = new GiftCertificate
                {
                    CertificateCode = GiftCertificateService.GenerateCertificateCode(),
                    ToName = txtTo.Text,
                    FromName = txtFrom.Text,
                    Sum = Convert.ToSingle(txtSum.Text.Trim()),
                    CertificateMessage = txtMessage.Text,
                    Enable = true,
                    ToEmail = txtEmail.Text
                };

            var orderContact = new OrderContact
                {
                    Address = string.Empty,
                    City = string.Empty,
                    Country = string.Empty,
                    Name = string.Empty,
                    Zip = string.Empty,
                    Zone = string.Empty
                };

            var taxes = TaxServices.CalculateCertificateTaxes(certificate.Sum);

            var taxOverPay = taxes.Where(tax => !tax.Key.ShowInPrice).Sum(tax => tax.Value);

            float orderSum = certificate.Sum + taxOverPay;

            var payment = PaymentService.GetPaymentMethod(hfPaymentMethod.Value.TryParseInt());
            float paymentPrice = payment.Extracharge == 0 ? 0 : (payment.ExtrachargeType == ExtrachargeType.Fixed ? payment.Extracharge : payment.Extracharge / 100 * certificate.Sum + taxOverPay);

            var baseCurrency = CurrencyService.BaseCurrency;

            var order = new Order
                {
                    OrderDate = DateTime.Now,
                    OrderCustomer = new OrderCustomer
                        {
                            CustomerID = CustomerContext.CurrentCustomer.Id,
                            Email = txtEmailFrom.Text,
                            FirstName = CustomerContext.CurrentCustomer.FirstName,
                            LastName = CustomerContext.CurrentCustomer.LastName,
                            CustomerIP = HttpContext.Current.Request.UserHostAddress
                        },
                    OrderCurrency = new OrderCurrency
                        {
                            //CurrencyCode = CurrencyService.CurrentCurrency.Iso3,
                            //CurrencyNumCode = CurrencyService.CurrentCurrency.NumIso3,
                            //CurrencyValue = CurrencyService.CurrentCurrency.Value,
                            //CurrencySymbol = CurrencyService.CurrentCurrency.Symbol,
                            //IsCodeBefore = CurrencyService.CurrentCurrency.IsCodeBefore
                            CurrencyCode = baseCurrency.Iso3,
                            CurrencyValue = baseCurrency.Value,
                            CurrencySymbol = baseCurrency.Symbol,
                            CurrencyNumCode = baseCurrency.NumIso3,
                            IsCodeBefore = baseCurrency.IsCodeBefore
                        },
                    OrderStatusId = OrderService.DefaultOrderStatus,
                    AffiliateID = 0,
                    ArchivedShippingName = Resource.Client_GiftCertificate_DeliveryByEmail,
                    PaymentMethodId = Convert.ToInt32(hfPaymentMethod.Value),
                    ArchivedPaymentName = payment.Name,
                    PaymentDetails = null,
                    Sum = orderSum + paymentPrice,
                    PaymentCost = paymentPrice,
                    OrderCertificates = new List<GiftCertificate>
                        {
                            certificate
                        },
                    TaxCost = taxes.Sum(tax => tax.Value),
                    Taxes = taxes.Select(tax => new OrderTax() { TaxID = tax.Key.TaxId, TaxName = tax.Key.Name, TaxShowInPrice = tax.Key.ShowInPrice, TaxSum = tax.Value }).ToList(),
                    ShippingContact = orderContact,
                    BillingContact = orderContact,
                    Number = OrderService.GenerateNumber(1)
                };

            if (order.PaymentMethod.Type == PaymentType.QIWI)
            {
                order.PaymentDetails = new PaymentDetails() { Phone = txtPhone.Text };
            }

            OrderId = order.OrderID = OrderService.AddOrder(order);
            OrderNumber = order.Number = OrderService.GenerateNumber(order.OrderID);
            OrderService.UpdateNumber(order.OrderID, order.Number);
            OrderService.ChangeOrderStatus(order.OrderID, OrderService.DefaultOrderStatus);

            string email = txtEmailFrom.Text;

            string htmlOrderTable = OrderService.GenerateHtmlOrderCertificateTable(order.OrderCertificates,
                                                                                   CurrencyService.CurrentCurrency,
                                                                                   order.PaymentCost, order.TaxCost);

            var orderMailTemplate = new NewOrderMailTemplate(order.OrderID.ToString(), order.Number, email, string.Empty,
                                                             order.ArchivedShippingName,
                                                             order.ArchivedPaymentName, htmlOrderTable,
                                                             CurrencyService.CurrentCurrency.Iso3, order.Sum.ToString(),
                                                             order.CustomerComment,
                                                             OrderService.GetBillingLinkHash(order));
            orderMailTemplate.BuildMail();

            SendMail.SendMailNow(email, orderMailTemplate.Subject, orderMailTemplate.Body, true);
            SendMail.SendMailNow(SettingsMail.EmailForOrders, orderMailTemplate.Subject, orderMailTemplate.Body, true);

            return OrderId;
        }

        protected void btnBuyGiftCertificate_Click(object sender, EventArgs e)
        {
            if (IsValidData())
            {
                var orderid = CreateCertificateOrder();
                if (orderid != 0)
                {
                    Session["orderId"] = OrderId;
                    Response.Redirect("~/orderconfirmation.aspx?tab=FinalTab");
                }
            }
        }
    }
}