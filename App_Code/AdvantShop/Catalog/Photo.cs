//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Catalog
{
    public enum PhotoType
    {
        Product,
        CategoryBig,
        CategorySmall,
        CategoryIcon,
        News,
        StaticPage,
        Brand,
        Carousel,
        MenuIcon,
        Shipping,
        Payment,
        Color
    }

    [Serializable]
    public class Photo
    {
        private readonly int _photoId;
        public int PhotoId { get { return _photoId; } }

        private readonly int _objId;
        public int ObjId { get { return _objId; } }

        private readonly PhotoType _type;
        public PhotoType Type { get { return _type; } }


        public string PhotoName { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Description { get; set; }

        public int PhotoSortOrder { get; set; }

        public bool Main { get; set; }

        public string OriginName { get; set; }

        public int? ColorID { get; set; }

        //constructors
        public Photo(int photoId, int objId, PhotoType type)
        {
            _photoId = photoId;
            _objId = objId;
            _type = type;
        }
    }
}