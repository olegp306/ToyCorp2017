<%@ WebHandler Language="C#" Class="Videos" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop;
using Newtonsoft.Json;

public class Videos : IHttpHandler
{

    public void ProcessRequest(HttpContext context) {
        context.Response.ContentType = "application/JSON";

        if (context.Request["productId"].IsNotEmpty() == false)
        {
            context.Response.Write(JsonConvert.SerializeObject(null));
            return;
        }

        var productVideos = ProductVideoService.GetProductVideos(context.Request["productId"].TryParseInt());

        context.Response.Write(JsonConvert.SerializeObject(new { Videos = productVideos })); 
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}