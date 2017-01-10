//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.FilePath;

namespace AdvantShop.Mails
{
    public abstract class MailTemplate
    {
        public abstract MailType Type { get; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public void BuildMail()
        {
            var logo = SettingsMain.LogoImageName.IsNotEmpty()
                           ? String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                                           SettingsMain.SiteUrl.Trim('/') + '/' +
                                           FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false),
                                           SettingsMain.ShopName)
                           : string.Empty;

            var mailFormat = MailFormatService.GetByType((int) Type);

            Body = mailFormat != null && mailFormat.FormatText != null
                       ? FormatString(mailFormat.FormatText).Replace("#LOGO#", logo)
                       : string.Empty;

            Subject = mailFormat != null && mailFormat.FormatSubject != null
                          ? FormatString(mailFormat.FormatSubject)
                          : string.Empty;
        }

        protected virtual string FormatString(string formatedStr)
        {
            return string.Empty;
        }
    }
    
    public class RegistrationMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnRegistration; }
        }

        private readonly string _shopUrl;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _regDate;
        private readonly string _password;
        private readonly string _subsrcibe;
        private readonly string _customerEmail;

        public RegistrationMailTemplate(string shopUrl, string firstName, string lastName, string regDate,
                                        string password, string subsrcibe, string customerEmail)
        {
            _shopUrl = shopUrl;
            _firstName = firstName;
            _lastName = lastName;
            _regDate = regDate;
            _password = password;
            _subsrcibe = subsrcibe;
            _customerEmail = customerEmail;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#EMAIL#", _customerEmail);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#LASTNAME#", _lastName);
            formatedStr = formatedStr.Replace("#REGDATE#", _regDate);
            formatedStr = formatedStr.Replace("#PASSWORD#", _password);
            formatedStr = formatedStr.Replace("#SUBSRCIBE#", _subsrcibe);
            formatedStr = formatedStr.Replace("#SHOPURL#", _shopUrl);
            
            return formatedStr;
        }
    }

    public class PwdRepairMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnPwdRepair; }
        }

        private readonly string _recoveryCode;
        private readonly string _email;
        private readonly string _link;

        public PwdRepairMailTemplate(string recoveryCode, string email, string link)
        {
            _recoveryCode = recoveryCode;
            _email = email;
            _link = link;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#RECOVERYCODE#", _recoveryCode);
            formatedStr = formatedStr.Replace("#LINK#", _link);
            return formatedStr;
        }
    }

    public class NewOrderMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnNewOrder; }
        }

        private readonly string _customerContacts;
        private readonly string _shippingMethod;
        private readonly string _paymentType;
        private readonly string _orderTable;
        private readonly string _currentCurrencyCode;
        private readonly string _totalPrice;
        private readonly string _comments;
        private readonly string _email;
        private readonly string _orderId;
        private readonly string _number;
        private readonly string _hash;

        public NewOrderMailTemplate(string orderId, string number, string email, string customerContacts,
                                    string shippingMethod, string paymentType, string orderTable,
                                    string currentCurrencyCode, string totalPrice, string comments, string hash)
        {
            _orderId = orderId;
            _number = number;
            _email = email;
            _customerContacts = customerContacts;
            _shippingMethod = shippingMethod;
            _paymentType = paymentType;
            _orderTable = orderTable;
            _currentCurrencyCode = currentCurrencyCode;
            _totalPrice = totalPrice;
            _comments = comments;
            _hash = hash;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDER_ID#", _orderId);
            formatedStr = formatedStr.Replace("#NUMBER#", _number);
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#CUSTOMERCONTACTS#", _customerContacts);
            formatedStr = formatedStr.Replace("#SHIPPINGMETHOD#", _shippingMethod);
            formatedStr = formatedStr.Replace("#PAYMENTTYPE#", _paymentType);
            formatedStr = formatedStr.Replace("#ORDERTABLE#", _orderTable);
            formatedStr = formatedStr.Replace("#CURRENTCURRENCYCODE#", _currentCurrencyCode);
            formatedStr = formatedStr.Replace("#TOTALPRICE#", _totalPrice);
            formatedStr = formatedStr.Replace("#COMMENTS#", _comments);
            formatedStr = formatedStr.Replace("#BILLING_LINK#",
                                                SettingsMain.SiteUrl.Trim('/') + "/billing.aspx?orderid=" + _orderId + "&hash=" + _hash);

            return formatedStr;
        }
    }

    public class OrderStatusMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnChangeOrderStatus; }
        }

        private readonly string _orderId;
        private readonly string _orderStatus;
        private readonly string _statusComment;
        private readonly string _number;
        private readonly string _orderTable;

        public OrderStatusMailTemplate(string orderId, string orderStatus, string statusComment, string number, string orderTable)
        {
            _orderId = orderId;
            _orderStatus = orderStatus;
            _statusComment = statusComment;
            _number = number;
            _orderTable = orderTable;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDERID#", _orderId);
            formatedStr = formatedStr.Replace("#ORDERSTATUS#", _orderStatus);
            formatedStr = formatedStr.Replace("#STATUSCOMMENT#", _statusComment);
            formatedStr = formatedStr.Replace("#NUMBER#", _number);
            formatedStr = formatedStr.Replace("#ORDERTABLE#", _orderTable);
            return formatedStr;
        }
    }
    
    public class CustomMessageMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendMessage; }
        }

        private readonly string _name;
        private readonly string _messageText;

        public CustomMessageMailTemplate(string name, string messageText)
        {
            _name = name;
            _messageText = messageText;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#MESSAGETEXT#", _messageText);
            formatedStr = formatedStr.Replace("#NAME#", _name);
            return formatedStr;
        }
    }
    
    public class FeedbackMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnFeedback; }
        }

        private readonly string _shopUrl;
        private readonly string _shopName;
        private readonly string _userName;
        private readonly string _userEmail;
        private readonly string _userPhone;
        private readonly string _subjectMessage;
        private readonly string _textMessage;

        public FeedbackMailTemplate(string shopUrl, string shopName, string userName, string userEmail,
                                    string userPhone, string subjectMessage, string textMessage)
        {
            _shopUrl = shopUrl;
            _shopName = shopName;
            _userName = userName;
            _userEmail = userEmail;
            _userPhone = userPhone;
            _subjectMessage = subjectMessage;
            _textMessage = textMessage;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#SHOPURL#", _shopUrl);
            formatedStr = formatedStr.Replace("#STORE_NAME#", _shopName);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#USEREMAIL#", _userEmail);
            formatedStr = formatedStr.Replace("#USERPHONE#", _userPhone);
            formatedStr = formatedStr.Replace("#SUBJECTMESSAGE#", _subjectMessage);
            formatedStr = formatedStr.Replace("#TEXTMESSAGE#", _textMessage);
            return formatedStr;
        }
    }

    public class ProductDiscussMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnProductDiscuss; }
        }

        private readonly string _sku;
        private readonly string _productName;
        private readonly string _productLink;
        private readonly string _author;
        private readonly string _date;
        private readonly string _text;
        private readonly string _deleteLink;
        private readonly string _email;

        public ProductDiscussMailTemplate(string sku, string productName, string productLink, string author, string date,
                                          string text, string deleteLink, string email)
        {
            _sku = sku;
            _productName = productName;
            _productLink = productLink;
            _author = author;
            _date = date;
            _text = text;
            _deleteLink = deleteLink;
            _email = email;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#PRODUCTLINK#", _productLink);
            formatedStr = formatedStr.Replace("#USERMAIL#", _email);
            formatedStr = formatedStr.Replace("#SKU#", _sku);
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#DATE#", _date);
            formatedStr = formatedStr.Replace("#DELETELINK#", _deleteLink);
            formatedStr = formatedStr.Replace("#TEXT#", _text);
            return formatedStr;
        }
    }

    public class QuestionAboutProductMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnQuestionAboutProduct; }
        }

        private readonly string _sku;
        private readonly string _productName;
        private readonly string _productLink;
        private readonly string _author;
        private readonly string _date;
        private readonly string _text;
        private readonly string _userMail;

        public QuestionAboutProductMailTemplate(string sku, string productName, string productLink, string author,
                                                string date, string text, string userMail)
        {
            _sku = sku;
            _productName = productName;
            _productLink = productLink;
            _author = author;
            _date = date;
            _text = text;
            _userMail = userMail;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#DATE#", _date);
            formatedStr = formatedStr.Replace("#TEXT#", _text);
            formatedStr = formatedStr.Replace("#USERMAIL#", _userMail);
            formatedStr = formatedStr.Replace("#SKU#", _sku);
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#PRODUCTLINK#", _productLink);
            return formatedStr;
        }
    }

    public class SendToFriendMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendFriend; }
        }

        private readonly string _to;
        private readonly string _from;
        private readonly string _url;

        public SendToFriendMailTemplate(string to,  string from, string url)
        {
            _to = to;
            _from = from;
            _url = url;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#FROM#", _from);
            formatedStr = formatedStr.Replace("#URL#", _url);
            formatedStr = formatedStr.Replace("#TO#", _to);
            return formatedStr;
        }
    }

    public class OrderByRequestMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnOrderByRequest; }
        }

        private readonly string _orderByRequestId;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _userName;
        private readonly string _email;
        private readonly string _phone;
        private readonly string _comment;

        private readonly string _color;
        private readonly string _size;

        public OrderByRequestMailTemplate(string orderByRequestId, string artNo, string productName, string quantity,
                                          string userName, string email, string phone, string comment, string color,
                                          string size)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _email = email;
            _phone = phone;
            _comment = comment;
            _color = color;
            _size = size;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDERID#", _orderByRequestId);
            formatedStr = formatedStr.Replace("#ARTNO#", _artNo);
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#QUANTITY#", _quantity);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#PHONE#", _phone);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);

            formatedStr = formatedStr.Replace("#COLOR#", _color);
            formatedStr = formatedStr.Replace("#SIZE#", _size);
            return formatedStr;
        }
    }

    public class LinkByRequestMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendLinkByRequest; }
        }

        private readonly string _orderByRequestId;
        private readonly string _userName;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _code;
        private readonly string _color;
        private readonly string _size;
        private readonly string _comment;

        public LinkByRequestMailTemplate(string orderByRequestId, string artNo, string productName, string quantity,
                                             string code, string userName, string comment, string color, string size)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _comment = comment;
            _color = color;
            _size = size;
            _code = code;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#NUMBER#", _orderByRequestId);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#ARTNO#", _artNo);
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#QUANTITY#", _quantity);
            formatedStr = formatedStr.Replace("#LINK#", SettingsMain.SiteUrl + "/orderproduct.aspx?code=" + _code);

            formatedStr = formatedStr.Replace("#COLOR#", _color);
            formatedStr = formatedStr.Replace("#SIZE#", _size);

            formatedStr = formatedStr.Replace("#COMMENT#", _comment);

            return formatedStr;
        }
    }

    public class FailureByRequestMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendFailureByRequest; }
        }

        private readonly string _orderByRequestId;
        private readonly string _userName;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;

        private readonly string _color;
        private readonly string _size;

        public FailureByRequestMailTemplate(string orderByRequestId, string artNo, string productName,
                                                string quantity, string userName, string color, string size)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _color = color;
            _size = size;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#NUMBER#", _orderByRequestId);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#ARTNO#", _artNo);
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#QUANTITY#", _quantity);

            formatedStr = formatedStr.Replace("#COLOR#", _color);
            formatedStr = formatedStr.Replace("#SIZE#", _size);

            return formatedStr;
        }
    }

    public class CertificateMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendGiftCertificate; }
        }

        private readonly string _certificateCode;
        private readonly string _fromName;
        private readonly string _toName;
        private readonly string _sum;
        private readonly string _message;

        public CertificateMailTemplate(string certificateCode, string fromName, string toName, string sum,
                                       string message)
        {
            _certificateCode = certificateCode;
            _fromName = fromName;
            _toName = toName;
            _sum = sum;
            _message = message;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#CODE#", _certificateCode);
            formatedStr = formatedStr.Replace("#FROMNAME#", _fromName);
            formatedStr = formatedStr.Replace("#TONAME#", _toName);
            formatedStr = formatedStr.Replace("#LINK#", SettingsMain.SiteUrl);
            formatedStr = formatedStr.Replace("#SUM#", _sum);
            formatedStr = formatedStr.Replace("#MESSAGE#", _message);

            return formatedStr;
        }
    }

    public class BuyInOneClickMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnBuyInOneClick; }
        }

        private readonly string _orderId;
        private readonly string _orderNumber;
        private readonly string _name;
        private readonly string _phone;
        private readonly string _email;
        private readonly string _comment;
        private readonly string _orderTable;

        public BuyInOneClickMailTemplate(string orderId, string name, string phone, string comment, string orderTable, string email="", string orderNumber="")
        {
            _orderId = orderId;
            _orderNumber = orderNumber;
            _name = name;
            _phone = phone;
            _email = email;
            _comment = comment;
            _orderTable = orderTable;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDER_ID#", _orderId);
            formatedStr = formatedStr.Replace("#NUMBER#", _orderNumber);
            formatedStr = formatedStr.Replace("#NAME#", _name);
            formatedStr = formatedStr.Replace("#COMMENTS#", _comment);
            formatedStr = formatedStr.Replace("#PHONE#", _phone);
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#ORDERTABLE#", _orderTable);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);

            return formatedStr;
        }
    }

    public class BillingLinkMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnBillingLink; }
        }

        private readonly string _orderId;
        private readonly string _firstName;
        private readonly string _hash;
        private readonly string _comment;
        private readonly string _orderTable;
        private readonly string _customerContacts;

        public BillingLinkMailTemplate(string orderId, string firstName, string customerContacts, string hash, string comment, string orderTable)
        {
            _orderId = orderId;
            _firstName = firstName;
            _hash = hash;
            _comment = comment;
            _orderTable = orderTable;
            _customerContacts = customerContacts;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDER_ID#", _orderId);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#COMMENTS#", _comment);
            formatedStr = formatedStr.Replace("#BILLING_LINK#", 
                                                SettingsMain.SiteUrl.Trim('/') + "/billing.aspx?orderid=" + _orderId + "&hash=" + _hash);
            formatedStr = formatedStr.Replace("#ORDERTABLE#", _orderTable);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#CUSTOMERCONTACTS#", _customerContacts);

            return formatedStr;
        }
    }
}