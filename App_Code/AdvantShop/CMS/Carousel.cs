//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;

namespace AdvantShop.CMS

{
    public class Carousel
    {
        public int CarouselID { get; set; }
        public int SortOrder { get; set; }
        public string URL { get; set; }
        public bool Enabled { set; get; }

        private Photo _picture;
        public Photo Picture
        {
            get
            {
                return _picture ?? (_picture = PhotoService.GetPhotoByObjId(CarouselID, PhotoType.Carousel));
            }
            set
            {
                _picture = value;
            }
        }
    }
}