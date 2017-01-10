//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Repository.Currencies;
using Quartz;

namespace AdvantShop.Core.Scheduler
{
    [DisallowConcurrentExecution]
    public class UpdateCurrencyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (Configuration.SettingsMain.EnableAutoUpdateCurrencies)
            {
                CurrencyService.UpdateCurrenciesFromCentralBank();
            }
        }
    }
}