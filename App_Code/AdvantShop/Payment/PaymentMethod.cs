//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [Serializable]
    public class PaymentItem
    {
        public int PaymenMethodtId { get; set; }
        public string Name { get; set; }
        public ProcessType ProcessType { get; set; }
        public PaymentType Type { get; set; }
        public float Extracharge { get; set; }
        public ExtrachargeType ExtrachargeType { get; set; }

        public static implicit operator PaymentItem(PaymentMethod pm)
        {
            return new PaymentItem()
            {
                PaymenMethodtId = pm.PaymentMethodId,
                Name = pm.Name,
                ProcessType = pm.ProcessType,
                Type = pm.Type,
                Extracharge = pm.Extracharge,
                ExtrachargeType = pm.ExtrachargeType
            };
        }
    }

    [Serializable]
    public class PaymentMethod
    {
        public PaymentMethod()
        {
        }

        public PaymentMethod(PaymentType type)
        {
            Type = type;
        }

        public static PaymentMethod Create(PaymentType type)
        {
            var clas = type.GetClass();
            var retval = Activator.CreateInstance(clas);
            if (clas == typeof(PaymentMethod))
            {
                ((PaymentMethod)retval).Type = type;
            }
            return (PaymentMethod)retval;
        }

        public virtual PaymentType Type { get; private set; }

        public virtual ProcessType ProcessType
        {
            get { return ProcessType.None; }
        }

        public virtual NotificationType NotificationType
        {
            get { return NotificationType.None; }
        }

        public virtual UrlStatus ShowUrls { get { return UrlStatus.None; } }

        public virtual string SuccessUrl
        {
            get
            {
                return string.Format("http://{0}/paymentreturnurl/{1}", SettingsMain.SiteUrl.ToLower().Replace("http://", "").Replace("https://", ""), PaymentMethodId);
            }
        }
        public virtual string CancelUrl
        {
            get
            {
                return string.Format("http://{0}/PaymentCancelUrl.aspx", SettingsMain.SiteUrl.ToLower().Replace("http://", "").Replace("https://", ""));
            }
        }
        public virtual string FailUrl
        {
            get
            {
                return string.Format("http://{0}/PaymentFailUrl.aspx", SettingsMain.SiteUrl.ToLower().Replace("http://", "").Replace("https://", ""));
            }
        }
        public virtual string NotificationUrl
        {
            get
            {
                return string.Format("http://{0}/paymentnotification/{1}", SettingsMain.SiteUrl.ToLower().Replace("http://", "").Replace("https://", ""), PaymentMethodId);
            }
        }

        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }

        public ExtrachargeType ExtrachargeType { get; set; }
        public float Extracharge { get; set; }


        private Photo _picture;
        public Photo IconFileName
        {
            get { return _picture ?? (_picture = PhotoService.GetPhotoByObjId(PaymentMethodId, PhotoType.Payment)); }
            set { _picture = value; }
        }

        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        public virtual Dictionary<string, string> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public virtual void ProcessForm(Order order)
        {

        }

        public virtual string ProcessFormString(Order order, PaymentService.PageWithPaymentButton buttonSize)
        {
            return string.Empty;
        }

        public virtual string ProcessJavascript(Order order)
        {
            return string.Empty;
        }

        public virtual string ProcessJavascriptButton(Order order)
        {
            return string.Empty;
        }

        public virtual string ProcessServerRequest(Order order)
        {
            throw new NotImplementedException();
        }

        public virtual string ProcessResponse(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public static string GetOrderDescription(string orderNumber)
        {
            return string.Format(Resources.Resource.Payment_OrderDescription, orderNumber);
        }
    }
}