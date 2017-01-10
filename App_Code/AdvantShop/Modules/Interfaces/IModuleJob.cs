
using System.Collections.Generic;
using AdvantShop.Core.Scheduler;

namespace AdvantShop.Modules.Interfaces
{
    public interface IModuleTask : IModule
    {
        List<TaskSetting> GetTasks();
    }
}