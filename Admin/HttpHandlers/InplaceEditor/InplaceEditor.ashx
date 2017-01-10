<%@ WebHandler Language="C#" Class="Inplace" %>

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

public class Inplace : AdvantShop.Core.HttpHandlers.AdminHandler, IHttpHandler
{

    // override authorize function for inplace
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


    public new void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (!Authorize(context))
        {
            return;
        }

        object result = null;
        var isValid = false;
        var propStr = context.Request["prop"];
        var content = context.Request["content"];
        var type = (InplaceEditor.ObjectType)Enum.Parse(typeof(InplaceEditor.ObjectType), context.Request["type"]);
        var idStr = context.Request["id"];
        var id = idStr.TryParseInt();

        if (context.Request["type"].IsNullOrEmpty() || (context.Request["required"] == "true" && content.IsNullOrEmpty())) //  (idStr != null && id == 0)
        {
            context.Response.StatusCode = 400;
            context.Response.StatusDescription = "Parameters is invalid";
            context.Response.End();
        }

        switch (type)
        {
            case InplaceEditor.ObjectType.StaticPage:
                var propStaticPage = (InplaceEditor.StaticPage.Field)Enum.Parse(typeof(InplaceEditor.StaticPage.Field), propStr);
                isValid = ProcessPage(id, propStaticPage, content);
                break;
            case InplaceEditor.ObjectType.NewsItem:
                var propNewsItem = (InplaceEditor.NewsItem.Field)Enum.Parse(typeof(InplaceEditor.NewsItem.Field), propStr);
                isValid = ProcessNewsItem(id, propNewsItem, content);
                break;
            case InplaceEditor.ObjectType.StaticBlock:
                var propStaticBlock = (InplaceEditor.StaticBlock.Field)Enum.Parse(typeof(InplaceEditor.StaticBlock.Field), propStr);
                isValid = ProcessStaticBlock(id, propStaticBlock, content);
                break;
            case InplaceEditor.ObjectType.Category:
                var propCategory = (InplaceEditor.Category.Field)Enum.Parse(typeof(InplaceEditor.Category.Field), propStr);
                isValid = ProcessCategory(id, propCategory, content);
                break;
            case InplaceEditor.ObjectType.Product:
                var propProduct = (InplaceEditor.Product.Field)Enum.Parse(typeof(InplaceEditor.Product.Field), propStr);
                result = ProcessProduct(id, propProduct, content);
                isValid = (result != null);
                break;
            case InplaceEditor.ObjectType.Phone:
                var propPhone = (InplaceEditor.Phone.Field)Enum.Parse(typeof(InplaceEditor.Phone.Field), propStr);
                var cityId = context.Request["cityId"].TryParseInt();
                isValid = ProcessPhone(propPhone, cityId, content);
                break;
            case InplaceEditor.ObjectType.Offer:
                var propOffer = (InplaceEditor.Offer.Field)Enum.Parse(typeof(InplaceEditor.Offer.Field), propStr);
                result = ProcessOffer(id, propOffer, content, context.Request["customOptions"]);
                isValid = (result != null);
                break;
            case InplaceEditor.ObjectType.Property:
                var propProperty = (InplaceEditor.Property.Field)Enum.Parse(typeof(InplaceEditor.Property.Field), propStr);
                int productId = context.Request["productId"].TryParseInt();
                int propertyId = context.Request["propertyid"].TryParseInt();
                int propertyValueID = context.Request["propertyvalueid"].TryParseInt();
                string propertyName = context.Request["propertyName"];
                string propertyValueName = context.Request["propertyValue"];

                if (id == -1)
                {
                    result = ProcessPropertyAdd(productId, propertyId, propertyValueID, propertyName, propertyValueName);
                    isValid = result != null;
                }
                else if (content.IsNullOrEmpty())
                {
                    isValid = ProcessPropertyDelete(productId, id);
                }
                else
                {
                    isValid = ProcessProperty(id, propProperty, content, productId);
                }

                break;
            case InplaceEditor.ObjectType.Brand:
                var propBrand = (InplaceEditor.Brand.Field)Enum.Parse(typeof(InplaceEditor.Brand.Field), propStr);
                isValid = ProcessBrand(id, propBrand, content);
                break;
            case InplaceEditor.ObjectType.Image:

                var files = context.Request.Files;
                var propImage = (InplaceEditor.Image.Field)Enum.Parse(typeof(InplaceEditor.Image.Field), propStr);
                var commandImage = (InplaceEditor.Image.Commands)Enum.Parse(typeof(InplaceEditor.Image.Commands), context.Request["command"]);
                var objId = context.Request["objId"].TryParseInt();
                int? colorID = context.Request["colorID"].TryParseInt(true);


                if (commandImage == InplaceEditor.Image.Commands.Add || commandImage == InplaceEditor.Image.Commands.Update)
                {

                    if (FileHelpers.CheckFileExtension(files[0].FileName, FileHelpers.eAdvantShopFileTypes.Image) && files.Count > 0)
                    {
                        result = ProcessImage(id, propImage, content, files, commandImage, objId, context.Request["additionalData"], colorID);
                    }
                    else
                    {
                        isValid = false;
                        result = null;
                    }

                }
                else if (commandImage == InplaceEditor.Image.Commands.Delete)
                {
                    result = ProcessImage(id, propImage, content, files, commandImage, objId, context.Request["additionalData"], colorID);
                }

                isValid = result.ToString().IsNotEmpty();

                break;
            case InplaceEditor.ObjectType.Meta:
                MetaType metaType = context.Request["metatype"].TryParseEnume<MetaType>();
                string title = context.Request["title"].ToString();
                string h1 = context.Request["h1"].ToString();
                string metaKeywords = context.Request["metakeywords"].ToString();
                string metaDescription = context.Request["metadescription"].ToString();
                string name = context.Request["name"].ToString();

                isValid = ProcessMeta(id, name, metaType, title, h1, metaKeywords, metaDescription);

                break;

            default:
                throw new NotImplementedException();
        }

        if (isValid == false)
        {
            context.Response.StatusCode = 500;
            context.Response.StatusDescription = "Server Error";
            context.Response.End();
        }

        if (result != null)
        {
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "Response inplace editor";
            context.Response.Write(JsonConvert.SerializeObject(result));
            context.Response.End();
        }

    }

    private bool ProcessPage(int id, InplaceEditor.StaticPage.Field prop, string content)
    {

        var page = StaticPageService.GetStaticPage(id);

        if (page == null)
        {
            return false;
        }

        switch (prop)
        {
            case InplaceEditor.StaticPage.Field.PageName:
                page.PageName = content;
                break;
            case InplaceEditor.StaticPage.Field.PageText:
                page.PageText = content;
                break;

            default:
                throw new NotImplementedException();
        }

        page.ModifyDate = DateTime.Now;

        StaticPageService.UpdateStaticPage(page);

        return true;
    }

    private bool ProcessNewsItem(int id, InplaceEditor.NewsItem.Field prop, string content)
    {
        var newsItem = AdvantShop.News.NewsService.GetNewsById(id);

        if (newsItem == null)
        {
            return false;
        }

        switch (prop)
        {
            case InplaceEditor.NewsItem.Field.Title:
                newsItem.Title = content;
                break;
            case InplaceEditor.NewsItem.Field.TextAnnotation:
                newsItem.TextAnnotation = content;
                break;
            case InplaceEditor.NewsItem.Field.TextToPublication:
                newsItem.TextToPublication = content;
                break;

            default:
                throw new NotImplementedException();
        }

        return AdvantShop.News.NewsService.UpdateNews(newsItem);

    }

    private bool ProcessStaticBlock(int id, InplaceEditor.StaticBlock.Field prop, string content)
    {

        var staticBlocItem = StaticBlockService.GetPagePart(id);

        if (staticBlocItem == null)
        {
            return false;
        }

        if (prop == InplaceEditor.StaticBlock.Field.Content)
        {
            staticBlocItem.Content = content;
        }

        staticBlocItem.Modified = DateTime.Now;

        return StaticBlockService.UpdatePagePart(staticBlocItem);
    }

    private bool ProcessCategory(int id, InplaceEditor.Category.Field prop, string content)
    {

        var categoryItem = CategoryService.GetCategory(id);

        if (categoryItem == null)
        {
            return false;
        }

        switch (prop)
        {
            case InplaceEditor.Category.Field.Name:
                categoryItem.Name = content;
                break;
            case InplaceEditor.Category.Field.Description:
                categoryItem.Description = content;
                break;
            case InplaceEditor.Category.Field.BriefDescription:
                categoryItem.BriefDescription = content;
                break;

            default:
                throw new NotImplementedException();
        }

        return CategoryService.UpdateCategory(categoryItem, true);

    }

    private object ProcessProduct(int id, InplaceEditor.Product.Field prop, string content)
    {

        var productItem = ProductService.GetProduct(id);

        if (productItem == null)
        {
            return null;
        }

        switch (prop)
        {
            case InplaceEditor.Product.Field.Name:
                productItem.Name = content;
                break;
            case InplaceEditor.Product.Field.ArtNo:


                if (ProductService.IsUniqueArtNo(content))
                {
                    productItem.ArtNo = content;
                }
                else
                {
                    return ErrorMessage(Resources.Resource.Inplace_Error_ExistArtNo);
                }
                break;
            case InplaceEditor.Product.Field.Description:
                productItem.Description = content;
                break;
            case InplaceEditor.Product.Field.BriefDescription:
                productItem.BriefDescription = content;
                break;
            case InplaceEditor.Product.Field.Unit:
                productItem.Unit = content;
                break;
            case InplaceEditor.Product.Field.Weight:
                productItem.Weight = content.TryParseFloat();
                break;

            default:
                throw new NotImplementedException();
        }

        ProductService.UpdateProduct(productItem, true);

        return productItem.Unit;
    }

    private bool ProcessPhone(InplaceEditor.Phone.Field prop, int cityId, string content)
    {

        var result = false;

        if (prop == InplaceEditor.Phone.Field.Number)
        {

            if (cityId == 0)
            {
                SettingsMain.Phone = content;
            }
            else
            {
                AdvantShop.Repository.CityService.UpdatePhone(cityId, content);
            }

            result = true;
        }
        else
        {
            return false;
        }

        return result;
    }

    private object ProcessOffer(int id, InplaceEditor.Offer.Field prop, string content, string customOptions)
    {

        var offerItem = OfferService.GetOffer(id);
        float price = 0;

        if (offerItem == null)
        {
            return null;
        }

        var amountString = string.Empty;


        switch (prop)
        {
            case InplaceEditor.Offer.Field.Price:
                price = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat();
                offerItem.Price = price * AdvantShop.Repository.Currencies.CurrencyService.CurrentCurrency.Value;
                OfferService.UpdateOffer(offerItem);
                break;
            case InplaceEditor.Offer.Field.PriceWithDiscount:
                var priceWithDiscount = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat();
                var priceOld = offerItem.Price / AdvantShop.Repository.Currencies.CurrencyService.CurrentCurrency.Value;

                if (priceWithDiscount <= 0)
                {
                    offerItem.Product.Discount = 0;
                }
                else
                {

                    float _discount = 100 - (priceWithDiscount / priceOld) * 100;
                    offerItem.Product.Discount = _discount > 0 ? _discount : 0;
                }

                ProductService.UpdateProductDiscount(offerItem.ProductId, offerItem.Product.Discount);

                break;

            case InplaceEditor.Offer.Field.Amount:
                offerItem.Amount = content.TryParseFloat();
                OfferService.UpdateOffer(offerItem);

                var isAvailable = offerItem.Amount > 0;

                amountString = string.Format(
                "<div id='availability' class='{0}' {3}>{1}{2}</div>",
                isAvailable ? "available" : "not-available",
                isAvailable ? Resources.Resource.Client_Details_Available : Resources.Resource.Client_Details_NotAvailable,
                isAvailable && SettingsCatalog.ShowStockAvailability
                ? string.Format(" ({0}{1}<span data-inplace-update=\"amount\">{2}</span>)", offerItem.Amount, offerItem.Product.Unit.IsNotEmpty() ? " " : string.Empty, offerItem.Product.Unit.IsNotEmpty() ? offerItem.Product.Unit : string.Empty)
                : string.Empty,
                InplaceEditor.Offer.AttibuteAmount(offerItem.OfferId, offerItem.Amount));

                break;
            case InplaceEditor.Offer.Field.ArtNo:
                offerItem.ArtNo = content;
                OfferService.UpdateOffer(offerItem);

                if (offerItem.Product.Offers.Count == 1)
                {
                    var p = offerItem.Product;
                    p.ArtNo = content;
                    ProductService.UpdateProduct(p, true);
                }
                break;

            default:
                throw new NotImplementedException();
        }

        ProductService.PreCalcProductParams(offerItem.ProductId);
        
        
        return new
        {
            price = offerItem.Price,
            discount = offerItem.Product.Discount,
            amount = offerItem.Amount,
            amountString = amountString,
            artNo = offerItem.ArtNo
        };
    }

    private bool ProcessProperty(int id, InplaceEditor.Property.Field prop, string content, int productId)
    {

        var propertyValue = PropertyService.GetPropertyValueById(id);

        if (propertyValue == null)
        {
            return false;
        }

        propertyValue.Value = content;
        PropertyService.UpdatePropertyValue(propertyValue);

        return true;
    }

    private object ProcessPropertyAdd(int productId, int propertyID, int propertyValueID, string name, string value)
    {

        if (productId == 0)
        {
            return null;
        }

        if (propertyValueID == 0 && PropertyService.IsExistPropertyValueInProduct(productId, propertyID, value))
        {
            return null;
        }
        else if (propertyValueID != 0 && PropertyService.IsExistPropertyValueInProduct(productId, propertyValueID))
        {
            return null;
        }

        Property property = null;

        if (propertyID != 0)
        {
            property = PropertyService.GetPropertyById(propertyID);
        }

        if (property == null)
        {
            property = new Property()
            {
                Name = name,
                UseInDetails = true,
                UseInFilter = true,
                Type = 1
            };

            property.PropertyId = PropertyService.AddProperty(property);
        }


        if (value.IsNotEmpty())
        {

            PropertyValue propertyValue = null;

            if (propertyValueID != 0)
            {
                propertyValue = PropertyService.GetPropertyValueById(propertyValueID);
            }

            if (propertyValue == null)
            {
                propertyValue = new PropertyValue()
                {
                    Value = value,
                    PropertyId = property.PropertyId
                };

                propertyValue.PropertyValueId = PropertyService.AddPropertyValue(propertyValue);
            }

            PropertyService.AddProductProperyValue(propertyValue.PropertyValueId, productId, PropertyService.GetNewPropertyValueSortOrder(productId));
        }

        return new
        {
            property.PropertyId
        };
    }

    private bool ProcessPropertyDelete(int productId, int id)
    {
        PropertyService.DeleteProductPropertyValue(productId, id);

        return true;
    }

    private bool ProcessBrand(int id, InplaceEditor.Brand.Field prop, string content)
    {
        var brandItem = BrandService.GetBrandById(id);

        if (brandItem == null)
        {
            return false;
        }

        switch (prop)
        {
            case InplaceEditor.Brand.Field.Name:
                brandItem.Name = content;
                break;
            case InplaceEditor.Brand.Field.Description:
                brandItem.Description = content;
                break;
            case InplaceEditor.Brand.Field.BriefDescription:
                brandItem.BriefDescription = content;
                break;

            default:
                throw new NotImplementedException();
        }

        BrandService.UpdateBrand(brandItem);

        return true;
    }

    private List<string> ProcessImage(int id, InplaceEditor.Image.Field prop, string content, HttpFileCollection files, InplaceEditor.Image.Commands command, int objId, string additionalData, int? colorID)
    {
        List<string> result = new List<string>();

        switch (prop)
        {
            case InplaceEditor.Image.Field.Logo:


                switch (command)
                {
                    case InplaceEditor.Image.Commands.Add:
                    case InplaceEditor.Image.Commands.Update:
                        result.Add(UpdateLogo(files[0]));
                        TrialService.TrackEvent(TrialEvents.ChangeLogo, "");
                        break;

                    case InplaceEditor.Image.Commands.Delete:
                        result.Add(DeleteLogo());
                        break;
                    default:
                        throw new NotImplementedException();
                }



                break;

            case InplaceEditor.Image.Field.Brand:
                switch (command)
                {
                    case InplaceEditor.Image.Commands.Add:
                    case InplaceEditor.Image.Commands.Update:
                        result.Add(UpdateBrandImage(id, files[0]));
                        break;

                    case InplaceEditor.Image.Commands.Delete:
                        result.Add(DeleteBrandImage(id));
                        break;
                }
                break;

            case InplaceEditor.Image.Field.News:
                switch (command)
                {
                    case InplaceEditor.Image.Commands.Add:
                    case InplaceEditor.Image.Commands.Update:
                        result.Add(UpdateNewsImage(id, files[0]));
                        break;

                    case InplaceEditor.Image.Commands.Delete:
                        result.Add(DeleteNewsImage(id));
                        break;
                }
                break;
            case InplaceEditor.Image.Field.Carousel:
                switch (command)
                {
                    case InplaceEditor.Image.Commands.Add:

                        for (var i = 0; i < files.Count; i++)
                        {
                            result.Add(AddCarouselImage(files[i]));
                        }
                        TrialService.TrackEvent(TrialEvents.AddCarousel, "");

                        break;

                    case InplaceEditor.Image.Commands.Update:
                        result.Add(UpdateCarouselImage(id, files[0]));
                        break;

                    case InplaceEditor.Image.Commands.Delete:
                        result.Add(DeleteCarouselImage(id));
                        break;

                    default:
                        throw new NotImplementedException();
                }
                break;
            case InplaceEditor.Image.Field.CategorySmall:
            case InplaceEditor.Image.Field.CategoryBig:

                var categoryType = prop == InplaceEditor.Image.Field.CategorySmall ? PhotoType.CategorySmall : PhotoType.CategoryBig;

                switch (command)
                {
                    case InplaceEditor.Image.Commands.Add:
                        result.Add(AddCategoryImage(id, categoryType, files[0]));
                        break;

                    case InplaceEditor.Image.Commands.Update:
                        result.Add(AddCategoryImage(id, categoryType, files[0]));
                        break;

                    case InplaceEditor.Image.Commands.Delete:
                        result.Add(DeleteCategoryImage(id, categoryType));
                        break;
                    default:
                        throw new NotImplementedException();
                }
                break;
            case InplaceEditor.Image.Field.Product:

                var productImageType = (AdvantShop.FilePath.ProductImageType)Enum.Parse(typeof(AdvantShop.FilePath.ProductImageType), additionalData);

                switch (command)
                {
                    case InplaceEditor.Image.Commands.Add:
                        result.Add(AddProductImage(objId, colorID, productImageType, files[0]));
                        break;

                    case InplaceEditor.Image.Commands.Update:
                        result.Add(UpdateProductImage(id, objId, colorID, productImageType, files[0]));
                        break;

                    case InplaceEditor.Image.Commands.Delete:
                        PhotoService.DeleteProductPhoto(id);

                        var photoPathReturn = string.Empty;
                        var mainPhoto = PhotoService.GetMainProductPhoto(objId, colorID);

                        if (mainPhoto != null)
                        {
                            photoPathReturn = AdvantShop.FilePath.FoldersHelper.GetImageProductPath(productImageType, mainPhoto.PhotoName, false);
                        }
                        else
                        {
                            photoPathReturn = "images/nophoto_" + productImageType.ToString().ToLower() + ".jpg";
                        }

                        result.Add(photoPathReturn);
                        break;

                    default:
                        throw new NotImplementedException();

                }

                ProductService.PreCalcProductParams(objId);

                break;

            default:
                throw new NotImplementedException();

        }

        return result;
    }

    private string UpdateLogo(HttpPostedFile file)
    {
        FileHelpers.CreateDirectory(AdvantShop.FilePath.FoldersHelper.GetPathAbsolut(AdvantShop.FilePath.FolderType.ImageTemp));
        FileHelpers.DeleteFile(AdvantShop.FilePath.FoldersHelper.GetPathAbsolut(AdvantShop.FilePath.FolderType.Pictures, SettingsMain.LogoImageName));
        var newFile = file.FileName.FileNamePlusDate("logo");
        SettingsMain.LogoImageName = newFile;
        file.SaveAs(AdvantShop.FilePath.FoldersHelper.GetPathAbsolut(AdvantShop.FilePath.FolderType.Pictures, newFile));

        return AdvantShop.FilePath.FoldersHelper.GetPath(AdvantShop.FilePath.FolderType.Pictures, newFile, false);
    }

    private string DeleteLogo()
    {
        FileHelpers.DeleteFile(AdvantShop.FilePath.FoldersHelper.GetPathAbsolut(AdvantShop.FilePath.FolderType.Pictures, SettingsMain.LogoImageName));
        SettingsMain.LogoImageName = string.Empty;

        return "images/nophoto-logo.png";
    }


    private string UpdateBrandImage(int id, HttpPostedFile file)
    {
        PhotoService.DeletePhotos(id, PhotoType.Brand);

        var tempName = PhotoService.AddPhoto(new Photo(0, id, PhotoType.Brand) { OriginName = file.FileName });
        if (!string.IsNullOrWhiteSpace(tempName))
        {
            using (var image = System.Drawing.Image.FromStream(file.InputStream))
            {
                FileHelpers.SaveResizePhotoFile(AdvantShop.FilePath.FoldersHelper.GetPathAbsolut(AdvantShop.FilePath.FolderType.BrandLogo, tempName),
                                                SettingsPictureSize.BrandLogoWidth,
                                                SettingsPictureSize.BrandLogoHeight, image);
            }
        }


        return AdvantShop.FilePath.FoldersHelper.GetPath(AdvantShop.FilePath.FolderType.BrandLogo, tempName, false);
    }

    private string DeleteBrandImage(int id)
    {
        BrandService.DeleteBrandLogo(id);
        return "images/nophoto_xsmall.jpg";
    }

    private string UpdateNewsImage(int id, HttpPostedFile file)
    {

        PhotoService.DeletePhotos(id, PhotoType.News);

        var tempName = PhotoService.AddPhoto(new Photo(0, id, PhotoType.News) { OriginName = file.FileName });
        if (!string.IsNullOrWhiteSpace(tempName))
        {
            using (var image = System.Drawing.Image.FromStream(file.InputStream))
                FileHelpers.SaveResizePhotoFile(AdvantShop.FilePath.FoldersHelper.GetPathAbsolut(AdvantShop.FilePath.FolderType.News, tempName), SettingsPictureSize.NewsImageWidth, SettingsPictureSize.NewsImageHeight, image);
        }


        return AdvantShop.FilePath.FoldersHelper.GetPath(AdvantShop.FilePath.FolderType.News, tempName, false);
    }

    private string DeleteNewsImage(int id)
    {
        AdvantShop.News.NewsService.DeleteNewsImage(id);
        return "images/nophoto_small.jpg";
    }


    private string AddCarouselImage(HttpPostedFile file)
    {
        var carousel = new Carousel { URL = "#", SortOrder = CarouselService.GetMaxSortOrder(), Enabled = true };

        int id = CarouselService.AddCarousel(carousel);

        var li = new System.Text.StringBuilder();

        li.AppendFormat("<li><a href=\"{0}\"><img src=\"{1}\" alt=\"\" {2}></a></li>", carousel.URL, UpdateCarouselImage(id, file), InplaceEditor.Image.AttributesCarousel(id, InplaceEditor.Image.AttributesControls(true, true, true)));

        return li.ToString();
    }

    private string UpdateCarouselImage(int id, HttpPostedFile file)
    {

        PhotoService.DeletePhotos(id, PhotoType.Carousel);

        var tempName = PhotoService.AddPhoto(new Photo(0, id, PhotoType.Carousel) { OriginName = file.FileName });
        if (!string.IsNullOrWhiteSpace(tempName))
        {
            using (var image = System.Drawing.Image.FromStream(file.InputStream))
                FileHelpers.SaveResizePhotoFile(AdvantShop.FilePath.FoldersHelper.GetPathAbsolut(AdvantShop.FilePath.FolderType.Carousel, tempName), SettingsPictureSize.CarouselBigWidth, SettingsPictureSize.CarouselBigHeight, image);
        }


        return AdvantShop.FilePath.FoldersHelper.GetPath(AdvantShop.FilePath.FolderType.Carousel, tempName, false);
    }

    private string DeleteCarouselImage(int id)
    {
        AdvantShop.CMS.CarouselService.DeleteCarousel(id);

        return String.Format("<li><img id=\"carouselNoPhoto\" src=\"{0}\" alt=\"\" {1}><span class=\"inplace-slider-control js-inplace-slider-emulate\"></span></li>", "images/nophoto_carousel_" + SettingsMain.Language + ".jpg", InplaceEditor.Image.AttributesCarousel(-1, InplaceEditor.Image.AttributesControls(true, false, false)));
    }

    private string AddCategoryImage(int id, PhotoType photoType, HttpPostedFile file)
    {

        string result = string.Empty;

        PhotoService.DeletePhotos(id, photoType);

        var tempName = PhotoService.AddPhoto(new Photo(0, id, photoType) { OriginName = file.FileName });

        using (System.Drawing.Image image = System.Drawing.Image.FromStream(file.InputStream))
        {
            switch (photoType)
            {
                case PhotoType.CategoryBig:
                    FileHelpers.SaveResizePhotoFile(AdvantShop.FilePath.FoldersHelper.GetImageCategoryPathAbsolut(AdvantShop.FilePath.CategoryImageType.Big, tempName), SettingsPictureSize.BigCategoryImageWidth, SettingsPictureSize.BigCategoryImageHeight, image);
                    result = AdvantShop.FilePath.FoldersHelper.GetImageCategoryPath(AdvantShop.FilePath.CategoryImageType.Big, tempName, false);
                    break;

                case PhotoType.CategorySmall:
                    FileHelpers.SaveResizePhotoFile(AdvantShop.FilePath.FoldersHelper.GetImageCategoryPathAbsolut(AdvantShop.FilePath.CategoryImageType.Small, tempName), SettingsPictureSize.SmallCategoryImageWidth, SettingsPictureSize.SmallCategoryImageHeight, image);
                    result = AdvantShop.FilePath.FoldersHelper.GetImageCategoryPath(AdvantShop.FilePath.CategoryImageType.Small, tempName, false);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        return result;
    }

    private string DeleteCategoryImage(int id, PhotoType photoType)
    {
        PhotoService.DeletePhotos(id, photoType);

        return photoType == PhotoType.CategorySmall ? "images/nophoto_small.jpg" : "images/nophoto_big.jpg";

    }

    private string AddProductImage(int productID, int? colorID, AdvantShop.FilePath.ProductImageType productImageType, HttpPostedFile file, int photoSortOrder = 0)
    {

        var tempName =
            PhotoService.AddPhoto(new Photo(0, productID, PhotoType.Product)
            {
                OriginName = file.FileName,
                ColorID = colorID,
                PhotoSortOrder = photoSortOrder
            });

        using (System.Drawing.Image image = System.Drawing.Image.FromStream(file.InputStream, true))
        {
            FileHelpers.SaveProductImageUseCompress(tempName, image);
        }

        return AdvantShop.FilePath.FoldersHelper.GetImageProductPath(productImageType, tempName, false);
    }

    private string UpdateProductImage(int id, int productID, int? colorID, AdvantShop.FilePath.ProductImageType productImageType, HttpPostedFile file)
    {

        var photoSortOrder = PhotoService.GetPhoto(id).PhotoSortOrder;

        PhotoService.DeleteProductPhoto(id);

        return AddProductImage(productID, colorID, productImageType, file, photoSortOrder);
    }



    private bool ProcessMeta(int id, string name, MetaType metaType, string title, string h1, string metaKeywords, string metaDescription)
    {

        MetaInfo metaInfo;

        if (MetaInfoService.IsMetaExist(id, metaType))
        {
            metaInfo = MetaInfoService.GetMetaInfo(id, metaType);
            metaInfo.Title = title;
            metaInfo.H1 = h1;
            metaInfo.MetaKeywords = metaKeywords;
            metaInfo.MetaDescription = metaDescription;
        }
        else
        {
            metaInfo = new MetaInfo()
            {
                ObjId = id,
                Title = title,
                H1 = h1,
                MetaKeywords = metaKeywords,
                MetaDescription = metaDescription,
                Type = metaType
            };
        }

        InplaceEditor.Meta.Update(name, metaInfo);

        return true;
    }

    private object ErrorMessage(string message)
    {
        return new
        {
            isHasError = true,
            errorMessage = message
        };
    }

}