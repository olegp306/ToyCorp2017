//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Xml.Serialization;

namespace AdvantShop.Shipping.ShippingCdek
{
    [Serializable]
    [XmlRoot("InfoReport", IsNullable = false)]
    public class CdekOrderInfoReport
    {
        [XmlElement("Order", Type = typeof(CdekOrderInfoReportOrder))]
        public CdekOrderInfoReportOrder[] Orders { get; set; }
    }

    [Serializable]
    public class CdekOrderInfoReportOrder
    {
        [XmlAttribute("Number")]
        public string Number { get; set; }

        [XmlAttribute("Date")]
        public string Date { get; set; }

        [XmlAttribute("DispatchNumber")]
        public int DispatchNumber { get; set; }

        [XmlAttribute("TariffTypeCode")]
        public int TariffTypeCode { get; set; }

        [XmlAttribute("Weight")]
        public float Weight { get; set; }

        [XmlAttribute("DeliverySum")]
        public float DeliverySum { get; set; }

        [XmlIgnore]
        private DateTime dateLastChange { get; set; }

        [XmlAttribute("DateLastChange")]
        public string DateLastChange
        {
            get { return dateLastChange == DateTime.MinValue ? string.Empty : dateLastChange.ToString(); }
            set { if (!value.Equals("")) dateLastChange = DateTime.Parse(value); }
        }
        
        [XmlElement("SendCity", Type = typeof(CdekOrderInfoReportSendCity))]
        public CdekOrderInfoReportSendCity SendCity { get; set; }

        [XmlElement("RecCity", Type = typeof(CdekOrderInfoReportRecCity))]
        public CdekOrderInfoReportRecCity RecCity { get; set; }

        [XmlElement("AddedService", Type = typeof(CdekOrderInfoReportAddedService))]
        public CdekOrderInfoReportAddedService AddedService { get; set; }
    }

    [Serializable]
    public class CdekOrderInfoReportSendCity
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("PostCode")]
        public string PostCode { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    [Serializable]
    public class CdekOrderInfoReportRecCity
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("PostCode")]
        public string PostCode { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    [Serializable]
    public class CdekOrderInfoReportAddedService
    {
        [XmlAttribute("ServiceCode")]
        public int ServiceCode { get; set; }

        [XmlAttribute("Sum")]
        public float Sum { get; set; }
    }
}