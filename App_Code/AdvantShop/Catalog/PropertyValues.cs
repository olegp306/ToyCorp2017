//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Catalog
{
    public class PropertyValue
    {
        public int PropertyValueId { get; set; }
        public int PropertyId { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }

        public Property Property { get; set; }
    }
}