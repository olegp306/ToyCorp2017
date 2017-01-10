//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;

namespace AdvantShop.Catalog
{

    public enum CustomOptionInputType
    {
        DropDownList = 0,
        RadioButton,
        CheckBox,
        TextBoxSingleLine,
        TextBoxMultiLine
    }

    public enum CustomOptionField
    {
        Title = 1,
        SortOrder = 2
    }

    [Serializable]
    public class CustomOption :IDentable
    {
        private int _nullFields;
        public int CustomOptionsId { get; set; }
        public string Title { get; set; }
        public bool IsRequired { get; set; }
        public CustomOptionInputType InputType { get; set; }
        public int SortOrder { get; set; }
        public int ProductId { get; set; }
        private List<OptionItem> _options;

        public List<OptionItem> Options 
        { 
            get { return _options ?? (_options = CustomOptionsService.GetCustomOptionItems(CustomOptionsId)); }
            set { _options = value; }
        }

        public CustomOption()
        {
        }

        public CustomOption(bool nullFields)
        {
            if (nullFields)
            {
                _nullFields = (int)CustomOptionField.Title | (int)CustomOptionField.SortOrder;
            }
            else
            {
                _nullFields = 0;
            }
        }

        public void SetFieldToNull(CustomOptionField field)
        {
            _nullFields = _nullFields | (int)field;
        }

        public bool IsNull(CustomOptionField field)
        {
            return (_nullFields & (int)field) > 0;
        }

        public int ID
        {
            get { return CustomOptionsId; }
        }
    }
}