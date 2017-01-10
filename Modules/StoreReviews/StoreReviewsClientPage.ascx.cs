using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;
using System.Web;
using AdvantShop.Modules;

namespace Advantshop.Modules.UserControls.StoreReviews
{
    public partial class StoreReviewsClientPage : System.Web.UI.UserControl
    {
        protected const string _moduleStringId = "StoreReviews";

        protected bool ShowRatio;

        private string childTemplate = "<div class=\"shop-reviews-child-wrap\" data-sr-item=\"{0}\" data-sr-parentid=\"{1}\">" +
            "<div class=\"shop-reviews-row\"><span class=\"shop-reviews-name\">{2}</span> <span class=\"shop-reviews-date\">{3}</span></div>" +
            "<div class=\"shop-reviews-row shop-reviews-text\">{4}</div>" +
            "<div data-sr-form-btn=\"true\">{5} {6}</div>" +
            "{7}" +
            "</div>";

        private int _currentPage = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowRatio = ModuleSettingsProvider.GetSettingValue<bool>("ShowRatio", _moduleStringId);

            dp.PageSize = ModuleSettingsProvider.GetSettingValue<int>("PageSize", _moduleStringId);

            Bind();

            ltHeader.Text = String.Format("{0} ({1})", ModuleSettingsProvider.GetSettingValue<string>("PageTitle", _moduleStringId),
                          dp.TotalRowCount);
        }

        protected void btnClick(object sender, EventArgs e)
        {
            const string tpl = "<div class=\"error-item\">{0}</div>";
            var errList = new StringBuilder();

            int scope;
            var resultParse = int.TryParse(hfScope.Value, out scope);

            if (!resultParse)
            {
                errList.AppendFormat(tpl, GetLocalResourceObject("StoreReviews_InvalidScope"));
            }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                errList.AppendFormat(tpl, GetLocalResourceObject("StoreReviews_InvalidEmail"));
            }

            if (string.IsNullOrEmpty(txtReviewerName.Text.Trim()))
            {
                errList.AppendFormat(tpl, GetLocalResourceObject("StoreReviews_InvalidName"));
            }

            if (string.IsNullOrEmpty(txtReview.Text.Trim()))
            {
                errList.AppendFormat(tpl, GetLocalResourceObject("StoreReviews_InvalidReview"));
            }

            if (errList.Length > 0)
            {
                liError.InnerHtml = errList.ToString();
            }
            else
            {

                StoreReviewRepository.AddStoreReview(new StoreReview
                    {
                        Moderated = false,
                        Rate = ShowRatio ? scope : 0,
                        ParentId = 0,
                        ReviewerEmail = HttpUtility.HtmlEncode(txtEmail.Text),
                        ReviewerName = HttpUtility.HtmlEncode(txtReviewerName.Text),
                        Review = HttpUtility.HtmlEncode(txtReview.Text)
                    });

                txtReviewerName.Text = "";
                txtEmail.Text = "";
                txtReview.Text = "";
            }

            Response.Redirect(Request.RawUrl);
        }

        protected string RenderChild(List<StoreReview> childs)
        {
            var html = new StringBuilder();

            for (var i = 0; i < childs.Count; i++)
            {
                html.AppendFormat(childTemplate,
                                  childs[i].Id,
                                  childs[i].ParentId,
                                  childs[i].ReviewerName,
                                  childs[i].DateAdded.ToString("dd MMMM yyyy, HH:mm"),
                                  childs[i].Review,
                                  "<a href=\"javascript:void(0);\" data-sr-reply=\"true\">" +
                                  GetLocalResourceObject("StoreReviews_Answer") + "</a>",
                                  "",
                                  childs[i].HasChild ? RenderChild(childs[i].ChildrenReviews) : "");
            }

            return html.ToString();
        }

        protected void lvStoreReviews_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            dp.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            _currentPage = (e.StartRowIndex / e.MaximumRows) + 1;

            Bind();
        }


        protected void lvStoreReviews_PreRender(object sender, EventArgs e)
        {
            lvStoreReviews.DataBind();
            DataPagerControlsVisible();
        }

        private void Bind()
        {

            lvStoreReviews.DataSource = StoreReviewRepository.GetStoreReviewsByParentId(0,
                                                         ModuleSettingsProvider
                                                             .GetSettingValue<bool>(
                                                                 "ActiveModerateStoreReviews",
                                                                 _moduleStringId));
            lvStoreReviews.DataBind();
        }

        private void DataPagerControlsVisible()
        {
            dp.Visible = dp.TotalRowCount > dp.PageSize;

            var keyPrevious = dp.Controls[0].Controls[0];
            var previousLink = (LinkButton)dp.Controls[1].Controls[0];
            var nextLink = (LinkButton)dp.Controls[3].Controls[0];
            var keyNext = dp.Controls[4].Controls[0];

            previousLink.Attributes["data-sr-paging-prev"] = "true";
            nextLink.Attributes["data-sr-paging-next"] = "true";

            previousLink.Visible = keyPrevious.Visible = _currentPage > 1;
            nextLink.Visible = keyNext.Visible = (_currentPage * dp.PageSize) < dp.TotalRowCount;
        }
    }
}