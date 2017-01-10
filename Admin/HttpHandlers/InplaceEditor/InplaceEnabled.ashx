<%@ WebHandler Language="C#" Class="InplaceEnabled" %>

using System;
using System.Web;
using AdvantShop;
using AdvantShop.Trial;
using Newtonsoft.Json;

public class InplaceEnabled : AdvantShop.Core.HttpHandlers.AdminHandler, IHttpHandler
{

    public new void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (!TrialService.IsTrialEnabled && !Authorize(context))
        {
            context.Response.StatusCode = 401;
            context.Response.StatusDescription = "Access denied";
            context.Response.Write(JsonConvert.SerializeObject(new
            {
                isComplete = false
            }));
            context.Response.End();
        }

        bool isEnabled = context.Request["inplaceEnabled"].TryParseBool();

        AdvantShop.Configuration.SettingsMain.EnableInplace = isEnabled;

        context.Response.StatusCode = 200;
        context.Response.StatusDescription = "Response inplace enabled";
        context.Response.Write(JsonConvert.SerializeObject(new
        {
            isComplete = true,
            Enabled = isEnabled
        }));
        context.Response.End();
    }
}