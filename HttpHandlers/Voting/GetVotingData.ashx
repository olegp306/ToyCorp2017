<%@ WebHandler Language="C#" Class="GetVotingData" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using AdvantShop.CMS;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;


public class GetVotingData : IHttpHandler
{
    private string _cookieCollectionNameVoting
    {
        get { return HttpUtility.UrlEncode(AdvantShop.Configuration.SettingsMain.SiteUrl) + "_Voting"; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var theme = VoiceService.GetTopTheme();
        List<Answer> answers;
        if ((theme == null) || (!(answers = theme.Answers).Any() && !theme.IsHaveNullVoice))
        {
            context.Response.Write(JsonConvert.Null);
            return;
        }

        int userVoteId = GetUserAnswerId(context.Request.Browser.Cookies, theme.VoiceThemeId);

        var vote = new
        {
            Question = theme.Name,
            Answers = answers.Select(item => new { Text = item.Name, item.AnswerId }).ToList(),
            Result = new
            {
                Rows = answers.Select(item => new { Text = item.Name, Value = item.Percent, Selected = item.AnswerId == userVoteId }).ToList(),
                Count = theme.CountVoice
            },
            isVoted = userVoteId != -1 || theme.IsClose,
            theme.IsHaveNullVoice
        };

        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(vote));
    }

    private int GetUserAnswerId(bool haveCookies, int themeId)
    {
        int userAnsverId = -1;
        try
        {
            if (haveCookies)
            {
                var items = CommonHelper.GetCookieCollection(_cookieCollectionNameVoting);
                if (items != null && items[string.Format("ThemesID{0}", themeId)] != null)
                {
                    userAnsverId = Int32.Parse(items[string.Format("ThemesID{0}", themeId)]);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return userAnsverId;
    }

    public bool IsReusable
    {
        get { return true; }
    }
}