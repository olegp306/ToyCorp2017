//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Modules.Interfaces
{
    public interface IRenderIntoShoppingCart : IModule
    {
        string DoRenderToTop();

        string DoRenderToBottom();

        string ClientSideControlNameTop { get; }

        string ClientSideControlNameBottom { get; }

        bool ShowConfirmButtons { get; }
    }
}
