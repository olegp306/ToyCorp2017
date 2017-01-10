using System;
using System.Web.UI;
using AdvantShop.Catalog;

namespace UserControls.Details
{
    public partial class ProductVideoView : UserControl
    {
        public int ProductID { set; get; }
        public bool hasVideos { private set; get; }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            hasVideos = ProductVideoService.HasVideo(ProductID);
        }
    }
}