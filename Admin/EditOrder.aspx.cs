//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Modules;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using Resources;
using AdvantShop.Controls;

namespace Admin
{
    public partial class EditOrder : AdvantShopAdminPage
    {

        protected string OrderNumber
        {
            get { return (string)ViewState["OrderNumber"] ?? string.Empty; }
            set { ViewState["OrderNumber"] = value; }
        }

        protected int OrderID
        {
            get
            {
                if (ViewState["OrderID"] != null)
                {
                    return (int)ViewState["OrderID"];
                }
                return 0;
            }
            set
            {
                ViewState["OrderID"] = value;
            }
        }

        protected bool AddingNewOrder
        {
            get { return (Request["orderid"] != null && Request["orderid"].ToLower() == "addnew"); }
        }

        protected string RenderSplitter()
        {
            var str = new StringBuilder();
            str.Append("<td class=\'splitter\'  onclick=\'togglePanel();return false;\' >");
            str.Append("<div class=\'leftPanelTop\'></div>");
            switch (Resource.Admin_Catalog_SplitterLang)
            {
                case "rus":
                    str.Append("<div id=\'divHide\' class=\'hide_rus\'></div>");
                    str.Append("<div id=\'divShow\' class=\'show_rus\'></div>");
                    break;
                case "eng":
                    str.Append("<div id=\'divHide\' class=\'hide_en\'></div>");
                    str.Append("<div id=\'divShow\' class=\'show_en\'></div>");
                    break;
            }
            str.Append("</td>");
            return str.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1} {2}", SettingsMain.ShopName, Resource.Admin_ViewOrder_ItemNum, OrderID));

            MsgErr.Text = string.Empty;
            //CalendarExtender1.Format = SettingsMain.AdminDateFormat;

            if (!IsPostBack)
            {
                //Check item count for region dropDownList
                ddlCustomerRole.Items.Add(new ListItem(Resource.Admin_ViewCustomer_CustomerRole_User, "0"));
                ddlCustomerRole.Items.Add(new ListItem(Resource.Admin_ViewCustomer_CustomerRole_Moderator, "50"));
                if (CustomerContext.CurrentCustomer.IsAdmin)
                    ddlCustomerRole.Items.Add(new ListItem(Resource.Admin_ViewCustomer_CustomerRole_Administrator, "150"));

                foreach (var group in CustomerGroupService.GetCustomerGroupList())
                {
                    ddlCustomerGroup.Items.Add(new ListItem(string.Format("{0} - {1}%", group.GroupName, group.GroupDiscount), group.CustomerGroupId.ToString()));
                }

                divUseIn1c.Visible = Settings1C.Enabled;

                if (AddingNewOrder)
                {
                    btnSave.Text = Resource.Admin_OrderSearch_AddOrder;
                    btnSaveBottom.Text = Resource.Admin_OrderSearch_AddOrder;
                    cellPrint1.Visible = false;
                    cellPrint2.Visible = false;
                    cellPrint3.Visible = false;
                    cellPrint4.Visible = false;
                    lblOrderID.Text = Resource.Admin_OrderSearch_CreateNew;
                    ddlBillingCountry.DataBind();
                    ddlShippingCountry.DataBind();
                    if (ddlShippingCountry.Items.FindByValue(SettingsMain.SellerCountryId.ToString()) != null)
                        ddlShippingCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();//CountryService.GetCountryIdByIso3(Resource.Admin_Default_CountryISO3).ToString();
                    if (ddlBillingCountry.Items.FindByValue(SettingsMain.SellerCountryId.ToString()) != null)
                        ddlBillingCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();//CountryService.GetCountryIdByIso3(Resource.Admin_Default_CountryISO3).ToString();
                    lblOrderStatus.Text = string.Format("({0})", OrderService.GetStatusName(OrderService.DefaultOrderStatus));
                    lOrderDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
                    txtOrderTime.Text = DateTime.Now.ToString("HH:mm");
                    lCustomerIP.Text = Request.UserHostAddress;

                    chkCopyAddress.Checked = true;
                    txtBillingAddress.Enabled = false;
                    txtBillingCity.Enabled = false;
                    txtBillingName.Enabled = false;
                    txtBillingZip.Enabled = false;
                    txtBillingZone.Enabled = false;
                    ddlBillingCountry.Enabled = false;
                    lblGroupDiscount.Text = "";

                    List<PaymentMethod> listPayments = PaymentService.GetAllPaymentMethods(true).ToList();
                    ddlPaymentMethod.DataSource = listPayments;
                    ddlPaymentMethod.DataBind();

                    var currency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);
                    orderItems.SetCurrency(currency.Iso3, currency.Value, currency.NumIso3, currency.Symbol, currency.IsCodeBefore);

                    if (BonusSystem.IsActive)
                    {
                        bonusPurchaise.Visible = true;
                        useBonuses.Visible = true;
                    }
                }
                else
                {
                    int id;
                    if (!string.IsNullOrEmpty(Request["orderid"]) && int.TryParse(Request["orderid"], out id))
                    {
                        OrderID = id;
                        LoadOrder();
                    }
                    else if (OrderID == 0)
                    {
                        int ordId = OrderService.GetLastOrderId();
                        if (ordId == 0)
                        {
                            OrderID = 0;
                            pnOrder.Visible = false;
                            pnEmpty.Visible = true;
                        }
                        else
                        {
                            OrderID = ordId;
                            LoadOrder();
                        }
                    }
                }
            }
            else
            {
                SetEnabled();
                //CalendarExtender1.SelectedDate = null;
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (PopupGridCustomers.SelectedCustomers != null && PopupGridCustomers.SelectedCustomers.Count > 0)
            {
                LoadCustomer(PopupGridCustomers.SelectedCustomers[0], null);
                PopupGridCustomers.CleanSelection();
            }
        }

        protected void sds_Init(object sender, EventArgs e)
        {
            ((SqlDataSource)sender).ConnectionString = Connection.GetConnectionString();
        }

        private void LoadTotal()
        {
            Order ord = OrderService.GetOrder(OrderID);

            float currencyValue = orderItems.CurrencyValue;
            string currencyCode = orderItems.CurrencyCode;
            float orderDiscount = orderItems.OrderDiscount;
            string currencySymbol = orderItems.CurrencySymbol;
            bool isCodeBefore = orderItems.IsCodeBefore;
            var tempCur = CurrencyService.Currency(orderItems.CurrencyCode);


            lblCurrencySymbol.Text = tempCur != null ? CurrencyService.Currency(orderItems.CurrencyCode).Symbol : @"Get currency error";

            float shippingCost;

            if (float.TryParse(txtShippingPrice.Text, out shippingCost))
            {
                shippingCost = shippingCost * currencyValue;
                lblShippingPrice.Text = string.Format("{0}{1}", shippingCost > 0 ? "+" : "", CatalogService.GetStringPrice(shippingCost, currencyValue, currencyCode));
            }
            else
            {
                lblShippingPrice.Text = string.Format("{0}{1}", shippingCost > 0 ? "+" : "", CatalogService.GetStringPrice(0, currencyValue, currencyCode));
            }

            float taxCost = -0;
            if (ord != null)
            {
                taxCost = ord.Taxes.Where(tax => !tax.TaxShowInPrice).Sum(tax => tax.TaxSum);
                literalTaxCost.Text = TaxServices.BuildTaxTable(ord.Taxes, currencyValue, currencyCode, Resource.Admin_ViewOrder_Taxes);
            }

            float prodTotal = 0;

            if (ord != null && ord.OrderCertificates != null && ord.OrderCertificates.Count > 0)
            {
                prodTotal = ord.OrderCertificates.Sum(item => item.Sum);
            }
            else
            {
                prodTotal = orderItems.OrderItems.Sum(oi => oi.Price * oi.Amount);
            }

            lblTotalOrderPrice.Text = CatalogService.GetStringPrice(prodTotal, currencyValue, currencyCode);

            lblOrderDiscount.Text = string.Format("-{0}", CatalogService.GetStringDiscountPercent(prodTotal, orderDiscount,
                                                                                                  currencyValue, currencySymbol, isCodeBefore,
                                                                                                  CurrencyService.CurrentCurrency.PriceFormat, false));
            trDiscount.Visible = orderDiscount != 0;


            float totalDiscount = 0;
            totalDiscount += orderDiscount > 0 ? orderDiscount * prodTotal / 100 : 0;
            if (ord != null && ord.Certificate != null)
            {
                trCertificatePrice.Visible = ord.Certificate.Price != 0;
                lblCertificatePrice.Text = string.Format("-{0}", CatalogService.GetStringPrice(ord.Certificate.Price));
                totalDiscount += ord.Certificate.Price;
            }

            if (ord != null && ord.Coupon != null)
            {
                float couponValue;
                trCoupon.Visible = ord.Coupon.Value != 0;
                switch (ord.Coupon.Type)
                {
                    case CouponType.Fixed:
                        var productsPrice = orderItems.OrderItems.Where(p => p.IsCouponApplied).Sum(p => p.Price * p.Amount);
                        couponValue = productsPrice >= ord.Coupon.Value ? ord.Coupon.Value : productsPrice;
                        totalDiscount += couponValue;
                        lblCoupon.Text = String.Format("-{0} ({1})", CatalogService.GetStringPrice(couponValue), ord.Coupon.Code);
                        break;
                    case CouponType.Percent:
                        couponValue = orderItems.OrderItems.Where(p => p.IsCouponApplied).Sum(p => ord.Coupon.Value * p.Price / 100 * p.Amount);
                        totalDiscount += couponValue;

                        lblCoupon.Text = String.Format("-{0} ({1}%) ({2})", CatalogService.GetStringPrice(couponValue),
                                                       CatalogService.FormatPriceInvariant(ord.Coupon.Value),
                                                       ord.Coupon.Code);
                        break;
                }
            }

            float orderBonuses = 0;
            if (ord != null)
            {
                orderBonuses = ord.BonusCost;
            }
            else if (AddingNewOrder && BonusSystem.IsActive && chkUseBonuses.Checked)
            {
                var customer = CustomerService.GetCustomer(new Guid(hfContactID.Value));
                if (customer != null)
                {
                    var bonusCard = BonusSystemService.GetCard(customer.BonusCardNumber);
                    if (bonusCard != null && chkUseBonuses.Checked && bonusCard.BonusAmount > 0)
                    {
                        chkUseBonuses.Text = Resource.Admin_EditOrder_UseBonuses +
                                             string.Format(Resource.Admin_EditOrder_UseBonusesHint, bonusCard.BonusAmount);
                        orderBonuses =
                            BonusSystemService.GetBonusCost(prodTotal - totalDiscount + shippingCost, prodTotal - totalDiscount, bonusCard.BonusAmount);
                    }
                }
            }

            lblOrderBonuses.Text = "-" + CatalogService.GetStringPrice(orderBonuses, currencyValue, currencyCode);
            trBonuses.Visible = orderBonuses != 0;

            float paymentCost = 0;
            //if (ord != null)
            //{
            //    paymentCost = ord.PaymentCost;
            //    lblPaymentPrice.Text = (ord.PaymentCost > 0 ? "+" : "") + CatalogService.GetStringPrice(paymentCost);
            //}
            if (float.TryParse(txtPaymentPrice.Text, out paymentCost))
            {
                paymentCost = paymentCost * currencyValue;
                lblPaymentPrice.Text = string.Format("{0}", CatalogService.GetStringPrice(paymentCost, currencyValue, currencyCode));
            }
            else
            {
                lblPaymentPrice.Text = string.Format("+{0}", CatalogService.GetStringPrice(0, currencyValue, currencyCode));
            }


            float sum = taxCost + prodTotal + shippingCost + paymentCost - totalDiscount - orderBonuses;
            lblTotalPrice.Text = CatalogService.GetStringPrice(sum < 0 ? 0 : sum, currencyValue, currencyCode);
            upItems.Update();
        }

        private void LoadOrder()
        {
            //orderList.SelectedOrder = OrderID;

            hlExport.NavigateUrl = "HttpHandlers/ExportOrderExcel.ashx?OrderID=" + OrderID;
            hlExport2.NavigateUrl = "HttpHandlers/ExportOrderExcel.ashx?OrderID=" + OrderID;
            Order ord = OrderService.GetOrder(OrderID);
            if (ord != null)
            {
                lblOrderID.Text = ord.OrderCustomer != null
                                      ? string.Format("{0}{1} - {2} {3}", Resource.Admin_ViewOrder_ItemNum, OrderID,
                                                      ord.OrderCustomer.LastName, ord.OrderCustomer.FirstName)
                                      : string.Format("{0}{1}", Resource.Admin_ViewOrder_ItemNum, OrderID);

                lblGroupDiscount.Text = ord.GroupDiscountString;
                chkCopyAddress.Checked = ord.ShippingContactID == ord.BillingContactID;

                if (ord.OrderCustomer != null)
                {
                    LoadCustomer(ord.OrderCustomer.CustomerID, ord.OrderCustomer);
                    lCustomerIP.Text = ord.OrderCustomer.CustomerIP;
                }

                OrderNumber = ord.Number;
                lOrderDate.Text = ord.OrderDate.ToString("dd.MM.yyy");
                txtOrderTime.Text = ord.OrderDate.ToString("HH:mm");
                lNumber.Text = ord.Number;
                lblOrderStatus.Text = string.Format("({0})", ord.OrderStatus.StatusName);
                if (ord.OrderCurrency != null)
                    orderItems.SetCurrency(ord.OrderCurrency.CurrencyCode, ord.OrderCurrency.CurrencyValue, ord.OrderCurrency.CurrencyNumCode, ord.OrderCurrency.CurrencySymbol, ord.OrderCurrency.IsCodeBefore);
                orderItems.OrderDiscount = ord.OrderDiscount;
                orderItems.GroupDiscount = ord.GroupDiscount;
                orderItems.CouponCode = ord.Coupon != null ? ord.Coupon.Code : null;
                hforderShipName.Value = ord.ArchivedShippingName;

                if (BonusSystem.IsActive)
                {
                    var purchase = BonusSystemService.GetPurchase(ord.Number, ord.OrderID);
                    if (purchase != null)
                    {
                        bonusCardBlock.Visible = true;
                        lblBonusCardNumber.Text = purchase.CardNumber;
                        lblBonusCardAmount.Text = purchase.NewBonusAmount.ToString();
                    }
                }

                if (Settings1C.Enabled && ord.UseIn1C)
                {
                    chkUseIn1C.Checked = true;
                }

                // Billing ------------------------

                var billingCustomerContact = new CustomerContact();
                if (ord.BillingContact != null)
                {
                    billingCustomerContact = new CustomerContact
                        {
                            Name = ord.BillingContact.Name,
                            Address = ord.BillingContact.Address,
                            City = ord.BillingContact.City,
                            Country = ord.BillingContact.Country,
                            RegionName = ord.BillingContact.Zone,
                            Zip = ord.BillingContact.Zip,
                            CustomerGuid = ord.OrderCustomer.CustomerID,
                            CustomField1 = ord.BillingContact.CustomField1,
                            CustomField2 = ord.BillingContact.CustomField2,
                            CustomField3 = ord.BillingContact.CustomField3
                        };
                }

                LoadBilling(billingCustomerContact);
                if (ord.Certificate != null)
                {
                    lCertificateCode.Text = ord.Certificate.Code;
                    pnlCertificateCode.Visible = !string.IsNullOrEmpty(ord.Certificate.Code);
                }

                if (ord.OrderCustomer != null)
                {
                    hfBillingID.Value = (CustomerService.GetContactId(billingCustomerContact) ?? "New");
                }
                else
                {
                    hfBillingID.Value = "New";
                }

                // Shipping ----------------------------------
                //TODO: deal with countries and contacts
                var shippingCustomerContact = new CustomerContact();
                if (ord.ShippingContact != null)
                {
                    shippingCustomerContact = new CustomerContact
                        {
                            Name = ord.ShippingContact.Name,
                            Address = ord.ShippingContact.Address,
                            City = ord.ShippingContact.City,
                            Country = ord.ShippingContact.Country,
                            RegionName = ord.ShippingContact.Zone,
                            Zip = ord.ShippingContact.Zip,
                            CustomerGuid = ord.OrderCustomer == null ? Guid.Empty : ord.OrderCustomer.CustomerID,
                            CustomField1 = ord.ShippingContact.CustomField1,
                            CustomField2 = ord.ShippingContact.CustomField2,
                            CustomField3 = ord.ShippingContact.CustomField3,
                        };
                }

                txtShippingMethod.Text = ord.ArchivedShippingName;
                hfOrderShippingId.Value = ord.ShippingMethodId.ToString();
                if (ord.OrderCurrency != null)
                {
                    txtShippingPrice.Text = (ord.ShippingCost / ord.OrderCurrency.CurrencyValue).ToString("F2");
                    txtPaymentPrice.Text = (ord.PaymentCost / ord.OrderCurrency.CurrencyValue).ToString("F2");
                }

                if (ord.OrderPickPoint != null)
                {

                    ShippingRates.SelectShippingOptionEx = new ShippingOptionEx()
                    {
                        PickpointId = ord.OrderPickPoint.PickPointId,
                        PickpointAddress = ord.OrderPickPoint.PickPointAddress,
                        AdditionalData = ord.OrderPickPoint.AdditionalData
                    };

                    ltPickPointID.Text = ord.OrderPickPoint.PickPointId;
                    ltPickPointAddress.Text = ord.OrderPickPoint.PickPointAddress;

                    if (!string.IsNullOrEmpty(ord.OrderPickPoint.AdditionalData))
                    {
                        hfPickpointAdditional.Value = ord.OrderPickPoint.AdditionalData;
                    }
                }
                else
                {
                    ltPickPointID.Text = "";
                    ltPickPointAddress.Text = "";
                }

                LoadShipping(shippingCustomerContact);

                if (ord.OrderCustomer != null)
                {
                    hfShippingID.Value = (CustomerService.GetContactId(shippingCustomerContact) ?? "New");
                }
                else
                {
                    hfShippingID.Value = "New";
                }

                List<PaymentMethod> listPayments = PaymentService.GetAllPaymentMethods(true).ToList();
                //PaymentMethod cashMethod = new CashOnDelivery(null);

                //if (ord.PaymentMethodId == cashMethod.PaymentMethodID)
                //    listPayments.Add(cashMethod);

                //PaymentMethod pickPointMethod = new PickPoint();

                //if (ord.PaymentMethodId == pickPointMethod.PaymentMethodID)
                //    listPayments.Add(pickPointMethod);

                ddlPaymentMethod.DataSource = listPayments;
                ddlPaymentMethod.DataBind();

                // TODO сделать textbox
                ddlPaymentMethod.SelectedValue = ord.PaymentMethodId.ToString();

                //NOTE: Узкое место. проверить отображение в различных ситуациях
                if (ord.Payed)
                {
                    ddlPaymentMethod.Visible = false;
                    txtPaymentMethod.Text = ord.PaymentMethodName;
                    txtPaymentMethod.Visible = true;
                }
                else
                {
                    ddlPaymentMethod.Visible = true;
                    if (ord.PaymentMethod != null)
                        txtPaymentMethod.Visible = false;
                }
                if (ord.PaymentMethod == null)
                {
                    ddlPaymentMethod.Items.Insert(0, new ListItem(Resource.Admin_NotSet, "0"));
                }

                lblUserComment.Text = string.IsNullOrEmpty(ord.CustomerComment)
                                          ? Resource.Admin_OrderSearch_NoComment
                                          : ord.CustomerComment;
                txtAdminOrderComment.Text = string.Format("{0}", ord.AdminOrderComment);
                txtStatusComment.Text = string.Format("{0}", ord.StatusComment);


                PaperPaymentType pm = ord.PaymentMethod == null ? PaperPaymentType.NonPaperMethod : ord.PaymentMethod.Type.ToEnum<PaperPaymentType>();

                var printButtonText = new Dictionary<PaperPaymentType, string>
                    {
                        {PaperPaymentType.SberBank, Resource.Client_OrderConfirmation_PrintLuggage},
                        {PaperPaymentType.Bill, Resource.Client_OrderConfirmation_PrintBill},
                        {PaperPaymentType.Check, Resource.Client_OrderConfirmation_PrintCheck},
                        {PaperPaymentType.BillUa, Resource.Client_OrderConfirmation_PrintBill},
                    };

                paymentDetails.Visible = false;
                btnPrintPaymentDetails.Visible = false;

                if (pm != PaperPaymentType.NonPaperMethod)
                {
                    if (pm == PaperPaymentType.SberBank)
                    {
                        LocalizeClient_OrderConfirmation_OrganizationName.Text = Resource.Admin_EditOrder_CustomerName;
                    }
                    printPaymentDetails.Visible = true;
                    btnPrintPaymentDetails.Visible = true;
                    btnPrintPaymentDetails.Value = printButtonText[pm];

                    btnPrintPaymentDetails.Attributes.Add("onclick",
                                                          string.Format(
                                                              "javascript:open_printable_version(\'../Check_{0}.aspx?ordernumber={1}{2}",
                                                              pm, ord.Number,
                                                              pm != PaperPaymentType.Check
                                                                  ? (string.Format(
                                                                      "&bill_CompanyName=\' + escape(document.getElementById(\'{0}\').value) + \'&bill_INN=\' + escape(document.getElementById(\'{1}\').value));",
                                                                      txtCompanyName.ClientID, txtINN.ClientID))
                                                                  : "\');"));

                    if (pm == PaperPaymentType.Bill || pm == PaperPaymentType.SberBank)
                    {
                        paymentDetails.Visible = true;
                        btnPrintPaymentDetails.Visible = true;
                        if (ord.PaymentDetails != null)
                        {
                            txtCompanyName.Text = ord.PaymentDetails.CompanyName;
                            txtINN.Text = ord.PaymentDetails.INN;
                        }
                    }
                    else if (pm == PaperPaymentType.BillUa)
                    {
                        paymentDetails.Visible = true;
                    }
                }
                else if (ord.PaymentMethod is Qiwi)
                {
                    printPaymentDetails.Visible = true;
                    qiwiPanel.Visible = true;
                    if (ord.PaymentDetails != null)
                    {
                        txtPhoneQiwi.Text = ord.PaymentDetails.Phone;
                    }
                }


                if (ord.OrderCertificates == null || ord.OrderCertificates.Count == 0)
                {
                    orderItems.OrderItems = (List<OrderItem>)ord.OrderItems;
                }
                else
                {
                    orderCertificates.Certificates = ord.OrderCertificates;
                    orderCertificates.OrderCurrency = ord.OrderCurrency;
                    orderItems.Visible = false;
                }

                LoadTotal();


                if (ord.PaymentDate != null)
                {
                    pnlOderContent.Enabled = false;
                    lOrderContent.Visible = true;
                }

                pnEmpty.Visible = false;
                pnOrder.Visible = true;
            }
            else
            {
                Response.Redirect("OrderSearch.aspx");
                pnEmpty.Visible = true;
                lblNotFound.Text = @"Not found";
            }
            UpdatePanel1.Update();
        }

        private void LoadCustomer(Guid customerId, OrderCustomer orderCustomer)
        {
            hfContactID.Value = customerId.ToString();
            var customer = CustomerService.GetCustomer(customerId);

            if (orderCustomer != null)
            {
                txtOrderLastName.Text = orderCustomer.LastName;
                txtOrderFirstName.Text = orderCustomer.FirstName;
                txtOrderEmail.Text = orderCustomer.Email;
                txtOrderMobilePhone.Text = orderCustomer.MobilePhone;
            }
            else if (customer != null)
            {
                txtOrderLastName.Text = customer.LastName;
                txtOrderFirstName.Text = customer.FirstName;
                txtOrderEmail.Text = customer.EMail;
                txtOrderMobilePhone.Text = customer.Phone;
            }

            if (customer != null && customer.RegistredUser)
            {
                hlCustomer.Text = string.Format("{0} {1} - {2} - {3}", customer.FirstName, customer.LastName, customer.EMail, customer.Phone);
                hlCustomer.NavigateUrl = "ViewCustomer.aspx?CustomerID=" + customerId;
                hlCustomer.Visible = true;

                lblGroupDiscount.Text = customer.CustomerGroup != null ? customer.CustomerGroup.GroupName : string.Empty;
                lblChosingCustomer.Visible = false;
            }
            else
            {
                lblCustomer.Text = string.Format("{0} {1} - {2} - {3}", orderCustomer.FirstName, orderCustomer.LastName, orderCustomer.Email, orderCustomer.MobilePhone);
                lblCustomer.Visible = true;
                lblChosingCustomer.Visible = false;
                lblChosingCustomer.Visible = false;
            }

            LoadContacts(customerId, false);
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            var html = new StringBuilder();
            html.Append("<ul>");

            foreach (EvaluatedCustomOptions ev in evlist)
            {
                html.Append(string.Format("<li>{0}: {1}</li>", ev.CustomOptionTitle, ev.OptionTitle));
            }

            html.Append("</ul>");
            return html.ToString();
        }

        protected void SqlDataSource1_Init(object sender, EventArgs e)
        {
            SqlDataSource1.ConnectionString = Connection.GetConnectionString();
        }

        public string RenderDivHeader()
        {
            string divHeader;
            if (Request.Browser.Browser == "IE")
            {
                var c = new CultureInfo("en-us");
                divHeader = double.Parse(Request.Browser.Version, c.NumberFormat) < 7
                                ? "<div class=\'mtree_ie6\'>"
                                : "<div class=\'mtree_ie\'>";
            }
            else
            {
                divHeader = "<div class=\'mtree\'>";
            }
            return divHeader;
        }

        public string RenderDivBottom()
        {
            return "</div>";
        }

        protected void FillView(CustomerContact contact)
        {
            if (contact != null)
            {
                LoadBilling(contact);
                LoadShipping(contact);
            }
            else
            {
                CleanBilling();
                CleanShipping();
            }
        }

        private void LoadBilling(CustomerContact contact)
        {
            hfBillingID.Value = contact.CustomerContactID.ToString();
            txtBillingAddress.Text = HttpUtility.HtmlDecode(contact.Address);
            txtBillingCity.Text = HttpUtility.HtmlDecode(contact.City);
            txtBillingName.Text = HttpUtility.HtmlDecode(contact.Name);
            txtBillingZip.Text = HttpUtility.HtmlDecode(contact.Zip);

            txtBillingCustomField1.Text = HttpUtility.HtmlDecode(contact.CustomField1);
            txtBillingCustomField2.Text = HttpUtility.HtmlDecode(contact.CustomField2);
            txtBillingCustomField3.Text = HttpUtility.HtmlDecode(contact.CustomField3);

            ddlBillingCountry.DataBind();
            if (!string.IsNullOrEmpty(contact.Country))
            {
                ListItem temp = ddlBillingCountry.Items.FindByText(contact.Country);
                if (temp != null)
                {
                    ddlBillingCountry.SelectedValue = temp.Value;
                }
                else
                {
                    ddlBillingCountry.Items.Add(new ListItem(contact.Country, "0"));
                    ddlBillingCountry.SelectedValue = "0";
                }
            }
            else if (ddlBillingCountry.Items.FindByValue(SettingsMain.SellerCountryId.ToString()) != null)
            {
                ddlBillingCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();
            }

            txtBillingZone.Text = HttpUtility.HtmlDecode(contact.RegionName);
            SetEnabled();
        }

        private void SetEnabled()
        {
            txtBillingAddress.Enabled = !chkCopyAddress.Checked;
            txtBillingCity.Enabled = !chkCopyAddress.Checked;
            txtBillingName.Enabled = !chkCopyAddress.Checked;
            txtBillingZip.Enabled = !chkCopyAddress.Checked;
            txtBillingZone.Enabled = !chkCopyAddress.Checked;
            ddlBillingCountry.Enabled = !chkCopyAddress.Checked;
            txtBillingCustomField1.Enabled = !chkCopyAddress.Checked;
            txtBillingCustomField2.Enabled = !chkCopyAddress.Checked;
            txtBillingCustomField3.Enabled = !chkCopyAddress.Checked;
        }

        private void CleanBilling()
        {
            hfBillingID.Value = string.Empty;
            txtBillingAddress.Text = string.Empty;
            txtBillingCity.Text = string.Empty;
            txtBillingName.Text = string.Empty;
            txtBillingZip.Text = string.Empty;

            txtBillingCustomField1.Text = string.Empty;
            txtBillingCustomField2.Text = string.Empty;
            txtBillingCustomField3.Text = string.Empty;

            if (ddlBillingCountry.Items.FindByValue(SettingsMain.SellerCountryId.ToString()) != null)
                ddlBillingCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();//CountryService.GetCountryIdByIso3(Resource.Admin_Default_CountryISO3).ToString();
        }

        private void LoadShipping(CustomerContact contact)
        {
            hfShippingID.Value = contact.CustomerContactID.ToString();
            txtShippingAddress.Text = HttpUtility.HtmlDecode(contact.Address);
            txtShippingCity.Text = HttpUtility.HtmlDecode(contact.City);
            txtShippingName.Text = HttpUtility.HtmlDecode(contact.Name);
            txtShippingZip.Text = HttpUtility.HtmlDecode(contact.Zip);

            txtShippingCustomField1.Text = HttpUtility.HtmlDecode(contact.CustomField1);
            txtShippingCustomField2.Text = HttpUtility.HtmlDecode(contact.CustomField2);
            txtShippingCustomField3.Text = HttpUtility.HtmlDecode(contact.CustomField3);


            ddlShippingCountry.DataBind();
            if (!string.IsNullOrEmpty(contact.Country))
            {
                ListItem item = ddlShippingCountry.Items.FindByText(contact.Country);
                if (item != null)
                {
                    ddlShippingCountry.SelectedValue = item.Value;
                }
                else
                {
                    ddlShippingCountry.Items.Add(new ListItem(contact.Country, "0"));
                    ddlShippingCountry.SelectedValue = "0";
                }
            }
            else if (ddlShippingCountry.Items.FindByValue(SettingsMain.SellerCountryId.ToString()) != null)
            {
                ddlShippingCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();
            }
            txtShippingZone.Text = HttpUtility.HtmlDecode(contact.RegionName);

            var address = string.Empty;
            address += contact.Country + ",";
            address += contact.RegionName + ",";
            address += contact.City + ",";
            address += contact.Address;

            lnkMap.NavigateUrl = "http://maps.yandex.ru/?text=" + HttpUtility.UrlEncode(address);
        }

        private void CleanShipping()
        {
            hfShippingID.Value = string.Empty;
            txtShippingAddress.Text = string.Empty;
            txtShippingCity.Text = string.Empty;
            txtShippingName.Text = string.Empty;
            txtShippingZip.Text = string.Empty;
            if (ddlShippingCountry.Items.FindByValue(SettingsMain.SellerCountryId.ToString()) != null)
                ddlShippingCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();//CountryService.GetCountryIdByIso3(Resource.Admin_Default_CountryISO3).ToString();

            txtShippingZone.Text = string.Empty;

            txtShippingCustomField1.Text = string.Empty;
            txtShippingCustomField2.Text = string.Empty;
            txtShippingCustomField3.Text = string.Empty;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfContactID.Value))
            {
                msgErr(Resource.Admin_ViewOrder_NoUserError);
                return;
            }
            try
            {
                new Guid(hfContactID.Value);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                msgErr(Resource.Admin_ViewOrder_NoUserError);
                return;
            }

            try
            {
                Decimal.Parse(txtShippingPrice.Text);
            }
            catch (Exception)
            {
                //if error we don't save
                msgErr(Resource.Admin_OrderSearch_ErrorParseShipping);
                return;
            }

            int shipId = 0;
            if (!string.IsNullOrWhiteSpace(hfOrderShippingId.Value))
            {
                shipId = Convert.ToInt32(hfOrderShippingId.Value);
                var shippingMethod = ShippingMethodService.GetShippingMethod(shipId);
                if (shippingMethod == null && !AddingNewOrder)
                {
                    msgErr(Resource.Admin_OrderSearch_SelectShippingMethod);
                    return;
                }
            }
            else
            {
                msgErr(Resource.Admin_OrderSearch_SelectShippingMethod);
                return;
            }

            if (orderItems.OrderItems.Count == 0)
            {
                msgErr(Resource.Admin_EditOrder_NoOrderItems);
                return;
            }

            if (ddlPaymentMethod.SelectedValue == "0")
            {
                msgErr(Resource.Admin_EditOrder_SelectPayment);
                return;
            }

            // -- Order starting here
            bool shippingRefresh;
            Order order = BuildOrder(out shippingRefresh, shipId);
            if (order == null)
            {
                msgErr("Order ID invalid");
                return;
            }
            if (AddingNewOrder)
            {
                CreateOrder(order);
            }
            else
            {
                SaveOrder(order, shippingRefresh);
            }

            if (AddingNewOrder)
            {
                Response.Redirect("EditOrder.aspx?OrderID=" + order.OrderID);
            }
            else
            {
                LoadOrder();
            }
        }

        private void CreateOrder(Order order)
        {
            Customer customer = CustomerService.GetCustomer(new Guid(hfContactID.Value));
            //проверка на null
            order.OrderCustomer = new OrderCustomer
                {
                    CustomerID = new Guid(hfContactID.Value),
                    CustomerIP = Request.UserHostAddress,
                    FirstName = txtOrderFirstName.Text,
                    LastName = txtOrderLastName.Text,
                    Email = txtOrderEmail.Text,
                    MobilePhone = txtOrderMobilePhone.Text
                };
            order.GroupName = customer.CustomerGroup != null ? customer.CustomerGroup.GroupName : string.Empty;

            BonusCard bonusCard = null;
            if (AddingNewOrder && BonusSystem.IsActive)
            {
                bonusCard = BonusSystemService.GetCard(customer.BonusCardNumber);
                if (bonusCard != null && chkUseBonuses.Checked && bonusCard.BonusAmount > 0)
                {
                    var totalPrice = orderItems.OrderItems.Sum(oi => oi.Price * oi.Amount);
                    var discount = orderItems.OrderDiscount > 0 ? orderItems.OrderDiscount * totalPrice / 100 : 0;

                    order.BonusCost =
                        BonusSystemService.GetBonusCost(totalPrice - discount + order.ShippingCost, totalPrice - discount, bonusCard.BonusAmount);
                }
            }

            order.OrderStatusId = OrderService.DefaultOrderStatus;
            order.OrderDate = DateTime.Now;
            order.AffiliateID = 0;
            order.Number = OrderService.GenerateNumber(1); // For crash protection
            order.OrderID = OrderService.AddOrder(order);
            if (order.OrderID == 0)
            {
                msgErr(Resource.Admin_ViewOrder_CreateError);
                return;
            }
            order.Number = OrderService.GenerateNumber(order.OrderID); // new number
            OrderService.UpdateNumber(order.OrderID, order.Number);
            OrderID = order.OrderID;
            SaveOrderCart(order.OrderID);

            ModulesRenderer.OrderAdded(order.OrderID);

            float prodTotal = orderItems.OrderItems.Sum(oi => oi.Price * oi.Amount);
            float totalDiscount = orderItems.OrderDiscount > 0 ? orderItems.OrderDiscount * prodTotal / 100 : 0;

            var orderTable = OrderService.GenerateHtmlOrderTable(orderItems.OrderItems, CurrencyService.CurrentCurrency,
                                                                 prodTotal, order.OrderDiscount, order.Coupon,
                                                                 order.Certificate, totalDiscount, order.ShippingCost,
                                                                 order.PaymentCost, order.TaxCost, order.BonusCost, 0);

            var orderMailTemplate = new NewOrderMailTemplate(order.OrderID.ToString(), order.Number,
                order.OrderCustomer.Email,
                BuildCustomerContacts(customer, order.ShippingContact),
                order.ArchivedShippingName +
                (order.OrderPickPoint != null && !String.IsNullOrWhiteSpace(order.OrderPickPoint.PickPointAddress)
                    ? string.Format(" ({0})", order.OrderPickPoint.PickPointAddress)
                    : string.Empty),
                order.PaymentMethodName,
                orderTable, order.OrderCurrency.CurrencyCode,
                order.Sum.ToString(), order.CustomerComment,
                OrderService.GetBillingLinkHash(order));
            orderMailTemplate.BuildMail();

            SendMail.SendMailNow(customer.EMail, orderMailTemplate.Subject, orderMailTemplate.Body, true);
            SendMail.SendMailNow(SettingsMail.EmailForOrders, orderMailTemplate.Subject, orderMailTemplate.Body, true);

            if (AddingNewOrder && BonusSystem.IsActive && chkMakePurchaise.Checked && bonusCard != null)
            {
                var sumPrice = BonusSystem.BonusType == BonusSystem.EBonusType.ByProductsCostWithShipping
                    ? prodTotal - totalDiscount + order.ShippingCost
                    : prodTotal - totalDiscount;

                BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, sumPrice, order.BonusCost, order.Number, order.OrderID);
            }
        }

        private static string BuildCustomerContacts(Customer customer, OrderContact contact)
        {
            var customerSb1 = new StringBuilder();

            customerSb1.AppendFormat(Resource.Client_Registration_Name + " {0}<br/>", customer.FirstName);
            customerSb1.AppendFormat(Resource.Client_Registration_Surname + " {0}<br/>", customer.LastName);
            customerSb1.AppendFormat(Resource.Client_Registration_Country + " {0}<br/>", contact.Country);
            customerSb1.AppendFormat(Resource.Client_Registration_State + " {0}<br/>", contact.Zone);
            customerSb1.AppendFormat(Resource.Client_Registration_City + " {0}<br/>", contact.City);
            customerSb1.AppendFormat(Resource.Client_Registration_Zip + " {0}<br/>", contact.Zip);
            customerSb1.AppendFormat(Resource.Client_Registration_Address + ": {0}<br/>", string.IsNullOrEmpty(contact.Address)
                                                                                              ? Resource.Client_OrderConfirmation_NotDefined
                                                                                              : contact.Address);
            return customerSb1.ToString();
        }


        private void SaveOrder(Order order, bool shippingRefresh)
        {
            order.OrderID = Convert.ToInt32(OrderID);
            order.Number = OrderNumber;

            // -- Save main info
            OrderService.UpdateOrderMain(order);

            if (order.OrderPickPoint != null && order.OrderPickPoint.PickPointId == "delete")
            {
                OrderService.DeleteOrderPickPoint(order.OrderID);
            }
            else if (order.OrderPickPoint != null)
            {
                OrderService.AddUpdateOrderPickPoint(order.OrderID, order.OrderPickPoint);
            }
            else
            {
                OrderService.DeleteOrderPickPoint(order.OrderID);
            }

            OrderID = order.OrderID;
            // -- Order contacts

            OrderService.UpdateOrderContacts(order.OrderID, order.ShippingContact, order.BillingContact);
            // -- Order currency
            OrderService.UpdateOrderCurrency(order.OrderID, order.OrderCurrency.CurrencyCode, order.OrderCurrency.CurrencyValue);

            OrderService.UpdateOrderCustomer(order.OrderCustomer);
            // -- Ordered items

            OrderService.UpdatePaymentDetails(order.OrderID, order.PaymentDetails);

            shippingRefresh |= orderItems.OrderItems.AggregateHash() != order.OrderItems.AggregateHash();

            SaveOrderCart(order.OrderID, order.OrderItems, order.OrderStatus);

            ModulesRenderer.OrderUpdated(OrderID);

            if (shippingRefresh)
                modalRecheckShipping.Show();
        }


        private Order BuildOrder(out bool shippingRefresh, int shippingMethodId)
        {

            Order order = AddingNewOrder ? new Order() : OrderService.GetOrder(OrderID);
            order.PaymentMethodId = Convert.ToInt32(ddlPaymentMethod.SelectedValue);
            order.ShippingMethodId = shippingMethodId;
            order.AdminOrderComment = txtAdminOrderComment.Text;
            order.StatusComment = txtStatusComment.Text;
            order.ShippingCost = txtShippingPrice.Text.TryParseFloat() * orderItems.CurrencyValue;


            var payment = PaymentService.GetPaymentMethod(order.PaymentMethodId);
            if (payment != null)
            {
                txtPaymentPrice.Text = (payment.ExtrachargeType == ExtrachargeType.Fixed
                                           ? payment.Extracharge
                                           : payment.Extracharge / 100 *
                                                (orderItems.OrderItems.Sum(item => item.Price * item.Amount) -
                                                (orderItems.OrderItems.Sum(item => item.Price * item.Amount) / 100 * orderItems.OrderDiscount)
                                                - order.BonusCost + order.ShippingCost
                                                + order.Taxes.Where(tax => !tax.TaxShowInPrice).Sum(tax => tax.TaxSum))).ToString("F2");
            }

            order.PaymentCost = txtPaymentPrice.Text.TryParseFloat() * orderItems.CurrencyValue;

            if (order.PaymentDetails == null)
                order.PaymentDetails = new PaymentDetails();

            order.PaymentDetails.CompanyName = txtCompanyName.Text;
            order.PaymentDetails.INN = txtINN.Text;
            order.PaymentDetails.Phone = txtPhoneQiwi.Text;

            order.UseIn1C = Settings1C.Enabled && chkUseIn1C.Checked;

            order.ArchivedShippingName = txtShippingMethod.Text;

            if (!string.IsNullOrEmpty(ltPickPointID.Text) || !string.IsNullOrEmpty(ltPickPointAddress.Text) || !string.IsNullOrEmpty(hfPickpointAdditional.Value))
            {
                order.OrderPickPoint = new OrderPickPoint
                {
                    PickPointId = ltPickPointID.Text,
                    PickPointAddress = ltPickPointAddress.Text.IsNotEmpty() ? ltPickPointAddress.Text : txtShippingAddress.Text
                };

                if (!string.IsNullOrEmpty(hfPickpointAdditional.Value))
                {
                    order.OrderPickPoint.AdditionalData = hfPickpointAdditional.Value;
                }
            }
            else
            {
                order.OrderPickPoint = new OrderPickPoint { PickPointId = "delete" };
            }

            order.OrderCurrency = new OrderCurrency
                {
                    CurrencyCode = orderItems.CurrencyCode,
                    CurrencyValue = orderItems.CurrencyValue,
                    CurrencyNumCode = orderItems.CurrencyNumCode,
                    CurrencySymbol = orderItems.CurrencySymbol
                };
            order.OrderDiscount = orderItems.OrderDiscount;

            var shippingContact = new OrderContact
                {
                    Address = txtShippingAddress.Text,
                    City = txtShippingCity.Text,
                    Country = ddlShippingCountry.SelectedItem.Text,
                    Name = txtShippingName.Text,
                    Zip = txtShippingZip.Text,
                    Zone = txtShippingZone.Text,
                    CustomField1 = txtShippingCustomField1.Text,
                    CustomField2 = txtShippingCustomField2.Text,
                    CustomField3 = txtShippingCustomField3.Text,
                };
            shippingRefresh = !AddingNewOrder && !ContactChanged(order.ShippingContact, shippingContact);

            order.ShippingContact = shippingContact;
            order.BillingContact = chkCopyAddress.Checked
                                       ? order.ShippingContact
                                       : new OrderContact
                                           {
                                               Address = txtBillingAddress.Text,
                                               City = txtBillingCity.Text,
                                               Country = ddlBillingCountry.SelectedItem.Text,
                                               Name = txtBillingName.Text,
                                               Zip = txtBillingZip.Text,
                                               Zone = txtBillingZone.Text
                                           };

            order.OrderDate = GetDate();

            if (order.OrderCustomer == null)
            {
                order.OrderCustomer = new OrderCustomer();
            }

            order.OrderCustomer.CustomerID = new Guid(hfContactID.Value);
            order.OrderCustomer.FirstName = txtOrderFirstName.Text;
            order.OrderCustomer.LastName = txtOrderLastName.Text;
            order.OrderCustomer.Email = txtOrderEmail.Text;
            order.OrderCustomer.MobilePhone = txtOrderMobilePhone.Text;

            orderItems.SetCustomerDiscount(order.OrderCustomer.CustomerID);

            return order;
        }

        private bool ContactChanged(OrderContact shippingContact, OrderContact orderContact)
        {
            if (shippingContact == null || orderContact == null)
                return true;

            return shippingContact.Country == orderContact.Country
                   && shippingContact.City == orderContact.City
                   && shippingContact.Zip == orderContact.Zip;
        }

        protected void agv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectContact")
            {
                string[] values = ((string)e.CommandArgument).Split('^');
                SelectCustomer(values[0].TryParseGuid(), values[1], true);
            }
        }

        protected void SelectCustomer(Guid customerID, string customerEmail, bool fillView)
        {
            hfContactID.Value = customerID.ToString();
            lblChosingCustomer.Text = customerEmail;
            LoadContacts(customerID, fillView);
        }

        private void LoadContacts(Guid customerID, bool fillView)
        {
            List<CustomerContact> contacts = CustomerService.GetCustomerContacts(customerID);
            if (fillView)
            {
                FillView(contacts.Count > 0 ? contacts.First() : null);
            }

            if (contacts.Count == 0)
            {
                ErrMes.Text = string.Empty;
                ErrMes.Visible = true;
            }

            CustomerContacts.Items.Clear();
            var liNew = new ListItem
                {
                    Value = "New",
                    Text = Resource.Admin_OrderSearch_NewAddress
                };

            CustomerContacts.Items.Add(liNew);
            const string format = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{0}</b>&nbsp;{1}<br />";
            foreach (var customerRow in contacts)
            {
                var liText = new StringBuilder();
                liText.AppendFormat(
                    "&nbsp;<b>{0}:</b>&nbsp;{1}<br />",
                    Resource.Admin_ViewCustomer_ContactPerson, customerRow.Name);
                liText.AppendFormat(format,
                                    Resource.Admin_ViewCustomer_ContactCountry, customerRow.Country);
                liText.AppendFormat(format,
                                    Resource.Admin_ViewCustomer_ContactCity, customerRow.City);

                if (!string.IsNullOrEmpty(customerRow.RegionName.Trim()))
                {
                    liText.AppendFormat(format,
                                        Resource.Admin_ViewCustomer_ContactZone, customerRow.RegionName);
                }

                if (!string.IsNullOrEmpty(customerRow.Zip.Trim()))
                {
                    liText.AppendFormat(format,
                                        Resource.Admin_ViewCustomer_ContactZip, customerRow.Zip);
                }
                liText.AppendFormat(format,
                                    Resource.Admin_ViewCustomer_ContactAddress, customerRow.Address);

                var li = new ListItem { Text = liText.ToString(), Value = customerRow.CustomerContactID.ToString() };
                CustomerContacts.Items.Add(li);
            }

            var customer = CustomerService.GetCustomer(customerID);
            if (customer != null)
            {
                var group = CustomerGroupService.GetCustomerGroup(customer.CustomerGroupId);
                // Апдейтим имя группы и скидку группы
                Order order = OrderService.GetOrder(OrderID);
                if (order != null)
                {
                    order.GroupName = group.GroupName;
                    order.GroupDiscount = group.GroupDiscount;
                    OrderService.UpdateOrderMain(order);
                }
                orderItems.SetCustomerDiscount(customerID);
            }
            UpdatePanel4.Update();
        }

        private void msgErr_createUser(string messageText)
        {
            pnlMsgErr.Visible = true;
            Message.Text = "<br/>" + messageText;
        }

        private void msgErr(string messageText)
        {
            pnlMsgErr.Visible = true;
            MsgErr.Text = "<br/>" + messageText;
        }

        protected void btnCreateUser_Click(object sender, EventArgs e)
        {
            if (ValidateCustomer())
            {
                var customerId = CustomerService.InsertNewCustomer(new Customer
                    {
                        Password = txtPassword.Text,
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        Phone = txtPhone.Text,
                        SubscribedForNews = chkSubscribed4News.Checked,
                        EMail = txtEmail.Text.Trim(),
                        CustomerRole = (Role)SQLDataHelper.GetInt(ddlCustomerRole.SelectedValue),
                        CustomerGroupId = SQLDataHelper.GetInt(ddlCustomerGroup.SelectedValue)
                    });
                if (!customerId.Equals(Guid.Empty))
                {
                    ClearCustomer();
                    var customer = CustomerService.GetCustomer(customerId);
                    //SelectCustomer(customer.Id, customer.EMail, true);
                    LoadCustomer(customer.Id, new OrderCustomer
                        {
                            CustomerID = customerId,
                            Email = customer.EMail,
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            MobilePhone = customer.Phone,
                            OrderID = OrderID
                        });

                    ModalPopupExtender2.Hide();
                    UpdatePanel1.Update();

                    // Bind user grid
                    hfBillingID.Value = "New";
                    hfShippingID.Value = "New";
                }
                else
                {
                    msgErr_createUser(Resource.Admin_ViewOrder_UserCreateError);
                    //bad thing happens. notify user about this
                }
            }
            else
            {
                msgErr_createUser(Resource.Admin_ViewOrder_PwdConfirmError);
            }
        }

        private void ClearCustomer()
        {
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtPasswordConfirm.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            chkSubscribed4News.Checked = false;
        }

        private bool ValidateCustomer()
        {
            bool boolIsValidPast = true;

            ulUserRegistarionValidation.InnerHtml = "";

            // ------------------------------------------------------

            string email = txtEmail.Text.Trim();
            if ((!string.IsNullOrEmpty(email)) && ValidationHelper.IsValidEmail(email) && (!CustomerService.ExistsEmail(email)))
            {
                txtEmail.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtEmail.CssClass = "OrderConfirmation_InvalidTextBox";
                boolIsValidPast = false;
            }

            if (!string.IsNullOrEmpty(txtPassword.Text) && txtPassword.Text.Length > 3)
            {
                txtPassword.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtPassword.CssClass = "OrderConfirmation_InvalidTextBox";
                boolIsValidPast = false;
            }

            if (string.IsNullOrEmpty(txtPasswordConfirm.Text) == false)
            {
                txtPasswordConfirm.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtPasswordConfirm.CssClass = "OrderConfirmation_InvalidTextBox";
                boolIsValidPast = false;
            }

            if ((string.IsNullOrEmpty(txtPasswordConfirm.Text) == false) &&
                (string.IsNullOrEmpty(txtPassword.Text) == false) && (txtPassword.Text == txtPasswordConfirm.Text))
            {
                txtPassword.CssClass = "OrderConfirmation_ValidTextBox";
                txtPasswordConfirm.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtPassword.CssClass = "OrderConfirmation_InvalidTextBox";
                txtPasswordConfirm.CssClass = "OrderConfirmation_InvalidTextBox";
                boolIsValidPast = false;
            }

            if (string.IsNullOrEmpty(txtFirstName.Text) == false)
            {
                txtFirstName.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtFirstName.CssClass = "OrderConfirmation_InvalidTextBox";
                boolIsValidPast = false;
            }

            if (string.IsNullOrEmpty(txtLastName.Text) == false)
            {
                txtLastName.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtLastName.CssClass = "OrderConfirmation_InvalidTextBox";
                boolIsValidPast = false;
            }

            // ------------------------------------------------------

            if (!boolIsValidPast)
            {
                ulUserRegistarionValidation.Visible = true;
                ulUserRegistarionValidation.InnerHtml += string.Format("<li>{0}</li>", Resource.Client_OrderConfirmation_EnterEmptyField);
            }
            else
                ulUserRegistarionValidation.Visible = false;
            return boolIsValidPast;
        }

        protected void btnSelectAddress_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomerContacts.SelectedValue))
            {
                switch (hfTypeBindAddress.Value)
                {
                    case "billing":
                        chkCopyAddress.Checked = false;
                        if (CustomerContacts.SelectedValue != "New")
                        {
                            LoadBilling(CustomerService.GetCustomerContact(CustomerContacts.SelectedValue));
                        }
                        else
                        {
                            CleanBilling();
                            hfBillingID.Value = "New";
                        }
                        break;
                    case "shipping":
                        if (CustomerContacts.SelectedValue != "New")
                        {
                            LoadShipping(CustomerService.GetCustomerContact(CustomerContacts.SelectedValue));
                            if (chkCopyAddress.Checked)
                            {
                                LoadBilling(CustomerService.GetCustomerContact(CustomerContacts.SelectedValue));
                            }
                        }
                        else
                        {
                            CleanShipping();
                            hfShippingID.Value = "New";
                        }
                        break;
                }
                UpdatePanel1.Update();
            }
        }

        protected void SaveOrderCart(int orderId)
        {
            OrderService.AddUpdateOrderItems(orderItems.OrderItems, orderId);
        }

        protected void SaveOrderCart(int orderId, IList<OrderItem> oldItems, OrderStatus status)
        {
            OrderService.AddUpdateOrderItems(orderItems.OrderItems, oldItems, orderId);

            if (status != null && status.Command == OrderStatusCommand.Increment)
            {
                OrderService.IncrementProductsCountAccordingOrder(orderId);
            }
            else if (status != null && status.Command == OrderStatusCommand.Decrement)
            {
                OrderService.DecrementProductsCountAccordingOrder(orderId);
            }
        }

        protected void orderItems_Updated(object sender, EventArgs args)
        {
            LoadTotal();
            txtShippingPrice.Text = (txtShippingPrice.Text.TryParseFloat() / (orderItems.CurrencyValue / orderItems.OldCurrencyValue)).ToString("F2");
        }

        private DateTime GetDate()
        {
            DateTime d;
            if (DateTime.TryParse(lOrderDate.Text, out d))
            {
                var hours = txtOrderTime.Text.Split(":").FirstOrDefault().TryParseInt();
                var minutes = txtOrderTime.Text.Split(":").LastOrDefault().TryParseInt();
                if (hours < 0 || hours > 23)
                {
                    hours = 0;
                }

                if (minutes < 0 || minutes > 59)
                {
                    minutes = 0;
                }
                return new DateTime(d.Year, d.Month, d.Day, hours, minutes, 0);
            }

            return DateTime.Now;
        }

        protected void btnSelectShipping_Click(object sender, EventArgs e)
        {
            LoadShippingMethods();

            if (ShippingRates.SelectedItem == null || ShippingRates.SelectedId == 0)
                return;

            float shipPrice = 0;

            Order ord = OrderService.GetOrder(OrderID);

            if (ord == null || ord.PaymentMethod == null || ord.PaymentMethod.Type != PaymentType.CashOnDelivery || ShippingRates.SelectedItem.Ext == null)
            {
                shipPrice = ShippingRates.SelectedItem.Rate;
            }
            else
            {
                shipPrice = ShippingRates.SelectedItem.Ext.PriceCash;
            }



            hfOrderShippingId.Value = ShippingRates.SelectedItem.MethodId.ToString();
            txtShippingPrice.Text = (shipPrice / orderItems.CurrencyValue).ToString("F2");
            txtShippingMethod.Text = ShippingRates.SelectedItem.MethodNameRate;
            if (ShippingRates.SelectShippingOptionEx != null)
            {
                ltPickPointID.Text = ShippingRates.SelectShippingOptionEx.PickpointId;
                ltPickPointAddress.Text = ShippingRates.SelectShippingOptionEx.PickpointAddress;
                hfPickpointAdditional.Value = ShippingRates.SelectShippingOptionEx.AdditionalData;
            }
            else
            {
                ltPickPointID.Text = string.Empty;
                ltPickPointAddress.Text = string.Empty;
                hfPickpointAdditional.Value = string.Empty;
            }

            LoadTotal();
            modalShipping.Hide();
        }

        protected void lbChangeShipping_Click(object sender, EventArgs e)
        {
            LoadShippingMethods();
            ShippingRates.LoadMethods();
            modalShipping.Show();
        }

        private void LoadShippingMethods()
        {
            ShippingRates.CountryId = Convert.ToInt32(ddlShippingCountry.SelectedValue);
            ShippingRates.Region = txtShippingZone.Text;
            ShippingRates.City = txtShippingCity.Text;
            ShippingRates.Zip = txtShippingZip.Text;
            ShippingRates.Currency = orderItems.Currency;
            
            var shoppingCart = new ShoppingCart();
            shoppingCart.AddRange(
                orderItems.OrderItems.Where(item => item.ProductID != null && item.ProductID != 0 )
                .Where(item =>  ProductService.GetProduct((int)item.ProductID).Offers.FirstOrDefault() != null)
                    .Select(item => new ShoppingCartItem
                    {
                        OfferId = ProductService.GetProduct((int)item.ProductID).Offers.First().OfferId,
                        Amount = item.Amount,
                    }));

            ShippingRates.OrderItems = orderItems.OrderItems;

            ShippingRates.ShoppingCart = shoppingCart;
        }

        protected void chkUseBonuses_CheckedChanged(object sender, EventArgs e)
        {
            LoadTotal();
        }

        protected void lbPaymentRecalc_Click(object sender, EventArgs e)
        {
            var order = AddingNewOrder ? new Order() : OrderService.GetOrder(OrderID);
            var payment = PaymentService.GetPaymentMethod(ddlPaymentMethod.SelectedValue.TryParseInt());
            if (payment != null)
            {
                txtPaymentPrice.Text = (payment.ExtrachargeType == ExtrachargeType.Fixed
                                           ? payment.Extracharge
                                           : payment.Extracharge / 100 *
                                                (orderItems.OrderItems.Sum(item => item.Price * item.Amount) -
                                                (orderItems.OrderItems.Sum(item => item.Price * item.Amount) / 100 * orderItems.OrderDiscount)
                                                - order.BonusCost + order.ShippingCost
                                                + order.Taxes.Where(tax => !tax.TaxShowInPrice).Sum(tax => tax.TaxSum))).ToString("F2");
            }
            LoadTotal();
        }
    }
}