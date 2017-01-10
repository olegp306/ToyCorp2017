//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;

namespace AdvantShop.Orders
{
    [Serializable]
    public class OrderItem : IOrderItem
    {
        public int OrderItemID { get; set; }

        public int OrderID { get; set; }

        public int? ProductID { get; set; }

        public int? CertificateID { get; set; }

        public string ArtNo { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public float Amount { get; set; }

        public float DecrementedAmount { get; set; }

        public string Color { get; set; }

        public string Size { get; set; }

        public bool IsCouponApplied { get; set; }

        public float SupplyPrice { get; set; }

        public float Weight { get; set; }

        public int? PhotoID { get; set; }

        public IList<EvaluatedCustomOptions> SelectedOptions { get; set; }

        public static explicit operator OrderItem(ShoppingCartItem item)
        {
            return new OrderItem
                       {
                           ProductID = item.Offer.ProductId,
                           Name = item.Offer.Product.Name,
                           ArtNo = item.Offer.ArtNo,
                           Price = item.Price,
                           Amount = item.Amount,
                           SupplyPrice = item.Offer.SupplyPrice,
                           SelectedOptions = CustomOptionsService.DeserializeFromXml(item.AttributesXml),
                           Weight = item.Offer.Product.Weight,
                           IsCouponApplied = item.IsCouponApplied,
                           Color = item.Offer.ColorID != null ? ColorService.GetColor(item.Offer.ColorID).ColorName : null,
                           Size = item.Offer.SizeID != null ? SizeService.GetSize(item.Offer.SizeID).SizeName : null,
                           PhotoID = item.Offer.Photo != null ? item.Offer.Photo.PhotoId : (int?)null
                       };
        }

        public static bool operator ==(OrderItem first, OrderItem second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (((object)first == null) || ((object)second == null))
            {
                return false;
            }

            return first.ProductID == second.ProductID && first.ArtNo== second.ArtNo && first.Color == second.Color && first.Size == second.Color && first.SelectedOptions.SequenceEqual(second.SelectedOptions);
        }

        public static bool operator !=(OrderItem first, OrderItem second)
        {
            return !(first == second);
        }

        public bool Equals(OrderItem other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.ArtNo == ArtNo &&
                other.Name == Name &&
                other.ProductID == ProductID &&
                other.Amount == Amount &&
                other.Price == Price &&
                other.SupplyPrice == SupplyPrice &&
                other.Color == Color &&
                other.Size == Size &&
                Equals(other.SelectedOptions, SelectedOptions) &&
                other.OrderItemID == OrderItemID)
            {
                return true;
            }

            //WARNING !!!!!! Equals() is same shit as == operator !!!!!!!!!!!
            return other == this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == typeof(OrderItem) && Equals((OrderItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ProductID ?? 1 * 397 ^ Amount.GetHashCode() * 397 ^ (SelectedOptions != null ? SelectedOptions.AggregateHash() : 1);
            }
        }
    }
}