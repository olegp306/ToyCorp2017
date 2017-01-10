<%@ WebHandler Language="C#" Class="GetShipping" %>

using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using Newtonsoft.Json;

public class GetShipping : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";

        var contactId = context.Request["contactId"];
        
        var countryId = context.Request["countryId"].TryParseInt();
        var countryName = context.Request["country"] ?? string.Empty;
        var region = context.Request["region"] ?? string.Empty;
        var city = context.Request["city"] ?? string.Empty;
        var zip = context.Request["zip"] ?? string.Empty;
        var distance = context.Request["distance"].TryParseInt();
        var pickpointId = context.Request["pickpointId"].TryParseInt();
        var pickpointStringId = context.Request["pickpointStringId"];
        var pickpointAddress = context.Request["pickpointAddress"] ?? string.Empty;
        var additionalData = context.Request["additionalData"] ?? string.Empty;
        
        var pageData = OrderConfirmationService.Get(CustomerContext.CurrentCustomer.Id);
        if (pageData == null)
        {
            return;
        }

        if (countryId == 0)
        {
            var country = CountryService.GetCountry(countryId) ?? CountryService.GetCountryByName(countryName) ?? CountryService.GetCountry(SettingsMain.SellerCountryId);
            countryId = country.CountryId;
        }

        var orderConfirmData = pageData.OrderConfirmationData;

        if (contactId.IsNotEmpty())
        {
            var contact = CustomerContext.CurrentCustomer.Contacts.Find(c => c.CustomerContactID.ToString() == contactId);

            if (contact != null)
            {
                orderConfirmData.BillingContact = orderConfirmData.ShippingContact = contact;

                countryId = contact.CountryId;
                region = contact.RegionName;
                city = contact.City;
                zip = contact.Zip;
            }
        }
        else
        {
            var country = CountryService.GetCountry(countryId) ?? CountryService.GetCountryByName(countryName) ?? CountryService.GetCountry(SettingsMain.SellerCountryId);
            
            orderConfirmData.BillingContact =
                orderConfirmData.ShippingContact = new CustomerContact()
                {
                    Country = country != null ? country.Name : string.Empty,
                    CountryId = country != null ? country.CountryId : SettingsMain.SellerCountryId,
                    RegionName = HttpUtility.HtmlEncode(region),
                    City = HttpUtility.HtmlEncode(city),
                    Zip = HttpUtility.HtmlEncode(zip)
                };
        }

        orderConfirmData.Distance = distance;
        
        var shippingManager = new ShippingManager();
        var shippingRates = shippingManager.GetShippingRates(countryId, zip, city, region, ShoppingCartService.CurrentShoppingCart, distance);
        ShippingManager.CurrentShippingRates = shippingRates;

        var shippingRate = shippingRates.FirstOrDefault();
        if (shippingRate != null)
        {
            orderConfirmData.SelectedShippingItem = ShippingMethodService.GetListShippingItem(shippingRate, pickpointId,
                pickpointAddress, additionalData, orderConfirmData.ShippingContact.City);
            
            if (orderConfirmData.SelectedShippingItem.Ext != null && !string.IsNullOrEmpty(pickpointStringId))
            {
                orderConfirmData.SelectedShippingItem.Ext.PickpointId = pickpointStringId;
            }
        }
        else
        {
            orderConfirmData.SelectedShippingItem = new ShippingItem();
        }

        OrderConfirmationService.Update(pageData);

        var selectedId = shippingRate != null ? shippingRates.First().Id : 0;

        var obj = new
        {
            SelectedShippingId = selectedId,
            ShippingRates = shippingRates.Select(rate => new
            {
                rate.Id,
                rate.MethodId,
                Img = string.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                    ShippingIcons.GetShippingIcon(rate.Type, rate.IconName, rate.MethodNameRate),
                    HttpUtility.HtmlEncode(rate.MethodNameRate)),

                Name = rate.MethodNameRate,
                Extanded = ShippingMethodService.RenderExtend(rate, distance, pickpointAddress, rate.Id == selectedId),
                Price = rate.Rate != 0 ? ((rate.IsMinimumRate ? "от " : "") + CatalogService.GetStringPrice(rate.Rate)): rate.ZeroPriceMessage,
                DeliveryTime = rate.DeliveryTime ?? string.Empty,
                rate.MethodDescription
            }).ToList(),
            ShowAddress = ShippingMethodService.ShowAddressField(orderConfirmData.UserType, shippingRate),
            ShowCustomFields = ShippingMethodService.ShowCustomField(orderConfirmData.UserType, shippingRate),
        };

        context.Response.Write(JsonConvert.SerializeObject(obj));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}