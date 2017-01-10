<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Statistic.GetNoticeStatistic" %>

using System.Text;
using System.Web;
using AdvantShop.Core.HttpHandlers;
using Newtonsoft.Json;

namespace Admin.HttpHandlers.Statistic
{
    public class GetNoticeStatistic : AdminHandler, IHttpHandler
    {

        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;
            
            context.Response.ContentType = "application/JSON";
            context.Response.ContentEncoding = Encoding.UTF8;

            context.Response.Write(JsonConvert.SerializeObject(new
                {
                    LastOrdersCount = AdvantShop.Statistic.StatisticService.GetLastOrdersCount(),
                    LastReviews = AdvantShop.Statistic.StatisticService.GetLastReviewsCount(),
                }));
        }
    }
}