//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.IO.Compression;
using System.Web;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Compress
{
    /// <summary>
    /// Summary description for CompressContent
    /// </summary>
    public class CompressContent : IHttpModule
    {
        public void Dispose()
        {
            // Nothing to dispose; 
        }
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += PreRequestHandlerExecute;
        }

        private static void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (bool.Parse(SettingProvider.GetConfigSettingValue("EnableCompressContent")) == false)
            {
                return;
            }
            
            var app = (HttpApplication)sender;
            HttpRequest request = app.Context.Request;
            HttpResponse response = app.Context.Response;

            //if (app.Context.CurrentHandler == null) return;

            //if (!(app.Context.CurrentHandler is Page || app.Context.CurrentHandler.GetType().Name == "SyncSessionlessHandler") || app.Request["http_x_microsoftajax"] != null)
            //{
            //    return;
            //}

            if (request.FilePath == "/" || request.FilePath.Contains(".aspx") || request.FilePath.Contains(".js") || request.FilePath.Contains(".css") || request.FilePath.Contains(".ashx") || request.FilePath.Contains(".tpl"))
            {
                if (request.RawUrl.Contains(".aspx") && request.RawUrl.Contains("TSM_HiddenField")) return;
                if (request.RawUrl.Contains(".js") && request.RawUrl.Contains(".axd")) return;
                if (!(request.Browser.IsBrowser("IE") && request.Browser.MajorVersion <= 6))
                {
                    string acceptEncoding = request.Headers[HttpConstants.HttpAcceptEncoding];
                    if (!string.IsNullOrEmpty(acceptEncoding))
                    {
                        acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);
                        if (acceptEncoding.Contains(HttpConstants.HttpContentEncodingGzip))
                        {
                            response.Filter = new HttpCompressStream(response.Filter, CompressionMode.Compress, HttpCompressStream.CompressionType.GZip);
                            response.AddHeader(HttpConstants.HttpContentEncoding, HttpConstants.HttpContentEncodingGzip);
                        }
                        else if (acceptEncoding.Contains(HttpConstants.HttpContentEncodingDeflate))
                        {
                            response.Filter = new HttpCompressStream(response.Filter, CompressionMode.Compress, HttpCompressStream.CompressionType.Deflate);
                            response.AddHeader(HttpConstants.HttpContentEncoding, HttpConstants.HttpContentEncodingDeflate);
                        }
                    }
                }
                else
                {
                    response.Filter = new HttpCompressStream(response.Filter, CompressionMode.Compress, HttpCompressStream.CompressionType.None);
                }
            }
        }
    }
}