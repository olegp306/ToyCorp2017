<%@ WebHandler Language="C#" Class="DeletedProducts" %>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.OneC;
using Newtonsoft.Json;

public class DeletedProducts : IHttpHandler
{
    
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        
        var apikey = context.Request["apikey"];

        if (!Settings1C.Enabled || string.IsNullOrWhiteSpace(apikey) || string.IsNullOrWhiteSpace(SettingsApi.ApiKey) ||
            apikey != SettingsApi.ApiKey)
        {
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Check apikey")));
            return;
        }

        try
        {
            DateTime from;
            DateTime to;
            
            List<string> productIds = null;

            if (!String.IsNullOrWhiteSpace(context.Request["from"]) && !String.IsNullOrWhiteSpace(context.Request["to"]))
            {
                if (DateTime.TryParseExact(context.Request["from"], "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out from) &&
                    DateTime.TryParseExact(context.Request["to"], "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out to))
                {
                    productIds = ProductService.GetDeletedProducts(from, to);
                }
            }
            else
            {
                productIds = ProductService.GetDeletedProducts(null, null);
            }

            context.Response.Write(
                JsonConvert.SerializeObject(new OneCDeletedItemsResponse()
                {
                    status = "ok",
                    ids = productIds != null ? String.Join(",", productIds) : ""
                }));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Error: " + ex.Message)));
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}