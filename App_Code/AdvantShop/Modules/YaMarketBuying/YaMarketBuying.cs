//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;

namespace AdvantShop.Modules
{
    public class YaMarketBuying : IModuleUrlRewrite, IOrderChanged
    {
        #region Module methods

        public static string ModuleStringId
        {
            get { return "YaMarketBuying"; }
        }

        string IModule.ModuleStringId
        {
            get { return ModuleStringId; }
        }

        public List<IModuleControl> ModuleControls
        {
            get { return new List<IModuleControl> {new YaMarketBuingModuleSetting(), new YaMarketBuingHistory()}; }
        }

        public bool HasSettings
        {
            get { return true; }
        }

        public bool CheckAlive()
        {
            return true;
        }

        public bool InstallModule()
        {
            return YaMarketByuingService.InstallModule();
        }

        public bool UninstallModule()
        {
            return true;
        }

        public bool RewritePath(string rawUrl, ref string newUrl)
        {
            return YaMarketByuingApiService.RewritePath(rawUrl, ref newUrl);
        }

        public bool UpdateModule()
        {
            return true;
        }
        
        public string ModuleName
        {
            get
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "ru":
                        return "Покупка в Яндекс.Маркете";

                    case "en":
                        return "Yandex.Market buying";

                    default:
                        return "Yandex.Market buying";
                }
            }
        }

        private class YaMarketBuingModuleSetting : IModuleControl
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
                get { return "YaMarketBuyingModuleSetting.ascx"; }
            }

            #endregion
        }
        private class YaMarketBuingHistory : IModuleControl
        {
            #region Implementation of IModuleControl

            public string NameTab
            {
                get
                {
                    switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                    {
                        case "ru":
                            return "История заказов";

                        case "en":
                            return "History";

                        default:
                            return "History";
                    }
                }
            }

            public string File
            {
                get { return "YaMarketBuyingHistory.ascx"; }
            }

            #endregion
        }

        #endregion

        #region IOrderChanged

        public void DoOrderAdded(IOrder order)
        {
        }

        public void DoOrderChangeStatus(IOrder order)
        {
            var status = order.GetOrderStatus();
            if (status != null)
                YaMarketByuingApiService.ChangeStatus(status, order);
        }

        public void DoOrderUpdated(IOrder order)
        {
        }

        public void DoOrderDeleted(int orderId)
        {
        }

        #endregion
    }
}