using System;
using AdvantShop.ExportImport;

namespace AdvantShop.Core.Attributes
{
    public class ProductFieldsStatusAttribute : Attribute, IAttribute<ProductFieldStatus>
    {
        private readonly ProductFieldStatus _name;
        public ProductFieldsStatusAttribute(ProductFieldStatus name)
        {
            _name = name;
        }

        public ProductFieldStatus Value
        {
            get { return _name; }
        }
    }
}