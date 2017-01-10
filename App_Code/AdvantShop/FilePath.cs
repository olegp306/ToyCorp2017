//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.IO;
using AdvantShop.Configuration;

namespace AdvantShop.FilePath
{
    public enum ProductImageType
    {
        Big,
        Middle,
        Small,
        XSmall,
        Original
    }

    public enum CategoryImageType
    {
        Big,
        Small,
        Icon
    }

    public enum ColorImageType
    {
        Details,
        Catalog,
    }

    public enum FolderType
    {
        Pictures,
        MenuIcons,
        Product,
        Color,
        Carousel,
        Category,
        News,
        StaticPage,
        BrandLogo,
        PaymentLogo,
        ShippingLogo,
        PriceTemp,
        ImageTemp,
        OneCTemp
    }

    public class FoldersHelper
    {
        public static readonly Dictionary<FolderType, string> PhotoFoldersPath = new Dictionary<FolderType, string>
                                                                                      {
                                                                                          {FolderType.Pictures, "pictures/"},
                                                                                          {FolderType.MenuIcons, "pictures/icons/"},
                                                                                          {FolderType.Product, "pictures/product/"},
                                                                                          {FolderType.Color, "pictures/color/"},
                                                                                          {FolderType.Carousel, "pictures/carousel/"},
                                                                                          {FolderType.News, "pictures/news/"},
                                                                                          {FolderType.Category, "pictures/category/"},
                                                                                          {FolderType.BrandLogo, "pictures/brand/"},
                                                                                          {FolderType.PaymentLogo, "pictures/payment/"},
                                                                                          {FolderType.ShippingLogo, "pictures/shipping/"},
                                                                                          {FolderType.StaticPage, "pictures/staticpage/"},
                                                                                          {FolderType.PriceTemp, "price_temp/"},
                                                                                          {FolderType.ImageTemp, "upload_images/"},
                                                                                          {FolderType.OneCTemp, "1c_temp/"},
                                                                                      };

        public static readonly Dictionary<CategoryImageType, string> CategoryPhotoPrefix = new Dictionary<CategoryImageType, string>
                                                                                            {
                                                                                                {CategoryImageType.Small, @"small/"},
                                                                                                {CategoryImageType.Big, @""},
                                                                                                {CategoryImageType.Icon, @"icon/"},
                                                                                            };

        public static readonly Dictionary<ColorImageType, string> ColorPhotoPrefix = new Dictionary<ColorImageType, string>
                                                                                            {
                                                                                                {ColorImageType.Details, @"details/"},
                                                                                                {ColorImageType.Catalog, @"catalog/"},
                                                                                            };
        public static readonly Dictionary<ProductImageType, string> ProductPhotoPrefix = new Dictionary<ProductImageType, string>
                                                                                            {
                                                                                                {ProductImageType.XSmall, @"xsmall/"},
                                                                                                {ProductImageType.Small, @"small/"},
                                                                                                {ProductImageType.Middle, @"middle/"},
                                                                                                {ProductImageType.Big, @"big/"},
                                                                                                {ProductImageType.Original, @"original/"}
                                                                                            };
        public static readonly Dictionary<ProductImageType, string> ProductPhotoPostfix = new Dictionary<ProductImageType, string>
                {
                    {ProductImageType.XSmall, @"_xsmall"},
                    {ProductImageType.Small, @"_small"},
                    {ProductImageType.Middle, @"_middle"},
                    {ProductImageType.Big, @"_big"},
                    {ProductImageType.Original, @"_original"}
                };

        private static string GetPath(string imagePathBase, bool isForAdministration)
        {
            return (isForAdministration ? "../" : string.Empty) + imagePathBase;
        }

        private static string GetPathAbsolut(string imagePathBase)
        {
            return SettingsGeneral.AbsolutePath + imagePathBase;
        }

        //_____________
        public static string GetPath(FolderType type, string photoPath, bool isForAdministration)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPath(PhotoFoldersPath[type], isForAdministration);
            return GetPath(PhotoFoldersPath[type], isForAdministration) + photoPath;
        }

        public static string GetPathAbsolut(FolderType type, string photoPath = "")
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[type]);
            return GetPathAbsolut(PhotoFoldersPath[type]) + photoPath;
        }
        //_____________


        #region ProductImage
        public static string GetImageProductPath(ProductImageType type, string photoPath, bool isForAdministration)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return "images/nophoto" + ProductPhotoPostfix[type] + ".jpg";
            return GetPath(PhotoFoldersPath[FolderType.Product], isForAdministration) + ProductPhotoPrefix[type] + Path.GetFileNameWithoutExtension(photoPath) + ProductPhotoPostfix[type] + Path.GetExtension(photoPath);
        }

        public static string GetImageProductPathAbsolut(ProductImageType type, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.Product]) + ProductPhotoPrefix[type];
            return GetPathAbsolut(PhotoFoldersPath[FolderType.Product]) + ProductPhotoPrefix[type] + Path.GetFileNameWithoutExtension(photoPath) + ProductPhotoPostfix[type] + Path.GetExtension(photoPath);
        }
        #endregion


        #region CategoryImage
        public static string GetImageCategoryPathAbsolut(CategoryImageType type, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.Category]) + CategoryPhotoPrefix[type];

            return GetPathAbsolut(PhotoFoldersPath[FolderType.Category]) + CategoryPhotoPrefix[type] + photoPath;
        }

        public static string GetImageCategoryPath(CategoryImageType type, string photoPath, bool isForAdministration)
        {
            return GetPath(PhotoFoldersPath[FolderType.Category], isForAdministration) + CategoryPhotoPrefix[type] + photoPath;
        }
        #endregion

        public static string GetImageColorPathAbsolut(ColorImageType type, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.Color]) + ColorPhotoPrefix[type];

            return GetPathAbsolut(PhotoFoldersPath[FolderType.Color]) + ColorPhotoPrefix[type] + photoPath;
        }

        public static string GetImageColorPath(ColorImageType type, string photoPath, bool isForAdministration)
        {
            return GetPath(PhotoFoldersPath[FolderType.Color], isForAdministration) + ColorPhotoPrefix[type] + photoPath;
        }

    }
}