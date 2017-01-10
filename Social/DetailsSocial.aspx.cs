//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Resources;

public partial class DetailsSocial : AdvantShopClientPage
{
    protected Product CurrentProduct;
	protected Offer CurrentOffer;
	
    protected int ProductId
    {
        get { return Request["productid"].TryParseInt(); }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ProductId == 0)
        {
            Error404();
            return;
        }

        //if not have category
        if (ProductService.GetCountOfCategoriesByProductId(ProductId) == 0)
        {
            Error404();
            return;
        }


        // --- Check product exist ------------------------
        CurrentProduct = ProductService.GetProduct(ProductId);
        if (CurrentProduct == null || CurrentProduct.Enabled == false || CurrentProduct.CategoryEnabled == false)
        {
            Error404();
            return;
        }

        //var firstOrffer = CurrentProduct.Offers.FirstOrDefault();
        //if (firstOrffer != null && (firstOrffer.Amount <= 0 || firstOrffer.Price == 0))
        {
            divAmount.Visible = false;
        }

		CurrentOffer = CurrentProduct.Offers.FirstOrDefault(o => o.Main) ?? CurrentProduct.Offers.FirstOrDefault();
		
		if(CurrentOffer != null){
			//
		}else{
                pnlPrice.Visible = false;
		}
		
		
        rating.ProductId = CurrentProduct.ID;
        rating.Rating = CurrentProduct.Ratio;
        rating.ShowRating = SettingsCatalog.EnableProductRating;
        rating.ReadOnly = RatingService.DoesUserVote(ProductId, CustomerContext.CustomerId);


        sizeColorPicker.ProductId = ProductId;

        pnlSize.Visible = !string.IsNullOrEmpty(CurrentProduct.Size) && (CurrentProduct.Size != "0|0|0");
        pnlWeight.Visible = CurrentProduct.Weight != 0;
        pnlBrand.Visible = CurrentProduct.Brand != null;

        productPropertiesView.ProductId = ProductId;
        productPhotoView.Product = CurrentProduct;
        ProductVideoView.ProductID = ProductId;
        relatedProducts.ProductIds.Add(ProductId);
        alternativeProducts.ProductIds.Add(ProductId);
        breadCrumbs.Items = CategoryService.GetParentCategories(CurrentProduct.CategoryId).Reverse().Select(cat => new BreadCrumbs
                                                                                                 {
                                                                                                     Name = cat.Name,
                                                                                                     Url = "social/catalogsocial.aspx?categoryid=" + cat.CategoryId
                                                                                                 }).ToList();
        breadCrumbs.Items.Insert(0, new BreadCrumbs
                                        {
                                            Name = Resource.Client_MasterPage_MainPage,
                                            Url = UrlService.GetAbsoluteLink("social/catalogsocial.aspx")
                                        });

        breadCrumbs.Items.Add(new BreadCrumbs { Name = CurrentProduct.Name, Url = null });

        RecentlyViewService.SetRecentlyView(CustomerContext.CustomerId, ProductId);

        SetMeta(CurrentProduct.Meta, CurrentProduct.Name);

        productReviews.EntityType = EntityType.Product;
        productReviews.EntityId = ProductId;

        int reviewsCount = SettingsCatalog.ModerateReviews ? ReviewService.GetCheckedReviewsCount(ProductId, EntityType.Product) : ReviewService.GetReviewsCount(ProductId, EntityType.Product);
        if (reviewsCount > 0)
        {
            lReviewsCount.Text = string.Format("({0})", reviewsCount);
        }
		
        GetOffer();

    }

    private void GetOffer()
    {
	
	
		if(CurrentOffer != null){

			var isAvalable = CurrentOffer.Amount > 0;

			lAvailiableAmount.Text = string.Format("<span id=\"status-not-available\" class=\"not-available\" style=\"display:{1};\">{0}</span>", Resource.Client_Details_NotAvailable, isAvalable ? "none" : "inline-block")
								   + string.Format("<span id=\"status-available\" class=\"available\" style=\"display:{1};\">{0}</span>", Resource.Client_Details_Available, isAvalable ? "inline-block" : "none");



            if (CurrentProduct.Offers.Count == 1)
            {
                btnOrderByRequest.Visible = CurrentOffer.Amount <= 0 && CurrentProduct.AllowPreOrder;
                btnAdd.Visible = CurrentProduct.MainPrice > 0 && CurrentProduct.TotalAmount > 0;
                btnAdd.Attributes["data-cart-add-productid"] = ProductId.ToString();
            }
            else
            {
                btnOrderByRequest.Attributes["style"] = (CurrentOffer.Amount <= 0 && CurrentProduct.AllowPreOrder) ? "" : "display:none;";
                btnAdd.Attributes["style"] = (CurrentProduct.MainPrice > 0 && CurrentProduct.TotalAmount > 0) ? "" : "display:none;";
                btnAdd.Attributes["data-cart-add-offerid"] = CurrentOffer.OfferId.ToString();
            }
			
		}else{
            lAvailiableAmount.Text = Resource.Client_Details_NotAvailable;
        }
    }
    
    protected void btnOrderByRequest_Click(object sender, EventArgs e)
    {
        //Response.Redirect("sendrequestonproduct.aspx?offerid=" + C, true);
    }

    protected string RenderSpinBox()
    {
        return
            string.Format(
                "<input data-plugin=\"spinbox\" type=\"text\" id=\"txtAmount\" value=\"{0}\" data-spinbox-options=\"{{min:{0},max:{1},step:{2}}}\"/>",
                CurrentProduct.MinAmount ?? 1,
                CurrentProduct.MaxAmount ?? Int16.MaxValue,
                CurrentProduct.Multiplicity);
    }
}