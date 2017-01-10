<%@ WebHandler Language="C#" Class="InplaceGetContent" %>

using System;
using System.Linq;
using System.Web;
using AdvantShop.Customers;
using AdvantShop.Trial;
using Newtonsoft.Json;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using System.Collections.Generic;

public class InplaceGetContent : AdvantShop.Core.HttpHandlers.AdminHandler, IHttpHandler
{

    private new bool Authorize(HttpContext context)
    {
        if (CustomerContext.CurrentCustomer.IsAdmin || TrialService.IsTrialEnabled)
            return true;

        if (CustomerContext.CurrentCustomer.IsModerator)
        {
            var type = (InplaceEditor.ObjectType)Enum.Parse(typeof(InplaceEditor.ObjectType), context.Request["type"]);
            switch (type)
            {
                case InplaceEditor.ObjectType.StaticPage:
                    return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayStaticPages);

                case InplaceEditor.ObjectType.NewsItem:
                    return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayNews);

                case InplaceEditor.ObjectType.StaticBlock:
                case InplaceEditor.ObjectType.Category:
                case InplaceEditor.ObjectType.Product:
                case InplaceEditor.ObjectType.Offer:
                case InplaceEditor.ObjectType.Property:
                    return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayCatalog);

                case InplaceEditor.ObjectType.Image:
                    var propImage = (InplaceEditor.Image.Field)Enum.Parse(typeof(InplaceEditor.Image.Field), context.Request["prop"]);
                    switch (propImage)
                    {
                        case InplaceEditor.Image.Field.Product:
                        case InplaceEditor.Image.Field.CategorySmall:
                        case InplaceEditor.Image.Field.CategoryBig:
                            return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayCatalog);

                        case InplaceEditor.Image.Field.Brand:
                            return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayBrands);

                        case InplaceEditor.Image.Field.Carousel:
                            return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayCarousel);

                        case InplaceEditor.Image.Field.Logo:
                            return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayCommonSettings);

                        case InplaceEditor.Image.Field.News:
                            return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayNews);

                        default:
                            throw new NotImplementedException();
                    }

                case InplaceEditor.ObjectType.Phone:
                case InplaceEditor.ObjectType.Meta:
                    return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayCommonSettings);

                case InplaceEditor.ObjectType.Brand:
                    return CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayBrands);

                default:
                    throw new NotImplementedException();
            }
        }

        return false;
    }
    
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

        if (!Authorize(context))
        {
            return;
        }
        
        var id = context.Request["id"].TryParseInt();
        var type = (InplaceEditor.ObjectType)Enum.Parse(typeof(InplaceEditor.ObjectType), context.Request["type"]);

        context.Response.Write(InplaceEditor.GetContent(type, id));
    }
}