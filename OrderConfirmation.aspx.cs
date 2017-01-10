//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Mails;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Security;
using AdvantShop.SEO;
using AdvantShop.Shipping;
using AdvantShop.Trial;
using Resources;
using AdvantShop.CMS;
using AdvantShop.Core.UrlRewriter;

namespace ClientPages
{
    public partial class OrderConfirmation : AdvantShopClientPage
    {
        protected AdvantShop.Orders.OrderConfirmation PageData;

        #region Protected

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var showConfirmButtons = true;
                foreach (var module in AttachedModules.GetModules<IRenderIntoShoppingCart>())
                {
                    var moduleObject = (IRenderIntoShoppingCart)Activator.CreateInstance(module, null);
                    showConfirmButtons &= moduleObject.ShowConfirmButtons;
                }

                if (!showConfirmButtons && !string.Equals(Request["tab"], "FinalTab"))
                {
                    Response.Redirect("shoppingcart.aspx");
                }

                if (OrderConfirmationService.OrderID != 0 && string.Equals(Request["tab"], "FinalTab"))
                {
                    mvOrderConfirm.SetActiveView(ViewOrderConfirmationFinal);
                    StepSuccess.OrderID = OrderConfirmationService.OrderID;
                    OrderConfirmationService.OrderID = 0;
                    return;
                }
            }
            var shoppingCart = ShoppingCartService.CurrentShoppingCart;
            PageData = LoadOrderConfirmationData(CustomerContext.CustomerId);

            if (!shoppingCart.CanOrder && (PageData.OrderConfirmationData.ActiveTab != EnActiveTab.FinalTab))
            {
                Response.Redirect("shoppingcart.aspx");
                return;
            }

            if (PageData.OrderConfirmationData.ActiveTab == EnActiveTab.FinalTab)
            {
                OrderConfirmationService.Delete(CustomerContext.CustomerId);
                LoadOrderConfirmationData(CustomerContext.CustomerId);
            }

            ShowTab(PageData.OrderConfirmationData.ActiveTab);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(new MetaInfo(string.Format("{0} - {1}", Resource.Client_OrderConfirmation_DrawUpOrder, SettingsMain.ShopName)), string.Empty);

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = PageData != null &&
                                             PageData.OrderConfirmationData.ActiveTab == EnActiveTab.FinalTab
                                                 ? GoogleTagManager.ePageType.purchase
                                                 : GoogleTagManager.ePageType.order;

                tagManager.ProdIds = ShoppingCartService.CurrentShoppingCart.Select(item => item.Offer.ArtNo).ToList();
                tagManager.TotalValue = ShoppingCartService.CurrentShoppingCart.TotalPrice;
                StepSuccess.TagManager = tagManager;

            }

        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (PageData != null && PageData.OrderConfirmationData.ActiveTab != EnActiveTab.FinalTab)
            {
                if (PageData.OrderConfirmationData.SelectedShippingItem.Id == 0 ||
                    PageData.OrderConfirmationData.SelectedPaymentItem.PaymenMethodtId == 0)
                {
                    btnConfirm.CssClass += " btn-disabled";
                }

                lblTotalPrice.Text = CatalogService.GetStringPrice(PageData.OrderConfirmationData.Sum);
                lblBonusPlus.Text = CatalogService.GetStringPrice(PageData.OrderConfirmationData.BonusPlus);
                bonusplusbottom.Visible = PageData.OrderConfirmationData.BonusPlus > 0;
            }

            breadCrumbs.Items = new List<BreadCrumbs>
            {
                new BreadCrumbs
                {
                    Name = Resource.Client_MasterPage_MainPage,
                    Url = UrlService.GetAbsoluteLink("/")
                },
                new BreadCrumbs
                {
                    Name = Resource.Client_OrderConfirmation_OrderConfirmation,
                    Url = UrlService.GetAbsoluteLink("orderconfirmation.aspx")
                },
            };

            ltGaECommerce.Text = StepSuccess.GoogleAnalyticString;
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            UpdatePageData();

            if (!IsValidPageData())
            {
                PageData.OrderConfirmationData.ActiveTab = EnActiveTab.NoTab;
                OrderConfirmationService.Update(PageData);
                ShowActiveTab(false);
            }
            else
            {
                PageData.OrderConfirmationData.ActiveTab = EnActiveTab.FinalTab;
                Redirect();
            }
        }

        #endregion

        #region Private methods

        private void Redirect()
        {
            //if (Request.UrlReferrer != null && !Request.UrlReferrer.AbsoluteUri.Contains("orderconfirmation"))
            //{
            //    PageData.OrderConfirmationData.ActiveTab = EnActiveTab.UserTab;
            //}
            OrderConfirmationService.Update(PageData);
            Response.Redirect("orderconfirmation.aspx?tab=" + PageData.OrderConfirmationData.ActiveTab.ToString());
        }

        /// <summary>
        /// Load or create new order confirmation data for current state
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private AdvantShop.Orders.OrderConfirmation LoadOrderConfirmationData(Guid id)
        {
            if (OrderConfirmationService.IsExist(id))
                return OrderConfirmationService.Get(id);

            var res = new AdvantShop.Orders.OrderConfirmation
            {
                CustomerId = id,
                OrderConfirmationData = new OrderConfirmationData
                {
                    UserType = CustomerContext.CurrentCustomer.RegistredUser ? EnUserType.RegisteredUser : EnUserType.NoUser,
                    BillingIsShipping = true
                }
            };

            OrderConfirmationService.Add(res);
            return res;
        }

        private void ShowTab(EnActiveTab tab)
        {
            PageData.OrderConfirmationData.ActiveTab = tab;
            ShowActiveTab(IsPostBack);
        }

        private void ShowActiveTab(bool isPostback)
        {
            switch (PageData.OrderConfirmationData.ActiveTab)
            {
                case EnActiveTab.NoTab:
                case EnActiveTab.DefaultTab:
                case EnActiveTab.UserTab:
                case EnActiveTab.ShippingTab:
                case EnActiveTab.PaymentTab:
                case EnActiveTab.SumTab:
                    //if (!isPostback)
                    {
                        Address.PageData = PageData.OrderConfirmationData;
                        Shipping.PageData = PageData.OrderConfirmationData;
                        Payment.PageData = PageData.OrderConfirmationData;
                        Basket.PageData = PageData;
                        Bonuses.PageData = PageData.OrderConfirmationData;
                        Confirm.PageData = PageData.OrderConfirmationData;

                        mvOrderConfirm.SetActiveView(ViewCheckout);
                    }
                    break;

                case EnActiveTab.FinalTab:
                    var order = DoCreateOrder();
                    StepSuccess.OrderID = order.OrderID;

                    mvOrderConfirm.SetActiveView(ViewOrderConfirmationFinal);
                    break;

                default:
                    mvOrderConfirm.SetActiveView(ViewCheckout);
                    break;
            }
        }

        private void UpdatePageData()
        {
            Address.UpdatePageData(PageData.OrderConfirmationData);
            Shipping.UpdatePageData(PageData.OrderConfirmationData);
            Payment.UpdatePageData(PageData.OrderConfirmationData);
            Confirm.UpdatePageData(PageData.OrderConfirmationData);
        }

        private bool IsValidPageData()
        {
            var shoppingCart = ShoppingCartService.CurrentShoppingCart;

            return Address.IsValidData(PageData.OrderConfirmationData) &&
                   PageData.OrderConfirmationData.SelectedShippingItem.Id != 0 &&
                   PageData.OrderConfirmationData.SelectedPaymentItem.PaymenMethodtId != 0 &&
                   Shipping.IsValidData(PageData.OrderConfirmationData) &&
                   Confirm.IsValidData() &&
                   shoppingCart.HasItems &&
                   shoppingCart.GetHashCode() == PageData.OrderConfirmationData.CheckSum;
        }

        private Order DoCreateOrder()
        {
            var shoppingCart = ShoppingCartService.CurrentShoppingCart;
            if (shoppingCart.GetHashCode() != PageData.OrderConfirmationData.CheckSum || !shoppingCart.HasItems)
            {
                Response.Redirect("shoppingcart.aspx");
                return null;
            }

            if (PageData.OrderConfirmationData.UserType == EnUserType.JustRegistredUser)
            {
                RegistrationNow();
            }

            if (PageData.OrderConfirmationData.UserType == EnUserType.RegisteredUserWithoutAddress)
            {
                UpdateCustomerContact();
            }

            var order = CreateOrder(shoppingCart);

            SendOrderMail(order, shoppingCart);

            var certificate = shoppingCart.Certificate;
            if (certificate != null)
            {
                certificate.ApplyOrderNumber = order.Number;
                certificate.Used = true;
                certificate.Enable = true;

                GiftCertificateService.DeleteCustomerCertificate(certificate.CertificateId);
                GiftCertificateService.UpdateCertificateById(certificate);
            }

            var coupon = shoppingCart.Coupon;
            if (coupon != null && shoppingCart.TotalPrice >= coupon.MinimalOrderPrice)
            {
                coupon.ActualUses += 1;
                CouponService.UpdateCoupon(coupon);
                CouponService.DeleteCustomerCoupon(coupon.CouponID);
            }

            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, PageData.OrderConfirmationData.Customer.Id);
            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, CustomerContext.CustomerId);

            ShippingManager.CurrentShippingRates.Clear();

            OrderConfirmationService.Delete(CustomerContext.CustomerId);

            return order;
        }

        private Order CreateOrder(ShoppingCart shoppingCart)
        {
            var orderConfirmData = PageData.OrderConfirmationData;
            var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

            var baseCurrency = CurrencyService.BaseCurrency;

            var ord = new Order
            {
                OrderCustomer = new OrderCustomer
                {
                    CustomerIP = Request.UserHostAddress,
                    CustomerID = orderConfirmData.Customer.Id,
                    FirstName = orderConfirmData.Customer.FirstName,
                    LastName = orderConfirmData.Customer.LastName,
                    Patronymic = orderConfirmData.Customer.Patronymic,
                    Email = orderConfirmData.Customer.EMail,
                    MobilePhone = orderConfirmData.Customer.Phone,
                },
                OrderCurrency = new OrderCurrency
                {
                    //CurrencyCode = CurrencyService.CurrentCurrency.Iso3,
                    //CurrencyValue = CurrencyService.CurrentCurrency.Value,
                    //CurrencySymbol = CurrencyService.CurrentCurrency.Symbol,
                    //CurrencyNumCode = CurrencyService.CurrentCurrency.NumIso3,
                    //IsCodeBefore = CurrencyService.CurrentCurrency.IsCodeBefore
                    CurrencyCode = baseCurrency.Iso3,
                    CurrencyValue = baseCurrency.Value,
                    CurrencySymbol = baseCurrency.Symbol,
                    CurrencyNumCode = baseCurrency.NumIso3,
                    IsCodeBefore = baseCurrency.IsCodeBefore
                },
                OrderStatusId = OrderService.DefaultOrderStatus,
                AffiliateID = 0,
                OrderDate = DateTime.Now,
                CustomerComment = orderConfirmData.CustomerComment,
                ShippingContact = new OrderContact
                {
                    Name = orderConfirmData.ShippingContact.Name,
                    Country = orderConfirmData.ShippingContact.Country,
                    Zone = orderConfirmData.ShippingContact.RegionName,
                    City = orderConfirmData.ShippingContact.City,
                    Zip = orderConfirmData.ShippingContact.Zip,
                    Address = orderConfirmData.ShippingContact.Address,
                    CustomField1 = orderConfirmData.ShippingContact.CustomField1,
                    CustomField2 = orderConfirmData.ShippingContact.CustomField2,
                    CustomField3 = orderConfirmData.ShippingContact.CustomField3
                },

                GroupName = customerGroup.GroupName,
                GroupDiscount = customerGroup.GroupDiscount,
                OrderDiscount = shoppingCart.DiscountPercentOnTotalPrice
            };

            foreach (var orderItem in shoppingCart.Select(item => (OrderItem)item))
            {
                ord.OrderItems.Add(orderItem);
            }

            if (!orderConfirmData.BillingIsShipping)
            {
                ord.BillingContact = new OrderContact
                {
                    Name = orderConfirmData.BillingContact.Name,
                    Country = orderConfirmData.BillingContact.Country,
                    Zone = orderConfirmData.BillingContact.RegionName,
                    City = orderConfirmData.BillingContact.City,
                    Zip = orderConfirmData.BillingContact.Zip,
                    Address = orderConfirmData.BillingContact.Address,
                    CustomField1 = orderConfirmData.BillingContact.CustomField1,
                    CustomField2 = orderConfirmData.BillingContact.CustomField2,
                    CustomField3 = orderConfirmData.BillingContact.CustomField3
                };
            }

            ord.ShippingMethodId = orderConfirmData.SelectedShippingItem.MethodId;
            ord.PaymentMethodId = orderConfirmData.SelectedPaymentItem.PaymenMethodtId;

            ord.ArchivedShippingName = orderConfirmData.SelectedShippingItem.MethodNameRate;
            ord.ArchivedPaymentName = orderConfirmData.SelectedPaymentItem.Name;

            ord.PaymentDetails = orderConfirmData.PaymentDetails;

            if (orderConfirmData.SelectedShippingItem.Ext != null &&
                orderConfirmData.SelectedShippingItem.Ext.PickpointAddress.IsNotEmpty())
            {
                ord.OrderPickPoint = new OrderPickPoint
                {
                    PickPointId = orderConfirmData.SelectedShippingItem.Ext.PickpointId,
                    PickPointAddress = orderConfirmData.SelectedShippingItem.Ext.PickpointAddress,
                    AdditionalData = orderConfirmData.SelectedShippingItem.Ext.AdditionalData
                };
            }
            else if (orderConfirmData.SelectedShippingItem.Ext != null &&
                orderConfirmData.SelectedShippingItem.Ext.Type == ExtendedType.Cdek &&
                orderConfirmData.SelectedShippingItem.Ext.PickpointAddress.IsNullOrEmpty())
            {
                ord.OrderPickPoint = new OrderPickPoint
                {
                    PickPointId = orderConfirmData.SelectedShippingItem.Ext.PickpointId,
                    PickPointAddress = ord.ShippingContact.Address,
                    AdditionalData = orderConfirmData.SelectedShippingItem.Ext.AdditionalData
                };
            }
            else if (orderConfirmData.SelectedShippingItem.Type == ShippingType.CheckoutRu &&
                orderConfirmData.SelectedShippingItem.Ext != null)
            {
                ord.OrderPickPoint = new OrderPickPoint
                {
                    PickPointId = orderConfirmData.SelectedShippingItem.Ext.PickpointId,
                    PickPointAddress = orderConfirmData.SelectedShippingItem.Ext.Type != ExtendedType.CashOnDelivery 
                        ? ord.ShippingContact.Address
                        : string.Empty,
                    AdditionalData = orderConfirmData.SelectedShippingItem.Ext.AdditionalData
                };
            }

            var certificate = shoppingCart.Certificate;
            var coupon = shoppingCart.Coupon;

            if (certificate != null)
            {
                ord.Certificate = new OrderCertificate()
                    {
                        Code = certificate.CertificateCode,
                        Price = certificate.Sum
                    };
            }
            if (coupon != null && shoppingCart.TotalPrice >= coupon.MinimalOrderPrice)
            {
                ord.Coupon = new OrderCoupon()
                {
                    Code = coupon.Code,
                    Type = coupon.Type,
                    Value = coupon.Value
                };
            }

            var shippingPrice = orderConfirmData.SelectedPaymentItem.Type == PaymentType.CashOnDelivery && (orderConfirmData.SelectedShippingItem.Type != ShippingType.Cdek || orderConfirmData.SelectedShippingItem.Type != ShippingType.CheckoutRu)
                                ? orderConfirmData.SelectedShippingItem.Ext != null
                                      ? orderConfirmData.SelectedShippingItem.Ext.PriceCash
                                      : 0
                                : orderConfirmData.SelectedShippingItem.Rate;

            BonusCard bonusCard = null;
            if (BonusSystem.IsActive)
            {
                bonusCard = BonusSystemService.GetCard(orderConfirmData.Customer.BonusCardNumber);
                if (bonusCard != null && orderConfirmData.UseBonuses && bonusCard.BonusAmount > 0)
                {
                    ord.BonusCost =
                        BonusSystemService.GetBonusCost(
                            shoppingCart.TotalPrice - shoppingCart.TotalDiscount + shippingPrice,
                            shoppingCart.TotalPrice - shoppingCart.TotalDiscount, bonusCard.BonusAmount);
                }
            }

            var taxTotal =
                AdvantShop.Taxes.TaxServices.CalculateTaxes(shoppingCart.TotalPrice - shoppingCart.TotalDiscount +
                                                            shippingPrice - ord.BonusCost).Where(tax => !tax.Key.ShowInPrice).Sum(tax => tax.Value);

            var paymentPrice = orderConfirmData.SelectedPaymentItem.ExtrachargeType == ExtrachargeType.Percent
                                    ? (shoppingCart.TotalPrice - shoppingCart.TotalDiscount + shippingPrice - ord.BonusCost + taxTotal) * orderConfirmData.SelectedPaymentItem.Extracharge / 100
                                    : orderConfirmData.SelectedPaymentItem.Extracharge;

            ord.ShippingCost = shippingPrice;
            ord.PaymentCost = paymentPrice;


            ord.Number = OrderService.GenerateNumber(1); // For crash protection

            ord.OrderID = OrderService.AddOrder(ord);
            ord.Number = OrderService.GenerateNumber(ord.OrderID); // new number
            OrderService.UpdateNumber(ord.OrderID, ord.Number);

            ModulesRenderer.OrderAdded(ord.OrderID);

            OrderService.ChangeOrderStatus(ord.OrderID, OrderService.DefaultOrderStatus);

            if (BonusSystem.IsActive && bonusCard != null)
            {
                var sumPrice = BonusSystem.BonusType == BonusSystem.EBonusType.ByProductsCostWithShipping
                       ? shoppingCart.TotalPrice - shoppingCart.TotalDiscount + shippingPrice
                       : shoppingCart.TotalPrice - shoppingCart.TotalDiscount;

                BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, sumPrice, ord.BonusCost, ord.Number, ord.OrderID);
            }


            

            TrialService.TrackEvent(
                ord.OrderItems.Any(item => item.Name.Contains("SM-G900F"))
                    ? TrialEvents.BuyTheProduct
                    : TrialEvents.CheckoutOrder, string.Empty);

            return ord;
        }

        private void SendOrderMail(Order order, ShoppingCart shoppingCart)
        {
            var htmlOrderTable = OrderService.GenerateHtmlOrderTable(order.OrderItems, CurrencyService.CurrentCurrency,
                                                                    shoppingCart.TotalPrice,
                                                                    shoppingCart.DiscountPercentOnTotalPrice,
                                                                    order.Coupon, order.Certificate,
                                                                    shoppingCart.TotalDiscount,
                                                                    order.ShippingCost, order.PaymentCost,
                                                                    order.TaxCost,
                                                                    order.BonusCost, PageData.OrderConfirmationData.BonusPlus);

            // Build a new mail
            var customerSb = new StringBuilder();
            customerSb.AppendFormat(SettingsOrderConfirmation.CustomerFirstNameField + ": {0}<br/>",
                                    PageData.OrderConfirmationData.Customer.FirstName);

            if (SettingsOrderConfirmation.IsShowLastName)
                customerSb.AppendFormat(Resource.Client_Registration_Surname + ": {0}<br/>",
                                    PageData.OrderConfirmationData.Customer.LastName);

            if (SettingsOrderConfirmation.IsShowPatronymic)
                customerSb.AppendFormat(Resource.Client_Registration_Patronymic + ": {0}<br/>",
                                    PageData.OrderConfirmationData.Customer.Patronymic);

            if (SettingsOrderConfirmation.IsShowPhone)
                customerSb.AppendFormat(SettingsOrderConfirmation.CustomerPhoneField + ": {0}<br/>",
                                    PageData.OrderConfirmationData.Customer.Phone);

            if (SettingsOrderConfirmation.IsShowCountry)
                customerSb.AppendFormat(Resource.Client_Registration_Country + ": {0}<br/>",
                                    PageData.OrderConfirmationData.ShippingContact.Country);

            if (SettingsOrderConfirmation.IsShowState && PageData.OrderConfirmationData.ShippingContact.RegionName.IsNotEmpty())
                customerSb.AppendFormat(Resource.Client_Registration_State + ": {0}<br/>",
                                    PageData.OrderConfirmationData.ShippingContact.RegionName);

            if (SettingsOrderConfirmation.IsShowCity)
                customerSb.AppendFormat(Resource.Client_Registration_City + ": {0}<br/>",
                                    PageData.OrderConfirmationData.ShippingContact.City);

            if (SettingsOrderConfirmation.IsShowZip && PageData.OrderConfirmationData.ShippingContact.Zip.IsNotEmpty())
                customerSb.AppendFormat(Resource.Client_Registration_Zip + ": {0}<br/>",
                                    PageData.OrderConfirmationData.ShippingContact.Zip);

            if (SettingsOrderConfirmation.IsShowAddress)
                customerSb.AppendFormat(Resource.Client_Registration_Address + ": {0}<br/>",
                    string.IsNullOrEmpty(PageData.OrderConfirmationData.ShippingContact.Address)
                        ? Resource.Client_OrderConfirmation_NotDefined
                        : PageData.OrderConfirmationData.ShippingContact.Address);

            if (SettingsOrderConfirmation.IsShowCustomShippingField1 && PageData.OrderConfirmationData.ShippingContact.CustomField1.IsNotEmpty())
                customerSb.AppendFormat(SettingsOrderConfirmation.CustomShippingField1 + ": {0}<br/>",
                                    PageData.OrderConfirmationData.ShippingContact.CustomField1);

            if (SettingsOrderConfirmation.IsShowCustomShippingField2 && PageData.OrderConfirmationData.ShippingContact.CustomField2.IsNotEmpty())
                customerSb.AppendFormat(SettingsOrderConfirmation.CustomShippingField2 + ": {0}<br/>",
                                    PageData.OrderConfirmationData.ShippingContact.CustomField2);

            if (SettingsOrderConfirmation.IsShowCustomShippingField3 && PageData.OrderConfirmationData.ShippingContact.CustomField3.IsNotEmpty())
                customerSb.AppendFormat(SettingsOrderConfirmation.CustomShippingField3 + ": {0}<br/>",
                                    PageData.OrderConfirmationData.ShippingContact.CustomField3);

            customerSb.AppendFormat("Email: {0}<br/>", PageData.OrderConfirmationData.Customer.EMail);

            var email = PageData.OrderConfirmationData.Customer.EMail;
            var orderMailTemplate = new NewOrderMailTemplate(order.OrderID.ToString(), order.Number, email,
                                                             customerSb.ToString(),
                                                             PageData.OrderConfirmationData.SelectedShippingItem
                                                                     .MethodNameRate +
                                                             (PageData.OrderConfirmationData.SelectedShippingItem.Ext != null &&
                                                             PageData.OrderConfirmationData.SelectedShippingItem.Ext.PickpointAddress.IsNotEmpty()
                                                                 ? "<br />" + PageData.OrderConfirmationData.SelectedShippingItem.Ext.PickpointAddress
                                                                 : ""),
                                                             PageData.OrderConfirmationData.SelectedPaymentItem.Name,
                                                             htmlOrderTable, CurrencyService.CurrentCurrency.Iso3,
                                                             order.Sum.ToString(), order.CustomerComment,
                                                             OrderService.GetBillingLinkHash(order));
            orderMailTemplate.BuildMail();

            var paymentMethod = PaymentService.GetPaymentMethod(order.PaymentMethodId);

            if (!CustomerContext.CurrentCustomer.IsVirtual)
            {
                // TODO check it
                if (paymentMethod != null)
                {
                    SendMail.SendMailNow(email, orderMailTemplate.Subject, orderMailTemplate.Body, true);
                    SendMail.SendMailNow(SettingsMail.EmailForOrders, orderMailTemplate.Subject, orderMailTemplate.Body,
                                         true);
                }
                else
                {
                    SendMail.SendMailNow(SettingsMail.EmailForOrders, orderMailTemplate.Subject,
                                         " ERROR: paymentMethod is not exist. " + orderMailTemplate.Body, true);
                }
            }
        }


        private void RegistrationNow()
        {
            try
            {
                if (CustomerService.CheckCustomerExist(PageData.OrderConfirmationData.Customer.EMail))
                {
                    ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_CustomerExist);
                    return;
                }


                var id = CustomerService.InsertNewCustomer(new Customer
                {
                    CustomerGroupId = CustomerGroupService.DefaultCustomerGroup,
                    Password = PageData.OrderConfirmationData.Customer.Password,
                    FirstName = PageData.OrderConfirmationData.Customer.FirstName,
                    LastName = PageData.OrderConfirmationData.Customer.LastName,
                    Phone = PageData.OrderConfirmationData.Customer.Phone,
                    SubscribedForNews = false,
                    EMail = PageData.OrderConfirmationData.Customer.EMail,
                    CustomerRole = Role.User,
                    BonusCardNumber = PageData.OrderConfirmationData.Customer.BonusCardNumber
                });

                if (id == Guid.Empty)
                    return;

                PageData.OrderConfirmationData.Customer.Id = id;

                AuthorizeService.SignIn(PageData.OrderConfirmationData.Customer.EMail,
                    PageData.OrderConfirmationData.Customer.Password, false, true);

                var newContact = PageData.OrderConfirmationData.ShippingContact;
                CustomerService.AddContact(newContact, PageData.OrderConfirmationData.Customer.Id);

                if (!PageData.OrderConfirmationData.BillingIsShipping)
                {
                    newContact = PageData.OrderConfirmationData.BillingContact;
                    CustomerService.AddContact(newContact, PageData.OrderConfirmationData.Customer.Id);
                }

                var registrationMail = new RegistrationMailTemplate(SettingsMain.SiteUrl,
                                                                    PageData.OrderConfirmationData.Customer.FirstName,
                                                                    PageData.OrderConfirmationData.Customer.LastName,
                                                                    AdvantShop.Localization.Culture.ConvertDate(DateTime.Now),
                                                                    PageData.OrderConfirmationData.Customer.Password,
                                                                    Resource.Client_Registration_No,
                                                                    PageData.OrderConfirmationData.Customer.EMail);
                registrationMail.BuildMail();

                if (CustomerContext.CurrentCustomer.IsVirtual)
                {
                    ShowMessage(Notify.NotifyType.Notice,
                                Resource.Client_Registration_Whom + PageData.OrderConfirmationData.Customer.EMail + '\r' +
                                Resource.Client_Registration_Text + registrationMail.Body);
                }
                else
                {
                    SendMail.SendMailNow(PageData.OrderConfirmationData.Customer.EMail, registrationMail.Subject,
                                         registrationMail.Body, true);
                    SendMail.SendMailNow(SettingsMail.EmailForRegReport, registrationMail.Subject, registrationMail.Body,
                                         true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private void UpdateCustomerContact()
        {
            CustomerService.UpdateCustomer(PageData.OrderConfirmationData.Customer);

            var newContact = PageData.OrderConfirmationData.ShippingContact;
            CustomerService.AddContact(newContact, PageData.OrderConfirmationData.Customer.Id);

            if (!PageData.OrderConfirmationData.BillingIsShipping)
            {
                newContact = PageData.OrderConfirmationData.BillingContact;
                CustomerService.AddContact(newContact, PageData.OrderConfirmationData.Customer.Id);
            }
        }

        #endregion
    }
}