//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------


using System;
using System.Web.UI;
using AdvantShop.Catalog;
using AdvantShop.Repository.Currencies;

namespace Social.UserControls
{
    public partial class FilterPrice : UserControl
    {
        public int CategoryId { set; get; }
        public bool InDepth { set; get; }
        public float CurValMin;
        public float CurValMax;

        protected float Min;
        protected float Max;
    
    
        static int Get10Pow(float src)
        {
            int pow = 1;
            while (src / (10 * pow) >= 1)
            {
                pow *= 10;
            }
            return pow;
        }

        static void MegaRound(ref float src1, ref float src2)
        {
            int pow = Get10Pow(Math.Max(src1, src2));
            int pow2 = Get10Pow(src1);
            src1 = (float)Math.Floor((src1 / pow2)) * pow2;
            src2 = (float)Math.Ceiling((src2 / pow)) * pow;
        }
    
        public void Page_Load(object sender, EventArgs e)
        {
            var prices = CategoryService.GetPriceRange(CategoryId, InDepth);
            Min = (float)Math.Floor(prices.Key / CurrencyService.CurrentCurrency.Value);
            Max = (float)Math.Ceiling(prices.Value / CurrencyService.CurrentCurrency.Value);
            MegaRound(ref Min, ref Max);
            Visible = Min != Max;

            if (CurValMin < Min)
                CurValMin = Min;

            if (CurValMax > Max)
                CurValMax = Max;
        }
    }
}