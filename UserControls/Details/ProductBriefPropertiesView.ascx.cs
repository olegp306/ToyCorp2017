using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using System.Linq;

namespace UserControls.Details
{
    public partial class ProductBriefPropertiesView : System.Web.UI.UserControl
    {
        public int ProductId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var propertyValues = new List<PropertyValue>();
            var prodPropertyValues = PropertyService.GetPropertyValuesByProductId(ProductId).Where(v => v.Property.UseInBrief).ToList();

            foreach (var propValue in prodPropertyValues)
            {
                if (propertyValues.All(x => x.PropertyId != propValue.PropertyId))
                {
                    var value = propValue;
                    propertyValues.Add(new PropertyValue()
                    {
                        Property = value.Property,
                        PropertyId = value.PropertyId,
                        PropertyValueId = value.PropertyValueId,
                        SortOrder = value.SortOrder,
                        Value = String.Join(", ", prodPropertyValues.Where(x => x.PropertyId == value.PropertyId).Select(x => x.Value))
                    });
                }
            }

            Visible = propertyValues.Any();

            lvBriefProperties.DataSource = propertyValues;
            lvBriefProperties.DataBind();
        }
    }
}