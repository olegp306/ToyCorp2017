//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Xml;
using AdvantShop.Core.Caching;

namespace AdvantShop.Core
{
    public class ModeConfigService
    {
        private const string ModeConfigCacheKey = "ModeConfig.";

        public enum Modes
        {
            DemoMode,
            TrialMode,
            SaasMode
        }

        public static bool IsModeEnabled(Modes mode)
        {
            var cacheKey = ModeConfigCacheKey + mode;
            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<bool>(cacheKey);

            var myXmlDocument = new XmlDocument();
            myXmlDocument.Load(Configuration.SettingsGeneral.AbsolutePath + "Web.ModeSettings.config");
            var root = myXmlDocument.ChildNodes.OfType<XmlNode>().First(p => p.Name.Equals("modesettings"));
            if (root != null)
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name == mode.ToString() && node.Attributes != null)
                    {
                        var value = Boolean.Parse(node.Attributes["value"].Value);
                        CacheManager.Insert(cacheKey, value);

                        return value;
                    }
                }
            }
            throw new NotImplementedException("this mode is not configured in ~/Web.ModeSettings.config");
        }

    }
}