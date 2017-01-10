using System;
namespace AdvantShop.ExportImport
{
    public class ProductCsvFilterModel
    {
        public string ModuleName { get; set; }
        public bool ExportNoInCategory { get; set; }
        public bool AllProducts { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}