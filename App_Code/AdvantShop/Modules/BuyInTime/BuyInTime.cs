//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Modules.Interfaces;

namespace AdvantShop.Modules
{
    public class BuyInTime : IDiscount, IRenderIntoMainPage, IModuleDetails, IRenderIntoHtml, ILabel
    {
        #region Module methods

        public static string ModuleStringId
        {
            get { return "BuyInTime"; }
        }

        string IModule.ModuleStringId
        {
            get { return ModuleStringId; }
        }

        public List<IModuleControl> ModuleControls
        {
            get { return new List<IModuleControl> { new BuyInTimeSetting() }; }
        }

        public bool HasSettings
        {
            get { return true; }
        }

        public bool CheckAlive()
        {
            return ModulesRepository.IsInstallModule(ModuleStringId);
        }

        public bool InstallModule()
        {
            return BuyInTimeService.InstallBuyInTimeModule() && BuyInTimeService.UpdateBuyInTimeModule();
        }

        public bool UninstallModule()
        {
            return BuyInTimeService.UninstallBuyInTimeModule();
        }

        public bool UpdateModule()
        {
            return BuyInTimeService.UpdateBuyInTimeModule();
        }
        
        public string ModuleName
        {
            get
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "ru":
                        return "Успей купить";

                    case "en":
                        return "BuyInTime";

                    default:
                        return "BuyInTime";
                }
            }
        }

        private class BuyInTimeSetting : IModuleControl
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
                get { return "BuyInTimeModule.ascx"; }
            }

            #endregion
        }

        #endregion

        #region Implementation of IDiscount

        public float GetDiscount(int productId)
        {
            var model = BuyInTimeService.GetByProduct(productId, DateTime.Now);
            return model != null ? model.Discount : 0;
        }

        public List<ProductDiscount> GetProductDiscountsList()
        {
            return BuyInTimeService.GetProductDiscountsList(DateTime.Now);
        }

        #endregion

        #region Implementation of IModuleDetails

        public string RenderToRightColumn()
        {
            return string.Empty;
        }

        public string RenderToProductInformation(int productId)
        {
            var discountModel = BuyInTimeService.GetByProduct(productId, DateTime.Now);
            if (discountModel == null)
                return string.Empty;
            
            var actionTitle = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeActionTitle", ModuleStringId);
            var countdown = discountModel.DateExpired != null
                                ? string.Format(BuyInTimeService.CountdownScript, ((DateTime)discountModel.DateExpired).ToString("dd.MM.yyyy HH:mm:ss"))
                                : string.Empty;

            return string.Format("<div class=\"buy-in-time-product\"><div class=\"buy-in-time-action-b\">{0}</div> {1}</div>", actionTitle, countdown);
        }

        public string RenderToProductInformation()
        {
            return string.Empty;
        }

        public string RenderToBottom()
        {
            return string.Empty;
        }

        #endregion

        #region Implementation of IRenderIntoHtml

        public string DoRenderIntoHead()
        {
            return
                String.Format(
                    "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" /> ",
                    "modules/buyintime/css/styles.css");
        }

        public string DoRenderAfterBodyStart()
        {
            return string.Empty;
        }

        public string DoRenderBeforeBodyEnd()
        {
            return
                String.Format(
                    "<script type=\"text/javascript\" src=\"{0}\"></script> "+
                    "<script type=\"text/javascript\" src=\"{1}\"></script> ",
                    "modules/buyintime/js/plugins/countdown/countdown.js",
                    "modules/buyintime/js/plugins/countdown/countdownInit.js?1");
        }

        #endregion

        #region Implementation of IRenderIntoMainPage

        public string GetMainPageProductsUcPath()
        {
            return string.Empty;
        }

        public string RenderMainPageProducts()
        {
            var cacheKey = BuyInTimeService.CacheKey + BuyInTimeService.eShowMode.Horizontal;
            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<string>(cacheKey);

            var action = BuyInTimeService.GetByShowMode((int)BuyInTimeService.eShowMode.Horizontal, DateTime.Now);
            var actionVertical = BuyInTimeService.GetByShowMode((int)BuyInTimeService.eShowMode.Vertical, DateTime.Now);

            // thx to support
            if (actionVertical != null && action != null && action.Id < actionVertical.Id)
            {
                CacheManager.Insert(cacheKey, "");
                return string.Empty;
            }

            if (action != null)
            {
                if (action.IsRepeat && action.DateExpired < DateTime.Now)
                {
                    action.DateExpired = BuyInTimeService.GetExpireDateTime(action.DateExpired, action.DaysRepeat);
                }

                var actionTitle = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeActionTitle", ModuleStringId);
                var countdown = string.Format(BuyInTimeService.CountdownScript, action.DateExpired.ToString("dd.MM.yyyy HH:mm:ss"));

                var actionText = action.ActionText.Replace("#ActionTitle#", actionTitle)
                                                  .Replace("#Countdown#", countdown);

                var product = ProductService.GetProduct(action.ProductId);
                if (product == null)
                    return actionText;

                var offer = product.Offers.FirstOrDefault(o => o.Main) ?? product.Offers.FirstOrDefault();
                if (offer == null)
                    return actionText;

                actionText =
                    actionText.Replace("#ProductPicture#", action.Picture.IsNotEmpty()
                        ? string.Format("<img src=\"modules/{0}/pictures/{1}\" />", ModuleStringId.ToLower(), action.Picture)
                        : string.Empty)
                        .Replace("#ProductLink#",
                            UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId))
                        .Replace("#ProductName#", product.Name)
                        .Replace("#OldPrice#", CatalogService.GetStringPrice(offer.Price).Replace("руб", "р"))
                        .Replace("#NewPrice#",
                            CatalogService.GetStringPrice(offer.Price, action.DiscountInTime).Replace("руб", "р"))
                        .Replace("#DiscountPrice#",
                            CatalogService.GetStringPrice(offer.Price*action.DiscountInTime/100).Replace("руб", "р"))
                        .Replace("#DiscountPercent#", action.DiscountInTime.ToString("0.##"));

                CacheManager.Insert(cacheKey, actionText);

                return actionText;
            }

            return string.Empty;
        }

        public string RenderMainPageAfterCarousel()
        {
            var cacheKey = BuyInTimeService.CacheKey + BuyInTimeService.eShowMode.Vertical;
            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<string>(cacheKey);

            var action = BuyInTimeService.GetByShowMode((int)BuyInTimeService.eShowMode.Vertical, DateTime.Now);
            var actionHorizontal = BuyInTimeService.GetByShowMode((int)BuyInTimeService.eShowMode.Horizontal, DateTime.Now);

            if (actionHorizontal != null && action != null && action.Id < actionHorizontal.Id)
            {
                CacheManager.Insert(cacheKey, "");
                return string.Empty;
            }

            if (action != null)
            {
                if (action.IsRepeat && action.DateExpired < DateTime.Now)
                {
                    action.DateExpired = BuyInTimeService.GetExpireDateTime(action.DateExpired, action.DaysRepeat);
                }

                var actionTitle = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeActionTitle", ModuleStringId);
                var countdown = string.Format(BuyInTimeService.CountdownScript, action.DateExpired.ToString("dd.MM.yyyy HH:mm:ss"));

                var actionText = action.ActionText.Replace("#ActionTitle#", actionTitle)
                                                  .Replace("#Countdown#", countdown);

                var product = ProductService.GetProduct(action.ProductId);
                if (product == null)
                    return actionText;

                var offer = product.Offers.FirstOrDefault(o => o.Main) ?? product.Offers.FirstOrDefault();
                if (offer == null)
                    return actionText;

                actionText =
                    actionText.Replace("#ProductPicture#", action.Picture.IsNotEmpty()
                        ? string.Format("<img src=\"modules/{0}/pictures/{1}\" />", ModuleStringId.ToLower(), action.Picture)
                        : string.Empty)
                        .Replace("#ProductLink#",
                            UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId))
                        .Replace("#ProductName#", product.Name)
                        .Replace("#OldPrice#", CatalogService.GetStringPrice(offer.Price).Replace("руб", "р"))
                        .Replace("#NewPrice#",
                            CatalogService.GetStringPrice(offer.Price, action.DiscountInTime).Replace("руб", "р"))
                        .Replace("#DiscountPrice#",
                            CatalogService.GetStringPrice(offer.Price*action.DiscountInTime/100).Replace("руб", "р"))
                        .Replace("#DiscountPercent#", action.DiscountInTime.ToString("0.##"));

                CacheManager.Insert(cacheKey, actionText);

                return actionText;
            }

            return string.Empty;
        }

        #endregion

        #region Implementation of ILabel

        public ProductLabel GetLabel()
        {
            var labelCode = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeLabel", ModuleStringId);
            if (labelCode.IsNullOrEmpty())
                return null;

            var productDiscounts = BuyInTimeService.GetProductDiscountsList(DateTime.Now);
            if (productDiscounts == null || productDiscounts.Count == 0)
                return null;

            return new ProductLabel()
            {
                LabelCode = labelCode,
                ProductIds = productDiscounts.Select(p => p.ProductId).ToList()
            };
        }

        #endregion
    }
}