//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Modules.Interfaces
{
    public interface IProductTabs : IModule
    {
        List<ITab> GetProductTabsCollection(int productId);

        List<ITab> GetProductDetailsTabsCollection(int productId);

        void SaveProductDetailsTab(int productId, int tabTitleId, string tabBody);

        void DeleteProductDetailsTab(int productId, int tabTitleId);
    }
}
