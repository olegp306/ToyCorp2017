//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Orders;

namespace AdvantShop.Modules.Interfaces
{
    public interface IOrderRenderIntoHtml : IModule
    {
        string DoRenderIntoFinalStep();

        string DoRenderIntoFinalStep(IOrder order);
    }
}
