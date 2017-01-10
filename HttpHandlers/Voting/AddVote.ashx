<%@ WebHandler Language="C#" Class="AddVote" %>

using System;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.CMS;
using AdvantShop.Helpers;
using Newtonsoft.Json;

public class AddVote : IHttpHandler
{
    private string _cookieCollectionNameVoting
    {
        get { return HttpUtility.UrlEncode(AdvantShop.Configuration.SettingsMain.SiteUrl) + "_Voting"; }
    }
    
    public void ProcessRequest(HttpContext context)
    {
        bool voteAdded = false; 
        var currentTheme = VoiceService.GetTopTheme();
        int answerId = context.Request["answerId"].TryParseInt();

        if (answerId != 0 && currentTheme != null)
        {
            if (VoiceService.GetAllAnswers(currentTheme.VoiceThemeId).Any(answer => answer.AnswerId == answerId))
            {
                if (context.Request.Browser.Cookies)
                {
                    var items = CommonHelper.GetCookieCollection(_cookieCollectionNameVoting);

                    if (items["ThemesID" + currentTheme.VoiceThemeId] == null)
                    {
                        items.Add("ThemesID" + currentTheme.VoiceThemeId, answerId.ToString());
                        CommonHelper.SetCookieCollection(_cookieCollectionNameVoting, items, new TimeSpan(365, 0, 0, 0));
                        VoiceService.AddVote(answerId);
                        voteAdded = true;
                    }
                }  
            }
        }
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(voteAdded));
        context.Response.End();
     }
   
    public bool IsReusable
    {
        get { return true;}
    }
}