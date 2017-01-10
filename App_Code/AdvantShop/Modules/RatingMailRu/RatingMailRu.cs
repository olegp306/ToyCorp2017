//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using Quartz.Xml.JobSchedulingData20;

namespace AdvantShop.Modules
{
    public class RatingMailRu : IRenderIntoHtml, IOrderRenderIntoHtml
    {
        #region Module methods

        public static string ModuleID
        {
            get { return "RatingMailRu"; }
        }

        string IModule.ModuleStringId
        {
            get { return ModuleID; }
        }

        public string ModuleName
        {
            get { return "Рейтинг@Mail.Ru"; }
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
            return true;
        }

        public bool UninstallModule()
        {
            return true;
        }

        public bool UpdateModule()
        {
            return true;
        }

        public List<IModuleControl> ModuleControls
        {
            get { return new List<IModuleControl> {new YaMetrikaSetting()}; }
        }

        private class YaMetrikaSetting : IModuleControl
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
                get { return "RatingMailRuSettings.ascx"; }
            }

            #endregion
        }

        #endregion

        public string DoRenderIntoHead()
        {
            return string.Empty;
        }

        public string DoRenderAfterBodyStart()
        {
            var counter = ModuleSettingsProvider.GetSettingValue<string>("COUNTER", ModuleID);

            if (string.IsNullOrEmpty(counter))
                return string.Empty;

            return "<div style='display:none !important;'>" + counter + "</div>";
        }

        public string DoRenderBeforeBodyEnd()
        {
            string res;

            var listid = ModuleSettingsProvider.GetSettingValue<string>("PriceListID", ModuleID);

            string url = HttpContext.Current.Request.Url.ToString().ToLower();

            if (url.Contains("default.aspx"))
            {
                res = string.Format("<script type='text/javascript'>var _tmr = _tmr || [];_tmr.push({{type: 'itemView', productid: '', pagetype: 'home',totalvalue: '', list: '{0}' }});</script>", listid);
            }
            else if (url.Contains("catalog.aspx"))
            {
                res =
                   string.Format("<script type='text/javascript'>var _tmr = _tmr || [];_tmr.push({{type: 'itemView', productid: '', pagetype: 'category', totalvalue:'', list: '{0}' }});</script>",listid);
            }
            else if (url.Contains("details.aspx"))
            {
                Offer mainOffer;
                var product = ProductService.GetProduct(HttpContext.Current.Request["ProductID"].TryParseInt());
                if (product != null && 
                    (mainOffer = OfferService.GetMainOffer(product.Offers, product.AllowPreOrder, 
                                    HttpContext.Current.Request["color"].TryParseInt(true), HttpContext.Current.Request["size"].TryParseInt(true))) !=null)
                {
                    res =
                        string.Format("<script type='text/javascript'>var _tmr = _tmr || [];_tmr.push({{type: 'itemView',productid: '{0}',pagetype: 'product', totalvalue:'{1}',list: '{2}' }});</script>", mainOffer.OfferId, mainOffer.Price.ToString("F2").Replace(",", "."), listid);
                }
                else
                {
                    res = string.Empty;
                }
            }
            else if (url.Contains("shoppingcart.aspx"))
            {
                var cart = ShoppingCartService.CurrentShoppingCart;

                res = string.Format("<script type='text/javascript'>var _tmr = _tmr || [];_tmr.push({{type: 'itemView',productid: [{0}], pagetype: 'cart', totalvalue:'{1}', list: '{2}' }});</script>", cart.Select(o => "'" + o.OfferId + "'").AggregateString(","), cart.TotalPrice.ToString("F2").Replace(",", "."), listid);
            }
            else
            {
                res = string.Format("<script type='text/javascript'>var _tmr = _tmr || [];_tmr.push({{type: 'itemView', productid: '', pagetype: 'other', totalvalue: '', list: '{0}' }});</script>", listid);
            }

            return res;
        }

        public string DoRenderIntoFinalStep()
        {
            return string.Empty;
        }

        public string DoRenderIntoFinalStep(IOrder order)
        {
            var listid = ModuleSettingsProvider.GetSettingValue<string>("PriceListID", ModuleID);
            var offers = order.OrderItems.Select(item => OfferService.GetOffer(item.ArtNo));
            return string.Format("<script type='text/javascript'>var _tmr = _tmr || [];_tmr.push({{type: 'itemView', productid: [{0}], pagetype: 'purchase', totalvalue: '{1}', list: '{2}' }});</script>", offers.Select(o => "'" + o.OfferId + "'").AggregateString(","), order.OrderItems.Sum(item => item.Price * item.Amount), listid);
        }

    }
}