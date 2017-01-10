<%@ WebHandler Language="C#" Class="UploadCategoryPicture" %>

using System;
using System.Web;

public class UploadCategoryPicture : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        var urlPathcategory = context.Request["categoryname"];
        var categoryid = AdvantShop.Catalog.CategoryService.GetCategoryIdByUrlPath(urlPathcategory);
        string filename = "";
        string photofullpath = "images/nophoto_xsmall.jpg"; ;
        var categoryphoto = AdvantShop.Catalog.PhotoService.GetPhotoByObjId(categoryid, AdvantShop.Catalog.PhotoType.CategorySmall);
        if (categoryphoto != null)
        {
            filename = AdvantShop.Catalog.PhotoService.GetPathByPhotoId(categoryphoto.PhotoId);
            photofullpath = "pictures/category/small/" + filename;
        }


        context.Response.Write(photofullpath);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}