﻿//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using AdvantShop.Helpers;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Core
{

    public class AdvantshopConfigService
    {
        private enum SectionName
        {
            localization,
            payments,
            shippings,
            modules,
            commonsettings,
            authproviders
        }

        private static List<XmlNode> GetNodesFromSection(SectionName sectionName)
        {
            var myXmlDocument = new XmlDocument();
            myXmlDocument.Load(Configuration.SettingsGeneral.AbsolutePath + "App_Data\\advantshop.config");
            var root = myXmlDocument.ChildNodes.OfType<XmlNode>().FirstOrDefault(p => p.Name.Equals("advantshop"));
            if (root != null)
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals(sectionName.ToString()))
                    {
                        return node.ChildNodes.OfType<XmlNode>().ToList();
                    }
                }
            }
            return new List<XmlNode>();
        }

        public static string GetLocalization()
        {
            var lang = GetNodesFromSection(SectionName.localization).FirstOrDefault();
            return (lang != null) ? lang.Attributes["value"].Value : "ru-RU";
        }

        public static List<XmlNode> GetPayments()
        {
            return GetNodesFromSection(SectionName.payments);
        }

        public static List<XmlNode> GetShippings()
        {
            return GetNodesFromSection(SectionName.shippings);
        }


        public static List<XmlNode> GetModules()
        {
            return GetNodesFromSection(SectionName.modules);
        }

        public static List<XmlNode> GetCommonSettings()
        {
            return GetNodesFromSection(SectionName.commonsettings);
        }


        public static List<XmlNode> GetAuthProviders()
        {
            return GetNodesFromSection(SectionName.authproviders);
        }

        public static List<ListItem> GetDropdownPayments()
        {
            var payments = new List<ListItem>();
            foreach (var payment in GetPayments())
            {
                PaymentType value;
                if (payment.Attributes == null || !PaymentType.TryParse(payment.Attributes["name"].Value, out value) || !SQLDataHelper.GetBoolean(payment.Attributes["enabled"].Value))
                {
                    continue;
                }

                payments.Add(new ListItem
                    {
                        Value = ((int) value).ToString(),
                        Text = payment.Attributes["localizename"].Value
                    });
            }
            return payments;
        }

        public static List<object> GetDropdownShippings()
        {
            var shippings = new List<object>();
            foreach (var shipping in GetShippings())
            {
                ShippingType value;
                if (shipping.Attributes == null || !ShippingType.TryParse(shipping.Attributes["name"].Value, out value) || !SQLDataHelper.GetBoolean(shipping.Attributes["enabled"].Value))
                {
                    continue;
                }
                shippings.Add(new { value = ((int)value).ToString(), name = shipping.Attributes["localizename"].Value });
            }
            return shippings;
        }

        #region Get all activities in section

        public static Dictionary<string, bool> GetActivities(List<XmlNode> collection)
        {
            var result = new Dictionary<string, bool>();
            foreach (var item in collection)
            {
                if (item.Attributes != null && !result.ContainsKey(item.Attributes["name"].Value))
                {
                    result.Add(item.Attributes["name"].Value, SQLDataHelper.GetBoolean(item.Attributes["enabled"].Value));
                }
            }
            return result;
        }

        public static Dictionary<string, bool> GetActivityModules()
        {
            return GetActivities(GetModules());
        }


        public static Dictionary<string, bool> GetActivityCommonSettings()
        {
            return GetActivities(GetCommonSettings());
        }

        public static Dictionary<string, bool> GetActivityAuthProviders()
        {
            return GetActivities(GetAuthProviders());
        }

        #endregion

        #region Get single activity setting

        public static bool GetActivity(List<XmlNode> collection, string value)
        {
            var setting = collection.FirstOrDefault(item => item.Attributes != null && item.Attributes["name"].Value.Equals(value));
            if (setting != null)
            {
                return SQLDataHelper.GetBoolean(setting.Attributes["enabled"].Value);
            }
            return false;
        }

        public static bool GetActivityCommonSetting(string value)
        {
            return GetActivity(GetCommonSettings(), value);
        }

        public static bool GetActivityModule(string value)
        {
            return GetActivity(GetModules(), value);
        }

        public static bool GetActivityPayment(string value)
        {
            return GetActivity(GetPayments(), value);
        }

        public static bool GetActivityShipping(string value)
        {
            return GetActivity(GetShippings(), value);
        }

        public static bool GetActivityAuthProvider(string value)
        {
            return GetActivity(GetAuthProviders(), value);
        }
        #endregion
    }
}