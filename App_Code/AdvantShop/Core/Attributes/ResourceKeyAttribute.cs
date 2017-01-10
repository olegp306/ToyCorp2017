using System;
using System.Resources;
using Resources;

namespace AdvantShop.Core.Attributes
{
    public class ResourceKeyAttribute : Attribute, IAttribute<string>
    {
        private readonly string _resourceKey;

        private readonly ResourceManager _resource;
        public ResourceKeyAttribute(string resourceKey)
        {
            _resource = new ResourceManager(typeof(Resource));
            _resourceKey = resourceKey;
        }

        public string Value
        {
            get
            {
                var displayName = _resource.GetString(_resourceKey);
                return string.IsNullOrEmpty(displayName) ? string.Format("[[{0}]]", _resourceKey) : displayName;
            }
        }
    }
}