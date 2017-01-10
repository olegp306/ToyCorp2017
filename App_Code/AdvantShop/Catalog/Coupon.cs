//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Catalog
{
    public enum CouponType
    {
        Fixed = 1,
        Percent = 2
    }

    public class Coupon
    {
        public int CouponID { get; set; }
        public string Code { get; set; }
        public CouponType Type { get; set; }
        public float Value { get; set; }
        public DateTime AddingDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int PossibleUses { get; set; }
        public int ActualUses { get; set; }
        public bool Enabled { get; set; }
        public float MinimalOrderPrice { get; set; }

        private IEnumerable<int> _categoryIds;
        public IEnumerable<int> CategoryIds
        {
            get
            {
                return _categoryIds ?? (_categoryIds = CouponService.GetCategoriesIDsByCoupon(CouponID));
            }
        }

        private IEnumerable<int> _productsIds;
        public IEnumerable<int> ProductsIds
        {
            get
            {
                return _productsIds ?? (_productsIds = CouponService.GetCategoriesIDsByCoupon(CouponID));
            }
        }

        public override int GetHashCode()
        {
            return CouponID.GetHashCode() ^ Code.GetHashCode() ^ PossibleUses.GetHashCode() ^ ActualUses.GetHashCode() ^
                   MinimalOrderPrice.GetHashCode() ^ ExpirationDate.GetHashCode() ^ Type.GetHashCode() ^ Value.GetHashCode();
        }
    }
}