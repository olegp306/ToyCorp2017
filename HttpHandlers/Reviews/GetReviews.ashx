<%@ WebHandler Language="C#" Class="GetReviews" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using AdvantShop.CMS;
using AdvantShop.Customers;
using Newtonsoft.Json;

public class GetReviews : IHttpHandler, IRequiresSessionState
{
    private IEnumerable<Review> _reviews;
    private bool _isAdmin = false;

    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/JSON";
        context.Response.ContentEncoding = Encoding.UTF8;

        int requestId;
        int entityType;

        if (!Int32.TryParse(context.Request["entityid"], out requestId) || !Int32.TryParse(context.Request["entitytype"], out entityType))
        {
            return;
        }
        _reviews = AdvantShop.Configuration.SettingsCatalog.ModerateReviews
            ? ReviewService.GetCheckedReviews(requestId, (EntityType)entityType).Where(review => review.ParentId == 0) 
            : ReviewService.GetReviews(requestId, (EntityType)entityType).Where(review => review.ParentId == 0);

        _isAdmin = CustomerContext.CurrentCustomer.IsAdmin ||
                   (CustomerContext.CurrentCustomer.IsModerator &&
                    CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayComments));
        
        var list = new List<object>();

        foreach (Review review in _reviews)
        {
            list.Add(new
            {
                isAdmin = _isAdmin,
                Id = review.ReviewId,
                Text = review.Text.Replace("\r\n", "<br />"),
                Name = review.Name,
                Date = review.AddDate.ToString(AdvantShop.Configuration.SettingsMain.ShortDateFormat + " - HH:mm"),
                Children = GetChildren(review)
            });
        }

        context.Response.Write(JsonConvert.SerializeObject(new { Reviews = list }));
    }

    private List<object> GetChildren(Review review)
    {
        if (review.Children.Any())
        {
            var list = new List<object>();
            foreach (var item in review.Children.Where(r=> r.Checked || !AdvantShop.Configuration.SettingsCatalog.ModerateReviews))
            {
                list.Add(new
                             {
                                 isAdmin = _isAdmin,
                                 Id = item.ReviewId,
                                 Text = item.Text.Replace("\r\n", "<br />"),
                                 Name = item.Name,
                                 Date = item.AddDate.ToString(AdvantShop.Configuration.SettingsMain.ShortDateFormat + " - HH:mm"),
                                 Children = GetChildren(item)
                             });
            }
            return list;
        }
        else
        {
            return null;
        }
    }


    public bool IsReusable
    {
        get { return true; }
    }
}