
using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.Modules.Interfaces
{
    public interface ILabel : IModule
    {
        ProductLabel GetLabel();
    }
}