using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Customers;

namespace UserControls.Details
{
    public partial class ProductPropertiesView : System.Web.UI.UserControl
    {
        #region Fields

        public bool HasProperties { get; private set; }
        public int ProductId { get; set; }
        protected int? CurrentGroupID;

        protected int LineCounter = 1;


        protected readonly bool ShowInPlaceEditor = (CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled) && SettingsMain.EnableInplace;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var propertyValues = new List<PropertyValue>();
            var prodPropertyValues = PropertyService.GetPropertyValuesByProductId(ProductId).Where(v => v.Property.UseInDetails).ToList();

            if (!ShowInPlaceEditor)
            {
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
            }
            else
            {
                propertyValues = prodPropertyValues;
            }


            if (propertyValues.Count == 0 && !ShowInPlaceEditor)
            {
                HasProperties = false;
                Visible = false;
            }
            else
            {
                HasProperties = true;
                Visible = true;
            }

            lvProperties.DataSource = propertyValues;
            lvProperties.DataBind();
        }

        protected string RenderGroupHeader(PropertyGroup group)
        {
            if (group != null && CurrentGroupID != group.PropertyGroupId)
            {
                CurrentGroupID = group.PropertyGroupId;
                LineCounter = 1;
                return string.Format("<li class=\"propgroup\">{0}</li>", group.Name);
            }

            if (group == null && CurrentGroupID != null)
            {
                CurrentGroupID = null;
                LineCounter = 1;
                return String.Format("<li class=\"propgroup\">{0}</li>", Resources.Resource.UserControl_ProductPropertiesView_Other);
            }

            LineCounter++;

            return string.Empty;

        }
    }
}