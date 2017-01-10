//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Resources;

namespace UserControls.Details
{
    public partial class CustomOptionsControl : UserControl
    {
        private List<Control> _controls;
        public bool IsValid { get; private set; }
        public List<CustomOption> CustomOptions { get; private set; }
        public List<OptionItem> SelectedOptions { get; private set; }
        public bool ShowValidation { get; set; }

        public int ProductId { get { return Request["productid"].TryParseInt(); } }

        private Product _product;
        public Product Product
        {
            get
            {
                return _product ?? (_product = ProductService.GetProduct(ProductId));
            }
        }

        public CustomOptionsControl()
        {
            IsValid = true;
            ShowValidation = true;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request["productid"]))
            {
                return;
            }
            _controls = new List<Control>();
            CustomOptions = CustomOptionsService.GetCustomOptionsByProductId(ProductId);
            //CustomOptions.ForEach(x => x.Options.ForEach(y => y.PriceBc = GetPrice(y.PriceBc)));

            if (CustomOptions == null || CustomOptions.Count == 0) return;

            if (Request["edititem"] != null)
            {
                var shpCart = ShoppingCartService.CurrentShoppingCart;

                int editItem = -1;
                IList<EvaluatedCustomOptions> evlist = null;

                if (int.TryParse(Request["edititem"], out editItem))
                {
                    if (shpCart.Count > editItem && editItem >= 0)
                    {
                        evlist = shpCart[editItem].EvaluatedCustomOptions;
                    }
                }
                GetControls(CustomOptions, evlist);
            }
            else
            {
                GetControls(CustomOptions, null);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (CustomOptions != null && CustomOptions.Count > 0)
            {
                SelectedOptions = new List<OptionItem>();
            }
            else
            {
                SelectedOptions = null;
                return;
            }

            int idx = 0;
            foreach (Control ctrl in _controls)
            {

                if (CustomOptions.Count <= idx || CustomOptions[idx].Options == null || CustomOptions[idx].Options.Count == 0)
                {
                    continue;
                }
                
                if (ctrl is DropDownList)
                {
                    int id = SQLDataHelper.GetInt(((DropDownList)ctrl).SelectedValue);
                    SelectedOptions.Add(id > 0 ? CustomOptions[idx].Options.WithId(id) : null);
                    idx++;
                }
                else if (ctrl is RadioButtonList)
                {
                    int id = SQLDataHelper.GetInt(((RadioButtonList)ctrl).SelectedValue);
                    SelectedOptions.Add(id > 0 ? CustomOptions[idx].Options.WithId(id) : null);
                    idx++;
                }
                else if (ctrl is CheckBox)
                {
                    if (((CheckBox)ctrl).Checked)
                    {
                        CustomOptions[idx].Options[0].Title = Resource.Client_UserControls_CustomOptions_Yes;
                        SelectedOptions.Add(CustomOptions[idx].Options[0]);
                    }
                    else
                    {
                        if (CustomOptions[idx].IsRequired)
                        {
                            CustomOptions[idx].Options[0].Title = Resource.Client_UserControls_CustomOptions_No;
                            SelectedOptions.Add(CustomOptions[idx].Options[0]);

                            var chkControl = ((CheckBox)ctrl);
                            chkControl.Checked = true;
                            chkControl.Enabled = false;
                        }
                        else
                        {
                            SelectedOptions.Add(null);
                        }
                    }
                    idx++;
                }

                else if (ctrl is TextBox)
                {
                    if (((TextBox)ctrl).TextMode == TextBoxMode.MultiLine)
                    {
                        if (string.IsNullOrEmpty(((TextBox)ctrl).Text))
                        {
                            SelectedOptions.Add(null);
                            if (CustomOptions[idx].IsRequired)
                            {
                                IsValid = false;
                            }
                            if (ShowValidation)
                            {
                                ((TextBox)ctrl).CssClass = "valid-required group-cOptions";
                            }

                        }
                        else
                        {
                            CustomOptions[idx].Options = new List<OptionItem>();
                            var opt = new OptionItem { Title = ((TextBox)ctrl).Text };
                            CustomOptions[idx].Options.Add(opt);
                            SelectedOptions.Add(CustomOptions[idx].Options[0]);
                        }
                        idx++;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(((TextBox)ctrl).Text))
                        {
                            SelectedOptions.Add(null);
                            if (CustomOptions[idx].IsRequired)
                            {
                                IsValid = false;
                            }
                            if (ShowValidation)
                            {
                                ((TextBox)ctrl).CssClass = "valid-required group-cOptions";
                            }
                        }
                        else
                        {
                            CustomOptions[idx].Options = new List<OptionItem>();
                            var opt = new OptionItem { Title = ((TextBox)ctrl).Text };
                            CustomOptions[idx].Options.Add(opt);
                            SelectedOptions.Add(CustomOptions[idx].Options[0]);
                        }
                        idx++;
                    }
                }
            }
        }


        ///**

        private void GetControls(IEnumerable<CustomOption> customOptions, IList<EvaluatedCustomOptions> evlist)
        {
            foreach (var copt in customOptions)
            {
                var row = new HtmlGenericControl("div");
                row.Attributes.Add("class", "prop-str");
                switch (copt.InputType)
                {
                    case CustomOptionInputType.DropDownList:
                        GetDropDownList(row, copt, evlist);
                        break;
                    case CustomOptionInputType.RadioButton:
                        GetRadioButton(row, copt, evlist);
                        break;
                    case CustomOptionInputType.CheckBox:
                        GetCheckBox(row, copt, evlist);
                        break;
                    case CustomOptionInputType.TextBoxSingleLine:
                        GetTextBoxSingleLine(row, copt, evlist);
                        break;
                    case CustomOptionInputType.TextBoxMultiLine:
                        GetTextBoxMultiLine(row, copt, evlist);
                        break;
                }
                panel.Controls.Add(row);
            }
        }

        //****** Private controls set
        private void GetTextBoxSingleLine(HtmlGenericControl row, CustomOption customOption, IList<EvaluatedCustomOptions> evlist)
        {
            var title = new HtmlGenericControl("div");
            title.Attributes.Add("class", "param-name");
            var l = new Label { Text = customOption.Title + ":" };
            title.Controls.Add(l);

            var value = new HtmlGenericControl("div");
            value.Attributes.Add("class", "param-value");

            var subDiv = new HtmlGenericControl("div");
            subDiv.Attributes.Add("style", "width:200px;float:left");


            var ctrl = new TextBox { ID = (customOption.ID + customOption.Title).GetHashCode().ToString() };
            ctrl.Attributes.Add("style", "width:170px;");
            if (ShowValidation)
            {
                ctrl.Attributes.Add("class", "valid-required group-cOptions");
            }

            if (evlist != null)
            {
                EvaluatedCustomOptions ev = evlist.WithCustomOptionId(customOption.CustomOptionsId);
                if (ev != null)
                {
                    ctrl.Text = ev.OptionTitle;
                }
            }
            subDiv.Controls.Add(ctrl);
            value.Controls.Add(subDiv);
            _controls.Add(ctrl);


            var lbApply = new HyperLink() { ID = (customOption.ID + customOption.Title + "lb").GetHashCode().ToString(), Text = Resource.Client_Details_Apply };
            lbApply.Attributes.Add("style", "float:right");
            lbApply.Attributes.Add("class", "group-cOptions");
            lbApply.NavigateUrl = "javascript:void(0);";
            value.Controls.Add(lbApply);
            _controls.Add(lbApply);

            row.Controls.Add(title);
            row.Controls.Add(value);
        }

        private void GetTextBoxMultiLine(HtmlGenericControl row, CustomOption customOption, IList<EvaluatedCustomOptions> evlist)
        {
            var title = new HtmlGenericControl("div");
            title.Attributes.Add("class", "param-name");
            var l = new Label { Text = customOption.Title + ":" };
            title.Controls.Add(l);

            var value = new HtmlGenericControl("div");
            value.Attributes.Add("class", "param-value");

            var ctrl = new TextBox { TextMode = TextBoxMode.MultiLine, ID = customOption.Title };
            if (evlist != null)
            {
                EvaluatedCustomOptions ev = evlist.WithCustomOptionId(customOption.CustomOptionsId);
                if (ev != null)
                {
                    ctrl.Text = ev.OptionTitle;
                }
            }
            value.Controls.Add(ctrl);
            _controls.Add(ctrl);

            row.Controls.Add(title);
            row.Controls.Add(value);
        }

        private void GetCheckBox(HtmlGenericControl row, CustomOption customOption, IList<EvaluatedCustomOptions> evlist)
        {
            var title = new HtmlGenericControl("div");
            title.Attributes.Add("class", "param-name");
            var l = new Label { Text = customOption.Title + ":" };
            title.Controls.Add(l);

            var value = new HtmlGenericControl("div");
            value.Attributes.Add("class", "param-value");

            var price = customOption.Options[0].PriceBc;
            var ctrl = new CheckBox { ID = (customOption.ID + customOption.Title).GetHashCode().ToString() }; // , AutoPostBack = true

            if (price != 0)
            {
                var prefix = (price > 0) ? " +" : " ";

                switch (customOption.Options[0].PriceType)
                {
                    case OptionPriceType.Fixed:
                        // price = GetPrice(price);
                        ctrl.Text = prefix + CatalogService.GetStringPrice(price);
                        break;

                    case OptionPriceType.Percent:
                        ctrl.Text = prefix + price.ToString("#,0.##") + @"%";
                        break;
                }
            }
            else
            {
                ctrl.Text = string.Empty;
            }

            if (evlist != null)
            {
                EvaluatedCustomOptions ev = evlist.WithCustomOptionId(customOption.CustomOptionsId);
                if (ev != null)
                {
                    ctrl.Checked = ev.OptionId > 0;
                }
            }

            value.Controls.Add(ctrl);
            _controls.Add(ctrl);

            row.Controls.Add(title);
            row.Controls.Add(value);

        }

        private void GetRadioButton(HtmlGenericControl row, CustomOption customOption, IList<EvaluatedCustomOptions> evlist)
        {
            var title = new HtmlGenericControl("div");
            title.Attributes.Add("class", "param-name");
            var l = new Label { Text = customOption.Title + ":" };
            title.Controls.Add(l);

            var value = new HtmlGenericControl("div");
            value.Attributes.Add("class", "param-value");

            var ctrl = new RadioButtonList { ID = (customOption.ID + customOption.Title).GetHashCode().ToString(), CssClass = "table-customOptions" }; // , AutoPostBack = true
            if (!customOption.IsRequired)
            {
                var item = new ListItem { Value = @"-1", Text = Resource.Client_UserControls_CustomOptions_None };
                ctrl.Items.Add(item);
            }
            foreach (var opt in customOption.Options)
            {
                var price = opt.PriceBc;
                var item = new ListItem { Value = opt.OptionId.ToString() };

                if (price != 0)
                {
                    var prefix = price > 0 ? " +" : " ";

                    switch (opt.PriceType)
                    {
                        case OptionPriceType.Fixed:
                            item.Text = opt.Title + prefix + CatalogService.GetStringPrice(price);
                            break;

                        case OptionPriceType.Percent:
                            item.Text = opt.Title + prefix + price.ToString("#,0.##") + @"%";
                            break;
                    }
                }
                else
                {
                    item.Text = opt.Title;
                }

                ctrl.Items.Add(item);
            }
            if (evlist != null)
            {
                EvaluatedCustomOptions ev = evlist.WithCustomOptionId(customOption.CustomOptionsId);
                if (ev != null)
                {
                    ctrl.SelectedValue = ev.OptionId.ToString();
                }
                else
                {
                    ctrl.SelectedIndex = 0;
                }
            }
            else
            {
                ctrl.SelectedIndex = 0;
            }
            value.Controls.Add(ctrl);
            _controls.Add(ctrl);

            row.Controls.Add(title);
            row.Controls.Add(value);

        }

        private void GetDropDownList(HtmlGenericControl row, CustomOption customOption, IList<EvaluatedCustomOptions> evlist)
        {
            var title = new HtmlGenericControl("div");
            title.Attributes.Add("class", "param-name");
            var l = new Label { Text = customOption.Title + ":" };
            title.Controls.Add(l);

            var value = new HtmlGenericControl("div");
            value.Attributes.Add("class", "param-value");

            var ctrl = new DropDownList
                {
                    Width = 210,
                    ID = (customOption.ID + customOption.Title).GetHashCode().ToString(),
                    //AutoPostBack = true
                };

            if (!customOption.IsRequired)
            {
                var item = new ListItem { Value = @"-1", Text = Resource.Client_UserControls_CustomOptions_None };
                ctrl.Items.Add(item);
            }
            foreach (var opt in customOption.Options)
            {
                var price = opt.PriceBc;
                var item = new ListItem { Value = opt.OptionId.ToString() };

                if (price != 0)
                {
                    var prefix = price > 0 ? " +" : " ";

                    switch (opt.PriceType)
                    {
                        case OptionPriceType.Fixed:
                            //price = GetPrice(price);
                            item.Text = opt.Title + prefix + CatalogService.GetStringPrice(price);
                            break;

                        case OptionPriceType.Percent:
                            item.Text = opt.Title + prefix + price.ToString("#,0.##") + @"%";
                            break;
                    }
                }
                else
                {
                    item.Text = opt.Title;
                }
                ctrl.Items.Add(item);
            }

            if (evlist != null)
            {
                EvaluatedCustomOptions ev = evlist.WithCustomOptionId(customOption.CustomOptionsId);
                if (ev != null)
                {
                    ctrl.SelectedValue = ev.OptionId.ToString();
                }
            }
            value.Controls.Add(ctrl);
            _controls.Add(ctrl);

            row.Controls.Add(title);
            row.Controls.Add(value);

        }

        private float GetPrice(float price)
        {
            return CatalogService.CalculateProductPrice(price, Product.Discount, CustomerContext.CurrentCustomer.CustomerGroup, null);
        }
    }
}