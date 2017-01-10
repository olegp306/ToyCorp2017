//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Catalog;
using AdvantShop.SEO;

namespace AdvantShop.News
{
    public class NewsItem //: IMetaContainer
    {
        public int NewsID { get; set; }

        public int NewsCategoryID { get; set; }

        public string Title { get; set; }

        //public string Picture { get; set; }

        private Photo _picture;
        public Photo Picture
        {
            get
            {
                return _picture ?? (_picture = PhotoService.GetPhotoByObjId(NewsID, PhotoType.News));
            }
            set
            {
                _picture = value;
            }
        }

        public string TextToPublication { get; set; }

        public string TextToEmail { get; set; }

        public string TextAnnotation { get; set; }

        public bool ShowOnMainPage { get; set; }

        public DateTime AddingDate { get; set; }

        private string _urlPath;
        public string UrlPath
        {
            get { return _urlPath; }
            set { _urlPath = value.ToLower(); }
        }

        public MetaType MetaType
        {
            get { return MetaType.News; }
        }

        private MetaInfo _meta;
        public MetaInfo Meta
        {
            get
            {
                return _meta ??
                       (_meta =
                        MetaInfoService.GetMetaInfo(NewsID, MetaType) ??
                        MetaInfoService.GetDefaultMetaInfo(MetaType, Title));
            }
            set
            {
                _meta = value;
            }
        }

        public int ID
        {
            get { return NewsID; }
        }
    }
}