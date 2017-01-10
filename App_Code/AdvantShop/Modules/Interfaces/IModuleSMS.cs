//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Modules.Interfaces
{
    public interface IModuleSms : IModule
    {
        //bool IsActive { get; }

        void SendSms(string phoneNumber, string text);
    }
}