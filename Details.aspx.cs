//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using AdvantShop.Security;
using AdvantShop.SEO;
using Resources;
using AdvantShop.Payment;
using System.Web.UI.WebControls;

namespace ClientPages
{
    public partial class Details : AdvantShopClientPage
    {
        #region Fields

        protected Product CurrentProduct;
        protected Offer CurrentOffer;
        protected Brand CurrentBrand;
        protected MetaInfo metaInfo;

        private int _productId;
        protected int ProductId
        {
            get { return _productId != 0 ? _productId : (_productId = Request["productid"].TryParseInt()); }
        }

        private string displaySku = string.Empty;
        public string DisplaySku
        {
            get {
                if (displaySku.Length > 0) return displaySku;
                var prop = PropertyService.GetPropertyValuesByProductId(ProductId).FirstOrDefault(x => x.Property.Name == "Артикул");
                if (prop != null ) {
                    displaySku = prop.Value;
                }
                if (displaySku.Length == 0) {
                    displaySku = CurrentProduct.ArtNo;
                }
                return displaySku; 
            
            }
        }

        #endregion

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

            btnAdd.Text = SettingsCatalog.BuyButtonText;
            btnOrderByRequest.Text = SettingsCatalog.PreOrderButtonText;

            if (CurrentProduct.TotalAmount <= 0 || CurrentProduct.MainPrice == 0)
            {
                divAmount.Visible = false;
            }


            //CompareControl.ProductId = ProductId;

            CurrentOffer = OfferService.GetMainOffer(CurrentProduct.Offers, CurrentProduct.AllowPreOrder, Request["color"].TryParseInt(true), Request["size"].TryParseInt(true));

            if (CurrentOffer != null)
            {
                BuyInOneClick.OfferID = CurrentOffer.OfferId;

                sizeColorPicker.SelectedOfferId = CurrentOffer.OfferId;

                //CompareControl.Visible = divCompare.Visible = SettingsCatalog.EnableCompareProducts;
                //CompareControl.OfferId = CurrentOffer.OfferId;
                //CompareControl.IsSelected =
                //    ShoppingCartService.CurrentCompare.Any(p => p.Offer.OfferId == CurrentOffer.OfferId);

                //WishlistControl.OfferId = CurrentOffer.OfferId;
                //divWishlist.Visible = SettingsDesign.WishListVisibility;
            }
            else
            {
                //CompareControl.Visible = divCompare.Visible = false;
                //divWishlist.Visible = false;
                pnlPrice.Visible = false;
            }

            BuyInOneClick.ProductId = CurrentProduct.ProductId;
            BuyInOneClick.SelectedOptions = productCustomOptions.SelectedOptions;
            BuyInOneClick.CustomOptions = productCustomOptions.CustomOptions;

            sizeColorPicker.ProductId = ProductId;

            divUnit.Visible = (CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled) && SettingsMain.EnableInplace ? true : CurrentProduct.Unit.IsNotEmpty();

            rating.ProductId = CurrentProduct.ID;
            rating.Rating = CurrentProduct.Ratio;
            rating.ShowRating = SettingsCatalog.EnableProductRating;
            rating.ReadOnly = RatingService.DoesUserVote(ProductId, CustomerContext.CustomerId);

            pnlSize.Visible = !string.IsNullOrEmpty(CurrentProduct.Size) && (CurrentProduct.Size != "0|0|0") && SettingsCatalog.DisplayDimensions;
            pnlWeight.Visible = CurrentProduct.Weight != 0 && SettingsCatalog.DisplayWeight;

            CurrentBrand = CurrentProduct.Brand;
            pnlBrand.Visible = CurrentBrand != null && CurrentBrand.Enabled;
            //pnlBrnadLogo.Visible = CurrentBrand != null && CurrentBrand.Enabled && CurrentBrand.BrandLogo != null;

            productPropertiesView.ProductId = ProductId;
            productBriefPropertiesView.ProductId = ProductId;

            productPhotoView.Product = CurrentProduct;

            ProductVideoView.ProductID = ProductId;
            relatedProducts.ProductIds.Add(ProductId);
            alternativeProducts.ProductIds.Add(ProductId);
            breadCrumbs.Items =
                CategoryService.GetParentCategories(CurrentProduct.CategoryId).Reverse().Select(cat => new BreadCrumbs
                {
                    Name = cat.Name,
                    Url = UrlService.GetLink(ParamType.Category, cat.UrlPath, cat.ID)
                }).ToList();
            breadCrumbs.Items.Insert(0, new BreadCrumbs
            {
                Name = Resource.Client_MasterPage_MainPage,
                Url = UrlService.GetAbsoluteLink("/")
            });

            breadCrumbs.Items.Add(new BreadCrumbs { Name = CurrentProduct.Name, Url = null });

            RecentlyViewService.SetRecentlyView(CustomerContext.CustomerId, ProductId);

            productReviews.EntityType = EntityType.Product;
            productReviews.EntityId = ProductId;

            int reviewsCount = SettingsCatalog.ModerateReviews
                                   ? ReviewService.GetCheckedReviewsCount(ProductId, EntityType.Product)
                                   : ReviewService.GetReviewsCount(ProductId, EntityType.Product);
            if (reviewsCount > 0)
            {
                lReviewsCount.Text = string.Format("({0})", reviewsCount);
            }

            //Добавим новые meta
            MetaInfo newMetaInfo = new MetaInfo();

            newMetaInfo = CurrentProduct.Meta;
            newMetaInfo.Title = CurrentProduct.Name + " купить в интернет-магазине Корпорация Игрушек";
            newMetaInfo.MetaDescription = "Интернет-магазин Корпорация Игрушек представляет: " + CurrentProduct.Name + " и еще более 5,5 тысяч видов товаров по оптовым ценам. Порадуйте своего ребенка!";
            newMetaInfo.MetaKeywords = CurrentProduct.Name;

            metaInfo = SetMeta(newMetaInfo, CurrentProduct.Name);

            //metaInfo = SetMeta(CurrentProduct.Meta, CurrentProduct.Name,
            //    CategoryService.GetCategory(CurrentProduct.CategoryId).Name,
            //    CurrentProduct.Brand != null ? CurrentProduct.Brand.Name : string.Empty,
            //    CatalogService.GetStringPrice(CurrentProduct.MainPrice -
            //                                  CurrentProduct.MainPrice * CurrentProduct.Discount / 100));

            if (SettingsSEO.ProductAdditionalDescription.IsNotEmpty())
            {
                liAdditionalDescription.Text =
                    GlobalStringVariableService.TranslateExpression(
                        SettingsSEO.ProductAdditionalDescription, MetaType.Product, CurrentProduct.Name,
                        CategoryService.GetCategory(CurrentProduct.CategoryId).Name,
                        CurrentProduct.Brand != null ? CurrentProduct.Brand.Name : string.Empty,
                        CatalogService.GetStringPrice(CurrentProduct.MainPrice -
                                                      CurrentProduct.MainPrice * CurrentProduct.Discount / 100));
            }

            LoadModules();

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.product;
                tagManager.ProdId = CurrentOffer != null ? CurrentOffer.ArtNo : CurrentProduct.ArtNo;
                tagManager.ProdName = CurrentProduct.Name;
                tagManager.ProdValue = CurrentOffer != null ? CurrentOffer.Price : 0;
                tagManager.CatCurrentId = CurrentProduct.MainCategory.ID;
                tagManager.CatCurrentName = CurrentProduct.MainCategory.Name;
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (CustomerContext.CurrentCustomer.CustomerRole == Role.Administrator ||
                (CustomerContext.CurrentCustomer.CustomerRole == Role.Moderator &&
                 RoleAccess.Check(CustomerContext.CurrentCustomer, "product.aspx")))
            {
                hrefAdmin.Visible = true;
            }

            GetOffer();
        }

        protected string RenderSpinBox()
        {
            return
                string.Format(
                    "<input class=\"spinbox\" data-plugin=\"spinbox\" type=\"text\" id=\"txtAmount\" value=\"{0}\" data-spinbox-options=\"{{min:{0},max:{1},step:{2}}}\"/>",
                    CurrentProduct.MinAmount != null ? CurrentProduct.MinAmount.ToString().Replace(",", ".") : "1",
                    CurrentProduct.MaxAmount != null
                        ? CurrentProduct.MaxAmount.ToString().Replace(",", ".")
                        : Int16.MaxValue.ToString(),
                    CurrentProduct.Multiplicity.ToString().Replace(",", "."));
        }

        private void GetOffer()
        {
            if (CurrentOffer != null)
            {
                bool isMultiOffers = CurrentProduct.Offers.Count > 1;
                bool isAvailable = CurrentOffer.Amount > 0;
                bool isUnavalable = CurrentOffer.Amount <= 0;

                lAvailiableAmount.Text = string.Format(
                    "<div id='availability' class='{0}' {3}>{1}{2}</div>",
                    isAvailable ? "available" : "not-available",
                    isAvailable ? Resource.Client_Details_Available : Resource.Client_Details_NotAvailable,
                    isAvailable && SettingsCatalog.ShowStockAvailability
                    ? string.Format(" ({0}{1}<span {3}>{2}</span>)",
                    CurrentOffer.Amount, CurrentProduct.Unit.IsNotEmpty() ? " " : string.Empty,
                    CurrentProduct.Unit.IsNotEmpty() ? CurrentProduct.Unit : string.Empty,
                    (CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled) ? "data-inplace-update=\"amount\"" : "")
                    : string.Empty,
                    InplaceEditor.Offer.AttibuteAmount(CurrentOffer.OfferId, CurrentOffer.Amount));


                btnOrderByRequest.Attributes["data-offerid"] = CurrentOffer.OfferId.ToString();
                btnOrderByRequest.Attributes["data-productid"] = CurrentProduct.ProductId.ToString();

                btnAdd.Attributes["data-cart-add-productid"] = ProductId.ToString();
                btnAdd.Attributes["data-cart-add-offerid"] = CurrentOffer.OfferId.ToString();
                btnAdd.Attributes["data-offerid"] = CurrentOffer.OfferId.ToString();

                ButtonSetVisible(btnOrderByRequest, (isUnavalable || CurrentOffer.Price == 0) && CurrentProduct.AllowPreOrder, isMultiOffers);
                ButtonSetVisible(btnAdd, CurrentOffer.Price > 0 && isAvailable, isMultiOffers);

                BuyInOneClick.Visible = SettingsOrderConfirmation.BuyInOneClick && CurrentOffer.Price > 0 && isAvailable;

                ShowCreditButtons(isMultiOffers);

                LoadBonusCard();
                LoadShippings();
            }
            else
            {
                lAvailiableAmount.Text = Resource.Client_Details_NotAvailable;
            }
        }

        private void LoadBonusCard()
        {
            if (!BonusSystem.IsActive || CurrentOffer.Price <= 0)
                return;

            var bonusCard = BonusSystemService.GetCard(CustomerContext.CurrentCustomer.BonusCardNumber);
            if (bonusCard != null)
            {
                lblProductBonus.Text =
                    CatalogService.RenderBonusPrice(bonusCard.BonusPercent, CurrentOffer.Price,
                        CurrentProduct.CalculableDiscount, CustomerContext.CurrentCustomer.CustomerGroup, CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions, productCustomOptions.SelectedOptions));
            }
            else if (BonusSystem.BonusFirstPercent != 0)
            {
                lblProductBonus.Text =
                    CatalogService.RenderBonusPrice(BonusSystem.BonusFirstPercent, CurrentOffer.Price,
                        CurrentProduct.CalculableDiscount, CustomerContext.CurrentCustomer.CustomerGroup, CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions, productCustomOptions.SelectedOptions));
            }
        }

        private void LoadShippings()
        {
            if (SettingsDesign.ShowShippingsMethodsInDetails != SettingsDesign.eShowShippingsInDetails.Never)
            {
                liShipping.Text = string.Format("<div class=\"js-details-delivery\" data-value=\"{0}\"></div>",
                    (int) SettingsDesign.ShowShippingsMethodsInDetails);
            }
            else
            {
                dShipping.Visible = false;
            }
        }

        #region Modules

        private void LoadModules()
        {
            LoadTabModules();
            LoadProductInformationModules();

            //ltrlRightColumnModules.Text = ModulesRenderer.RenderDetailsModulesToRightColumn();
        }

        private void LoadTabModules()
        {
            var listDetailsTabs = new List<ITab>();

            foreach (var detailsTabsModule in AttachedModules.GetModules<IProductTabs>())
            {
                var classInstance = (IProductTabs)Activator.CreateInstance(detailsTabsModule, null);
                listDetailsTabs.AddRange(classInstance.GetProductDetailsTabsCollection(CurrentProduct.ProductId));
            }

            lvTabsBodies.DataSource = listDetailsTabs;
            lvTabsTitles.DataSource = listDetailsTabs;

            lvTabsBodies.DataBind();
            lvTabsTitles.DataBind();
        }

        private void LoadProductInformationModules()
        {
            foreach (var module in AttachedModules.GetModules<IModuleDetails>())
            {
                var classInstance = (IModuleDetails)Activator.CreateInstance(module, null);
                liProductInformation.Text += classInstance.RenderToProductInformation(CurrentProduct.ProductId);
            }
        }

        #endregion

        #region "Show/hide buttons"

        private void ShowCreditButtons(bool isMultiOffers)
        {
            var creditPayment = PaymentService.GetCreditPaymentMethods().FirstOrDefault();
            if (creditPayment != null && CurrentOffer != null)
            {
                btnAddCredit.Attributes["data-cart-add-productid"] = ProductId.ToString();
                btnAddCredit.Attributes["data-cart-add-offerid"] = CurrentOffer.OfferId.ToString();
                btnAddCredit.Attributes["data-cart-payment"] = creditPayment.PaymentMethodId.ToString();
                btnAddCredit.Attributes["data-cart-minprice"] = creditPayment.MinimumPrice.ToString();

                var productPrice = CatalogService.CalculateProductPrice(CurrentOffer.Price,
                            CurrentProduct.CalculableDiscount,
                            CustomerContext.CurrentCustomer.CustomerGroup,
                            CustomOptionsService.DeserializeFromXml(
                                CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions, productCustomOptions.SelectedOptions)));

                var isVisible = productPrice > creditPayment.MinimumPrice && CurrentOffer.Amount > 0;

                ButtonSetVisible(btnAddCredit, isVisible, isMultiOffers);
                ButtonSetVisible(lblFirstPaymentNote, isVisible && creditPayment.MinimumPrice > 0, isMultiOffers);
                ButtonSetVisible(lblFirstPayment, isVisible && creditPayment.MinimumPrice > 0, isMultiOffers);

                hfFirstPaymentPercent.Value = creditPayment.FirstPayment.ToString();

                lblFirstPayment.Text = creditPayment.FirstPayment > 0
                    ? CatalogService.GetStringPrice(productPrice * creditPayment.FirstPayment / 100) + @"*"
                    : string.Format("<div class=\"price\">{0}*</div>", Resource.Client_Details_WithoutFirstPayment);
            }
            else
            {
                ButtonSetVisible(btnAddCredit, false, false);
                ButtonSetVisible(lblFirstPaymentNote, false, false);
                ButtonSetVisible(lblFirstPayment, false, false);
            }
        }

        private void ButtonHide(WebControl btn, bool isMultiOffers)
        {
            if (isMultiOffers == true)
            {
                btn.Attributes["style"] = "display:none;";
            }
            else
            {
                btn.Visible = false;
            }
        }

        private void ButtonShow(WebControl btn, bool isMultiOffers)
        {
            if (isMultiOffers == true)
            {
                btn.Attributes["style"] = "display:inline-block;";
            }
            else
            {
                btn.Visible = true;
            }
        }

        private void ButtonSetVisible(WebControl btn, bool isVisible, bool isMultiOffers)
        {
            if (isVisible == true)
            {
                ButtonShow(btn, isMultiOffers);
            }
            else
            {
                ButtonHide(btn, isMultiOffers);
            }
        }

        #endregion

        protected int GetCustomBrandID()
        {
            if (CurrentProduct == null) return 0;

            var customBrand = CurrentProduct.ProductPropertyValues.SingleOrDefault(p => p.Property.Name == "Бренд");
            if (customBrand == null) return 0;

            return customBrand.PropertyValueId;
        }
    }
}