//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Admin.UserControls.Products;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;
using AdvantShop.Trial;
using Resources;

namespace Admin
{
    public partial class ProductPage : AdvantShopAdminPage
    {
        private bool _valid = true;
        private Product _product;
        private string _productPhoto;

        private int ProductId
        {
            get
            {
                int id;
                int.TryParse(Request["productid"], out id);
                return id;
            }
        }

        private int CategoryID
        {
            get
            {
                var id = CategoryService.DefaultNonCategoryId;
                if (!int.TryParse(Request["categoryid"], out id))
                {
                    id = ProductService.GetFirstCategoryIdByProductId(ProductId);
                }
                return id;
            }
        }

        protected bool AddingNewProduct
        {
            //get { return (ProductId == 0); }
            get { return string.IsNullOrEmpty(Request["productid"]); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            fckBriefDescription.Language = fckDescription.Language = CultureInfo.CurrentCulture.ToString();
            if (AdvantShop.Localization.Culture.Language == AdvantShop.Localization.Culture.SupportLanguage.Russian)
            {
                slimboxStyle.Attributes["href"] = UrlService.GetAdminAbsoluteLink("/css/slimbox2_ru.css");
            }
            if (string.IsNullOrEmpty(Request["categoryid"]) && string.IsNullOrEmpty(Request["productid"]))
            {
                Page.Response.Redirect("Catalog.aspx");
            }


            btnSave.Text = AddingNewProduct ? Resource.Admin_Product_AddProduct : Resource.Admin_Product_Save;
            btnCopyProduct.Visible = !AddingNewProduct;

            relatedProducts.ProductID =
                alternativeProducts.ProductID = productPhotos.ProductID = productVideos.ProductID
                = productCustomOption.ProductId = productProperties.ProductId = rightNavigation.ProductID = ProductId;

            productProperties.CategoryId = CategoryID;
            rightNavigation.CategoryID = CategoryID;

            lRelatedProduct.Text = SettingsCatalog.RelatedProductName;
            lAlternativeProduct.Text = SettingsCatalog.AlternativeProductName;

            if (!IsPostBack)
            {
                if (!AddingNewProduct)
                {
                    _product = ProductService.GetProduct(ProductId);
                    if (_product == null)
                    {
                        Response.Redirect("Catalog.aspx");
                        return;
                    }
                    SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, _product.Name));

                    LoadProduct(_product);
                }
                else
                {
                    _product = new Product
                        {
                            Name = Resource.Admin_Product_AddNewProduct,
                            Offers = new List<Offer> { new Offer() }
                        };
                    txtTitle.Text = string.Empty;
                    txtMetaKeywords.Text = string.Empty;
                    txtMetaDescription.Text = string.Empty;

                    SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_Product_AddNewProduct));

                    LoadProduct(_product);
                }
                txtName.Focus();
            }

            productOffers.ArtNo = txtStockNumber.Text;

            UpdateMainPhoto();
            LoadSiteNavigation();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var brand = popUpBrand.SelectBrandId != 0 ? BrandService.GetBrandById(popUpBrand.SelectBrandId) : null;
            lBrand.Text = brand != null ? brand.Name : Resource.Admin_Product_NotSelected;
            ibRemoveBrand.Visible = popUpBrand.SelectBrandId != 0;

            if (!AddingNewProduct)
            {
                aToClient.HRef = "../" + UrlService.GetLinkDB(ParamType.Product, ProductId);
                aToClient.Visible = true;
            }
            else
            {
                aToClient.Visible = false;
            }

            if (ProductId!= 0)
            {
                var listDetailsTabs = new List<ITab>();

                foreach (var detailsTabsModule in AttachedModules.GetModules<IProductTabs>())
                {
                    var classInstance = (IProductTabs) Activator.CreateInstance(detailsTabsModule, null);
                    if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                    {
                        listDetailsTabs.AddRange(classInstance.GetProductTabsCollection(ProductId));
                    }
                }

                lvTabBodies.DataSource = listDetailsTabs;
                lvTabTitles.DataSource = listDetailsTabs;

                lvTabBodies.DataBind();
                lvTabTitles.DataBind();
            }
        }

        private void LoadSiteNavigation()
        {
            if (CategoryID == CategoryService.DefaultNonCategoryId)
            {
                sn.Visible = false;
                Localize_Admin_Catalog_CategoryLocation.Visible = false;
            }
            else
            {
                sn.Visible = true;
                Localize_Admin_Catalog_CategoryLocation.Visible = true;
                sn.BuildNavigationAdmin(CategoryID);
            }
        }

        private void LoadProduct(Product product)
        {
            SetMeta(GetPageTitle());

            lProductName.Text = product.Name;
            lbIsProductActive.ForeColor = _product.Enabled
                                              ? System.Drawing.Color.FromArgb(2, 125, 194)
                                              : System.Drawing.Color.Red;
            lbIsProductActive.Visible = !AddingNewProduct;
            lbIsProductActive.Text = _product.Enabled
                                         ? Resource.Admin_m_Product_Active
                                         : Resource.Admin_Catalog_ProductDisabled;

            //btnAdd.Visible = !AddingNewProduct;

            if (product.ProductId != 0)
            {
                //btnAdd.OnClientClick = string.Format("window.location=\'Product.aspx?CategoryID={0}\';return false;", CategoryID > 0 ? CategoryID : 0);
                lblProductId.Text = product.ProductId.ToString();
                txtStockNumber.Text = product.ArtNo;
                txtName.Text = product.Name;
                txtSynonym.Text = product.UrlPath;
                chkEnabled.Checked = product.Enabled;
                chkAllowPreOrder.Checked = product.AllowPreOrder;
                txtWeight.Text = product.Weight.ToString();
                txtPopularityManually.Text = product.RecomendedManual.ToString();

                var temp = product.Size.Split('|');
                if (temp.Length == 3)
                {
                    txtSizeLength.Text = temp[0];
                    txtSizeWidth.Text = temp[1];
                    txtSizeHeight.Text = temp[2];
                }
                else txtSizeLength.Text = string.IsNullOrEmpty(product.Size) ? "0" : product.Size;

                popUpBrand.SelectBrandId = product.BrandId;
                lBrand.Text = product.BrandId == 0 ? Resource.Admin_Product_NotSelected : product.Brand.Name;

                chkBestseller.Checked = product.BestSeller;
                chkRecommended.Checked = product.Recomended;
                chkNew.Checked = product.New;
                chkOnSale.Checked = product.OnSale;
                var flagEnabled = ProductService.GetCountOfCategoriesByProductId(product.ProductId) > 0;
                chkBestseller.Enabled = flagEnabled;
                chkRecommended.Enabled = flagEnabled;
                chkNew.Enabled = flagEnabled;
                chkOnSale.Enabled = flagEnabled;
                lblMarkersDisabled.Visible = !flagEnabled;


                txtShippingPrice.Text = product.ShippingPrice.ToString("#0.00") ?? "0";
                txtUnit.Text = product.Unit;

                txtMaxAmount.Text = product.MaxAmount.ToString();
                txtMinAmount.Text = product.MinAmount.ToString();
                txtMultiplicity.Text = product.Multiplicity.ToString();
                txtSalesNote.Text = product.SalesNote;
                txtGtin.Text = product.Gtin;
                txtGoogleProductCategory.Text = product.GoogleProductCategory;
                chbAdult.Checked = product.Adult;
                chbManufacturerWarranty.Checked = product.ManufacturerWarranty;

                txtDiscount.Text = product.Discount.ToString();
                fckDescription.Text = product.Description;
                fckBriefDescription.Text = product.BriefDescription;

                productOffers.Offers = product.Offers;
                productOffers.HasMultiOffer = product.HasMultiOffer;
                productOffers.ProductID = ProductId;
                productOffers.ArtNo = product.ArtNo;

                var meta = MetaInfoService.GetMetaInfo(product.ProductId, MetaType.Product);
                if (meta == null)
                {
                    _product.Meta = new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);
                    chbDefaultMeta.Checked = true;
                }
                else
                {
                    chbDefaultMeta.Checked = false;
                    _product.Meta = meta;
                    txtTitle.Text = _product.Meta.Title;
                    txtMetaKeywords.Text = _product.Meta.MetaKeywords;
                    txtMetaDescription.Text = _product.Meta.MetaDescription;
                    txtH1.Text = _product.Meta.H1;
                }

                LoadCategoryTree();
            }
            else
            {
                productOffers.HasMultiOffer = false;
                productOffers.ProductID = 0;
            }
        }

        protected string GetPageTitle()
        {
            return string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_Product_SubHeader);
        }

        protected void sds_Init(object sender, EventArgs e)
        {
            ((SqlDataSource)sender).ConnectionString = Connection.GetConnectionString();
        }

        private void Msg(string messageText)
        {
            lMessage.Visible = true;
            lMessage.Text = messageText;
        }

        #region Photos
        protected string HtmlProductImage()
        {
            return string.IsNullOrEmpty(_productPhoto)
                       ? "<img style=\'margin-right: 30px; border: solid 1px gray;\' src=\'images/nophoto.gif\' />"
                       : (_productPhoto.Contains("://")
                              ? "<a rel=\"lightbox\" href=\"" + _productPhoto +
                                "\"><img style=\'margin-right: 30px; border: solid 1px gray;\' width=\'120\' src=\'" +
                                _productPhoto + "\' /></a>"
                              : "<a rel=\"lightbox\" href=\"" +
                                FoldersHelper.GetImageProductPath(ProductImageType.Big, _productPhoto, true) +
                                "\"><img style=\'margin-right: 30px; border: solid 1px gray;\' src=\'" +
                                FoldersHelper.GetImageProductPath(ProductImageType.Small, _productPhoto, true) + "\' /></a>");
        }
        protected void productPhotos_OnPhotoMessage(object sender, ProductPhotos.PhotoMessageEventArgs e)
        {
            Msg(e.Message);
        }

        protected void productPhotos_OnMainPhotoUpdate(object sender, EventArgs e)
        {
            UpdateMainPhoto();
        }

        protected void UpdateMainPhoto()
        {
            var product = ProductService.GetProduct(ProductId);
            _productPhoto = product == null ? null : product.Photo;
            ltPhoto.Text = HtmlProductImage();
            upPhoto.Update();
        }
        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            _valid = ValidateInput();
            productOffers.ArtNo = txtStockNumber.Text;
            if (!productOffers.RefreshOffers())
                return;

            string redir = null;
            txtShippingPrice.Text = txtShippingPrice.Text.Replace(" ", string.Empty);

            if (AddingNewProduct)
            {
                var id = CreateProduct();
                SaveProductTabs(id);
                var catId = 0;
                if (id != 0 && int.TryParse(Request["categoryid"], out catId) && catId > 0)
                {
                    if (CategoryService.IsExistCategory(catId))
                    {
                        ProductService.EnableDynamicProductLinkRecalc();
                        ProductService.AddProductLink(id, catId, 0, true);
                        ProductService.DisableDynamicProductLinkRecalc();
                        ProductService.SetProductHierarchicallyEnabled(id);
                    }
                }

                redir = id == 0 ? null : string.Format("Product.aspx?ProductID={0}{1}", id, catId == 0 ? "" : "&CategoryID=" + Request["categoryid"]);
            }
            else
            {
                if (string.IsNullOrEmpty(txtStockNumber.Text.Trim()))
                {
                    lStockNumberError.Text = Resource.Admin_Product_ArtNoEmpty;
                    return;
                }
                if (UpdateProduct())
                {
                    if (ProductId != 0)
                    {
                        SaveProductTabs(ProductId);
                    }
                    Response.Redirect(Request.RawUrl + "#tabid=" + tabid.Value);
                }
            }

            if (!string.IsNullOrEmpty(redir))
            {
                Response.Redirect(redir);
            }
        }

        private void SaveProductTabs(int productId)
        {
            foreach (var detailsTabsModule in AttachedModules.GetModules<IProductTabs>())
            {
                var classInstance = (IProductTabs)Activator.CreateInstance(detailsTabsModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                {
                    foreach (ListViewItem tabBody in lvTabBodies.Items)
                    {
                        var tabTitleId = 0;
                        var stringTabTitleId = ((HiddenField)tabBody.FindControl("hfTabTitleId")).Value;
                        var stringTabBody = ((TextBox)tabBody.FindControl("fckTabBody")).Text;

                        if (!int.TryParse(stringTabTitleId, out tabTitleId))
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(stringTabBody))
                        {
                            classInstance.DeleteProductDetailsTab(productId, tabTitleId);
                        }
                        else if (!string.IsNullOrEmpty(stringTabTitleId))
                        {
                            classInstance.SaveProductDetailsTab(productId, tabTitleId, stringTabBody);
                        }
                    }
                }
            }
        }

        private int CreateProduct()
        {
            int id = 0;
            var artNo = txtStockNumber.Text;
            bool validArtNo = true;
            try
            {
                // проверяем свободен ли артикул
                if (ProductService.GetProductId(artNo) != 0)
                {
                    validArtNo = false;
                    Msg(Resource.Admin_Product_Duplicate);
                }

                Validate();
                //проверяем свободен ли урл
                if (!UrlService.IsAvailableUrl(ParamType.Product, txtSynonym.Text))
                {
                    validArtNo = false;
                    Msg(Resource.Admin_SynonymExist);
                }

                imgExcl1.Visible = !IsValidTab(1) || !validArtNo;
                imgExcl2.Visible = !IsValidTab(2);
                if (IsValid && validArtNo)
                {
                    ProductService.EnableDynamicProductLinkRecalc();
                    var productToCreate = GetProductFromForm();
                    if (productToCreate == null)
                        return 0;

                    productToCreate.AddManually = true;

                    id = ProductService.AddProduct(productToCreate, true);
                    ProductService.DisableDynamicProductLinkRecalc();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Msg("Erorr at create product");
                return 0;
            }
            TrialService.TrackEvent(TrialEvents.AddProduct, "");
            return id;
        }

        private bool UpdateProduct()
        {
            bool validArtNo = true;
            var artNo = txtStockNumber.Text;
            try
            {
                //проверяем свободен ли артикул
                var tempId = ProductService.GetProductId(artNo);
                if (tempId != 0 && tempId != ProductId)
                {
                    validArtNo = false;
                    Msg(Resource.Admin_Product_Duplicate);
                }

                Validate();
                if (IsValidTab(1))
                {
                    var synonym = txtSynonym.Text;
                    if (!string.IsNullOrEmpty(synonym))
                    {
                        if (!UrlService.IsAvailableUrl(ProductId, ParamType.Product, synonym))
                        {
                            Msg(Resource.Admin_SynonymExist);
                            return false;
                        }
                    }
                }
                imgExcl1.Visible = !IsValidTab(1) || !validArtNo;
                imgExcl2.Visible = !IsValidTab(2);


                imgExcl6.Visible = !_valid;

                if (IsValid && _valid && validArtNo)
                {
                    ProductService.EnableDynamicProductLinkRecalc();
                    var productToUpdate = GetProductFromForm();
                    if (productToUpdate == null)
                        return false;

                    productToUpdate.AddManually = true;

                    ProductService.UpdateProduct(productToUpdate, true);
                    ProductService.DisableDynamicProductLinkRecalc();
                    productCustomOption.SaveCustomOption();
                    productProperties.SaveProperties();
                    _product = ProductService.GetProduct(ProductId);
                    LoadProduct(_product);
                    UpdateMainPhoto();
                }
                else
                {
                    return false;
                }

                LoadSiteNavigation();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
            return true;
        }

        private Product GetProductFromForm()
        {
            _product = AddingNewProduct
                           ? new Product { Meta = new MetaInfo(), Offers = new List<Offer>() }
                           : ProductService.GetProduct(ProductId);
            if (_product == null)
                return null;
            _product.ArtNo = txtStockNumber.Text;
            _product.Name = txtName.Text;
            _product.UrlPath = txtSynonym.Text;
            _product.BriefDescription = fckBriefDescription.Text == "<br />" || fckBriefDescription.Text == "&nbsp;" || fckBriefDescription.Text == "\r\n"
                                            ? string.Empty
                                            : fckBriefDescription.Text;

            _product.Description = fckDescription.Text == "<br />" || fckDescription.Text == "&nbsp;" || fckDescription.Text == "\r\n"
                                       ? string.Empty
                                       : fckDescription.Text;

            _product.Weight = txtWeight.Text.TryParseFloat();
            _product.RecomendedManual = txtPopularityManually.Text.TryParseInt();
            _product.Size = txtSizeLength.Text + "|" + txtSizeWidth.Text + "|" + txtSizeHeight.Text;
            _product.Discount = txtDiscount.Text.TryParseFloat();
            _product.Enabled = chkEnabled.Checked;
            _product.AllowPreOrder = chkAllowPreOrder.Checked;

            _product.BestSeller = chkBestseller.Checked;
            _product.Recomended = chkRecommended.Checked;
            _product.New = chkNew.Checked;
            _product.OnSale = chkOnSale.Checked;
            _product.BrandId = popUpBrand.SelectBrandId;
            _product.SalesNote = txtSalesNote.Text;

            _product.Gtin = txtGtin.Text;
            _product.GoogleProductCategory = txtGoogleProductCategory.Text;

            _product.Adult = chbAdult.Checked;
            _product.ManufacturerWarranty = chbManufacturerWarranty.Checked;

            _product.ShippingPrice = txtShippingPrice.Text.TryParseFloat();
            _product.Unit = txtUnit.Text;

            _product.Multiplicity = txtMultiplicity.Text.TryParseFloat();
            _product.MaxAmount = txtMaxAmount.Text.IsNotEmpty() ? txtMaxAmount.Text.TryParseFloat() : (float?)null;
            _product.MinAmount = txtMinAmount.Text.IsNotEmpty() ? txtMinAmount.Text.TryParseFloat() : (float?)null;

            _product.Offers = productOffers.Offers;
            _product.HasMultiOffer = productOffers.HasMultiOffer;

            _product.Meta.Title = txtTitle.Text;
            _product.Meta.H1 = txtH1.Text;
            _product.Meta.MetaDescription = txtMetaDescription.Text;
            _product.Meta.MetaKeywords = txtMetaKeywords.Text;
            _product.Meta.Type = MetaType.Product;
            _product.Meta.ObjId = _product.ProductId;
            return _product;
        }


        protected bool ValidateInput()
        {
            return _valid;
        }

        protected bool IsValidTab(int tab)
        {
            return
                (from BaseValidator v in Validators where v.ValidationGroup.Equals(tab.ToString()) && !v.IsValid select v).
                    ToArray().Length == 0;
        }

        protected void IsValidTabb(int tab)
        {
            lSize.Visible = (from BaseValidator v in Validators where v.ValidationGroup.Equals(tab.ToString()) && !v.IsValid select v).ToArray().Length == 0;
        }

        #region CategoryTree

        protected void LoadCategoryTree()
        {
            if (!IsPostBack)
            {
                var node = new TreeNode { Text = Resource.Admin_m_Category_Root, Value = @"0", Selected = true, SelectAction = TreeNodeSelectAction.None };
                LinksProductTree.Nodes.Add(node);
                LoadChildCategories(LinksProductTree.Nodes[0]);
                FillListBox();
            }
        }

        protected void btnDelLink_Click(object sender, EventArgs e)
        {
            int categoryId;
            if (!string.IsNullOrEmpty(ListlinkBox.SelectedValue) && int.TryParse(ListlinkBox.SelectedValue, out categoryId))
            {
                ProductService.DeleteProductLink(ProductId, categoryId);
                CategoryService.RecalculateProductsCountManual();
                FillListBox();
            }
        }
        protected void lnAddLink_Click(object sender, EventArgs e)
        {
            if ((LinksProductTree.SelectedValue != null) && LinksProductTree.SelectedValue != "0" && LinksProductTree.SelectedValue.IsInt())
            {
                int temp;
                Int32.TryParse(LinksProductTree.SelectedValue, out temp);
                if (temp != 0)
                {
                    ProductService.EnableDynamicProductLinkRecalc();
                    ProductService.AddProductLink(ProductId, temp, 0, true);
                    ProductService.DisableDynamicProductLinkRecalc();
                    ProductService.SetProductHierarchicallyEnabled(ProductId);
                }
                FillListBox();
            }
        }
        public void PopulateNode(object sender, TreeNodeEventArgs e)
        {
            LoadChildCategories(e.Node);
        }

        private void LoadChildCategories(TreeNode node)
        {
            foreach (Category c in CategoryService.GetChildCategoriesByCategoryId(Convert.ToInt32(node.Value), false))
            {
                if (c.CategoryId != Convert.ToInt32(Request["id"]))
                {
                    var newNode = new TreeNode { Text = string.Format("{0}", c.Name), Value = c.CategoryId.ToString() };
                    if (c.HasChild)
                    {
                        newNode.Expanded = false;
                        newNode.PopulateOnDemand = true;
                        //newNode.ShowCheckBox = true;
                        //newNode.NavigateUrl = "javascript:void(0)";
                    }
                    else
                    {
                        newNode.Expanded = true;
                        newNode.PopulateOnDemand = false;

                        //newNode.ShowCheckBox = true;
                        //newNode.NavigateUrl = "javascript:void(0)";
                    }
                    node.ChildNodes.Add(newNode);
                }
            }
        }
        public void FillListBox()
        {
            ListlinkBox.Items.Clear();
            try
            {
                foreach (var catId in ProductService.GetCategoriesIDsByProductId(ProductId))
                {
                    var item = new ListItem();

                    IList<Category> parentCategories = CategoryService.GetParentCategories(catId);

                    var way = new StringBuilder();
                    for (int i = parentCategories.Count - 1; i >= 0; i--)
                    {
                        if (way.Length == 0)
                        {
                            way.Append(parentCategories[i].Name);
                        }
                        else
                        {
                            way.Append(" > " + parentCategories[i].Name);
                        }
                    }
                    if (ProductService.IsMainLink(ProductId, catId))
                        way.AppendMany(" (", Resource.Admin_Product_MainCategory, ")");
                    item.Text = way.ToString();
                    item.Value = catId.ToString();
                    ListlinkBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                //Debug.LogError(ex, ProductId);
                Debug.LogError(ex);
            }
        }
        #endregion

        protected void btnMainLink_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ListlinkBox.SelectedValue)) return;
            ProductService.SetMainLink(ProductId, Convert.ToInt32(ListlinkBox.SelectedValue));
            //RouteService.DeleteFromCache(ProductId, ParamType.Product);
            FillListBox();
        }

        protected void ibRemoveBrand_Click(object sender, EventArgs e)
        {
            popUpBrand.SelectBrandId = 0;
            ibRemoveBrand.Visible = false;

            if (Request["ProductID"].TryParseInt() != 0)
            {
                ProductService.DeleteBrand(Request["ProductID"].TryParseInt());
            }

        }
    }
}