//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Modules.Interfaces
{
    public interface IClientPageModule : IModule
    {
        string ClientPageControlFileName { get; }
        string UrlPath { get; }
        string PageTitle { get; }
        string MetaKeyWords { get; }
        string MetaDescription { get; }
    }
}
