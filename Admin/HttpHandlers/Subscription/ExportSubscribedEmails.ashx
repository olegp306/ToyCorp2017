<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Subscription.ExportSubscribedEmails" %>

using System;
using System.IO;
using System.Web;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Customers;
using AdvantShop.FilePath;

namespace Admin.HttpHandlers.Subscription
{
    public class ExportSubscribedEmails : AdminHandler, IHttpHandler
    {

        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + "subscribers" + DateTime.Now.ToShortDateString() + ".csv";
            using (var streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("E-mail;Subscribe;SubscribeDate;UnsubscribeDate;UnsubscribeReason;");
                foreach (var subscriber in SubscriptionService.GetSubscriptions())
                {
                    streamWriter.WriteLine("{0};{1};{2};{3};{4};", subscriber.Email, subscriber.Subscribe ? "1" : "0", subscriber.SubscribeDate, subscriber.UnsubscribeDate, subscriber.UnsubscribeReason);
                }

            }
            var fileInfo = new FileInfo(filePath);
            context.Response.Clear();
            context.Response.ContentType = "application/download";
            context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", fileInfo.Name));
            context.Response.TransmitFile(filePath);
        }
    }
}