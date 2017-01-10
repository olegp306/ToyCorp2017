<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Settings.SaveTemplateSettings" %>

using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Core.Caching;

namespace Admin.HttpHandlers.Settings
{
    public class SaveTemplateSettings : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            if (context.Request["settings"].IsNullOrEmpty())
            {
                ReturnResult(context, Resources.Resource.Admin_TemplateSettings_ErrorSaveSettings);
                return;
            }

            foreach (var setting in context.Request["settings"].Split(','))
            {
                var settingArr = setting.Split('~');

                if (settingArr.Length == 2)
                {
                    if (!TemplateSettingsProvider.SetTemplateSetting(settingArr[0], settingArr[1]))
                    {
                        ReturnResult(context, Resources.Resource.Admin_TemplateSettings_ErrorSaveSettings);
                        return;
                    }
                }
            }

            CacheManager.Clean();
            ReturnResult(context, Resources.Resource.Admin_TemplateSettings_SuccessSaveSettings);
        }

        private static void ReturnResult(HttpContext context, string result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new {result}));
            context.Response.End();
        }

    }
}