<%@ WebHandler Language="C#" Class="CopyProduct" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using Newtonsoft.Json;

public class CopyProduct : AdminHandler, IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        if (!Authorize(context))
        {
            return;
        }

        context.Response.ContentType = "application/json";

        var productId = context.Request["productId"].TryParseInt();
        var currentProduct = ProductService.GetProduct(productId);

        if (currentProduct == null)
        {
            return;
        }

        var offers = new List<Offer>();
        for (int i = 0; i < currentProduct.Offers.Count; i++)
        {
            var offer = currentProduct.Offers[i];
            offer.ArtNo += i;
            offer.ProductId = 0;

            int count = 0;
            while (count++ < 10)
            {
                if (OfferService.GetOffer(offer.ArtNo) == null)
                    break;

                offer.ArtNo += "_" + i;
            }

            offers.Add(offer);
        }

        var meta = currentProduct.Meta;
        meta.ObjId = 0;

        var product = new Product()
        {
            ProductId = 0,
            ArtNo = null,
            UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, currentProduct.Name),

            Name = currentProduct.Name + " - " + Resources.Resource.Admin_CopyOfProduct,
            BriefDescription = currentProduct.BriefDescription,
            Description = currentProduct.Description,
            Weight = currentProduct.Weight,
            Size = currentProduct.Size,

            Discount = currentProduct.Discount,
            ShippingPrice = currentProduct.ShippingPrice,
            Unit = currentProduct.Unit,
            Multiplicity = currentProduct.Multiplicity,
            MaxAmount = currentProduct.MaxAmount,
            MinAmount = currentProduct.MinAmount,

            Enabled = currentProduct.Enabled,
            AllowPreOrder = currentProduct.AllowPreOrder,

            BestSeller = currentProduct.BestSeller,
            Recomended = currentProduct.Recomended,
            New = currentProduct.New,
            OnSale = currentProduct.OnSale,
            BrandId = currentProduct.BrandId,

            SalesNote = currentProduct.SalesNote,
            Gtin = currentProduct.Gtin,
            GoogleProductCategory = currentProduct.GoogleProductCategory,
            Adult = currentProduct.Adult,

            Offers = offers,
            HasMultiOffer = currentProduct.HasMultiOffer,

            Meta = meta
        };

        product.ProductId = ProductService.AddProduct(product, true);
        if (product.ProductId == 0)
        {
            context.Response.Write(JsonConvert.SerializeObject(new { Ok = false }));
            return;
        }

        var categories = ProductService.GetCategoriesByProductId(currentProduct.ProductId);
        if (categories != null && categories.Count > 0)
        {
            ProductService.EnableDynamicProductLinkRecalc();
            foreach (var category in categories)
            {
                ProductService.AddProductLink(product.ProductId, category.CategoryId, 0, true);
            }
            ProductService.DisableDynamicProductLinkRecalc();
            ProductService.SetProductHierarchicallyEnabled(product.ProductId);
        }

        var propertyValues = PropertyService.GetPropertyValuesByProductId(currentProduct.ProductId);
        foreach (var propertyValue in propertyValues)
        {
            PropertyService.AddProductProperyValue(propertyValue.PropertyValueId, product.ProductId, propertyValue.SortOrder);
        }

        var photos = PhotoService.GetPhotos(currentProduct.ProductId, PhotoType.Product).ToList();
        if (photos.Count > 0)
        {
            try
            {
                foreach (var photo in photos)
                {
                    var tempPhotoName = PhotoService.AddPhoto(new Photo(0, product.ProductId, PhotoType.Product)
                    {
                        Description = photo.Description,
                        OriginName = photo.OriginName,
                        ColorID = photo.ColorID,
                        Main = photo.Main,
                        PhotoSortOrder = photo.PhotoSortOrder
                    });
                    if (string.IsNullOrWhiteSpace(tempPhotoName)) continue;

                    var photoPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, photo.PhotoName);
                    if (!System.IO.File.Exists(photoPath))
                    {
                        photoPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photo.PhotoName);
                    }

                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(photoPath))
                    {
                        FileHelpers.SaveProductImageUseCompress(tempPhotoName, image);
                    }
                }
                ProductService.PreCalcProductParams(product.ProductId);
            }
            catch (Exception)
            {
            }
        }

        var videos = ProductVideoService.GetProductVideos(currentProduct.ProductId);
        foreach (var video in videos)
        {
            video.ProductId = product.ProductId;
            ProductVideoService.AddProductVideo(video);
        }


        var customOptions = CustomOptionsService.GetCustomOptionsByProductId(currentProduct.ProductId);
        if (customOptions != null)
        {
            foreach (var customOption in customOptions)
            {
                customOption.ProductId = product.ProductId;
                CustomOptionsService.AddCustomOption(customOption);
            }
        }

        foreach (var relatedProduct in ProductService.GetRelatedProducts(currentProduct.ProductId, RelatedType.Related))
        {
            ProductService.AddRelatedProduct(product.ProductId, relatedProduct.ProductId, RelatedType.Related);
        }

        foreach (var relatedProduct in ProductService.GetRelatedProducts(currentProduct.ProductId, RelatedType.Alternative))
        {
            ProductService.AddRelatedProduct(product.ProductId, relatedProduct.ProductId, RelatedType.Alternative);
        }

        context.Response.Write(JsonConvert.SerializeObject(new
        {
            Ok = true,
            Link = UrlService.GetAdminAbsoluteLink("Product.aspx?ProductID=" + product.ProductId)
        }));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}