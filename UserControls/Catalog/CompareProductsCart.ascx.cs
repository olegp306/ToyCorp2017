//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web.UI;
using AdvantShop.Core.UrlRewriter;

namespace UserControls.Catalog
{
    public partial class CompareProductsCart : UserControl
    {
        protected string GetAbsoluteLink(string link)
        {
            return UrlService.GetAbsoluteLink(link);
        }
    }
}