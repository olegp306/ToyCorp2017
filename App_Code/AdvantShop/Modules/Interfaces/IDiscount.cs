

using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.Modules.Interfaces
{
    public interface IDiscount : IModule
    {
        float GetDiscount(int productId);

        List<ProductDiscount> GetProductDiscountsList();
    }
}