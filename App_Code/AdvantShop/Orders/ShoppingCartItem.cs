//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Orders
{
    public class ShoppingCartItem
    {
        [JsonIgnore]
        public int ShoppingCartItemId { get; set; }

        [JsonIgnore]
        public ShoppingCartType ShoppingCartType { get; set; }
        
        [JsonIgnore]
        public Guid CustomerId { get; set; }

        [JsonIgnore]
        public string AttributesXml { get; set; }

        public float Amount { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; }

        [JsonIgnore]
        public DateTime UpdatedOn { get; set; }

        public int OfferId { get; set; }

        private Coupon _coupon;

        [JsonIgnore]
        public bool IsCouponApplied
        {
            get
            {
                if (_coupon == null)
                    _coupon = CouponService.GetCustomerCoupon(CustomerId);
                return _coupon != null && CouponService.IsCouponAppliedToProduct(_coupon.CouponID, Offer.ProductId);
            }
        }

        private Offer _offer;
        [JsonIgnore]
        public Offer Offer
        {
            get
            {
                return _offer ?? (_offer = OfferService.GetOffer(OfferId));
            }
        }


        [JsonIgnore]
        public IList<EvaluatedCustomOptions> EvaluatedCustomOptions
        {
            get { return CustomOptionsService.DeserializeFromXml(AttributesXml); }
        }

        public override int GetHashCode()
        {
            return OfferId ^ Amount.GetHashCode() ^ AttributesXml.GetHashCode();
        }

        private CustomerGroup _customerGroup;
        private CustomerGroup CustomerGroup
        {
            get
            {
                return _customerGroup ?? (_customerGroup = CustomerContext.CurrentCustomer.RegistredUser
                    ? CustomerContext.CurrentCustomer.CustomerGroup
                    : CustomerGroupService.GetCustomerGroup(CustomerGroupService.DefaultCustomerGroup));
            }
        }

        private float _price = float.MinValue;
        public float Price
        {
            get
            {
                return _price != float.MinValue
                    ? _price
                    : (_price =
                        CatalogService.CalculateProductPrice(Offer.Price, Offer.Product.CalculableDiscount,
                            CustomerGroup, CustomOptionsService.DeserializeFromXml(AttributesXml)));
            }
        }
    }
}