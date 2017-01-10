using AdvantShop.Catalog;
using System;
using AdvantShop.Configuration;

namespace UserControls.Details
{
    public partial class SizeColorPicker_Details : System.Web.UI.UserControl
    {
        public int ProductId { get; set; }
        public bool ManuallyInit { get; set; }

        public int SelectedOfferId { get; set; }

        public int ImageWidth
        {
            get { return SettingsPictureSize.ColorIconWidthDetails; }
        }

        public int ImageHeight
        {
            get { return SettingsPictureSize.ColorIconHeightDetails; }
        }

        protected Product Product; 

        protected void Page_Load(object sender, EventArgs e)
        {
            Product = ProductService.GetProduct(ProductId);
        }
    }
}