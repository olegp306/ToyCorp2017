<%@ WebHandler Language="C#" Class="ValidateCaptcha" %>

using System;
using System.Web;
using AdvantShop;
using AdvantShop.Helpers;
using Newtonsoft.Json;

public class ValidateCaptcha : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["enteredvalue"].IsNotEmpty() && context.Request["captchasource"].IsNotEmpty())
        {
            bool result = context.Request["captchasource"] == HttpUtility.UrlEncode(context.Request["enteredvalue"].ToUpper().Md5());
            context.Response.Write(JsonConvert.SerializeObject(result));
        }
        else
        {
            context.Response.Write(JsonConvert.SerializeObject(false));
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
