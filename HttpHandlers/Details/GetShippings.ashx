<%@ WebHandler Language="C#" Class="GetShippings" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using Newtonsoft.Json;

public class GetShippings : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        if (context.Request["offer"].IsNullOrEmpty())
        {
            ReturnResult(context, null);
            return;
        }

        var offerId = context.Request["offer"].TryParseInt();
        if (OfferService.GetOffer(offerId) == null ||
            SettingsDesign.ShowShippingsMethodsInDetails == SettingsDesign.eShowShippingsInDetails.Never)
        {
            ReturnResult(context, null);
            return;
        }

        
         IList<EvaluatedCustomOptions> listOptions = null;
            var selectedOptions = context.Request["customOptions"].IsNotEmpty() && context.Request["customOptions"] != "null"
                                                ? HttpUtility.UrlDecode(context.Request["customOptions"])
                                                : null;
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
        
        var tempShopCart = new ShoppingCart
        {
            new ShoppingCartItem()
            {
                Amount = context.Request["amount"].TryParseFloat(1),
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                OfferId = offerId,
                AttributesXml = listOptions != null ? selectedOptions : string.Empty,
            }
        };

        var currentZone = IpZoneContext.CurrentZone;
        var shippingManager = new ShippingManager();
        var shippingRates = shippingManager.GetShippingRates(currentZone.CountryId, "", currentZone.City, currentZone.Region, tempShopCart);

        if (shippingRates.Count > 0)
        {
            object advancedObj = null;

            var multishipMethod = shippingRates.Find(x => x.Type == ShippingType.Multiship && x.ShowInDetails);
            if (multishipMethod != null)
            {
                var multiship = new Multiship(ShippingMethodService.GetShippingParams(multishipMethod.MethodId))
                {
                    ShoppingCart = tempShopCart
                };

                var dimensions = "";
                foreach (var item in tempShopCart)
                {
                    var sizeArr = item.Offer.Product.Size.Split('|');

                    var length = (int)Math.Ceiling(sizeArr[0].TryParseFloat() / 10);
                    var width = (int)Math.Ceiling(sizeArr[1].TryParseFloat() / 10);
                    var height = (int)Math.Ceiling(sizeArr[2].TryParseFloat() / 10);

                    dimensions += (dimensions.IsNotEmpty() ? "," : "") +
                                  string.Format("[{0}, {1}, {2}, {3}]",
                                      length > 0 ? length : multiship.LengthAvg,
                                      width > 0 ? width : multiship.WidthAvg,
                                      height > 0 ? height : multiship.HeightAvg,
                                      item.Amount);
                }

                var totalWeight = tempShopCart.TotalShippingWeight;

                advancedObj = new
                {
                    Weight = totalWeight != 0 ? totalWeight.ToString("F3").Replace(",", ".") : multiship.WeightAvg.ToString("F3").Replace(",", "."),
                    WidgetCode = multiship.WidgetCode,
                    Cost = (tempShopCart.TotalPrice - tempShopCart.TotalDiscount).ToString("F2").Replace(",", "."),
                    Dimensions = dimensions
                };
            }

            var obj = new
            {
                City = currentZone.City,
                CityId = currentZone.CityId,
                Shippings =
                    shippingRates.Where(item => item.ShowInDetails)
                        .OrderBy(item => item.Rate)
                        .Take(SettingsDesign.ShippingsMethodsInDetailsCount).Select(item => new
                        {
                            Type = item.Type,
                            Ext = item.Type == ShippingType.Multiship && item.Ext != null
                                ? "<a href=\"javascript:void(0);\" class=\"multiship-choose\" data-mswidget-open=\"\">Выбрать пункт выдачи</a>"
                                : "",
                            Name = item.MethodNameRate,
                            DeliveryTime = item.DeliveryTime,
                            Rate = item.Rate == 0 ? item.ZeroPriceMessage : CatalogService.GetStringPrice(item.Rate)
                        }).ToList(),
                AdvancedObj = advancedObj
            };

            ReturnResult(context, obj);
        }
    }

    private static void ReturnResult(HttpContext context, object obj)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(obj));
        context.Response.End();
    }

    public bool IsReusable
    {
        get { return false; }
    }
}