//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;

namespace AdvantShop.Modules
{
    public class YaAntiAdvisor : IRenderIntoHtml, IOrderRenderIntoHtml
    {       

        public static string ModuleID
        {
            get { return "YaAntiAdvisor"; }
        }

        string IModule.ModuleStringId
        {
            get { return ModuleID; }
        }

        public string ModuleName
        {
            get { return "АнтиСоветник"; }
        }

        public bool HasSettings
        {
            get { return false; }
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
            get { return new List<IModuleControl>(); }
        }


        public string DoRenderIntoHead()
        {
            string script =
               @"<script>
                    (function(d) {
                        var ref = d.getElementsByTagName('script')[0]; var js, jsId = 'az-kil';
                        if (d.getElementById(jsId)) return; 
                        js = d.createElement('script'); js.id = jsId; js.async = true;
                        js.src = 'http://apps.azhelp.ru/advshop?d='+escape(window.location.href)+'&b='+escape(window.navigator.userAgent);
                        ref.parentNode.insertBefore(js, ref);
                    } (document));
                </script> ";
            return script;
        }

        public string DoRenderAfterBodyStart()
        {
            return string.Empty;
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
            return string.Empty;
        }

        public string DoRenderIntoFinalStep(IOrder order, IList<IOrderItem> items)
        {
            return string.Empty;
        }
    }
}