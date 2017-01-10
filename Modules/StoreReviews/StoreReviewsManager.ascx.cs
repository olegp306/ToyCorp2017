using System;
using System.Web.UI.WebControls;
using AdvantShop.Modules;

namespace Advantshop.UserControls.Modules.StoreReviews
{
    public partial class Admin_StoreReviewsManager : System.Web.UI.UserControl
    {
        private const string ModuleName = "StoreReviews";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            lvReviews.DataSource = StoreReviewRepository.GetStoreReviews();
            lvReviews.DataBind();
        }

        protected void lvReviewsItemCommand(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "deleteReview":
                    int reviewId;
                    if (Int32.TryParse(e.CommandArgument.ToString(), out reviewId))
                    {
                        StoreReviewRepository.DeleteStoreReviewsById(reviewId);
                    }
                    break;
            }
        }
    }
}