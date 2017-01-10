<%@ WebHandler Language="C#" Class="AddRating" %>

using System.Web;
using AdvantShop.Catalog;
using Newtonsoft.Json;
using AdvantShop;

public class AddRating : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        int productId = context.Request["productid"].TryParseInt();
        int rating = context.Request["rating"].TryParseInt();
        
        float newRating = 0;
        if(productId != 0 && rating != 0)
        {
            newRating = RatingService.Vote(productId, rating);
        }

        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(newRating));
        context.Response.End();
    }
    
    public bool IsReusable
    {
        get { return true;}
    }
}