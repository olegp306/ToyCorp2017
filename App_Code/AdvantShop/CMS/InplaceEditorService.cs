using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.News;
using AdvantShop.Repository;
using AdvantShop.SEO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AdvantShop.CMS
{
    public class InplaceEditor
    {
        public enum ObjectType
        {
            StaticPage = 0,
            StaticBlock = 1,
            NewsItem = 2,
            Category = 3,
            Product = 4,
            Phone = 5,
            Offer = 6,
            Property = 7,
            Brand = 8,
            Image = 9,
            Meta = 10
        }

        public class StaticPage
        {
            public enum Field
            {
                PageName = 0,
                PageText = 1
            }

            public static string AttributeName(int staticPageId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayStaticPages))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{type: 'modal'}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}', required : true}}\"", staticPageId, ObjectType.StaticPage, Field.PageName);
                }

                return result;
            }

            public static string Attribute(int staticPageId, Field field)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayStaticPages))
                {
                    result = String.Format("data-plugin=\"inplace\"  data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", staticPageId, ObjectType.StaticPage, field);
                }

                return result;
            }

        }

        public class StaticBlock
        {
            public enum Field
            {
                Content = 0
            }

            public static string Attribute(int staticBlockId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayStaticBlocks))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", staticBlockId, ObjectType.StaticBlock, Field.Content);
                }

                return result;
            }
        }

        public class NewsItem
        {
            public enum Field
            {
                Title = 0,
                TextAnnotation = 1,
                TextToPublication = 2
            }

            public static string Attribute(int newsId, Field field)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayNews))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", newsId, ObjectType.NewsItem, field);
                }

                return result;
            }
        }

        public class Category
        {
            public enum Field
            {
                Name = 0,
                Description = 1,
                BriefDescription = 2
            }

            public static string AttributeName(int categoryId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result =
                        String.Format(
                            "data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline', updateObj: ['crumbs', 'tree']}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}', required : true}}\"",
                            categoryId, ObjectType.Category, Field.Name);
                }

                return result;
            }

            public static string Attribute(int categoryId, Field field)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", categoryId, ObjectType.Category, field);
                }

                return result;
            }

        }

        public class Product
        {
            public enum Field
            {
                Name = 0,
                ArtNo = 1,
                BriefDescription = 2,
                Description = 3,
                Unit = 4,
                Weight = 5
            }

            public static string AttibuteName(int productId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline', updateObj: ['crumbs']}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}', required : true}}\"", productId, ObjectType.Product, Field.Name);
                }

                return result;
            }

            public static string AttibuteArtNo(int productId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline'}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", productId, ObjectType.Product, Field.ArtNo);
                }

                return result;
            }

            public static string AttibuteDescription(int productId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\"  data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", productId, ObjectType.Product, Field.Description);
                }

                return result;
            }

            public static string AttibuteUnit(int productId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline', updateObj: ['amount']}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", productId, ObjectType.Product, Field.Unit);
                }

                return result;
            }

            public static string AttibuteWeight(int productId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline'}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}', float: true}}\"", productId, ObjectType.Product, Field.Weight);
                }

                return result;
            }
        }

        public class Phone
        {
            public enum Field
            {
                Number = 0
            }

            public static string Attribute()
            {
                return Attribute(IpZoneContext.CurrentZone.CityId);
            }

            public static string Attribute(int cityId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCommonSettings))
                {
                    result = String.Format("data-inplace-update=\"phone\" data-plugin=\"inplace\" data-inplace-options=\"{{updateObj: ['phone']}}\" data-inplace-params=\"{{ type: '{0}', prop: '{1}', cityId: {2}}}\"", ObjectType.Phone, Field.Number, cityId);
                }

                return result;
            }

        }

        public class Offer
        {
            public enum Field
            {
                Price = 0,
                PriceWithDiscount = 1,
                Amount = 2,
                ArtNo = 3
            }

            public static string AttributePriceDetails(int productId, Field field)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline', updateObj: ['priceDetails']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}', 'customOptions':null}}\"", productId, ObjectType.Offer, field);
                }

                return result;
            }

            public static string AttibuteAmount(int offerId, float amount = 0)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline'}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}', content: '{3}', float: true}}\"", offerId, ObjectType.Offer, Field.Amount, amount);
                }

                return result;
            }

            public static string AttibuteArtNo(int offerId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode:'singleline'}}\" data-inplace-params=\"{{id: {0}, type: '{1}', prop: '{2}'}}\"", offerId, ObjectType.Offer, Field.ArtNo);
                }

                return result;
            }
        }

        public class Property
        {
            public enum Field
            {
                Name = 0,
                Value = 1
            }

            public static string AttributeEdit(int propertyValueId, int propertyId, int productId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode: 'singleline', type: 'autocomplete', autocompleteUrl: './httphandlers/getpropertiesnames.ashx'}}\" data-inplace-params=\"{{'id':{0}, propertyId: {1}, productId: {2}, 'type':'{3}', 'prop':'{4}'}}\"", propertyValueId, propertyId, productId, ObjectType.Property, Field.Value);
                }

                return result;
            }

            public static string AttributeAdd(int productId, Field field)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{mode: 'singleline', type: 'autocomplete', autocompleteUrl: './httphandlers/getpropertiesnames.ashx'}}\" data-inplace-params=\"{{productId: {0}, 'type':'{1}', 'prop':'{2}'}}\"", productId, ObjectType.Property, field);
                }

                return result;
            }

            public static string RenderDeleteButton()
            {
                return String.Format("<a href=\"javascript:void(0);\" class=\"inplace-property-delete\" data-inplace-property-delete=\"true\">Delete</a>");
            }
        }

        public class Brand
        {
            public enum Field
            {
                Name = 0,
                Description = 1,
                BriefDescription = 2
            }

            public static string Attribute(int brandId, Field field)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayBrands))
                {
                    result = Attribute(brandId, field, string.Empty, string.Empty);
                };

                return result;
            }

            public static string Attribute(int brandId, Field field, string options, string paramsAdditional)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayBrands))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{{0}}}\" data-inplace-params=\"{{'id':{1}, 'type':'{2}', 'prop':'{3}', {4}}}\"", options, brandId, ObjectType.Brand, field, paramsAdditional);
                }

                return result;
            }
        }

        public class Image
        {
            public enum Field
            {
                Logo = 0,
                Brand = 1,
                News = 2,
                Carousel = 3,
                CategoryBig = 4,
                CategorySmall = 5,
                Product = 6
            }

            public enum Commands
            {
                Add = 0,
                Update = 1,
                Delete = 2
            }

            public static string AttributesLogo()
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCommonSettings))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"logo\" data-inplace-options=\"{{'type': 'image', 'updateObj':['logo']}}\" data-inplace-params=\"{{'id':'-1', 'type':'{0}', 'prop':'{1}'}}\"", ObjectType.Image, Field.Logo);
                }

                return result;
            }

            public static string AttributesBrand(int brandId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayBrands))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"{3}\" data-inplace-options=\"{{'type': 'image', 'updateObj':['{3}']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}'}}\"",
                        brandId,
                        ObjectType.Image,
                        Field.Brand,
                        ObjectType.Image.ToString() + Field.Brand.ToString() + brandId);
                }

                return result;
            }

            public static string AttributesNews(int newsId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayNews))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"{3}\" data-inplace-options=\"{{'type': 'image', 'updateObj':['{3}']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}'}}\"",
                        newsId,
                        ObjectType.Image,
                        Field.News,
                        ObjectType.Image.ToString() + Field.News.ToString() + newsId);
                }

                return result;
            }

            public static string AttributesCarousel(int carouselImageId)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCarousel))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"{3}\" data-inplace-options=\"{{'type': 'image', 'updateObj':['{3}']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}'}}\"",
                        carouselImageId,
                        ObjectType.Image,
                        Field.Carousel,
                        ObjectType.Image.ToString() + Field.Carousel.ToString() + carouselImageId);
                }

                return result;
            }

            public static string AttributesCarousel(int carouselImageId, string controlsOptions)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCarousel))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"{3}\" data-inplace-options=\"{{'type': 'image', 'updateObj':['{3}']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}'}}\" {4}",
                        carouselImageId,
                        ObjectType.Image,
                        Field.Carousel,
                        ObjectType.Image.ToString() + Field.Carousel.ToString() + carouselImageId,
                        controlsOptions);
                }

                return result;
            }

            public static string AttributesCategory(int categoryId, Field categoryType)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"{3}\" data-inplace-options=\"{{'type': 'image', 'updateObj':['{3}']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}'}}\"",
                        categoryId,
                        ObjectType.Image,
                        categoryType.ToString(),
                        ObjectType.Image.ToString() + categoryType.ToString() + categoryId);
                }

                return result;
            }

            public static string AttributesProduct(int photoId, int productId, AdvantShop.FilePath.ProductImageType productImageType)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"{3}\" data-inplace-options=\"{{'type': 'image', 'updateObj':['{3}']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}', 'objId':{4}, 'additionalData':'{5}'}}\"",
                        photoId,
                        ObjectType.Image,
                        Field.Product,
                        ObjectType.Image.ToString() + Image.Field.Product.ToString() + photoId,
                        productId,
                        productImageType);
                }

                return result;
            }

            public static string AttributesProduct(int photoId, int productId, AdvantShop.FilePath.ProductImageType productImageType, bool btnAdd, bool btnUpdate, bool btnDelete)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCatalog))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-update=\"{3}\" data-inplace-options=\"{{'type': 'image', 'updateObj':['{3}']}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'prop':'{2}', 'objId':{4}, 'additionalData':'{5}'}}\" {6}",
                        photoId,
                        ObjectType.Image,
                        Field.Product,
                        ObjectType.Image.ToString() + Field.Product.ToString() + photoId,
                        productId,
                        productImageType,
                        AttributesControls(btnAdd, btnUpdate, btnDelete));
                }

                return result;
            }

            public static string AttributesControls(bool btnAdd = false, bool btnUpdate = true, bool btnDelete = true)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCarousel))
                {
                    result = String.Format("data-inplace-controls=\"{{'add': {0}, 'update':{1}, 'delete':{2}}}\"", btnAdd.ToString().ToLower(), btnUpdate.ToString().ToLower(), btnDelete.ToString().ToLower());
                }

                return result;
            }

        }

        public class Meta
        {
            public enum Field
            {
                Name = 0,
                H1 = 1,
                MetaKeywords = 2,
                MetaDescription = 3,
                Title = 4
            }

            public static string Attribute(MetaType metaType, int Id)
            {
                var result = "";

                if (CanUseInplace(RoleActionKey.DisplayCommonSettings))
                {
                    result = String.Format("data-plugin=\"inplace\" data-inplace-options=\"{{type: 'modal'}}\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', metaType: '{2}'}}\"", Id, ObjectType.Meta, metaType);
                }

                return result;
            }

            public static void Update(string name, MetaInfo metaInfo)
            {

                var query = string.Empty;

                switch (metaInfo.Type)
                {
                    case MetaType.Brand:
                        query = "UPDATE Catalog.Brand SET BrandName=@name Where BrandID=@objId";
                        break;
                    case MetaType.Category:
                        query = "UPDATE [Catalog].[Category] SET [Name] = @name WHERE CategoryID = @objId";
                        break;
                    case MetaType.News:
                        query = "UPDATE [Settings].[News] SET [Title] = @name WHERE NewsID = @objId";
                        break;
                    case MetaType.Product:
                        query = "UPDATE [Catalog].[Product] SET [Name] = @name WHERE [ProductID] = @objId";
                        break;
                    case MetaType.StaticPage:
                        query = "UPDATE [CMS].[StaticPage] SET [PageName] = @name WHERE [StaticPageID] = @objId";
                        break;
                }

                SQLDataAccess.ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@name", name), new SqlParameter("@objId", metaInfo.ObjId));

                if (MetaType.Category == metaInfo.Type)
                {
                    CategoryService.ClearCategoryCache();
                }

                MetaInfoService.SetMeta(metaInfo);
            }

        }

        public static bool CanUseInplace(RoleActionKey role)
        {
            return SettingsMain.EnableInplace && (CustomerContext.CurrentCustomer.IsAdmin
                || (CustomerContext.CurrentCustomer.IsModerator && CustomerContext.CurrentCustomer.HasRoleAction(role))
                || Trial.TrialService.IsTrialEnabled);
        }

        public static string GetContent(ObjectType objectType, int objId)
        {
            var result = string.Empty;

            switch (objectType)
            {
                case ObjectType.StaticPage:
                    var sp = StaticPageService.GetStaticPage(objId);
                    result = sp != null ? sp.PageText : string.Empty;
                    break;
                case ObjectType.StaticBlock:
                    var sb = StaticBlockService.GetPagePart(objId);
                    result = sb != null ? sb.Content : string.Empty;
                    break;
            }

            return result;
        }

    }
}