//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Taxes
{
    public class TaxElement
    {
        public int TaxId { get; set; }
        public string Name { get; set; }
        public bool  Enabled { get; set; }
        public float Rate { get; set; }
        public bool ShowInPrice { get; set; }
    }
}