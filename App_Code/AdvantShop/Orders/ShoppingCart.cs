//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;

namespace AdvantShop.Orders
{
    public class ShoppingCart : List<ShoppingCartItem>
    {
        private float _totalPrice = float.MinValue;
        public float TotalPrice
        {
            get
            {
                return _totalPrice != float.MinValue
                    ? _totalPrice
                    : (_totalPrice = this.Sum(shpItem => shpItem.Price * shpItem.Amount));
            }
        }

        public float TotalShippingWeight
        {
            get { return this.Sum(shpItem => shpItem.Offer.Product.Weight * shpItem.Amount); }
        }

        public float TotalItems
        {
            get { return this.Sum(item => item.Amount); }
        }

        public bool HasItems
        {
            get { return this.Any(); }
        }

        public float MinimalPrice
        {
            get { return SettingsOrderConfirmation.MinimalOrderPrice; }
        }

        public bool CanOrder
        {
            get
            {
                if (TotalPrice < SettingsOrderConfirmation.MinimalOrderPrice || !HasItems)
                    return false;
                return
                    !this.Any(
                        item =>
                        !item.Offer.Product.Enabled ||
                         (item.Amount > item.Offer.Amount && SettingsOrderConfirmation.AmountLimitation && !item.Offer.CanOrderByRequest) || 
                         item.Amount < item.Offer.Product.MinAmount ||
                         item.Amount > item.Offer.Product.MaxAmount);
            }
        }

        public float DiscountPercentOnTotalPrice
        {
            get
            {
                return (Coupon == null && CustomerContext.CurrentCustomer.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup)
                    ? OrderService.GetDiscount(TotalPrice)
                    : 0;
            }
        }

        public float TotalDiscount
        {
            get
            {
                if (CustomerContext.CurrentCustomer.CustomerGroupId != CustomerGroupService.DefaultCustomerGroup)
                    return 0;

                float discount = 0;
                discount += DiscountPercentOnTotalPrice > 0 ? DiscountPercentOnTotalPrice * TotalPrice / 100 : 0;
                discount += Certificate != null ? Certificate.Sum : 0;

                if (Coupon != null)
                {
                    if (this.Where(shpItem => shpItem.IsCouponApplied).Sum(shpItem => shpItem.Price * shpItem.Amount) >= Coupon.MinimalOrderPrice)
                    {
                        switch (Coupon.Type)
                        {
                            case CouponType.Fixed:
                                var productsPrice = this.Where(shpItem => shpItem.IsCouponApplied).Sum(shpItem => shpItem.Price * shpItem.Amount);
                                discount += productsPrice >= Coupon.Value ? Coupon.Value : productsPrice;
                                break;
                            case CouponType.Percent:
                                discount += this.Where(shpItem => shpItem.IsCouponApplied).Sum(shpItem => Coupon.Value * shpItem.Price / 100 * shpItem.Amount);
                                break;
                        }
                    }
                }
                return discount;
            }
        }

        private GiftCertificate _certificate;

        public GiftCertificate Certificate
        {
            get
            {
                if (_certificate == null)
                {
                    _certificate = GiftCertificateService.GetCustomerCertificate();
                }

                if (_certificate != null && _coupon != null)
                    throw new Exception("Coupon and Certificate can't be used together");

                if (_certificate != null && (!_certificate.Paid || _certificate.Used || !_certificate.Enable))
                {
                    GiftCertificateService.DeleteCustomerCertificate(_certificate.CertificateId);
                    return null;
                }

                return _certificate;
            }
        }

        private Coupon _coupon;
        public Coupon Coupon
        {
            get
            {
                if (_coupon == null)
                {
                    _coupon = CouponService.GetCustomerCoupon();
                }

                if (_coupon != null && _certificate != null)
                    throw new Exception("Coupon and Certificate can't be used together");

                if (_coupon != null && ((_coupon.ExpirationDate != null && _coupon.ExpirationDate < DateTime.Now) || (_coupon.PossibleUses != 0 && _coupon.PossibleUses <= _coupon.ActualUses) || !_coupon.Enabled))
                {
                    CouponService.DeleteCustomerCoupon(_coupon.CouponID);
                }

                return _coupon;
            }
        }

        public override int GetHashCode()
        {
            return this.Aggregate(0, (curr, val) => val.GetHashCode()) + (Certificate != null ? Certificate.GetHashCode() : 0) + (Coupon != null ? Coupon.GetHashCode() : 0);
        }
    }
}