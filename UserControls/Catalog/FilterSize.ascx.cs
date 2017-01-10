//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop.Catalog;

namespace UserControls.Catalog
{
    public partial class FilterSize : UserControl
    {
        public List<int> AvalibleSizesIDs { set; get; }
        public List<int> SelectedSizesIDs { set; get; }
        public int CategoryId { get; set; }
        public bool InDepth { get; set; }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            List<Size> sizes = SizeService.GetSizesByCategoryID(CategoryId, InDepth).Where(s => AvalibleSizesIDs == null || AvalibleSizesIDs.Any(s2 => s2 == s.SizeId)).ToList();

            if (sizes.Any())
            {
                lvSizes.DataSource = sizes;
                lvSizes.DataBind();
            }
            else
            {
                this.Visible = false;
            }
        }
    }
}
