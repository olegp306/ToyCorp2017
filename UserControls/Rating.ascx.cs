//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.UI;

namespace UserControls
{
    public partial class RatingControl : UserControl
    {
        public int ProductId { get; set; }
        public bool ShowRating { get; set; }
        public double Rating { get; set; }
        public bool ReadOnly { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Visible = ShowRating;
        }
    }
}