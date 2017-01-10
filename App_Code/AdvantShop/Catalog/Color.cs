namespace AdvantShop.Catalog
{
    public class Color
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public int SortOrder { get; set; }

        private Photo _picture;
        public Photo IconFileName
        {
            get
            {
                return _picture ?? (_picture = PhotoService.GetPhotoByObjId(ColorId, PhotoType.Color));
            }
            set
            {
                _picture = value;
            }
        }
        public override string ToString()
        {
            return ColorName;
        }
    }
}