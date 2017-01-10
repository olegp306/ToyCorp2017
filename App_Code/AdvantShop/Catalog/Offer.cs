//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Catalog
{
    [Serializable]
    public class Offer
    {

        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public float Amount { get; set; }
        public float Price { get; set; }
        public float SupplyPrice { get; set; }
        public int? ColorID { get; set; }
        public int? SizeID { get; set; }
        public bool Main { get; set; }
        public string ArtNo { get; set; }

        [NonSerialized]
        private Color _color;

        [System.Xml.Serialization.XmlIgnore]
        public Color Color
        {
            get { return _color ?? (_color = ColorService.GetColor(ColorID)); }
        }

        [NonSerialized]
        private Size _size;

        [System.Xml.Serialization.XmlIgnore]
        public Size Size
        {
            get
            {
                return _size ?? (_size = SizeService.GetSize(SizeID));
            }
        }

        [NonSerialized]
        private Product _product;

        [System.Xml.Serialization.XmlIgnore]
        public Product Product
        {
            get
            {
                return _product ?? (_product = ProductService.GetProduct(ProductId));
            }
        }

        [NonSerialized]
        private Photo _photo;

        [System.Xml.Serialization.XmlIgnore]
        public Photo Photo
        {
            get
            {
                return _photo ?? (_photo = PhotoService.GetMainProductPhoto(ProductId, ColorID));
            }
        }

        public bool CanOrderByRequest
        {
            get { return Product != null && Product.AllowPreOrder && (Amount <= 0 || Price == 0); }
        }

    }
}
