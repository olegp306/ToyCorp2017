//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Security;
using AdvantShop.Statistic;
using Quartz;

namespace AdvantShop.Core.Scheduler
{
    [DisallowConcurrentExecution]
    public class ClearExpiredJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ShoppingCartService.DeleteExpiredShoppingCartItems(DateTime.Today.AddMonths(-3));
            OrderConfirmationService.DeleteExpired(DateTime.Today.AddMonths(-3));
            RecentlyViewService.DeleteExpired();
            InternalServices.DeleteExpiredAppRestartLogData();
            Secure.DeleteExpiredAuthorizeLog();
            //JsCssTool.Clear();
        }
    }
}