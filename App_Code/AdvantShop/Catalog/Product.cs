//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;

namespace AdvantShop.Catalog
{
    public enum RelatedType
    {
        Related = 0,
        Alternative = 1
    }

    public class Product //: IMetaContainer
    {
        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string PhotoDesc { get; set; } // убрать или сделать lazy
        public double Ratio { get; set; } // убрать или сделать lazy
        public float Discount { get; set; }
        public float Weight { get; set; }
        public string Size { get; set; }
        /*public int PhotoId { get; set; }*/

        public string BriefDescription { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool Recomended { get; set; }
        public bool New { get; set; }
        public bool BestSeller { get; set; }
        public bool OnSale { get; set; }
        public bool AllowPreOrder { get; set; }
        public bool CategoryEnabled { get; set; }
        
        public string Unit { get; set; }
        public float ShippingPrice { get; set; }

        public float? MinAmount { get; set; }
        public float? MaxAmount { get; set; }
        public float Multiplicity { get; set; }

        public string SalesNote { get; set; }
        public string GoogleProductCategory { get; set; }
        public string Gtin { get; set; }
        public bool Adult { get; set; }

        public bool AddManually { get; set; }

        public bool ManufacturerWarranty { get; set; }

        public int PopularityManually { get; set; }

        //public bool CanOrderByRequest
        //{
        //    get { return ((ProductService.IsExists(ProductId)) && (AllowPreOrder) && (Offers[0].Amount <= 0)); }
        //}

        public int BrandId { get; set; }

        private Brand _brand;
        public Brand Brand
        {
            get { return _brand ?? (_brand = BrandService.GetBrandById(BrandId)); }
        }

        
        private string _urlPath;
        public string UrlPath
        {
            get { return _urlPath; }
            set { _urlPath = value.ToLower(); }
        }


        public bool HasMultiOffer { get; set; }

        /// <summary>
        /// Get from DB collection of Offer and set collection
        /// </summary>
        private List<Offer> _offers;
        public List<Offer> Offers
        {
            get { return _offers ?? (_offers = OfferService.GetProductOffers(ProductId)); }
            set
            {
                _offers = value;
            }
        }

        public float MainPrice
        {
            get { return (Offers == null) || (Offers.Count == 0) ? 0 : Offers.OrderByDescending(offer=>offer.Main).First().Price; }
        }

        public float TotalAmount
        {
            get { return (Offers == null) || (Offers.Count == 0) ? 0 : Offers.Sum(offer=>offer.Amount); }
        }

        public float CalculableDiscount
        {
            get 
            {
                foreach (var discountModule in AttachedModules.GetModules<IDiscount>())
                {
                    var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                    var discount = classInstance.GetDiscount(ProductId);
                    if (discount != 0)
                        return discount;
                    break;
                }

                if (Discount == 0)
                    return DiscountByTimeService.GetDiscountByTime();

                return Discount;
            }
        }

        public MetaType MetaType
        {
            get { return MetaType.Product; }
        }

        private MetaInfo _meta;
        public MetaInfo Meta
        {
            get
            {
                return _meta ??
                       (_meta =
                        MetaInfoService.GetMetaInfo(ProductId, MetaType) ??
                        MetaInfoService.GetDefaultMetaInfo(MetaType, string.Empty));
            }
            set
            {
                _meta = value;
            }
        }

        /// <summary>
        /// return collection of ProductPhoto
        /// </summary>
        private List<Photo> _productphotos;
        public List<Photo> ProductPhotos
        {
            get { return _productphotos ?? (_productphotos = PhotoService.GetPhotos(ProductId, PhotoType.Product).ToList()); }
        }
        private List<PropertyValue> _productPropertyValues;

        private List<ProductVideo> _productVideos;
        public List<ProductVideo> ProductVideos
        {
            get { return _productVideos ?? (_productVideos = ProductVideoService.GetProductVideos(ProductId)); }
        }

        public List<PropertyValue> ProductPropertyValues
        {
            get
            {
                return _productPropertyValues ??
                       (_productPropertyValues = PropertyService.GetPropertyValuesByProductId(ProductId));
            }
        }

        private int _categoryId;
        public int CategoryId
        {
            get { return _categoryId == 0 || _categoryId == CategoryService.DefaultNonCategoryId ? _categoryId = ProductService.GetFirstCategoryIdByProductId(ProductId) : _categoryId; }
        }

        private Category _mainCategory;
        public Category MainCategory
        {
            get { return _mainCategory ?? (_mainCategory = CategoryService.GetCategory(CategoryId)); }
        }

        private List<Category> _productCategories;
        [SoapIgnore]
        [XmlIgnoreAttribute]
        public List<Category> ProductCategories
        {
            get { return _productCategories ?? (_productCategories = ProductService.GetCategoriesByProductId(ProductId)); }
        }

        private List<Product> _relatedProducts;
        public List<Product> RelatedProducts
        {
            get
            {
                return _relatedProducts ??
                       (_relatedProducts = ProductService.GetRelatedProducts(ProductId, RelatedType.Related));
            }
        }

        private List<Product> _alternativeProducts;
        public List<Product> AlternativeProduct
        {
            get
            {
                return _alternativeProducts ??
                       (_alternativeProducts = ProductService.GetRelatedProducts(ProductId, RelatedType.Alternative));
            }
        }

        public int ID
        {
            get { return ProductId; }
        }
    }
}