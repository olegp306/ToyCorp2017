//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Modules.Interfaces
{
    public interface IUserControlInSc
    {
        List<int> ProductIds { get; set; }

        bool HasProducts { get; set; }
    }
}
