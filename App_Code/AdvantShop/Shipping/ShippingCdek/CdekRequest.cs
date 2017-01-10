//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Shipping
{
    /// <summary>
    /// 
    /// </summary>
    public class CdekGoods
    {
        public string weight { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CdekResponse
    {
        public CdekAnswerResult result { get; set; }
        public List<CdekAnswerError> error { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CdekAnswerError
    {
        public int code { get; set; }
        public string text { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CdekAnswerResult
    {
        public float price { get; set; }
        public int deliveryPeriodMin { get; set; }
        public int deliveryPeriodMax { get; set; }
        public string deliveryDateMin { get; set; }
        public string deliveryDateMax { get; set; }
        public int tariffId { get; set; }
        public float cashOnDelivery { get; set; }
        public float priceByCurrency { get; set; }
        public string currency { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CdekTariff
    {
        public int tariffId { get; set; }
        public string name { get; set; }
        public string mode { get; set; }
        public bool active { get; set; }
    }
}