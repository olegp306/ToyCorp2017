//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace AdvantShop.Catalog
{
    [Serializable]
    public class Category //: IMetaContainer
    {
        public Category()
        {
            CategoryId = CategoryService.DefaultNonCategoryId;
        }

        public Category(int id, string name)
        {
            CategoryId = id;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, SQLDataHelper.GetString(CategoryId));
        }

        public int ID
        {
            get { return CategoryId; }
        }

        public string Description { get; set; }
        public string BriefDescription { get; set; }
        public bool Enabled { get; set; }
        public bool HasChild { get; set; }
        public int SortOrder { get; set; }
        public int ProductsCount { get; set; }
        public int TotalProductsCount { get; set; }
        public int ParentCategoryId { get; set; }
        public int CategoryId { get; set; }

        private Photo _picture;
        public Photo Picture
        {
            get
            {
                return _picture ?? (_picture = PhotoService.GetPhotoByObjId(CategoryId, PhotoType.CategoryBig));
            }
            set
            {
                _picture = value;
            }
        }

        private Photo _minipicture;
        public Photo MiniPicture
        {
            get
            {
                return _minipicture ?? (_minipicture = PhotoService.GetPhotoByObjId(CategoryId, PhotoType.CategorySmall));
            }
            set
            {
                _minipicture = value;
            }
        }

        private Photo _icon;
        public Photo Icon
        {
            get
            {
                return _icon ?? (_icon = PhotoService.GetPhotoByObjId(CategoryId, PhotoType.CategoryIcon));
            }
            set
            {
                _icon = value;
            }
        }


        public string Name { get; set; }
        public string DisplayStyle { get; set; }
        public bool DisplayChildProducts { get; set; }
        public bool DisplayBrandsInMenu { get; set; }
        public bool DisplaySubCategoriesInMenu { get; set; }
        public bool ParentsEnabled { get; set; }

        private Category _parentcategory;

        public Category ParentCategory { get { return _parentcategory ?? (_parentcategory = CategoryService.GetCategory(ParentCategoryId)); } }
        
        private List<Category> _childCategories;

        [SoapIgnore]
        [XmlIgnoreAttribute]
        public List<Category> ChildCategories
        {
            get
            {
                return _childCategories ??
                       (_childCategories =
                           (List<Category>) CategoryService.GetChildCategoriesByCategoryId(CategoryId, false));
            }
            set { _childCategories = value; }
        }


        private List<int> _productIDs;
        public List<int> ProductIDs
        {
            get { return _productIDs ?? (_productIDs = CategoryService.GetProductIDs(CategoryId).ToList()); }
        }


        private List<Product> _products;
        [SoapIgnore]
        [XmlIgnoreAttribute]
        public List<Product> Products
        {
            get { return _products ?? (_products = (List<Product>)CategoryService.GetProductsByCategoryId(CategoryId, DisplayChildProducts)); }
        }



        public MetaType MetaType
        {
            get { return MetaType.Category; }
        }

        private string _urlPath;
        public string UrlPath
        {
            get { return _urlPath; }
            set { _urlPath = value.ToLower(); }
        }

        private MetaInfo _meta;
        public MetaInfo Meta
        {
            get
            {
                return _meta ??
                       (_meta =
                        MetaInfoService.GetMetaInfo(CategoryId, MetaType) ??
                        MetaInfoService.GetDefaultMetaInfo(MetaType, Name));
            }
            set
            {
                _meta = value;
            }
        }

        private int _productCount = -1;
        public int GetProductCount()
        {
            if (_productCount != -1) return _productCount;
            _productCount = CategoryService.GetEnabledProductsCountInCategory(CategoryId, DisplayChildProducts);
            return _productCount;
        }

        public ESortOrder Sorting { get; set; }

        //private int _hierarchyProductsCount = -1;
        //public int HierarchyProductsCount
        //{
        //    get
        //    {
        //        if (_hierarchyProductsCount == -1)
        //        {
        //            _hierarchyProductsCount = CategoryService.GetHierarchyProductsCount(CategoryId);
        //        }

        //        return _hierarchyProductsCount;
        //    }
        //}
    }
}
