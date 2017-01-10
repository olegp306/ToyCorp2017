<%@ WebHandler Language="C#" Class="GetAnswerDiv" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using AdvantShop.CMS;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Resources;

public class GetAnswerDiv : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        int parrentId;
        if (!Int32.TryParse(context.Request["parrentId"], out parrentId))
        {
            ReturnNothing(context);
            return;
        }
        
        context.Response.ContentType = "text/HTML";
        context.Response.Write(GetHtml(parrentId));
        context.Response.End();
    }

    private static string GetHtml(int parrentId)
    {
        var sb = new StringBuilder();
        sb.Append("<div id=\"answerDiv\" class=\"divContentNewComment\" >\n");
        if (!CustomerContext.CurrentCustomer.RegistredUser)
        {
            sb.Append("Имя: <input type=\"text\" size=\"50\" id=\"txtName\" /><span id=\"errorName\" class=\"errorLabel\" style=\"display: none; color: #FF0000\">*&nbsp;Введите имя</span><br />");
            sb.Append("E-mail: <input type=\"text\" size=\"50\" id=\"txtEmail\" /><span id=\"errorEmail\" class=\"errorLabel\" style=\"display: none; color: #FF0000\">*&nbsp;Введите e-mail</span><br /><br />");
        }
        
        sb.Append("<span id=\"errorText\" style=\"display: none; color: #FF0000\">*&nbsp;Введите текст сообщения</span><br />");
        sb.Append("<div class=\"divBlockTextMessage\">\n");
        sb.Append("Тескт сообщения\n");
        sb.Append("</div>\n");
        sb.Append("<textarea id=\"txtMessage\" runat=\"server\" class=\"NewTextMessage\"></textarea>\n");
        sb.Append("<div class=\"FooterMessageWrite\">\n");
        sb.Append("<div class=\"FMessageWriteBtn\">\n");
        sb.Append(
            string.Format(
                "<input id=\"sendMessage\" type=\"image\" src=\"~/images/btnWrite.gif\" alt=\"\" value={0} onclick=\"messageClick(this); return false;\" />\n",
                parrentId));
        sb.Append("</div>\n");
        sb.Append("</div>\n");
        sb.Append("</div>\n");
        return sb.ToString();
    }

    private static void ReturnNothing(HttpContext context)
    {
        context.Response.ContentType = "text/HTML";
        context.Response.Write("");
        context.Response.End();
    }

    public bool IsReusable
    {
        get { return true; }
    }
}