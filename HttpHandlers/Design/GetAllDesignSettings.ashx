<%@ WebHandler Language="C#" Class="GetAllDesignSettings" %>


using System;
using System.Globalization;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop;
using AdvantShop.Design;
using AdvantShop.Trial;
using Newtonsoft.Json;

public class GetAllDesignSettings : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();

        object array = new
            {
                Themes = DesignService.GetDesigns(eDesign.Theme),
                Colors = DesignService.GetDesigns(eDesign.Color),
                Backgrounds = DesignService.GetDesigns(eDesign.Background),
                Structures = Enum.GetNames(typeof(SettingsDesign.eMainPageMode)),
                DesignCurrent = GetCurrentDesign(),
                isTrial = TrialService.IsTrialEnabled && AdvantShop.Helpers.CommonHelper.GetCookieString("isTrialService").IsNullOrEmpty(),
                isTemplate = SettingsDesign.Template != TemplateService.DefaultTemplateId                
            };

        AdvantShop.Helpers.CommonHelper.SetCookie("isTrialService", TrialService.IsTrialEnabled.ToString(CultureInfo.InvariantCulture));

        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(array));
    }


    private object GetCurrentDesign()
    {
        object obj = new { };
        if (Demo.IsDemoEnabled)
        {
            obj = new
            {
                Theme = AdvantShop.Helpers.CommonHelper.GetCookieString("theme").IsNotEmpty() ? AdvantShop.Helpers.CommonHelper.GetCookieString("theme") : SettingsDesign.Theme,
                ColorScheme = AdvantShop.Helpers.CommonHelper.GetCookieString("colorscheme").IsNotEmpty() ? AdvantShop.Helpers.CommonHelper.GetCookieString("colorscheme") : SettingsDesign.ColorScheme,
                Background = AdvantShop.Helpers.CommonHelper.GetCookieString("background").IsNotEmpty() ? AdvantShop.Helpers.CommonHelper.GetCookieString("background") : SettingsDesign.BackGround,
                Structure = AdvantShop.Helpers.CommonHelper.GetCookieString("structure").IsNotEmpty() ? AdvantShop.Helpers.CommonHelper.GetCookieString("structure") : SettingsDesign.MainPageMode.ToString(),
                Template = SettingsDesign.Template != TemplateService.DefaultTemplateId ? SettingsDesign.Template : string.Empty
            };
        }
        else
        {
            obj = new
               {
                   Theme = SettingsDesign.Theme,
                   ColorScheme = SettingsDesign.ColorScheme,
                   Background = SettingsDesign.BackGround,
                   Structure = SettingsDesign.MainPageMode.ToString(),
                   Template = SettingsDesign.Template != TemplateService.DefaultTemplateId ? SettingsDesign.Template : string.Empty
               };
        }

        return obj;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}