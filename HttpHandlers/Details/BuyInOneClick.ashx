<%@ WebHandler Language="C#" Class="HttpHandlers.Details.BuyInOneClickHandler" %>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Mails;
using AdvantShop.Modules;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using Resources;

namespace HttpHandlers.Details
{
    public class BuyInOneClickHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!SettingsOrderConfirmation.BuyInOneClick)
            {
                ReturnResult(context, "error", true);
                return;
            }
            
            var amount = 0;
            var page = BuyInOneclickPage.details;

            var valid = true;
            valid &= context.Request["page"].IsNotEmpty() && Enum.TryParse(context.Request["page"], true, out page);
           
            if (page == BuyInOneclickPage.details)
            {
                valid &= context.Request["amount"].IsNotEmpty() && Int32.TryParse(context.Request["amount"], out amount);
            }

            if (SettingsOrderConfirmation.IsShowBuyInOneClickName &&
                SettingsOrderConfirmation.IsRequiredBuyInOneClickName)
            {
                valid &= context.Request["name"].IsNotEmpty();
            }

            if (SettingsOrderConfirmation.IsShowBuyInOneClickEmail &&
                SettingsOrderConfirmation.IsRequiredBuyInOneClickEmail)
            {
                valid &= context.Request["email"].IsNotEmpty();
            }

            if (SettingsOrderConfirmation.IsShowBuyInOneClickPhone &&
                SettingsOrderConfirmation.IsRequiredBuyInOneClickPhone)
            {
                valid &= context.Request["phone"].IsNotEmpty();
            }
            

            if (!valid)
            {
                ReturnResult(context, Resources.Resource.Client_OneClick_WrongData, true);
            }

            var orderItems = new List<OrderItem>();
            float discountPercentOnTotalPrice = 0;
            float totalPrice = 0;

            OrderCertificate orderCertificate = null;
            OrderCoupon orderCoupon = null;

            if (page == BuyInOneclickPage.details)
            {
                Offer offer;

                if (context.Request["offerid"].IsNullOrEmpty())
                {
                    var p = ProductService.GetProduct(context.Request["productid"].TryParseInt());
                    if (p == null || p.Offers.Count == 0)
                    {
                        ReturnResult(context, Resources.Resource.Client_ShoppingCart_Error, true);
                        return;
                    }
                    offer = p.Offers.First();
                }
                else
                {
                    offer = OfferService.GetOffer(context.Request["offerid"].TryParseInt());
                }

                IList<EvaluatedCustomOptions> listOptions = null;
                string selectedOptions = HttpUtility.UrlDecode(context.Request["customOptions"]);
                if (selectedOptions.IsNotEmpty())
                {
                    try
                    {
                        listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                    }
                    catch (Exception)
                    {
                        listOptions = null;
                    }
                }

                discountPercentOnTotalPrice = OrderService.GetDiscount(offer.Price * amount);
                totalPrice = (offer.Price - (offer.Price*offer.Product.Discount/100))*amount;

                if (totalPrice < SettingsOrderConfirmation.MinimalOrderPrice)
                {
                    ReturnResult(context, 
                        string.Format(Resources.Resource.Client_ShoppingCart_MinimalOrderPrice,
                            CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice),
                            CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice - totalPrice)), true);
                    return;
                }

                var errorAvailable = GetAvalible(offer, amount);
                if (!string.IsNullOrEmpty(errorAvailable))
                {
                    ReturnResult(context, errorAvailable, true);
                    return;
                }
                
                orderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductID = offer.ProductId,
                        Name = offer.Product.Name,
                        ArtNo = offer.ArtNo,
                        Price = CatalogService.CalculateProductPrice(offer.Price, offer.Product.CalculableDiscount, CustomerContext.CurrentCustomer.CustomerGroup, listOptions),
                        Amount = amount,
                        SupplyPrice = offer.SupplyPrice,
                        SelectedOptions = listOptions,
                        Weight = offer.Product.Weight,
                        Color = offer.Color != null ? offer.Color.ColorName : null,
                        Size = offer.Size != null ? offer.Size.SizeName : null,
                        PhotoID = offer.Photo != null ? offer.Photo.PhotoId : (int?) null
                    }
                };
            }
            else if (page == BuyInOneclickPage.shoppingcart || page == BuyInOneclickPage.orderconfirmation)
            {
                var shoppingCart = ShoppingCartService.CurrentShoppingCart;
                discountPercentOnTotalPrice = shoppingCart.DiscountPercentOnTotalPrice;
                totalPrice = shoppingCart.TotalPrice;

                if (totalPrice < SettingsOrderConfirmation.MinimalOrderPrice)
                {
                    ReturnResult(context, string.Format(Resources.Resource.Client_ShoppingCart_MinimalOrderPrice,
                                                        CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice),
                                                        CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice - totalPrice)), true);
                    return;
                }

                foreach (var item in shoppingCart)
                {
                    var errorAvailable = GetAvalible(item.Offer, item.Amount);
                    if (!string.IsNullOrEmpty(errorAvailable))
                    {
                        ReturnResult(context, errorAvailable, true);
                        return;
                    }
                }

                orderItems.AddRange(shoppingCart.Select(item => (OrderItem) item));

                GiftCertificate certificate = shoppingCart.Certificate;
                Coupon coupon = shoppingCart.Coupon;

                if (certificate != null)
                {
                    orderCertificate = new OrderCertificate()
                        {
                            Code = certificate.CertificateCode,
                            Price = certificate.Sum
                        };
                }
                if (coupon != null && shoppingCart.TotalPrice >= coupon.MinimalOrderPrice)
                {
                    orderCoupon = new OrderCoupon()
                    {
                        Code = coupon.Code,
                        Type = coupon.Type,
                        Value = coupon.Value
                    };
                }
            }

            var orderContact = new OrderContact
                {
                    Address = string.Empty,
                    City = IpZoneContext.CurrentZone.City,
                    Country = IpZoneContext.CurrentZone.CountryName,
                    Name = string.Empty,
                    Zip = string.Empty,
                    Zone = IpZoneContext.CurrentZone.Region
                };
            
            //var orderContact = new OrderContact
            //    {
            //        Address = string.Empty,
            //        City = string.Empty,
            //        Country = string.Empty,
            //        Name = string.Empty,
            //        Zip = string.Empty,
            //        Zone = string.Empty
            //    };

            var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

            var baseCurrency = CurrencyService.BaseCurrency;
            
            var order = new Order
            {
                CustomerComment = context.Request["comment"],
                OrderDate = DateTime.Now,
                OrderCurrency = new OrderCurrency
                {
                    //CurrencyCode = CurrencyService.CurrentCurrency.Iso3,
                    //CurrencyNumCode = CurrencyService.CurrentCurrency.NumIso3,
                    //CurrencySymbol = CurrencyService.CurrentCurrency.Symbol,
                    //CurrencyValue = CurrencyService.CurrentCurrency.Value,
                    //IsCodeBefore = CurrencyService.CurrentCurrency.IsCodeBefore
                    CurrencyCode = baseCurrency.Iso3,
                    CurrencyValue = baseCurrency.Value,
                    CurrencySymbol = baseCurrency.Symbol,
                    CurrencyNumCode = baseCurrency.NumIso3,
                    IsCodeBefore = baseCurrency.IsCodeBefore
                },
                OrderCustomer = new OrderCustomer
                {
                    CustomerID = CustomerContext.CurrentCustomer.Id,
                    Email =
                        context.Request["email"].IsNotEmpty()
                            ? context.Request["email"]
                            : CustomerContext.CurrentCustomer.EMail,
                    FirstName = context.Request["name"].IsNotEmpty() ? context.Request["name"] : CustomerContext.CurrentCustomer.FirstName,
                    LastName = string.Empty,
                    MobilePhone = context.Request["phone"].IsNotEmpty() ? context.Request["phone"] : CustomerContext.CurrentCustomer.Phone,
                    CustomerIP = HttpContext.Current.Request.UserHostAddress
                },

                OrderStatusId = OrderService.DefaultOrderStatus,
                AffiliateID = 0,
                GroupName = customerGroup.GroupName,
                GroupDiscount = customerGroup.GroupDiscount,

                OrderItems = orderItems,
                OrderDiscount = discountPercentOnTotalPrice,
                Number = OrderService.GenerateNumber(1),
                ShippingContact = orderContact,
                BillingContact = orderContact,
                Certificate = orderCertificate,
                Coupon = orderCoupon,
                AdminOrderComment = Resources.Resource.Client_BuyInOneClick_Header
            };

            order.OrderID = OrderService.AddOrder(order);
            order.Number = OrderService.GenerateNumber(order.OrderID);
            OrderService.UpdateNumber(order.OrderID, order.Number);
            OrderService.ChangeOrderStatus(order.OrderID, OrderService.DefaultOrderStatus);

            if (order.OrderID != 0)
            {
                try
                {
                    var orderTable = OrderService.GenerateHtmlOrderTable(order.OrderItems,
                                                                        CurrencyService.CurrentCurrency, totalPrice,
                                                                        discountPercentOnTotalPrice, orderCoupon,
                                                                        orderCertificate,
                                                                        discountPercentOnTotalPrice > 0
                                                                            ? discountPercentOnTotalPrice*totalPrice/100
                                                                            : 0,
                                                                        0, 0, 0, 0, 0);

                    var mailTemplate = new BuyInOneClickMailTemplate(order.OrderID.ToString(),
                        HttpUtility.HtmlEncode(order.OrderCustomer.FirstName),
                        HttpUtility.HtmlEncode(order.OrderCustomer.MobilePhone),
                        HttpUtility.HtmlEncode(order.CustomerComment),
                        orderTable, HttpUtility.HtmlEncode(order.OrderCustomer.Email), order.Number);
                    
                    mailTemplate.BuildMail();
                    
                    SendMail.SendMailNow(SettingsMail.EmailForOrders, mailTemplate.Subject, mailTemplate.Body, true);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            ModulesRenderer.OrderAdded(order.OrderID);

            OrderConfirmationService.OrderID = order.OrderID;
            
            if (order.OrderID != 0 && (page == BuyInOneclickPage.shoppingcart || page == BuyInOneclickPage.orderconfirmation))
            {
                ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, CustomerContext.CustomerId);
                ReturnResult(context, SettingsOrderConfirmation.BuyInOneClickGoToFinalStep ?"goto_finalstep": "reload", false);
                return;
            }

            ReturnResult(context, SettingsOrderConfirmation.BuyInOneClickGoToFinalStep ? "goto_finalstep" : "goto_finishForm", false);
        }

        private static void ReturnResult(HttpContext context, string result, bool error)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(error
                ? Newtonsoft.Json.JsonConvert.SerializeObject(new {type = "error", result})
                : Newtonsoft.Json.JsonConvert.SerializeObject(new {result}));
            context.Response.End();
        }

        private static string GetAvalible(Offer offer, float amount)
        {
            if (!offer.Product.Enabled || !offer.Product.CategoryEnabled)
            {
                return Resource.Client_ShoppingCart_NotAvailable + " 0 " + offer.Product.Unit;
            }

            if ((SettingsOrderConfirmation.AmountLimitation) && (amount > offer.Amount))
            {
                return Resource.Client_ShoppingCart_NotAvailable + " " + offer.Amount + " " + offer.Product.Unit;
            }

            if (amount > offer.Product.MaxAmount)
            {
                return Resource.Client_ShoppingCart_NotAvailable_MaximumOrder + " " + offer.Product.MaxAmount + " " + offer.Product.Unit;
            }

            if (amount < offer.Product.MinAmount)
            {
                return Resource.Client_ShoppingCart_NotAvailable_MinimumOrder + " " + offer.Product.MinAmount + " " + offer.Product.Unit;
            }

            return string.Empty;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

}