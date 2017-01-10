//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web.UI;
using AdvantShop.CMS;
using AdvantShop.Configuration;

namespace UserControls.Details
{
    public partial class ProductReviews : UserControl
    {
        public int EntityId { get; set; }
        public EntityType EntityType { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lvReviews.DataSource = ReviewService.GetReviews(EntityId, EntityType).Where(review => review.Checked || !SettingsCatalog.ModerateReviews);
            lvReviews.DataBind();
        }
    }
}