<%@ WebHandler Language="C#" Class="ColorSizePrice" %>

using System;
using System.Web;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using Newtonsoft.Json;
using Resources;

public class ColorSizePrice : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";

        var offerId = context.Request["offerId"].TryParseInt();
        var attributesXml = HttpUtility.UrlDecode(context.Request["AttributesXml"]);

        var offer = OfferService.GetOffer(offerId);

        if (offer == null)
        {
            context.Response.Write(JsonConvert.SerializeObject(RenderObj(0, string.Empty, string.Empty)));
            context.Response.End();
        }

        float discountByTime = DiscountByTimeService.GetDiscountByTime();
        float totalDiscount = offer.Product.Discount != 0 ? offer.Product.Discount : discountByTime;
        string bonusPrice = string.Empty;

        var customer = CustomerContext.CurrentCustomer;


        foreach (var discountModule in AttachedModules.GetModules<IDiscount>())
        {
            var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
            var productDiscountModels = classInstance.GetProductDiscountsList();
            if (productDiscountModels != null)
            {
                var prodDiscount = productDiscountModels.Find(d => d.ProductId == offer.Product.ProductId);
                if (prodDiscount != null)
                {
                    totalDiscount = prodDiscount.Discount;
                }
            }
            break;
        }

        var priceHtml = string.Empty;

        if (CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled)
        {
            priceHtml = CatalogService.RenderPriceInplace(offer.Price, totalDiscount, true, customer.CustomerGroup, attributesXml, offer.OfferId != 0 ? offer.OfferId : offer.Product.ProductId);
        }
        else
        {
            priceHtml = CatalogService.RenderPrice(offer.Price, totalDiscount, true,
            customer.CustomerGroup, attributesXml);
        }


        var priceNumber = CatalogService.CalculateProductPrice(offer.Price, totalDiscount,
            customer.CustomerGroup, CustomOptionsService.DeserializeFromXml(attributesXml));

        if (BonusSystem.IsActive && offer.Price > 0)
        {
            var bonusCard = BonusSystemService.GetCard(customer.BonusCardNumber);
            if (bonusCard != null)
            {
                bonusPrice = CatalogService.RenderBonusPrice(bonusCard.BonusPercent, priceNumber,
                        totalDiscount, CustomerContext.CurrentCustomer.CustomerGroup);
            }
            else if (BonusSystem.BonusFirstPercent != 0)
            {
                bonusPrice = CatalogService.RenderBonusPrice(BonusSystem.BonusFirstPercent, priceNumber,
                        totalDiscount, CustomerContext.CurrentCustomer.CustomerGroup);
            }
        }

        context.Response.Write(JsonConvert.SerializeObject(RenderObj(priceNumber, priceHtml, bonusPrice)));
    }

    private object RenderObj(float priceNumber, string priceHtml, string bonusPriceHtml)
    {
        return new { PriceString = priceHtml, PriceNumber = priceNumber, Bonuses = bonusPriceHtml };
    }


    public bool IsReusable
    {
        get { return false; }
    }
}