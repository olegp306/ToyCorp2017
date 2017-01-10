//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.Shipping
{
    public enum ShippingType
    {
        None = 0,
        FreeShipping = 1,
        FixedRate = 2,
        ShippingByWeight = 3,
        FedEx = 4,
        Ups = 5,
        Usps = 6,
        eDost = 7,
        ShippingByShippingCost = 8,
        ShippingByOrderPrice = 9,
        ShippingByRangeWeightAndDistance = 10,
        ShippingNovaPoshta = 11,
        SelfDelivery = 12,
        Multiship = 13,
        Cdek = 14,
        ShippingByProductAmount = 15,
        ShippingByEmsPost = 16,
        CheckoutRu = 17
    }

    [Flags]
    public enum ExtendedType
    {
        None = 0,
        CashOnDelivery = 1,
        Pickpoint = 2,
        Boxberry = 4,
        Cdek = 8,
        Checkout = 16
    }

    public class ShippingOption
    {
        public int ShippingMethodId { get; set; }
        public float Rate { get; set; }
        public bool IsMinimumRate { get; set; }
        public string Name { get; set; }
        public string DeliveryTime { get; set; }

        public ShippingOptionEx Extend { get; set; }
        public List<ShippingPoint> ShippingPoints { get; set; }

        public ShippingOption()
        {
            ShippingPoints = new List<ShippingPoint>();
        }
    }

    public class ShippingOptionEx
    {
        public ExtendedType Type { get; set; }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public float Transfer { get; set; }
        public string Pickpointmap { get; set; }
        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        public string AdditionalData { get; set; }
        public int ShippingId { get; set; }
    }

    public class ShippingPoint
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string AdditionalData { get; set; }
        public float Rate { get; set; }
    }

    //*********************************************
    [Serializable]
    public class ShippingMethod
    {
        public int ShippingMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        public bool DisplayCustomFields { get; set; }
        public bool ShowInDetails { get; set; }

        public string ZeroPriceMessage { get; set; }

        private Photo _picture;
        public Photo IconFileName
        {
            get { return _picture ?? (_picture = PhotoService.GetPhotoByObjId(ShippingMethodId, PhotoType.Shipping)); }
            set { _picture = value; }
        }

        public virtual ShippingType Type { get; set; }

        //public bool IsCalcualtable
        //{
        //    get
        //    {
        //        return Type == ShippingType.FedEx || Type == ShippingType.Ups || Type == ShippingType.Usps ||
        //               Type == ShippingType.eDost || Type == ShippingType.Multiship;
        //    }
        //}

        private Dictionary<string, string> _params;
        public Dictionary<string, string> Params
        {
            get { return _params ?? (_params = ShippingMethodService.GetShippingParams(ShippingMethodId)); }
            set { _params = value; }
        }

        public static bool GetBooleanParam(Dictionary<string, string> items, string paramName)
        {
            return items.ElementOrDefault(paramName).TryParseBool();
        }
    }
}