using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Resources;

namespace Admin.UserControls.Order
{
    public partial class OrderItemsControl : System.Web.UI.UserControl
    {
        public float CurrencyValue { get; set; }
        public Currency Currency { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public int CurrencyNumCode { get; set; }
        public float OldCurrencyValue { get; set; }
        public bool IsCodeBefore { get; set; }

        public string CouponCode
        {
            get { return (string) ViewState["CouponCode"]; }
            set { ViewState["CouponCode"] = value; }
        }

        public float OrderDiscount
        {
            get { return txtDiscount.Text.TryParseFloat(); }
            set { txtDiscount.Text = CatalogService.FormatPriceInvariant(value); }
        }

        public float GroupDiscount
        {
            get { return ViewState["GroupDiscount"] == null ? 0 : (float)ViewState["GroupDiscount"]; }
            set { ViewState["GroupDiscount"] = value; }
        }

        private List<OrderItem> _orderItems;

        public List<OrderItem> OrderItems
        {
            get { return _orderItems ?? (_orderItems = LoadOrderItems()); }
            set { _orderItems = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Visible = false;

            txtDiscount.Text = CatalogService.FormatPriceInvariant(OrderDiscount);
            if (!IsPostBack)
            {
                LoadProducts();
            }

            if (string.IsNullOrEmpty(CurrencyCode))
            {
                if (IsPostBack)
                    UpdateCurrency();
                else
                    LoadDefaultCurrency();
            }
        }

        private void LoadDefaultCurrency()
        {
            ddlCurrs.DataBind();
            UpdateCurrency();
        }

        public void SetCurrency(string currencyCode, float currencyValue, int currencyNumCode, string currencySymbol, bool isCodeBefore)
        {
            ddlCurrs.DataBind();
            ddlCurrs.SelectedValue = currencyCode;
            lcurrency.Text = ddlCurrs.SelectedItem.Text;
            hfOldCurrencyValue.Value = currencyValue.ToString();
            OldCurrencyValue = currencyValue;
            CurrencyCode = currencyCode;
            CurrencyNumCode = currencyNumCode;
            CurrencySymbol = currencySymbol;
            Currency = CurrencyService.Currency(currencyCode);
            CurrencyValue = currencyValue;
            IsCodeBefore = isCodeBefore;

        }

        private List<OrderItem> LoadOrderItems()
        {
            return ViewState["OrderItems"] == null
                       ? new List<OrderItem>()
                       : ((OrderItem[])ViewState["OrderItems"]).ToList();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SaveOrderItems();
        }

        private void SaveOrderItems()
        {
            ViewState["OrderItems"] = OrderItems.ToArray();
        }

        private void LoadProducts()
        {
            DataListOrderItems.DataSource = OrderItems;
            DataListOrderItems.DataBind();
        }

        private void UpdateCurrency()
        {
            var cur = CurrencyService.Currency(ddlCurrs.SelectedValue);
            if (cur == null) return;

            CurrencyValue = cur.Value;
            CurrencyCode = cur.Iso3;
            CurrencySymbol = cur.Symbol;
            CurrencyNumCode = cur.NumIso3;
            IsCodeBefore = cur.IsCodeBefore;
            Currency = cur;
            OldCurrencyValue = !string.IsNullOrEmpty(hfOldCurrencyValue.Value) ? SQLDataHelper.GetFloat(hfOldCurrencyValue.Value) : CurrencyValue;
            hfOldCurrencyValue.Value = cur.Value.ToString();
        }

        protected void ddlCurrs_SelectedChanged(object sender, EventArgs e)
        {
            LoadProducts();
            ItemsUpdated(this, new EventArgs());
        }

        protected void sds_Init(object sender, EventArgs e)
        {
            ((SqlDataSource)sender).ConnectionString = Connection.GetConnectionString();
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            pTreeProduct.Show();
        }

        protected void dlItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SaveQuantity")
            {
                try
                {
                    float quantity = 0;
                    if (e.Item.FindControl("txtQuantity") != null &&
                        float.TryParse(((TextBox) e.Item.FindControl("txtQuantity")).Text, out quantity))
                    {
                        if (quantity > 0)
                        {
                            var item = OrderItems.Find(oi => oi.OrderItemID == SQLDataHelper.GetInt(e.CommandArgument));
                            item.Amount = quantity;
                            item.Price = ((TextBox) e.Item.FindControl("txtPrice")).Text.TryParseFloat();
                        }
                        else
                        {
                            OrderItems.RemoveAll(oi => oi.OrderItemID == SQLDataHelper.GetInt(e.CommandArgument));
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    lblError.Text = ex.Message;
                    lblError.Visible = true;
                }
                LoadProducts();
                ItemsUpdated(this, new EventArgs());
            }
            if (e.CommandName == "Delete")
            {
                try
                {
                    OrderItems.RemoveAll(oi => oi.OrderItemID == SQLDataHelper.GetInt(e.CommandArgument));
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    lblError.Text = ex.Message;
                    lblError.Visible = true;
                }

                LoadProducts();
                ItemsUpdated(this, new EventArgs());
            }
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            var html = new StringBuilder();
            html.Append("<ul>");

            foreach (EvaluatedCustomOptions ev in evlist)
            {
                html.Append(string.Format("<li>{0}: {1} - {2}</li>", ev.CustomOptionTitle, ev.OptionTitle, CatalogService.GetStringPrice(ev.OptionPriceBc, Currency)));
            }

            html.Append("</ul>");
            return html.ToString();
        }

        protected void pTreeProduct_NodeSelected(object sender, PopupTreeView.TreeNodeSelectedArgs args)
        {

            var items = new List<OrderItem>();
            foreach (var val in args.SelectedValues)
            {
                int treeSelectedValue;
                int.TryParse(val, out treeSelectedValue);
                var product = ProductService.GetProduct(SQLDataHelper.GetInt(treeSelectedValue));
                items.Add(new OrderItem
                    {
                        Name = product.Name,
                        Price = product.Offers[0].Price * (100 - Math.Max(product.Discount, GroupDiscount)) / 100,
                        SupplyPrice = product.Offers[0].SupplyPrice,
                        ProductID = product.ProductId,
                        Amount = 1,
                        ArtNo = product.ArtNo,
                        IsCouponApplied = CouponCode.IsNotEmpty() && CouponService.GetCouponByCode(CouponCode) != null && CouponService.IsCouponAppliedToProduct(CouponService.GetCouponByCode(CouponCode).CouponID, product.ProductId) ,
                        Weight = product.Weight,
                        PhotoID = product.ProductPhotos.OrderByDescending(p => p.Main).FirstOrDefault() != null 
                                ? product.ProductPhotos.OrderByDescending(p => p.Main).FirstOrDefault().PhotoId 
                                : (int?)null
                    });
            }

            if (OrderItems == null)
                OrderItems = new List<OrderItem>();

            foreach (var item in items)
            {
                item.OrderItemID = GenItemId();
                item.SelectedOptions = new List<EvaluatedCustomOptions>();
                if (OrderItems.Contains(item))
                {
                    OrderItems[OrderItems.IndexOf(item)].Amount += 1;
                }
                else
                {
                    OrderItems.Add(item);
                }
            }
            LoadProducts();
            ItemsUpdated(this, new EventArgs());
            pTreeProduct.UnSelectAll();
            pTreeProduct.Hide();
        }

        protected void btnAddProductByArtNo_Click(object sender, EventArgs e)
        {
            Product product = null;
            Offer offer = null;

            if (hfOffer.Value.IsNotEmpty())
            {
                int offerId = hfOffer.Value.Trim().TryParseInt();
                if (offerId != 0)
                {
                    offer = OfferService.GetOffer(offerId);
                    product = ProductService.GetProduct(offer.ProductId);
                }
            }
            else if (txtArtNo.Text.IsNotEmpty())
            {
                string searchTerm = txtArtNo.Text.Trim();

                product = ProductService.GetProduct(searchTerm, true);
                if (product == null)
                {
                    product = ProductService.GetProductByName(searchTerm);
                    if (product == null)
                        return;
                }
                else
                {
                    offer = product.Offers.Find(o => o.ArtNo == searchTerm);
                }
            }

            if (product == null)
                return;

            if (offer == null)
                offer = product.Offers[0];

            var item = new OrderItem
                {
                    OrderItemID = GenItemId(),
                    Name = product.Name,
                    Price = offer.Price * (100 - Math.Max(product.Discount, GroupDiscount)) / 100,
                    SupplyPrice = offer.SupplyPrice,
                    ProductID = product.ProductId,
                    Amount = 1,
                    ArtNo = offer.ArtNo,
                    Color = offer.Color != null ? offer.Color.ColorName : null,
                    Size = offer.Size != null ? offer.Size.SizeName : null,
                    PhotoID = offer.Photo != null ? offer.Photo.PhotoId : default(int),
                    SelectedOptions = new List<EvaluatedCustomOptions>(),
                    IsCouponApplied = CouponCode.IsNotEmpty() && CouponService.GetCouponByCode(CouponCode) != null && CouponService.IsCouponAppliedToProduct(CouponService.GetCouponByCode(CouponCode).CouponID, product.ProductId),
                    Weight = product.Weight
                };


            if (OrderItems == null)
                OrderItems = new List<OrderItem>();

            if (OrderItems.Contains(item))
            {
                OrderItems[OrderItems.IndexOf(item)].Amount += 1;
            }
            else
            {
                OrderItems.Add(item);
            }

            txtArtNo.Text = "";
            hfOffer.Value = "";
            LoadProducts();
            ItemsUpdated(this, new EventArgs());
        }

        private int GenItemId()
        {
            if (OrderItems.Count == 0)
                return 1;
            return OrderItems.Max(oi => oi.OrderItemID) + 1;
        }

        protected void pTreeProduct_Hiding(object sender, EventArgs args)
        {
            ItemsUpdated(this, new EventArgs());
        }

        public event Action<object, EventArgs> ItemsUpdated;

        protected void dlItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = e.Item;
            if ((item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem))
            {
                var strQuantity = ((TextBox)item.FindControl("txtQuantity")).Text;
                float quantity = 0;
                if (float.TryParse(strQuantity, out quantity))
                {
                    var offer = OfferService.GetOffer(((Literal)item.FindControl("ltArtNo")).Text);
                    if ((offer == null) || (quantity > offer.Amount))
                    {
                        var avail = offer == null ? 0 : offer.Amount;
                        ((Label)(item.FindControl("lbMaxCount"))).Text = string.Format("{0}: {1} {2}",
                                                                                       Resource.Client_ShoppingCart_NotAvailable,
                                                                                       avail, Resource.Client_ShoppingCart_NotAvailable_End);
                        ((Label)(item.FindControl("lbMaxCount"))).ForeColor = System.Drawing.Color.Red;
                    }
                    else if (quantity <= offer.Amount)
                    {
                        ((Label)(item.FindControl("lbMaxCount"))).Text = Resource.Client_ShoppingCart_Available;
                        ((Label)(item.FindControl("lbMaxCount"))).ForeColor = System.Drawing.Color.Green;
                    }
                }
                var btn = ((LinkButton) e.Item.FindControl("buttonDelete"));
                btn.Attributes["data-confirm"] = string.Format(Resource.Admin_OrderItem_Delete);
            }
        }

        protected string RenderPicture(int productId, int? photoId)
        {
            if (photoId != null)
            {
                var photo = PhotoService.GetPhoto((int)photoId);
                if (photo != null)
                {
                    return string.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.Small, photo.PhotoName, true));
                }
            }

            var p = ProductService.GetProduct(productId);
            if (p != null && !string.IsNullOrEmpty(p.Photo))
            {
                return string.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.Small, p.Photo, true));
            }

            return string.Format("<img src='{0}' alt=\"\"/>", AdvantShop.Core.UrlRewriter.UrlService.GetAbsoluteLink("images/nophoto_small.jpg"));
        }

        public void SetCustomerDiscount(Guid customerId)
        {
            var groupId = CustomerService.GetCustomerGroupId(customerId);
            var group = CustomerGroupService.GetCustomerGroup(groupId);

            if (group.CustomerGroupId != 0)
            {
                GroupDiscount = group.GroupDiscount;
            }
        }

        public void SetCustomerDiscount(string customerId)
        {
            SetCustomerDiscount(new Guid(customerId));
        }
    }
}