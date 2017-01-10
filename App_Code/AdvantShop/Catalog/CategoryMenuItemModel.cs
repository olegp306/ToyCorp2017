using System.Collections.Generic;

namespace AdvantShop.Catalog
{
    public class CategoryMenuItem
    {
        public CategoryMenuItem()
        {
            SubCategories = new List<CategoryMenuItem>();
            Brands = new List<Brand>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }
        public string UrlPath { get; set; }
        public bool HasChild { get; set; }
        public bool DisplayBrandsInMenu { get; set; }
        public bool DisplaySubCategoriesInMenu { get; set; }

        public List<CategoryMenuItem> SubCategories { get; set; }
        public List<Brand> Brands { get; set; }
        public string CategoryPicturePath { get; set; }
    }
}