//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Repository
{
    public class Region
    {
        public int RegionID { get; set; }
        public int CountryID { get; set; }
        public string Name { get; set; }
        public string RegionCode { get; set; }
        public int SortOrder { get; set; }
    }
}