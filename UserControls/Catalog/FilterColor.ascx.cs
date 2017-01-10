//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop.Catalog;
using AdvantShop.Configuration;

namespace UserControls.Catalog
{
    public partial class FilterColor : UserControl
    {
        public List<int> AvalibleColorsIDs { set; get; }
        public List<int> SelectedColorsIDs { set; get; }
        public int CategoryId { get; set; }
        public bool InDepth { get; set; }

        protected int ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
        protected int ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            List<Color> colors = ColorService.GetColorsByCategoryID(CategoryId, InDepth).Where(c => AvalibleColorsIDs == null || AvalibleColorsIDs.Any(c2 => c2 == c.ColorId)).ToList();

            if (colors.Any())
            {
                lvColors.DataSource = colors;
                lvColors.DataBind();
            }
            else
            {
                this.Visible = false;
            }
        }
    }
}
