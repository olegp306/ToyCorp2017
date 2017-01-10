//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using AdvantShop.Configuration;

namespace AdvantShop.SEO
{
    public class GoogleAnalyticsItem
    {
        public string OrderId { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }

    public class GoogleAnalyticsTrans
    {
        public string OrderId { get; set; }
        public string Affiliation { get; set; }
        public string Total { get; set; }
        public string Tax { get; set; }
        public string Shipping { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

    /// <summary>
    /// GoogleAnalyticsString
    /// </summary>
    public class GoogleAnalyticsString
    {
        public string Number
        {
            get { return SettingsSEO.GoogleAnalyticsNumber; }
        }

        public bool Enabled
        {
            get { return SettingsSEO.GoogleAnalyticsEnabled; }
        }

        public GoogleAnalyticsTrans Trans { get; set; }
        public List<GoogleAnalyticsItem> Items { get; set; }


        private const string GoogleAnalytics =
            "<script type=\"text/javascript\">\n " +

            "(function(i,s,o,g,r,a,m){{i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){{ " +
            "(i[r].q=i[r].q||[]).push(arguments)}},i[r].l=1*new Date();a=s.createElement(o), " +
            "m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m) " +
            "}})(window,document,'script','//www.google-analytics.com/analytics.js','ga'); \n" +

            "ga('create', 'UA-{0}', 'auto'); \n" +
            "{1}" + 
            "ga('send', 'pageview'); \n" +

            "</script>";

        private const string GaAddTransaction =
            "ga('ecommerce:addTransaction', {{ " +
                "'id': '{0}', " +
                "'affiliation': '{1}', " +
                "'revenue': '{2}', " +
                "'shipping': '{3}', " +
                "'tax': '{4}' " +
            "}}); ";

        private const string GaAddItems =
            "ga('ecommerce:addItem', {{ " +
                "'id': '{0}', " +
                "'name': '{1}', " +
                "'sku': '{2}', " +
                "'category': '{3}', " +
                "'price': '{4}', " +
                "'quantity': '{5}' " +
            "}}); ";


        public string GetGoogleAnalyticsString()
        {
            if (!Enabled || Number.IsNullOrEmpty())
                return string.Empty;

            return string.Format(GoogleAnalytics, Number,
                (SettingsSEO.GoogleAnalyticsEnableDemogrReports ? "ga('require', 'displayfeatures');\n" : ""));
        }

        public string GetEComerce()
        {
            if (!Enabled || Number.IsNullOrEmpty() || Trans == null || Items.Count < 1)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\"> ");
            sb.AppendLine("ga('require', 'ecommerce', 'ecommerce.js');");
            sb.AppendFormat(GaAddTransaction, Trans.OrderId, HttpUtility.HtmlEncode(Trans.Affiliation), Trans.Total, Trans.Shipping, Trans.Tax);

            foreach (var item in Items)
            {
                sb.AppendFormat(GaAddItems, item.OrderId, HttpUtility.HtmlEncode(item.Sku), HttpUtility.HtmlEncode(item.Name), HttpUtility.HtmlEncode(item.Category), item.Price, item.Quantity);
            }

            sb.AppendLine("ga('ecommerce:send');");
            sb.AppendLine("</script>");

            return sb.ToString();
        }
    }
}