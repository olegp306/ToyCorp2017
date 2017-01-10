//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;

namespace AdvantShop.Modules
{
    public class YaMetrika : IRenderIntoHtml, IOrderRenderIntoHtml
    {
        #region Module methods

        public static string ModuleID
        {
            get { return "YaMetrika"; }
        }

        string IModule.ModuleStringId
        {
            get { return ModuleID; }
        }

        public string ModuleName
        {
            get { return "Яндекс.Метрика"; }
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
                get { return "YaMetrikaSettings.ascx"; }
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
            var counterId = ModuleSettingsProvider.GetSettingValue<string>("COUNTER_ID", ModuleID);
            var counter = ModuleSettingsProvider.GetSettingValue<string>("COUNTER", ModuleID);

            if (string.IsNullOrEmpty(counterId) || string.IsNullOrEmpty(counter))
                return string.Empty;

            return "<div style='display:none !important;'>" + counter + "</div>" + string.Format("<script type=\"text/javascript\" src=\"{0}\"></script> " +
                                           "<div class=\"yacounterid\" data-counterId=\"{1}\"></div>",
                                           "modules/yametrika/js/tracking.js",
                                           counterId);
        }

        public string DoRenderBeforeBodyEnd()
        {
            return string.Empty;
        }

        public string DoRenderIntoFinalStep()
        {
            return string.Empty;
        }

        public string DoRenderIntoFinalStep(IOrder order)
        {
            var counterId = ModuleSettingsProvider.GetSettingValue<string>("COUNTER_ID", ModuleID);
            if (string.IsNullOrEmpty(counterId))
                return string.Empty;

            return string.Format(
                "<script type=\"text/javascript\">\r\n" +
                "$(setTimeout(function () {{\r\n" +
                "var yaParams = {{ order_id:\"{0}\", order_price: {1}, currency: \"{2}\", exchange_rate: 1, goods: [{3}]}};\r\n" +
                "yaCounter{4}.reachGoal('Order', yaParams);\r\n" +
                "}},3000));\r\n" +
                "</script>\r\n",
                order.OrderID, 
                order.Sum.ToString().Replace(",", "."), 
                order.OrderCurrency.CurrencyCode,
                string.Join(", ",
                    order.OrderItems.Select(orderItem => string.Format("{{ id: '{0}', name: '{1}', price: {2}, quantity: {3} }}",
                                                           HttpUtility.HtmlEncode( orderItem.ArtNo), HttpUtility.HtmlEncode(orderItem.Name), orderItem.Price.ToString().Replace(",", "."), orderItem.Amount.ToString().Replace(",", ".")))),
                counterId);
        }

        public string DoRenderIntoFinalStep(IOrder order, IList<IOrderItem> items)
        {
            return string.Empty;
        }
    }
}