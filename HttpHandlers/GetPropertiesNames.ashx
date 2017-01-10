<%@ WebHandler Language="C#" Class="GetProperties" %>

using System;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using System.Collections.Generic;
using Newtonsoft.Json;

public class GetProperties : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        var text = context.Request["q"].ToString();
        var propertyId = context.Request["propertyId"].TryParseInt();
        var productId = context.Request["productId"].TryParseInt();

        if (string.IsNullOrWhiteSpace(text))
        {
            context.Response.Write("\n");
            return;
        }

        var result = new object();

        switch (context.Request["prop"].ToLower())
        {
            case "name":
                result = AdvantShop.Catalog.PropertyService.GetPropertiesByName(text);
                break;
            case "value":
                result = AdvantShop.Catalog.PropertyService.GetPropertiesValuesByNameEndProductId(text, productId, propertyId);
                break;
        }


        context.Response.Write(JsonConvert.SerializeObject(result));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}