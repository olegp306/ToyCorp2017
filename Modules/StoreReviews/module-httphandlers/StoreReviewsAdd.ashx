<%@ WebHandler Language="C#" Class="StoreReviewsAdd" %>

using System;
using System.Web;
using AdvantShop.Modules;

public class StoreReviewsAdd : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

        int parentId;
        bool resultParseParentId = int.TryParse(context.Request["parentId"], out parentId);
        
        if (!resultParseParentId)
        {
            context.Response.Write("false");
            context.Response.End();
        }

        int scope;
        bool resultParseScope = int.TryParse(context.Request["scope"], out scope);

        if (!resultParseScope)
        {
            context.Response.Write("false");
            context.Response.End();
        }   
        
        if (string.IsNullOrEmpty(context.Request["name"]))
        {
            context.Response.Write("false");
            context.Response.End();
        }
        
        if (string.IsNullOrEmpty(context.Request["email"]))
        {
            context.Response.Write("false");
            context.Response.End();
        }
        
        if (string.IsNullOrEmpty(context.Request["review"]))
        {
            context.Response.Write("false");
            context.Response.End();
        }

        StoreReviewRepository.AddStoreReview(new StoreReview
        {
            Moderated = false,
            Rate = scope,
            ParentId = parentId,
            ReviewerEmail = HttpUtility.HtmlEncode(context.Request["email"]),
            ReviewerName = HttpUtility.HtmlEncode(context.Request["name"]),
            Review = HttpUtility.HtmlEncode(context.Request["review"])
        });

        context.Response.Write("success");   
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}