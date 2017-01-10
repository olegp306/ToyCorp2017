//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
namespace AdvantShop.Modules.Interfaces
{
    public interface IOrderConfirmation : IModule
    {
        string PageName { get; }
        string FileUserControlOrderConfirmation { get; }
    }
}