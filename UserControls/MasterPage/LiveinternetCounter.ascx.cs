//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.CMS;
using AdvantShop.Trial;

namespace UserControls.MasterPage
{
    public partial class LiveinternetCounter : System.Web.UI.UserControl
    {
        public string RenderLiveCounter()
        {
            if (Visible)
            {
                var block = StaticBlockService.GetPagePartByKey("LiveCounter");
                if (block != null && block.Enabled)
                    return block.Content;
            }
            return string.Empty;
        }
    }
}

