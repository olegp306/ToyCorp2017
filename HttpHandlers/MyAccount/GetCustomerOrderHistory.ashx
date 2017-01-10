<%@ WebHandler Language="C#" Class="GetCustomerOrderHistory" %>

using System.Linq;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;

public class GetCustomerOrderHistory : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {

        AdvantShop.Localization.Culture.InitializeCulture();
        
        if (!AdvantShop.Customers.CustomerContext.CurrentCustomer.RegistredUser)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        context.Response.ContentType = "application/json";

        var orders = AdvantShop.Orders.OrderService.GetCustomerOrderHistory(AdvantShop.Customers.CustomerContext.CurrentCustomer.Id);

        var customerOrders = from item in orders
                             select new
                             {
                                 item.OrderID,
                                 item.ArchivedPaymentName,
                                 item.Status,
                                 item.ShippingMethodName,
                                 OrderDate = item.OrderDate.ToString(AdvantShop.Configuration.SettingsMain.ShortDateFormat),
                                 OrderTime = item.OrderDate.ToString("HH:mm"),
                                 Sum = AdvantShop.Catalog.CatalogService.GetStringPrice(item.Sum, item.CurrencyValue, item.CurrencyCode),
                                 item.OrderNumber,
                                 item.Payed
                             };
        var totalPrice = orders.Where(item => item.Payed).Sum(item => item.Sum);
        totalPrice = totalPrice / AdvantShop.Repository.Currencies.CurrencyService.CurrentCurrency.Value;
        
        context.Response.Write(JsonConvert.SerializeObject(
            new
                {
                    Orders = customerOrders,
                    TotalSum = AdvantShop.Catalog.CatalogService.GetStringPrice(totalPrice, 1, AdvantShop.Repository.Currencies.CurrencyService.CurrentCurrency.Iso3)
                }));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
