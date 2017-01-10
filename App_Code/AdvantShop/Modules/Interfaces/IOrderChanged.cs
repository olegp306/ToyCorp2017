//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using AdvantShop.Orders;

namespace AdvantShop.Modules.Interfaces
{
    public interface IOrderChanged : IModule
    {
        void DoOrderAdded(IOrder order);

        void DoOrderChangeStatus(IOrder order);
        
        /// <summary>
        /// Внимание! Данный метод может быть вызван несколько раз при одном изменении заказа
        /// </summary>
        void DoOrderUpdated(IOrder order);

        void DoOrderDeleted(int orderId);

    }
}