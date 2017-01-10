//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using Resources;
using System.Collections.Generic;
using System.Web.UI;

namespace AdvantShop.Helpers
{
    public class ValidElement
    {
        public Control Control { get; set; }
        public ValidType ValidType { get; set; }
        
        private string _message = "";
        public string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                    _message = errorMessage[ValidType];
                return _message;
            }
            set { _message = value; }
        }

        public Control ErrContent { get; set; }
        public bool Valid { get; set; }

        private Dictionary<ValidType, string> errorMessage = new Dictionary<ValidType, string>();

        public ValidElement()
        {
            errorMessage.Add(ValidType.Required, Resource.Helpers_ValidElement_Required);
            errorMessage.Add(ValidType.Email, Resource.Helpers_ValidElement_IncorrectEmail);
            errorMessage.Add(ValidType.Number, Resource.Helpers_ValidElement_IncorrectNumber);
            errorMessage.Add(ValidType.Money, Resource.Helpers_ValidElement_IncorrectMoneyFromat);
            errorMessage.Add(ValidType.Url, Resource.Helpers_ValidElement_IncorrectUrl);
        }
    }

    public enum ValidType
    {
        Required,
        Email,
        Number,
        Money,
        Url
    }
}