using System;

namespace AdvantShop.Core.Attributes
{
    public class StringNameAttribute : Attribute, IAttribute<string>
    {
        private readonly string _name;
        public StringNameAttribute(string name)
        {
            _name = name;
        }

        public string Value
        {
            get { return _name; }
        }
    }
}