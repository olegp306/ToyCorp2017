//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Customers
{

    [Serializable]
    public class CustomerTempShoppingCartItem
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string SelectedOptions { get; set; }

        public CustomerTempShoppingCartItem(int prodId, int qty, string selOpt)
        {
            ProductId = prodId;
            Quantity = qty;
            SelectedOptions = selOpt;
        }
    }
}
