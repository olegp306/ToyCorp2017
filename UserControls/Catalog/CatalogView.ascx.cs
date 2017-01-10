//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.UI;
using AdvantShop.Catalog;
using AdvantShop.Configuration;

namespace UserControls.Catalog
{
    public partial class CatalogView : UserControl
    {
        protected Category HeadCategory;
        protected bool DisplayProductsCount = SettingsCatalog.ShowProductsCount;
        public int CategoryID;

        protected void Page_Load(object sender, EventArgs e)
        {
            IList<Category> childs = CategoryService.GetChildCategoriesByCategoryIdForMenu(CategoryID, true);
            HeadCategory = CategoryService.GetCategory(childs.Count > 0 ? childs[0].ParentCategoryId : 0);
        
            lvChilds.DataSource = childs;
            lvChilds.DataBind();
        }
    }
}
