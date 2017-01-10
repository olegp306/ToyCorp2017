using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UserControls.Details
{
    public partial class Wishlist : System.Web.UI.UserControl
    {

        public int OfferId { get; set; }
        protected bool ExistInWishlist = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = SettingsDesign.WishListVisibility && OfferId != 0;
            ExistInWishlist = ShoppingCartService.CurrentWishlist.Any(item => item.OfferId == OfferId);
        }
    }
}
