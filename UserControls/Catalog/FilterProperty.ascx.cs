//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Catalog;

namespace UserControls.Catalog
{
    public partial class FilterProperty : UserControl
    {
        #region Fields

        public List<int> AvaliblePropertyIDs { set; get; }
        public List<int> SelectedPropertyIDs { set; get; }
        public int SelectedCategoryId { set; get; }

        public Dictionary<int, KeyValuePair<float, float>> SelectedRangePropertyIDs { set; get; }

        public int CategoryId { get; set; }

        protected string RenderedProperties;

        private Category _category;
        private Category Category
        {
            get { return _category ?? (_category = CategoryService.GetCategory(CategoryId)); }
        }

        #endregion

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var propertyValues = PropertyService.GetPropertyValuesByCategories(CategoryId, true);
            //if (propertyValues.Count == 0)
            //{
            //    foreach (var cat in CategoryService.GetChildCategoriesByCategoryId(CategoryId, true))
            //    {
            //        propertyValues.Add(PropertyService.GetPropertyValuesByCategories(CategoryId, Category.DisplayChildProducts));
            //    }
            //}
            if (propertyValues == null)
            {
                Visible = false;
                return;
            }

            var properties = propertyValues.Select(item => new UsedProperty
            {
                PropertyId = item.PropertyId,
                Name = item.Property.Name,
                Expanded = item.Property.Expanded,
                Unit = item.Property.Unit,
                Type = item.Property.Type,
                ValuesList = propertyValues.Where(value => value.PropertyId == item.PropertyId).ToList(),
            }).Distinct(new PropertyComparer());


            // если фильтр на главной странице то  выводим только главные свойства
            string[] mainPorpertyForFilter = { "Пол", "Возраст", "Цена", "Бренд", "Каталог" };
            if (Page.ToString() == "ASP.default_aspx")
                properties = properties.Where(value => mainPorpertyForFilter.Contains(value.Name));

            var sb = new StringBuilder();
            foreach (var property in properties)
            {
                var itemsValues = RenderPropertyValues(property);

                if (itemsValues.Length > 0)
                {
                    sb.AppendFormat("<article class=\"block-uc-inside\"> " +
                                    "<div class=\"title\" data-expander-control=\"#filter-prop-{0}\">{1}</div> " +
                                    "<div class=\"content ex-content\" id=\"filter-prop-{0}\"{2} hidden=\"true\"> " +
                                    "{3} " +
                                    "</div> " +
                                    "</article>",
                        property.PropertyId, property.Name,
                        property.Expanded ? " style=\"display:block;\"" : string.Empty,
                        itemsValues);
                }
            }
            // добавляем селект для выбора 'каталога'
            if (Page.ToString() == "ASP.default_aspx")
                sb.AppendFormat(RenderCategorySelectInFilter());

            RenderedProperties = sb.ToString();
            Visible = RenderedProperties.Length > 0;
        }

        private string RenderCategorySelectInFilter()
        {
            var result = new StringBuilder();

            CategoryId = AdvantShop.Catalog.CategoryService.GetCategoryIdByName("Каталог");
            List<Category> filterCategories = new List<Category>();
            filterCategories = AdvantShop.Catalog.CategoryService.GetChildCategoriesByCategoryIdForMenu(CategoryId).ToList();

            result.AppendFormat("<article class=\"block-uc-inside\"> " +
                                   "<div class=\"title\" data-expander-control=\"#filter-prop\">Каталог</div> " +
                                   "<div class=\"content ex-content\" id=\"filter-prop\" hidden=\"true\"> ");
            result.AppendFormat("<div class='filter-select'>");
            result.AppendFormat("<select  id='catalogselectinfilter' name='selectcategory'>");
            result.AppendFormat("<option value='0'>Не важно</option> ");
            var i = 1;
            foreach (var filtercategory in filterCategories)
            {
                string selected = "";
                if (filtercategory.CategoryId == SelectedCategoryId)
                {
                    selected = " selected=\"1\"";
                    //selected="olegattr='tt'";
                }

                result.AppendFormat("<option value='" + filtercategory.UrlPath.ToString() + "' " + selected + ">" + filtercategory.Name + "</option> ");
                i++;
            }
            result.AppendFormat("</select> ");
            result.AppendFormat("</div> " + "</article>");
            return result.ToString();
        }
        private string RenderPropertyValues(UsedProperty property)
        {
            switch (property.Type)
            {
                case (int)PropertyType.Checkbox:
                    return RenderCheckBox(property);

                case (int)PropertyType.Selectbox:
                    return RenderSelectBox(property);

                case (int)PropertyType.Range:
                    return RenderRange(property);
            }
            return string.Empty;
        }

        private string RenderCheckBox(UsedProperty property)
        {
            if (property.ValuesList.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append("<div class=\"chb-list propList\"> ");

            foreach (var value in property.ValuesList)
            {
                sb.AppendFormat("<div> <input name=\"prop_{0}\" type=\"checkbox\" id=\"prop_{1}\" value=\"{1}\"{2}{3} /> <label for=\"prop_{1}\">{4}</label></div> ",
                    property.PropertyId,
                    value.PropertyValueId,
                    AvaliblePropertyIDs != null && !AvaliblePropertyIDs.Contains(value.PropertyValueId) ? " disabled=\"disabled\"" : string.Empty,
                    SelectedPropertyIDs != null && SelectedPropertyIDs.Contains(value.PropertyValueId) ? " checked=\"checked\" " : string.Empty,
                    value.Value
                );
            }
            sb.Append("</div>");

            return sb.ToString();
        }

        private string RenderSelectBox(UsedProperty property)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("<div class=\"filter-select\"> <select id=\"prop_{0}\" name=\"prop_{0}\"> ", property.PropertyId);
            sb.AppendFormat("<option value=\"0\">{0}</option> ", Resources.Resource.Client_Catalog_FilterProperty_Any);

            var selectedChanged = false;

            foreach (var value in property.ValuesList)
            {
                if (AvaliblePropertyIDs == null ||
                (AvaliblePropertyIDs != null && AvaliblePropertyIDs.Contains(value.PropertyValueId)))
                {
                    var selected = !selectedChanged && SelectedPropertyIDs != null &&
                                   SelectedPropertyIDs.Contains(value.PropertyValueId);

                    sb.AppendFormat("<option value=\"{0}\"{1}>{2}</option> ",
                                    value.PropertyValueId, selected ? " selected=\"1\"" : string.Empty, value.Value);

                    if (selected)
                        selectedChanged = true;
                }
            }
            sb.AppendLine("</select> </div>");
            return sb.ToString();
        }

        private string RenderRange(UsedProperty property)
        {
            var sb = new StringBuilder();

            var list = property.ValuesList.Select(v => v.Value.TryParseFloat()).Where(v => v != 0).ToList();

            if (list.Count < 1)
                return string.Empty;

            var min = list.Min();
            var max = list.Max();

            if (min == max && min == 0)
                return string.Empty;

            var curmin = min;
            var curmax = max;

            if (SelectedRangePropertyIDs != null && SelectedRangePropertyIDs.ContainsKey(property.PropertyId))
            {
                curmin = SelectedRangePropertyIDs[property.PropertyId].Key;
                curmax = SelectedRangePropertyIDs[property.PropertyId].Value;
            }

            sb.AppendFormat(
                "<div class=\"content-price\" id=\"filter-pricerange-{0}\"> " +
                "<div class=\"slider\" data-current-min=\"{1}\" data-current-max=\"{2}\"> " +
                "<input autocomplete=\"off\" type=\"text\" class=\"min\" value=\"{3}\"/> " +
                "<input autocomplete=\"off\" type=\"text\" class=\"max\" value=\"{4}\" /> " +
                "</div> " +
                "</div>",
                property.PropertyId, curmin.ToString().Replace(",", "."), curmax.ToString().Replace(",", "."), min.ToString().Replace(",", "."), max.ToString().Replace(",", "."));

            return sb.ToString();
        }
    }
}
