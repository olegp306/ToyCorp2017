<%@ WebHandler Language="C#" Class="GetCart" %>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using Newtonsoft.Json;
using Resources;


public class GetCart : IHttpHandler, IRequiresSessionState
{
    protected float TotalPrice = 0;
    protected float DiscountOnTotalPrice = 0;
    protected float TotalDiscount = 0;
    protected float TotalItems = 0;
    
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";
        var shpCart = ShoppingCartService.CurrentShoppingCart;

        var cartProducts = (from item in shpCart
                            select new
                            {
                                Price = CatalogService.GetStringPrice(item.Price),
                                item.Amount,
                                SKU = item.Offer.ArtNo,
                                Photo = item.Offer.Photo != null
                                     ? String.Format("<img src='{0}' alt='{1}' class='img-cart' />", FoldersHelper.GetImageProductPath(ProductImageType.XSmall, item.Offer.Photo.PhotoName, false), item.Offer.Product.Name)
                                     : "<img src='images/nophoto_xsmall.jpg' alt='' class='img-cart' />",
                                item.Offer.Product.Name,
                                Link = UrlService.GetLink(ParamType.Product, item.Offer.Product.UrlPath, item.Offer.Product.ID),
                                Cost = CatalogService.GetStringPrice(item.Price * item.Amount),
                                item.ShoppingCartItemId,
                                SelectedOptions = CatalogService.RenderSelectedOptions(item.AttributesXml),
                                ColorName = item.Offer.Color != null ? item.Offer.Color.ColorName : null,
                                SizeName = item.Offer.Size != null ? item.Offer.Size.SizeName : null,
                                Avalible = GetAvalible(item),
                                item.Offer.CanOrderByRequest,
                                MinAmount = item.Offer.Product.MinAmount ?? 1,
                                MaxAmount = item.Offer.Product.MaxAmount ?? Int32.MaxValue,
                                item.Offer.Product.Multiplicity,
                                Description = item.Offer.Product.BriefDescription
                            }).ToList();
                                    
        TotalPrice = shpCart.TotalPrice;
        DiscountOnTotalPrice = shpCart.DiscountPercentOnTotalPrice;
        TotalDiscount = shpCart.TotalDiscount;
        TotalItems = shpCart.TotalItems;
        var count = ItemsCount();
        float bonusPlus = 0;

        if (BonusSystem.IsActive && TotalPrice > 0)
        {
            var bonusCard = BonusSystemService.GetCard(CustomerContext.CurrentCustomer.BonusCardNumber);
            if (bonusCard != null)
            {
                bonusPlus = BonusSystemService.GetBonusPlusCost(TotalPrice - TotalDiscount, TotalPrice - TotalDiscount, bonusCard.BonusPercent);
            }
            else if (BonusSystem.BonusFirstPercent != 0)
            {
                bonusPlus = BonusSystemService.GetBonusPlusCost(TotalPrice - TotalDiscount, TotalPrice - TotalDiscount, BonusSystem.BonusFirstPercent);
            }
        }

        var showConfirmButtons = true;
        foreach (var module in AttachedModules.GetModules<IRenderIntoShoppingCart>())
        {
            var moduleObject = (IRenderIntoShoppingCart) Activator.CreateInstance(module, null);
            showConfirmButtons &= moduleObject.ShowConfirmButtons;
        }

        object objects = new
        {
            CartProducts = cartProducts,
            ColorHeader = SettingsCatalog.ColorsHeader,
            SizeHeader = SettingsCatalog.SizesHeader,
            //TotalPrice = TotalPrice,
            TotalProductPrice = TotalPrice,
            TotalProductPriceString = CatalogService.GetStringPrice(TotalPrice),
            //DiscountOnTotalPrice = DiscountOnTotalPrice,
            //TotalDiscount = TotalDiscount,
            Summary = GetSummary(shpCart),
            Count = count,
            TotalItems,
            BonusPlus = bonusPlus > 0 ? CatalogService.GetStringPrice(bonusPlus) : string.Empty,
            Valid = Valid(context, shpCart),
            CouponInputVisible = shpCart.HasItems && shpCart.Coupon == null && shpCart.Certificate == null
                && CustomerContext.CurrentCustomer.CustomerGroup.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup && SettingsOrderConfirmation.DisplayPromoTextbox,
                
            ShowConfirmButtons = showConfirmButtons
        };

        context.Response.Write(JsonConvert.SerializeObject(objects));
    }

    public string Valid(HttpContext context, ShoppingCart shpCart)
    {
        var errorMessage = string.Empty;
        var itemsCount = TotalItems;

        if (itemsCount == 0)
        {
            errorMessage = Resource.Client_ShoppingCart_NoProducts;
        }

        if (TotalPrice < SettingsOrderConfirmation.MinimalOrderPrice)
        {
            errorMessage = string.Format(Resource.Client_ShoppingCart_MinimalOrderPrice,
                                         CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice),
                                         CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice - TotalPrice));
        }
        else if (shpCart.Any(item => GetAvalible(item).IsNotEmpty()))
        {
            errorMessage = string.Format(Resource.Client_ShoppingCart_NotAvailableProducts);
        }

        return errorMessage;
    }

    private string ItemsCount()
    {
        return TotalItems.ToString(CultureInfo.InvariantCulture);
    }

    private List<object> GetSummary(ShoppingCart shpCart)
    {
        var summary = new List<object>();

        if (TotalDiscount != 0)
        {
            summary.Add(new { Key = Resource.Client_UserControls_ShoppingCart_Sum, Value = CatalogService.GetStringPrice(TotalPrice) });
        }

        if (DiscountOnTotalPrice > 0)
        {
            summary.Add(
                new
                    {
                        Key = Resource.Client_UserControls_ShoppingCart_Discount,
                        Value = string.Format("<span class=\"discount\">{0}</span>",
                                  CatalogService.GetStringDiscountPercent(TotalPrice, DiscountOnTotalPrice, true))
                    });
        }

        if (shpCart.Certificate != null)
        {
            summary.Add(
                new
                    {
                        Key = Resource.Client_UserControls_ShoppingCart_Certificate,
                        Value = string.Format("-{0}<a class=\"cross\" data-cart-remove-cert=\"true\" title=\"{1}\"></a>",
                                  CatalogService.GetStringPrice(shpCart.Certificate.Sum),
                                  Resource.Client_ShoppingCart_DeleteCertificate)
                    });
        }

        if (shpCart.Coupon != null)
        {
            if (TotalDiscount == 0)
            {
                summary.Add(
                    new
                        {
                            Key = Resource.Client_UserControls_ShoppingCart_Coupon,
                            Value = string.Format("-{0} ({1}) <img src='images/question_mark.png' title='{3}'> <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                                      CatalogService.GetStringPrice(0), shpCart.Coupon.Code,
                                      Resource.Client_ShoppingCart_DeleteCoupon,
                                      Resource.Client_ShoppingCart_CouponNotApplied)
                        });
            }
            else
            {
                switch (shpCart.Coupon.Type)
                {
                    case CouponType.Fixed:
                        summary.Add(new
                                        {
                                            Key = Resource.Client_UserControls_ShoppingCart_Coupon,
                                            Value = string.Format("-{0} ({1}) <a class=\"cross\" data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                                                      CatalogService.GetStringPrice(TotalDiscount), shpCart.Coupon.Code,
                                                      Resource.Client_ShoppingCart_DeleteCoupon)
                                        });
                        break;
                    case CouponType.Percent:
                        summary.Add(new
                                        {
                                            Key = Resource.Client_UserControls_ShoppingCart_Coupon,
                                            Value = string.Format("-{0} ({1}%) ({2}) <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{3}\"></a>",
                                                        CatalogService.GetStringPrice(TotalDiscount),
                                                        CatalogService.FormatPriceInvariant(shpCart.Coupon.Value),
                                                        shpCart.Coupon.Code, Resource.Client_ShoppingCart_DeleteCoupon)
                                        });
                        break;
                }
            }
        }

        summary.Add(new
                        {
                            Key = string.Format("<span class=\"sum-result\">{0}</span>",
                                      Resource.Client_UserControls_ShoppingCart_Total),
                            Value = string.Format("<span class=\"sum-result\">{0}</span>",
                                      CatalogService.GetStringPrice(TotalPrice - TotalDiscount > 0
                                                                        ? TotalPrice - TotalDiscount
                                                                        : 0))
                        });
        return summary;
    }


    private static string GetAvalible(ShoppingCartItem item)
    {
        if (!item.Offer.Product.Enabled || !item.Offer.Product.CategoryEnabled)
        {
            return Resource.Client_ShoppingCart_NotAvailable + " 0 " + item.Offer.Product.Unit;
        }

        if (item.Offer.CanOrderByRequest)
            return string.Empty;

        if ((SettingsOrderConfirmation.AmountLimitation) && (item.Amount > item.Offer.Amount))
        {
            return Resource.Client_ShoppingCart_NotAvailable + " " + item.Offer.Amount + " " + item.Offer.Product.Unit;
        }

        if (item.Amount > item.Offer.Product.MaxAmount)
        {
            return Resource.Client_ShoppingCart_NotAvailable_MaximumOrder + " " + +item.Offer.Product.MaxAmount + " " + item.Offer.Product.Unit;
        }

        if (item.Amount < item.Offer.Product.MinAmount)
        {
            return Resource.Client_ShoppingCart_NotAvailable_MinimumOrder + " " + +item.Offer.Product.MinAmount + " " + item.Offer.Product.Unit;
        }

        return string.Empty;
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
