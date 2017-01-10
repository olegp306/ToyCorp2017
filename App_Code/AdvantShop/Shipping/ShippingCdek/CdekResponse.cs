using System;
using System.Xml.Serialization;

namespace AdvantShop.Shipping.ShippingCdek
{
    [Serializable]
    [XmlRoot("response", IsNullable = false)]
    public class CdekXmlResponse
    {
        [XmlElement("CallCourier")]
        public CdekXmlResponseObject CallCourier;
    }

    [Serializable]
    public class CdekXmlResponseObject
    {
        [XmlAttribute("Date")]
        public DateTime Date { get; set; }
        [XmlAttribute("ErrorCode")]
        public string ErrorCode { get; set; }
        [XmlAttribute("Msg")]
        public string Msg { get; set; }
    }

}