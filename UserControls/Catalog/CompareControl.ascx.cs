using System;
using Newtonsoft.Json;

namespace UserControls.Catalog
{
    public partial class CompareUserControl : System.Web.UI.UserControl
    {
        public string ResultOptions = "";

        public enum eType
        {
            Checkbox = 0,
            Icon = 1
        }

        public int ProductId { get; set; }
        public int OfferId { get; set; }
        public bool IsSelected { get; set; }
        public string CompareBasket { get; set; }
        public int AnimationSpeed { get; set; }
        public double AnimationOpacity { get; set; }
        public eType Type { get; set; }
        public string CssClassContainer { get; set; }

        public string AnimationObj { get; set; }

        public CompareUserControl()
        {
            IsSelected = false;
            CompareBasket = "#toolbarBottomCompare";
            AnimationSpeed = 1200;
            AnimationOpacity = 0.1;
            Type = eType.Checkbox;
            CssClassContainer = "";
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            AnimationObj = AnimationObj != null ? AnimationObj : ".compare-" + OfferId;
        }

        protected string GetOptions()
        {
            var options = new
                {

                    compareBasket = CompareBasket,
                    animationSpeed = AnimationSpeed,
                    animationOpacity = AnimationOpacity,
                    type = Type.ToString().ToLower()
                };

            return JsonConvert.SerializeObject(options);

        }
    }
}