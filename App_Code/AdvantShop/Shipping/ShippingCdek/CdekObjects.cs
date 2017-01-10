//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Shipping.ShippingCdek
{
    /// <summary>
    /// for data exchange 
    /// </summary>
    public class CdekStatusAnswer
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }
    }

    /// <summary>
    /// for added service constant list
    /// </summary>
    public class AddedService
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// pickpoint object
    /// </summary>
    public class CdekPvz
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public string City { get; set; }
        public string WorkTime { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public CdekPvzWeightLimit WeightLimit { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CdekPvzWeightLimit
    {
        public string WeightMin { get; set; }
        public string WeightMax { get; set; }
    }
}