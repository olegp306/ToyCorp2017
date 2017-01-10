using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace UserControls.OrderConfirmation
{
    public partial class ShippingMethod : UserControl
    {
        #region Fields

        private List<ShippingItem> _shippingRates;

        public ShippingOptionEx SelectShippingOptionEx
        {
            //get
            //{
            //    if (SelectedItem == null)
            //        return null;

            //    var temp = SelectedItem.Ext;
            //    if (temp != null && temp.PickpointAddress.IsNotEmpty()) //  && temp.Type == ExtendedType.Pickpoint
            //    {
            //        temp.PickpointId = pickpointId.Value;
            //        temp.PickpointAddress = pickAddress.Value;
            //        temp.AdditionalData = pickAdditional.Value;
            //    }
            //    return temp;
            //}
            set
            {
                if (value != null)
                {
                    pickpointId.Value = value.PickpointId;
                    pickAddress.Value = value.PickpointAddress;
                    pickAdditional.Value = value.AdditionalData;
                }
            }
        }

        public int SelectedId
        {
            get { return Convert.ToInt32(_selectedID.Value); }
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
                _selectedItem = _selectedItem ?? ShippingManager.CurrentShippingRates.Find(x => x.Id == SelectedId);
                //if (_selectedItem.Type == ShippingType.Cdek)
                //{
                //    _selectedItem.Ext = new ShippingOptionEx
                //        {
                //            AdditionalData = pickAdditional.Value,
                //            PickpointAddress = pickAddress.Value,
                //            PickpointId = pickpointId.Value,
                //            Type = ExtendedType.Cdek
                //        }; 
                //}

                return _selectedItem;
            }
        }

        public int Distance
        {
            get { return hfDistance.Value.TryParseInt(); }
            set { hfDistance.Value = value.ToString(); }
        }

        public int PickpointId
        {
            get { return pickpointId.Value.TryParseInt(); }
            set { pickpointId.Value = value.ToString(); }
        }

        public int CountryId { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

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


        public void ClearRates()
        {
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

        private void CalculateShippingRates()
        {
            if (_shippingRates == null)
                _shippingRates = new List<ShippingItem>();

            var shippingManager = new ShippingManager();

            _shippingRates = shippingManager.GetShippingRates(CountryId, Zip, City, Region, ShoppingCart, Distance,  PickpointId);
            ShippingManager.CurrentShippingRates = _shippingRates;
        }

        public void LoadMethods()
        {
            CalculateShippingRates();
        }

        public void LoadMethods(int selectedId)
        {
            CalculateShippingRates();
            SelectedId = selectedId;

            lvShippingRates.DataSource = _shippingRates;

            if (SelectedItem != null)
                lvShippingRates.SelectedIndex = _shippingRates.FindIndex(p => p.MethodId == SelectedItem.MethodId && p.Id == SelectedId);

            lvShippingRates.DataBind();

            divPickpoint.Visible = _shippingRates.Find(x => x.Ext != null && x.Ext.Type == ExtendedType.Pickpoint) != null;

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
                Cost = (ShoppingCart.TotalPrice - ShoppingCart.TotalDiscount).ToString("F2").Replace(",", ".");

                foreach (var item in ShoppingCart)
                {
                    var sizeArr = item.Offer.Product.Size.Split('|');

                    var length = (int) Math.Ceiling(sizeArr[0].TryParseFloat()/10);
                    var width = (int) Math.Ceiling(sizeArr[1].TryParseFloat()/10);
                    var height = (int) Math.Ceiling(sizeArr[2].TryParseFloat()/10);

                    Dimensions += (Dimensions.IsNotEmpty() ? "," : "") +
                                  string.Format("[{0}, {1}, {2}, {3}]",
                                      length > 0 ? length : multiship.LengthAvg,
                                      width > 0 ? width : multiship.WidthAvg,
                                      height > 0 ? height : multiship.HeightAvg,
                                      item.Amount);
                }
            }
        }
    }
}