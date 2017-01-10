//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using Newtonsoft.Json;
using Resources;

namespace AdvantShop.Modules
{
    public class ShoppingCartPopup : IShoppingCartPopup, IRenderIntoHtml
    {
        private class RelatedProductsModel
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string ArtNo { get; set; }
            public string Link { get; set; }
            public string Price { get; set; }
            public string Photo { get; set; }
            public string Buttons { get; set; }
        }

        #region Module methods

        public static string ModuleID
        {
            get { return "ShoppingCartPopup"; }
        }

        string IModule.ModuleStringId
        {
            get { return ModuleID; }
        }

        public string ModuleName
        {
            get
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "ru":
                        return "Всплывающая корзина";

                    case "en":
                        return "ShoppingCartPopup";

                    default:
                        return "ShoppingCartPopup";
                }
            }
        }

        public List<IModuleControl> ModuleControls
        {
            get
            {
                return new List<IModuleControl> { new ShoppingCartPopupSetting() };
            }
        }

        private class ShoppingCartPopupSetting : IModuleControl
        {
            #region Implementation of IModuleControl

            public string NameTab
            {
                get
                {
                    switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                    {
                        case "ru":
                            return "Настройки";

                        case "en":
                            return "Settings";

                        default:
                            return "Settings";
                    }
                }
            }

            public string File
            {
                get { return "ShoppingCartPopupModule.ascx"; }
            }

            #endregion
        }

        public bool HasSettings
        {
            get { return true; }
        }

        public bool CheckAlive()
        {
            return ModulesRepository.IsInstallModule(ModuleID);
        }

        public bool InstallModule()
        {
            ModuleSettingsProvider.SetSettingValue("showmode", "related", ModuleID);
            return true;
        }

        public bool UninstallModule()
        {
            ModuleSettingsProvider.RemoveSqlSetting("showmode", ModuleID);
            return true;
        }

        public bool UpdateModule()
        {
            return true;
        }

        #endregion

        public string DoRenderIntoHead()
        {
            return string.Empty;
        }

        public string DoRenderAfterBodyStart()
        {
            return string.Empty;
        }

        public string DoRenderBeforeBodyEnd()
        {
            return String.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" /> <script type=\"text/javascript\" src=\"{1}\"></script> ", 
                                "Modules/ShoppingCartPopup/cartpopup.css",
                                "Modules/ShoppingCartPopup/localization/" + CultureInfo.CurrentCulture.ToString() + "/lang.js");
        }

        public string GetShoppingCartPopupJson(Offer offer, float amount, bool isSocialTemplate)
        {
            var product = offer.Product;

            var shpCart = ShoppingCartService.CurrentShoppingCart;

            var totalItems = shpCart.TotalItems;
            var totalDiscount = shpCart.TotalDiscount;
            var totalPrice = shpCart.TotalPrice;

            var totalCounts = string.Format("{0} {1}",
                                    totalItems == 0 ? "" : totalItems.ToString(CultureInfo.InvariantCulture),
                                    Strings.Numerals(totalItems, Resource.Client_UserControls_ShoppingCart_Empty,
                                                    Resource.Client_UserControls_ShoppingCart_1Product,
                                                    Resource.Client_UserControls_ShoppingCart_2Products,
                                                    Resource.Client_UserControls_ShoppingCart_5Products));

            var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

            var showMode = ModuleSettingsProvider.GetSettingValue<string>("showmode", ModuleID);

            var relatedProducts = new List<RelatedProductsModel>();

            if (showMode != "none")
            {
                relatedProducts =
                    ProductService.GetRelatedProducts(product.ProductId, showMode == "related" ? RelatedType.Related : RelatedType.Alternative)
                        .Select(item => new RelatedProductsModel
                        {
                            ProductId = item.ProductId,
                            Name = item.Name,
                            ArtNo = item.ArtNo,
                            Link = UrlService.GetLink(ParamType.Product, item.UrlPath, item.ProductId),
                            Price = CatalogService.RenderPrice(item.MainPrice, item.Discount, true, customerGroup),
                            Photo = item.Photo != null
                                ? String.Format("<img src='{0}' alt='{1}' class='img-cart' />",
                                    FoldersHelper.GetImageProductPath(ProductImageType.Small, item.Photo, false),
                                    item.PhotoDesc)
                                : "<img src='images/nophoto_xsmall.jpg' alt='' class='img-cart' />",
                            Buttons = GetItemButtons(item)
                        }).ToList();
            }


            var obj = new
            {
                status = "success",
                showCart = true,
                tpl = "Modules/ShoppingCartPopup/cartpopup.tpl",

                product.Name,
                Sku = offer.ArtNo,
                TotalItems = totalItems,
                Price = CatalogService.RenderPrice(offer.Price, product.CalculableDiscount, true, customerGroup),
                Photo =
                    offer.Photo != null
                        ? String.Format("<img src='{0}' alt='{1}' class='img-cart' />",
                                        FoldersHelper.GetImageProductPath(ProductImageType.Small, offer.Photo.PhotoName, false),
                                        offer.Product.Name)
                        : "<img src='images/nophoto_xsmall.jpg' alt='' class='img-cart' />",

                Link = isSocialTemplate
                            ? "social/detailssocial.aspx?productid=" + product.ProductId
                            : UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId),

                ColorHeader = SettingsCatalog.ColorsHeader,
                ColorName = offer.Color != null ? offer.Color.ColorName : null,
                SizeHeader = SettingsCatalog.SizesHeader,
                SizeName = offer.Size != null ? offer.Size.SizeName : null,

                TotalCount = totalCounts,
                TotalPrice = CatalogService.GetStringPrice(totalPrice - totalDiscount > 0 ? totalPrice - totalDiscount : 0),

                RelatedTitle = showMode == "related" 
                                    ? SettingsCatalog.RelatedProductName 
                                    : SettingsCatalog.AlternativeProductName,
                RelatedProducts = relatedProducts
            };

            return JsonConvert.SerializeObject(obj);
        }

        private string GetItemButtons(Product product)
        {
            var buttons = new StringBuilder();
            buttons.AppendFormat("<div class=\"pv-btns\">");

            var url = UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId);

            if (SettingsCatalog.DisplayBuyButton && product.MainPrice > 0 && product.TotalAmount > 0)
            {
                buttons.AppendFormat(
                    "<span class=\"btn-c\"><a href=\"{0}\" class=\"btn btn-add btn-small\" id=\"btnAdd\" data-cart-add-productid=\"{1}\">{2}</a></span> ",
                    url, product.ProductId, SettingsCatalog.BuyButtonText);
            }

            if (SettingsCatalog.DisplayPreOrderButton &&
                (!(product.MainPrice > 0 && product.TotalAmount > 0) && product.AllowPreOrder))
            {
                buttons.AppendFormat(
                    "<span class=\"btn-c\"><a href=\"sendrequestonproduct.aspx?productid={0}\" class=\"btn btn-action btn-small\" id=\"btnAction\">{1}</a></span> ",
                    product.ProductId, SettingsCatalog.PreOrderButtonText);
            }

            if (SettingsCatalog.DisplayMoreButton)
            {
                buttons.AppendFormat(
                    "<span class=\"btn-c\"><a href=\"{0}\" class=\"btn btn-buy btn-small\" id=\"btnBuy\">{1}</a></span>",
                    url, SettingsCatalog.MoreButtonText);
            }

            buttons.Append("</div>");
            return buttons.ToString();
        }     
    }
}