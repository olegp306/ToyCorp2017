//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Modules.Interfaces
{
    public interface IRenderIntoHtml : IModule
    {
        string DoRenderIntoHead();

        string DoRenderAfterBodyStart();

        string DoRenderBeforeBodyEnd();
    }
}
