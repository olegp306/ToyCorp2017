namespace AdvantShop.Core.Attributes
{
    public static class AttributeHelper
    {
        public static TReturn GetAttributeValue<TAttribute, TReturn>(object obj)
        {
            var attributeValue = default(TReturn);

            if (obj == null) return attributeValue;
            var fi = obj.GetType().GetField(obj.ToString());

            if (fi == null) return attributeValue;
            var attributes = fi.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

            if (attributes == null || attributes.Length <= 0) return attributeValue;
            var attribute = attributes[0] as IAttribute<TReturn>;

            if (attribute != null)
            {
                attributeValue = attribute.Value;
            }
            return attributeValue;
        }
    }
}