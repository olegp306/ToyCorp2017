//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Xml.Serialization;

namespace AdvantShop.Shipping.ShippingCdek
{
    [Serializable]
    [XmlRoot("StatusReport", IsNullable = false)]
    public class CdekOrderStatusInfo
    {
        [XmlAttribute("DateFirst")]
        public DateTime DateFirst { get; set; }

        [XmlAttribute("DateLast")]
        public DateTime DateLast { get; set; }

        [XmlElement("Order", Type = typeof(CdekStatusInfoOrder), IsNullable = true)]
        public CdekStatusInfoOrder[] Orders { get; set; }

        [XmlElement("ErrorCode", Type = typeof(string), IsNullable = true)]
        public string ErrorCode { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrder
    {
        [XmlAttribute("ActNumber")]
        public string ActNumber { get; set; }

        [XmlAttribute("Number")]
        public string Number { get; set; }

        [XmlAttribute("DispatchNumber")]
        public string DispatchNumber { get; set; }

        [XmlIgnore]
        private DateTime deliveryDate { get; set; }

        [XmlAttribute("DeliveryDate")]
        public string DeliveryDate
        {
            get { return deliveryDate == DateTime.MinValue ? string.Empty : deliveryDate.ToString(); }
            set { if (!value.Equals("")) deliveryDate = DateTime.Parse(value); }
        }

        [XmlAttribute("RecipientName")]
        public string RecipientName { get; set; }

        [XmlAttribute("ReturnDispatchNumber")]
        public int ReturnDispatchNumber { get; set; }

        [XmlElement("Status", Type = typeof(CdekStatusInfoOrderStatus))]
        public CdekStatusInfoOrderStatus Status { get; set; }

        [XmlElement("Reason", Type = typeof(CdekStatusInfoOrderReason))]
        public CdekStatusInfoOrderReason Reason { get; set; }

        [XmlElement("DelayReason", Type = typeof(CdekStatusInfoOrderDelayReason))]
        public CdekStatusInfoOrderDelayReason DelayReason { get; set; }

        [XmlElement("Call", Type = typeof(CdekStatusInfoOrderCall))]
        public CdekStatusInfoOrderCall Call { get; set; }
         
    }

    [Serializable]
    public class CdekStatusInfoOrderStatus
    {
        [XmlIgnore]
        private DateTime date { get; set; }
        [XmlAttribute("Date")]
        public string Date
        {
            get { return date == DateTime.MinValue ? string.Empty : date.ToString(); }
            set { if (!value.Equals("")) date = DateTime.Parse(value); }
        }

        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("CityCode")]
        public int CityCode { get; set; }

        [XmlAttribute("CityName")]
        public string CityName { get; set; }

        [XmlElement("State", Type = typeof(CdekStatusInfoOrderStatusState))]
        public CdekStatusInfoOrderStatusState[] State { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderStatusState
    {
        [XmlIgnore]
        private DateTime date { get; set; }
        [XmlAttribute("Date")]
        public string Date
        {
            get { return date == DateTime.MinValue ? string.Empty : date.ToString(); }
            set { if (!value.Equals("")) date = DateTime.Parse(value); }
        }

        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("CityCode")]
        public int CityCode { get; set; }

        [XmlAttribute("CityName")]
        public string CityName { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderReason
    {
        [XmlIgnore]
        private DateTime date { get; set; }
        [XmlAttribute("Date")]
        public string Date
        {
            get { return date == DateTime.MinValue ? string.Empty : date.ToString(); }
            set { if (!value.Equals("")) date = DateTime.Parse(value); }
        }

        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderDelayReason
    {
        [XmlIgnore]
        private DateTime date { get; set; }
        [XmlAttribute("Date")]
        public string Date
        {
            get { return date == DateTime.MinValue ? string.Empty : date.ToString(); }
            set { if (!value.Equals("")) date = DateTime.Parse(value); }
        }

        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderCall
    {
        [XmlElement("CallGood", Type = typeof(CdekStatusInfoOrderCallGoodGood))]
        public CdekStatusInfoOrderCallGoodGood CallGood { get; set; }

        [XmlElement("CallFail", Type = typeof(CdekStatusInfoOrderCallFailFail))]
        public CdekStatusInfoOrderCallFailFail CallFail { get; set; }

        [XmlElement("CallDelay", Type = typeof(CdekStatusInfoOrderCallDelayDelay))]
        public CdekStatusInfoOrderCallDelayDelay CallDelay { get; set; }

    }

    [Serializable]
    public class CdekStatusInfoOrderCallGoodGood
    {
        [XmlElement("Good", Type = typeof(CdekStatusInfoOrderCallGood))]
        public CdekStatusInfoOrderCallGood[] Good { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderCallFailFail
    {
        [XmlElement("Fail", Type = typeof(CdekStatusInfoOrderCallFail))]
        public CdekStatusInfoOrderCallFail[] Fail { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderCallDelayDelay
    {
        [XmlElement("Delay", Type = typeof(CdekStatusInfoOrderCallDelay))]
        public CdekStatusInfoOrderCallDelay[] Delay { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderCallGood
    {
        [XmlIgnore]
        private DateTime date { get; set; }
        [XmlAttribute("Date")]
        public string Date
        {
            get { return date == DateTime.MinValue ? string.Empty : date.ToString(); }
            set { if (!value.Equals("")) date = DateTime.Parse(value); }
        }
        
        [XmlIgnore]
        private DateTime dateDeliv { get; set; }
        [XmlAttribute("DateDeliv")]
        public string DateDeliv
        {
            get { return dateDeliv == DateTime.MinValue ? string.Empty : dateDeliv.ToString(); }
            set { if (!value.Equals("")) dateDeliv = DateTime.Parse(value); }
        }
    }

    [Serializable]
    public class CdekStatusInfoOrderCallFail
    {
        [XmlIgnore]
        private DateTime date { get; set; }
        [XmlAttribute("Date")]
        public string Date
        {
            get { return date == DateTime.MinValue ? string.Empty : date.ToString(); }
            set { if (!value.Equals("")) date = DateTime.Parse(value); }
        }

        [XmlAttribute("ReasonCode")]
        public string ReasonCode { get; set; }

        [XmlAttribute("ReasonDescription")]
        public string ReasonDescription { get; set; }
    }

    [Serializable]
    public class CdekStatusInfoOrderCallDelay
    {
        [XmlIgnore]
        private DateTime date { get; set; }
        [XmlAttribute("Date")]
        public string Date
        {
            get { return date == DateTime.MinValue ? string.Empty : date.ToString(); }
            set { if (!value.Equals("")) date = DateTime.Parse(value); }
        }

        [XmlIgnore]
        private DateTime dateNext { get; set; }
        [XmlAttribute("DateNext")]
        public string DateNext
        {
            get { return dateNext == DateTime.MinValue ? string.Empty : dateNext.ToString(); }
            set { if (!value.Equals("")) dateNext = DateTime.Parse(value); }
        }
    }
}