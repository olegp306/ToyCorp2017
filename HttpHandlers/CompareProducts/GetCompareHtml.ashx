<%@ WebHandler Language="C#" Class="GetCompareHtml" %>

using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;

public class GetCompareHtml : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var compareProducts = ShoppingCartService.CurrentCompare;
        context.Response.ContentType = "text/HTML";

        context.Response.Write(compareProducts.Count == 0 ? NoProductView() : DefaultView(compareProducts));
        context.Response.End();
        
    }

    private static string NoProductView()
    {
        var sb = new StringBuilder();
        sb.Append("<div style=\"margin-top: 17px; margin-bottom: 7px; text-align: center;\">");
        sb.Append("<span>" + Resources.Resource.Client_UserControls_CompareProducts_NoItems + "</span>");
        sb.Append("</div>");
        sb.Append("<img src=\"~/images/spliter.jpg\" alt=\"\" />");
        return sb.ToString();
    }

    private static string DefaultView(ShoppingCart compareProducts)
    {
        var sb = new StringBuilder();
        sb.Append("<table class=\"blockT\">");
        foreach (var product in compareProducts.Select(p => p.Offer.Product))
        {
            sb.Append("<tr>");
            sb.Append("<td class=\"p_photo\" style=\"width: 66px;\">");
            if (!string.IsNullOrEmpty(product.Photo))
            {
                sb.Append("<a href='" + UrlService.GetLinkDB(ParamType.Product, product.ProductId) + "'");
                sb.Append("title='" + product.Name + "'>");
                sb.Append("<img border='0' src='" + (product.Photo.Contains("://") ? product.Photo : FoldersHelper.GetImageProductPath(ProductImageType.XSmall, product.Photo, false)) + "' /></a>");
            }
            sb.Append("</td>");

            sb.Append("<td class=\"p_desc\"style=\"font-size: 11px; width: 98px;\">");
            sb.Append("<a href='" + UrlService.GetLinkDB(ParamType.Product, product.ProductId) + "'");
            sb.Append("title='" + product.Name + "' class=\"product_block\">");
            sb.Append(StringHelper.GetReSpacedString(product.Name, 25) + "</a>");

            sb.Append("<div class=\"remove\" style=\"cursor: pointer;\"onclick=\"DeleteCompareProduct(" + product.ProductId + "); return false;\" >");
            sb.Append("<a class=\"Link\" style=\"font-size: 11px; color: black;\"><img src=\"~/images/remove.gif\" style=\"vertical-align: middle\" />&nbsp;" + Resources.Resource.Client_UserControls_ShoppingCart_Remove + "</a>");
            sb.Append("</div>");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\" style=\"width: 164px;\">");
            sb.Append("<div class=\"prod_total\"></div>");
            sb.Append("<img src=\"~/images/spliter.jpg\" alt=\"\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</td>");
            sb.Append("</tr>");
        }

        sb.Append("</table>");
        return sb.ToString();
    }

    //private static string ProductCountView(ShoppingCart compareProducts)
    //{
    //    var sb = new StringBuilder();
    //    sb.Append("<div style=\"margin-top: 17px; margin-bottom: 7px; text-align: left;\">");
    //    sb.Append("<strong>" + Resources.Resource.Client_UserControls_CompareProducts_TotalProduct + " </strong>" + compareProducts.Count); 
    //    sb.Append("</div>");
    //    sb.Append("<img src=\"~/images/spliter.jpg\" alt=\"\" />"); 
    //    return sb.ToString();
    //}

    public bool IsReusable
    {
        get { return true;}
    }
}