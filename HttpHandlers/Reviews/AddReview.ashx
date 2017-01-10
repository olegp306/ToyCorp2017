<%@ WebHandler Language="C#" Class="AddReview" %>

using System;
using System.Web;
using System.Web.SessionState;
using AdvantShop.CMS;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;

public class AddReview : IHttpHandler, IRequiresSessionState
{
    private int _entityId;
    private EntityType _entityType;
    private int _parentId;
    private string _text;
    private string _name;
    private string _email;

    public void ProcessRequest(HttpContext context)
    {
        if (!IsValidData(context))
        {
            ReturnValue(context, false);
            return;
        }

        var customerId = CustomerContext.CustomerId;
        string ip = "local";
        if (context.Request.UserHostAddress != "::1" && context.Request.UserHostAddress != "127.0.0.1")
        {
            ip = context.Request.UserHostAddress;
        }

        ReviewService.AddReview(new Review
            {
                ParentId = _parentId,
                EntityId = _entityId,
                CustomerId = customerId,
                Text = _text,
                Type = _entityType,
                Name = _name,
                Email = _email,
                Ip = ip
            });

        try
        {
            var p = AdvantShop.Catalog.ProductService.GetProduct(_entityId);

            var mailTemplate = new ProductDiscussMailTemplate(p.ArtNo, p.Name,
                                                              SettingsGeneral.AbsoluteUrl + UrlService.GetLink(ParamType.Product, p.UrlPath, p.ProductId),
                                                              _name, DateTime.Now.ToString(), _text,
                                                              SettingsGeneral.AbsoluteUrl + UrlService.GetLink(ParamType.Product, p.UrlPath, p.ProductId), 
                                                              _email);

            mailTemplate.BuildMail();
            SendMail.SendMailNow(SettingsMail.EmailForProductDiscuss, mailTemplate.Subject, mailTemplate.Body, true);
        }
        catch (Exception ex)
        {
            AdvantShop.Diagnostics.Debug.LogError(ex);
        }

        ReturnValue(context, true);
    }

    private bool IsValidData(HttpContext context)
    {
        if (!Int32.TryParse(context.Request["entityId"], out _entityId))
            return false;

        try { _entityType = (EntityType)Int32.Parse(context.Request["entityType"]); }
        catch { return false; }

        if (!ReviewService.IsExistsEntity(_entityId, _entityType))
            return false;

        if (!Int32.TryParse(context.Request["parentId"], out _parentId))
            return false;
        
        _text = !string.IsNullOrEmpty(context.Request["text"]) 
                        ? HttpUtility.HtmlEncode(context.Request["text"].Trim()).Replace("\n", "<br />") : string.Empty;
        
        if (_text.Length < 3)
            return false;

        _name = !string.IsNullOrEmpty(context.Request["name"]) 
                        ? HttpUtility.HtmlEncode(context.Request["name"].Trim()) : string.Empty;
        
        if (_name.Length < 3)
            return false;

        _email = !string.IsNullOrEmpty(context.Request["email"]) 
                        ? HttpUtility.HtmlEncode(context.Request["email"].Trim()) : string.Empty;
        
        if (_email.Length < 3 || !AdvantShop.Helpers.ValidationHelper.IsValidEmail(_email))
            return false;
        
        return true;
    }

    private static void ReturnValue(HttpContext context, bool result)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(result ? JsonConvert.True : JsonConvert.False);
        context.Response.End();
    }

    public bool IsReusable
    {
        get { return true; }
    }
}