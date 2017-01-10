//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Compress;
using AdvantShop.Helpers;
using System.IO.Compression;

namespace ClientPages
{
    public partial class err500 : AdvantShopClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.DisableBrowserCache();
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 500;
            Response.Status = "500 Internal Server Error";

            if (bool.Parse(SettingProvider.GetConfigSettingValue("EnableCompressContent")))
            {
                Response.Filter = new HttpCompressStream(Response.Filter, CompressionMode.Compress,
                                                         HttpCompressStream.CompressionType.GZip);
                Response.AddHeader(HttpConstants.HttpContentEncoding, HttpConstants.HttpContentEncodingGzip);
            }

        }
    }
}