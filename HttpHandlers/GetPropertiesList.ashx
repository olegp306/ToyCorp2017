<%@ WebHandler Language="C#" Class="GetPropertiesList" %>

using System;
using System.Linq;
using System.Collections.Generic;

using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using System.Web;
using Newtonsoft.Json;

public class GetPropertiesList : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var productId = context.Request["productId"].TryParseInt();

        if (productId == 0)
        {
            context.Response.Write(JsonConvert.SerializeObject(null));
            return;
        }

        var prodPropertyValues = PropertyService.GetPropertyValuesByProductId(productId).Where(v => v.Property.UseInDetails).ToList();

        context.Response.Write(JsonConvert.SerializeObject(new
        {
            PropertyValues = prodPropertyValues,
            ProductID = productId,
            Type = InplaceEditor.ObjectType.Property.ToString(),
            Prop = InplaceEditor.Property.Field.Value.ToString()
        }));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}