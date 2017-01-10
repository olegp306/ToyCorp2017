<%@ WebHandler Language="C#" Class="ExportOrders" %>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.OneC;
using AdvantShop.Orders;
using Newtonsoft.Json;

public class ExportOrders : IHttpHandler
{
    
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        
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
            List<Order> orders = null;
            
            if (!String.IsNullOrWhiteSpace(context.Request["from"]) && !String.IsNullOrWhiteSpace(context.Request["to"]))
            {
                if (DateTime.TryParseExact(context.Request["from"], "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out from) &&
                    DateTime.TryParseExact(context.Request["to"], "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out to))
                {
                    orders = OrderService.GetOrdersFor1C(from, to.AddDays(1), Settings1C.OnlyUseIn1COrders);
                }
            }
            else
            {
                orders = OrderService.GetAllOrders();
            }
            
            if (orders == null)
            {
                context.Response.Write("");
                return;
            }

            var xml = new XmlDocument();
            
            using (var writer = new StringWriter())
            {
                OrderService.SerializeToXml(orders, writer, true);
                xml.Load(new StringReader(writer.ToString()));
            }

            context.Response.Write(xml.OuterXml);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            
            context.Response.Write("Error: " + ex.Message);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}