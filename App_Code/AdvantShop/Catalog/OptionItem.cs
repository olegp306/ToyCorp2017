//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Catalog
{
    public enum OptionPriceType
    {
        Fixed = 0,
        Percent = 1
    }

    public enum OptionField
    {
        Title = 1,
        PriceBc = 2,
        SortOrder = 4
    }

    [Serializable]
    public class OptionItem :IDentable
    {
        public OptionItem(bool nullFields)
        {
            if (nullFields)
            {
                _nullFields = (int)OptionField.PriceBc | (int)OptionField.SortOrder | (int)OptionField.Title;
            }
            else
            {
                _nullFields = 0;
            }
        }

        public OptionItem()
        {
        }

        public int OptionId { get; set; }
        public string Title { get; set; }
        public float PriceBc { get; set; }
        public OptionPriceType PriceType { get; set; }
        public int SortOrder { get; set; }
        //public int CustomOptionsId { get; set; }
        private int _nullFields;
        public void SetFieldToNull(OptionField field)
        {
            _nullFields = _nullFields | (int)field;
        }
        public bool IsNull(OptionField field)
        {
            return (_nullFields & (int)field) > 0;
        }

        public int ID
        {
            get { return OptionId; }
        }
    }
}