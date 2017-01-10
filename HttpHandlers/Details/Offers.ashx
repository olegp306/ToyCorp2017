<%@ WebHandler Language="C#" Class="Offers" %>

using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop;
using AdvantShop.Configuration;
using Newtonsoft.Json;

public class Offers : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/JSON";

        if (context.Request["productId"].IsNullOrEmpty())
        {
            context.Response.Write(JsonConvert.SerializeObject(null));
            return;
        }


        var offers = OfferService.GetProductOffers(context.Request["productId"].TryParseInt());

        var product = ProductService.GetProduct(context.Request["productId"].TryParseInt());

        var obj = new
            {
                Offers = offers.Select( //.Where(o => o.Amount > 0 || product.AllowPreOrder)
                    offer => new
                        {
                            offer.OfferId,
                            offer.ProductId,
                            offer.ArtNo,
                            Color = ColorService.GetColor(offer.ColorID),
                            Size = SizeService.GetSize(offer.SizeID),
                            offer.Price,
                            offer.Product.Discount,
                            offer.Amount,
                            offer.Main,
                        }),

                Sizes =
                    offers.Where(o => o.Size != null && (o.Amount > 0 || product.AllowPreOrder))
                          .OrderBy(o => o.Size.SortOrder)
                          .Select(o => new {o.Size.SizeId, o.Size.SizeName})
                          .Distinct(),
                Colors =
                    offers.Where(o => o.Color != null && (o.Amount > 0 || product.AllowPreOrder))
                          .OrderBy(o => o.Color.SortOrder)
                          .Select(o => new { o.Color.ColorId, o.Color.ColorName, o.Color.ColorCode, PhotoName =  o.Color.IconFileName != null ? o.Color.IconFileName.PhotoName : null })
                          .Distinct(),
                product.Unit,
                ShowStockAvailability = SettingsCatalog.ShowStockAvailability,
                AllowPreOrder = product.AllowPreOrder,
                ImageWidth = SettingsPictureSize.ColorIconWidthDetails,
                ImageHeight = SettingsPictureSize.ColorIconHeightDetails,
            };
        
        context.Response.Write(JsonConvert.SerializeObject(obj));
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}