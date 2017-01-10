using System;
using AdvantShop.Core.Attributes;
using AdvantShop.ExportImport;

namespace AdvantShop.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string ResourceKey(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValue<ResourceKeyAttribute, string>(enumValue);
        }

        public static string StrName(this Enum enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValue<StringNameAttribute, string>(enumValue);
            return string.IsNullOrEmpty(attrValue) ? enumValue.ToString().ToLower() : attrValue;
        }

        public static ProductFieldStatus Status(this ProductFields enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValue<ProductFieldsStatusAttribute, ProductFieldStatus>(enumValue);
            return attrValue;
        }
    }
}