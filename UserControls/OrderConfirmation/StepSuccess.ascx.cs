using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AdvantShop;
using AdvantShop.CMS;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.SEO;
using AdvantShop.Trial;

namespace UserControls.OrderConfirmation
{
    public partial class StepSuccess : System.Web.UI.UserControl
    {
        public int OrderID { get; set; }

        protected Order Order { get; set; }

        protected string SbOrderSuccessTopText;
        protected int SbOrderSuccessTopId;

        public GoogleTagManager TagManager { get; set; }

        public string GoogleAnalyticString { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack || OrderID == 0 || (Order = OrderService.GetOrder(OrderID)) == null) return;

            var sb = StaticBlockService.GetPagePartByKeyWithCache("OrderSuccessTop");
            if (sb == null || (!CustomerContext.CurrentCustomer.IsAdmin && !sb.Enabled))
            {
                SbOrderSuccessTopText = string.Empty;
            }
            else
            {
                SbOrderSuccessTopId = sb.StaticBlockId;
                SbOrderSuccessTopText = sb.Content ?? string.Empty;

                SbOrderSuccessTopText = SbOrderSuccessTopText.Replace("#ORDER_ID#", OrderID.ToString());
            }

            if (SettingsOrderConfirmation.SuccessOrderScript.IsNotEmpty())
            {
                string orderScript = SettingsOrderConfirmation.SuccessOrderScript
                                                              .Replace("#ORDER_ID#", Order.OrderID.ToString())
                                                              .Replace("#ORDER_SUM#", Order.Sum.ToString("#.##"))
                                                              .Replace("#CUSTOMER_EMAIL#", HttpUtility.HtmlEncode(Order.OrderCustomer.Email))
                                                              .Replace("#CUSTOMER_FIRSTNAME#", HttpUtility.HtmlEncode(Order.OrderCustomer.FirstName))
                                                              .Replace("#CUSTOMER_LASTNAME#", HttpUtility.HtmlEncode(Order.OrderCustomer.LastName))
                                                              .Replace("#CUSTOMER_PHONE#", HttpUtility.HtmlEncode(Order.OrderCustomer.MobilePhone))
                                                              .Replace("#CUSTOMER_ID#", Order.OrderCustomer.CustomerID.ToString());

                var regex = new Regex("<<(.*)>>");
                var match = regex.Match(orderScript);
                var products = new StringBuilder();

                if (match.Groups.Count > 0 && match.Groups[1].Value.IsNotEmpty())
                {
                    var productLine = match.Groups[1].Value;
                    foreach (var item in Order.OrderItems)
                    {
                        products.Append(
                            productLine.Replace("#PRODUCT_ARTNO#", HttpUtility.HtmlEncode(item.ArtNo))
                                       .Replace("#PRODUCT_NAME#", HttpUtility.HtmlEncode(item.Name))
                                       .Replace("#PRODUCT_PRICE#", item.Price.ToString("#.##"))
                                       .Replace("#PRODUCT_AMOUNT#", item.Amount.ToString("#.##")));
                    }

                    orderScript = orderScript.Replace("<<" + productLine + ">>", products.ToString());
                }

                lSuccessScript.Text = orderScript;
            }

            LoadGoogleAnalytics(Order);

            if (GoogleTagManager.Enabled)
            {
                TagManager.PageType = GoogleTagManager.ePageType.purchase;
                TagManager.TotalValue = Order.OrderItems.Sum(item => item.Price*item.Amount);

                TagManager.Transaction = new Transaction()
                {
                    TransactionAffiliation = SettingsMain.ShopName,
                    TransactionId = Order.OrderID,
                    TransactionTotal = Order.Sum - Order.ShippingCost,
                    TransactionShipping = Order.ShippingCost,
                    TransactionProducts =
                        new List<TransactionProduct>(
                            Order.OrderItems.Select(
                                item =>
                                    new TransactionProduct()
                                    {
                                        Name = item.Name,
                                        Price = item.Price,
                                        Quantity = item.Amount,
                                        SKU = item.ArtNo,
                                        Category =
                                            CategoryService.GetCategory(
                                                ProductService.GetFirstCategoryIdByProductId((int) item.ProductID)).Name
                                    }))
                };
            }
            TrialService.TrackEvent(TrialEvents.CheckoutOrder, OrderID.ToString());
        }

        private void LoadGoogleAnalytics(Order order)
        {
            var googleAnalystic = new GoogleAnalyticsString
            {
                Trans = new GoogleAnalyticsTrans
                {
                    OrderId = order.OrderID.ToString(),
                    Affiliation = SettingsMain.ShopName,
                    Total = (order.Sum - order.ShippingCost).ToString("F2", CultureInfo.InvariantCulture),
                    Shipping = order.ShippingCost.ToString("F2", CultureInfo.InvariantCulture),
                    City = order.ShippingContact.City,
                    State = string.Empty,
                    Country = order.ShippingContact.Country,
                },
                Items = GetListItemForGoogleAnalytics(order.OrderItems, order.OrderID).ToList()
            };

            GoogleAnalyticString = googleAnalystic.GetEComerce();
        }

        private IEnumerable<GoogleAnalyticsItem> GetListItemForGoogleAnalytics(List<OrderItem> orderItems, int orderid)
        {
            return from item in orderItems
                   let categoryId = ProductService.GetFirstCategoryIdByProductId((int)item.ProductID)
                   select new GoogleAnalyticsItem
                   {
                       OrderId = orderid.ToString(),
                       Sku = item.ArtNo,
                       Name = item.Name,
                       Category = categoryId != -1 ? CategoryService.GetCategory(categoryId).Name : "",
                       Price = item.Price.ToString("F2", CultureInfo.InvariantCulture),
                       Quantity = item.Amount.ToString()
                   };
        }

    }
}