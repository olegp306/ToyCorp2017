<%@ WebHandler Language="C#" Class="CheckApiKey" %>

using System.Web;
using AdvantShop.Configuration;

public class CheckApiKey : IHttpHandler
{
    
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        
        var apikey = context.Request["apikey"];

        if (!Settings1C.Enabled)
        {
            context.Response.Write("false");
            return;
        }

        if (string.IsNullOrWhiteSpace(apikey) || string.IsNullOrWhiteSpace(SettingsApi.ApiKey) ||
            apikey != SettingsApi.ApiKey)
        {
            context.Response.Write("false");
            return;
        }

        context.Response.Write("true");
    }

    public bool IsReusable
    {
        get { return false; }
    }
}