<%@ WebHandler Language="C#" Class="InplaceStatus" %>

using System;
using System.Web;
using AdvantShop;

public class InplaceStatus : AdvantShop.Core.HttpHandlers.AdminHandler, IHttpHandler
{

    public new void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (!AdvantShop.Trial.TrialService.IsTrialEnabled && !Authorize(context))
        {
            context.Response.StatusCode = 401;
            context.Response.StatusDescription = "Access denied";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                isComplete = false
            }));
            context.Response.End();
        }

        context.Response.StatusCode = 200;
        context.Response.StatusDescription = "Response inplace enabled";
        context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            isComplete = true,
            isEnabled = AdvantShop.Configuration.SettingsMain.EnableInplace
        }));
        context.Response.End();
    }
}