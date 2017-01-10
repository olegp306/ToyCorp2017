//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Modules.Interfaces
{
    public interface IModule
    {
        string ModuleStringId { get; }

        string ModuleName { get; }
        
        List<IModuleControl> ModuleControls { get; }

        bool HasSettings { get; }

        bool CheckAlive();

        bool InstallModule();

        bool UpdateModule();

        bool UninstallModule();
    }

    public interface IModuleControl
    {
        string NameTab { get; }
        string File { get; }
    }
}
