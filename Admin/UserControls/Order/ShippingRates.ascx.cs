using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Configuration;
using Newtonsoft.Json;

namespace Admin.UserControls.Order
{
    public partial class ShippingRatesControl : UserControl
    {
        #region Fields

        private const string PefiksId = "RadioShipRate_";

        private List<ShippingItem> _shippingRates;

        public ShippingOptionEx SelectShippingOptionEx
        {
            get
            {
                var temp = SelectedItem.Ext;
                if (temp != null 
                    //&& (temp.Type.HasFlag(ExtendedType.Pickpoint) || temp.Type.HasFlag(ExtendedType.CashOnDelivery) 
                    //|| temp.Type.HasFlag(ExtendedType.Cdek) || temp.Type.HasFlag(ExtendedType.Boxberry))
                    )
                {
                    temp.PickpointId = pickpointId.Value;
                    temp.PickpointAddress = pickAddress.Value;
                    temp.AdditionalData = pickAdditional.Value ?? string.Empty;
                }
                return temp;
            }
            set
            {
                if (value != null)
                {
                    pickpointId.Value = value.PickpointId;
                    pickAddress.Value = value.PickpointAddress;
                    pickAdditional.Value = value.AdditionalData ?? string.Empty;
                }
            }
        }
        
        public int SelectedId
        {
            get { return _selectedID.Value.Replace(PefiksId, "").TryParseInt(); }
            set
            {
                if (_shippingRates.Count > 0)
                {
                    _selectedID.Value = _shippingRates.Find(s => s.Id == value) != null
                        ? value.ToString()
                        : _shippingRates[0].Id.ToString();
                }
            }
        }

        private ShippingItem _selectedItem;
        public ShippingItem SelectedItem
        {
            get
            {
                if (_selectedItem != null)
                    return _selectedItem;

                _selectedItem = _selectedItem ?? ShippingManager.CurrentShippingRates.Find(x => x.Id == SelectedId);

                if (_selectedItem.Type == ShippingType.Multiship && _selectedItem.Ext != null)
                {
                    CalculateShippingRates();

                    _selectedItem = ShippingManager.CurrentShippingRates.Find(x => x.Id == SelectedId);
                }

                return _selectedItem;
            }
        }

        public int CountryId { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        private Currency _currency = CurrencyService.CurrentCurrency;
        public Currency Currency
        {
            get { return _currency; }
            set { if (value != null) _currency = value; }
        }

        protected int Amount;
        protected string Weight;
        protected string Cost;
        protected string WidgetCode;
        protected string Dimensions;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_shippingRates == null)
                _shippingRates = new List<ShippingItem>();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        public void ClearRates()
        {
            RadioList.Controls.Clear();
            RadioList.Visible = false;
            ShippingManager.CurrentShippingRates = new List<ShippingItem>();
            _shippingRates = new List<ShippingItem>();
        }

        public void Update()
        {
            if (_shippingRates.Count > 0)
            {
                ClearRates();
                CalculateShippingRates();
            }
        }

        public void LoadMethods()
        {
            CalculateShippingRates();
            GenerateShippingRates();
            LoadMultiShip();
        }

        public void LoadMethods(string selectedId)
        {
            SelectedId = Convert.ToInt32(selectedId);
            LoadMethods();
        }

        #region private

        private void CalculateShippingRates()
        {
            if (_shippingRates == null) _shippingRates = new List<ShippingItem>();

            var pickId = pickpointId.Value.TryParseInt();

            var shippingManager = new ShippingManager { Currency = Currency };
            _shippingRates.AddRange(shippingManager.GetShippingRates(CountryId, Zip, City, Region, ShoppingCart, 0, pickId, OrderItems));
            ShippingManager.CurrentShippingRates = _shippingRates;
        }

        private void LoadMultiShip()
        {
            var multishipMethod = _shippingRates.Find(x => x.Type == ShippingType.Multiship);
            if (multishipMethod != null)
            {
                divMultiShip.Visible = true;

                var multiship = new Multiship(ShippingMethodService.GetShippingParams(multishipMethod.MethodId))
                {
                    ShoppingCart = ShoppingCart
                };

                var totalWeight = ShoppingCart.TotalShippingWeight;
                Weight = totalWeight != 0
                    ? totalWeight.ToString("F3").Replace(",", ".")
                    : multiship.WeightAvg.ToString("F3").Replace(",", ".");

                WidgetCode = multiship.WidgetCode;
                Cost = ShoppingCart.TotalPrice.ToString("F2").Replace(",", ".");

                foreach (var item in ShoppingCart)
                {
                    var sizeArr = item.Offer.Product.Size.Split('|');

                    var length = sizeArr[0].TryParseInt() / 10;
                    var width = sizeArr[1].TryParseInt() / 10;
                    var height = sizeArr[2].TryParseInt() / 10;

                    Dimensions += (Dimensions.IsNotEmpty() ? "," : "") +
                                  string.Format("[{0}, {1}, {2}, {3}]",
                                      length > 0 ? length : multiship.LengthAvg,
                                      width > 0 ? width : multiship.WidthAvg,
                                      height > 0 ? height : multiship.HeightAvg,
                                      item.Amount);
                }
            }
        }

        private void GenerateShippingRates()
        {
            if ((_shippingRates != null) && (_shippingRates.Count != 0))
            {
                RadioList.Visible = true;
                int id = 1;
                string shippingRateGroup = string.Empty;

                var table = new HtmlTable();
                table.Style.Add(HtmlTextWriterStyle.Width, "100%");
                foreach (var shippingListItem in _shippingRates)
                {
                    if (shippingRateGroup != shippingListItem.MethodName && shippingListItem.Id != 1)
                    {
                        if (id != 1)
                        {
                            table.Controls.Add(GetTableLabel("<br />", id++));
                        }

                        shippingRateGroup = shippingListItem.MethodName;
                        table.Controls.Add(GetTableLabel(shippingRateGroup, id++));
                    }

                    table.Controls.Add(GetTableRadioButton(shippingListItem));
                }

                RadioList.Controls.Add(table);
            }
            else
            {
                lblNoShipping.Visible = true;
            }
        }

        private Control GetTableRadioButton(ShippingItem shippingListItem)
        {
            var tr = new HtmlTableRow();
            var td = new HtmlTableCell();

            if (divScripts.Visible == false)
                divScripts.Visible = shippingListItem.Ext != null && shippingListItem.Ext.Type == ExtendedType.Pickpoint;

            var radioButton = new RadioButton
                {
                    GroupName = "ShippingRateGroup",
                    ID = PefiksId + shippingListItem.Id, //+ "|" + shippingListItem.Rate
                    CssClass = "radio-shipping"
                };
            if (String.IsNullOrEmpty(_selectedID.Value.Replace(PefiksId, string.Empty)))
            {
                _selectedID.Value = radioButton.ID;
            }

            radioButton.Checked = radioButton.ID == _selectedID.Value;

            string strShippingPrice = shippingListItem.Rate != 0 ? CatalogService.GetStringPrice(shippingListItem.Rate, Currency.Value, Currency.Iso3) : shippingListItem.ZeroPriceMessage;
            radioButton.Text = string.Format("{0} <span class='price'>{1}</span>",
                                             shippingListItem.MethodNameRate, strShippingPrice);

            //if (shippingListItem.Ext != null && shippingListItem.Ext.Type == ExtendedType.Pickpoint)
            //{
            //    string temp;
            //    if (shippingListItem.Ext.Pickpointmap.IsNotEmpty())
            //        temp = string.Format(",{{city:'{0}', ids:null}}", shippingListItem.Ext.Pickpointmap);
            //    else
            //        temp = string.Empty;
            //    radioButton.Text +=
            //        string.Format(
            //            "<br/><div id=\"address\">{0}</div><a href=\"#\" onclick=\"PickPoint.open(SetPickPointAnswerAdmin{1});return false\">" +
            //            "{2}</a><input type=\"hidden\" name=\"pickpoint_id\" id=\"pickpoint_id\" value=\"\" /><br />",
            //            pickAddress.Value, temp, Resources.Resource.Client_OrderConfirmation_Select);
            //}

            radioButton.Text += ShippingMethodService.RenderExtend(shippingListItem, 0, pickAddress.Value, false);

            var panel = new Panel { CssClass = "inline-b" };

            using (var img = new Image{ImageUrl = SettingsGeneral.AbsoluteUrl + "/" + ShippingIcons.GetShippingIcon(shippingListItem.Type, shippingListItem.IconName, shippingListItem.MethodNameRate)})
            {
                panel.Controls.Add(img);
                td.Controls.Add(panel);
            }

            var panel2 = new Panel {CssClass = "inline-b"};
            panel2.Controls.Add(radioButton);

            td.Controls.Add(panel2);
            tr.Controls.Add(td);

            return tr;
        }

        private static Control GetTableLabel(string name, int id)
        {
            var tr = new HtmlTableRow();
            var td = new HtmlTableCell();

            var label = new Label { Text = @"<b>" + name + @"</b>", ID = "Label" + id };

            td.Controls.Add(label);
            tr.Controls.Add(td);

            return tr;
        }
        #endregion
    }
}