//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Shipping
{
    public struct MultishipTemplate
    {
        public const string CityFrom = "CityFrom";
        public const string ClientId = "ClientId";
        public const string SecretKeyDelivery = "SecretKeyDelivery";
        public const string SecretKeyCreateOrder = "SecretKeyCreateOrder";
        public const string IsActive = "IsActive";

        public const string WeightAvg = "WeightAvg";
        public const string HeightAvg = "HeightAvg";
        public const string WidthAvg = "WidthAvg";
        public const string LengthAvg = "LengthAvg";

        public const string SenderId = "SenderId";
        public const string RequisiteId = "RequisiteId";
        public const string WarehouseId = "WarehouseId";
        public const string WidgetCode = "WidgetCode";

        public const string MSobject = "MSobject";


        public class MultishipAnswer
        {
            public string status { get; set; }
            public List<MultishipData> data { get; set; }
        }

        public class MultishipData
        {
            /// <summary>
            /// Id прайс-листа
            /// </summary>
            public string price_id { get; set; }

            /// <summary>
            /// Id службы доставки
            /// </summary>
            public string delivery_id { get; set; }

            /// <summary>
            /// id ПВЗ (если null - доставка курьером)
            /// </summary>
            public string pickuppoint_id { get; set; }

            /// <summary>
            /// Адрес ПВЗ
            /// </summary>
            public string address { get; set; }

            /// <summary>
            /// Срок доставки. может быть число, может быть строка (1-3)
            /// </summary>
            public string days { get; set; }

            ///// <summary>
            ///// Базовая стоимость доставки
            ///// </summary>
            //public float cost { get; set; }

            /// <summary>
            /// Название службы доставки
            /// </summary>
            public string delivery_name { get; set; }

            /// <summary>
            /// Итоговая стоимость доставки для покупателя
            /// </summary>
            public float cost_with_rules { get; set; }
        }
    }

    /// <summary>
    /// Init error
    /// </summary>
    public class MultishipInitAnswerError
    {
        public string status { get; set; }
        public string error { get; set; }
    }

    /// <summary>
    /// Init answer
    /// </summary>
    public class MultishipInitAnswer
    {
        public MultishipConfig config { get; set; }
        public string moduleInstallId { get; set; }
        public Dictionary<string, string> widgetCode { get; set; }
        public string callbackKey { get; set; }
    }

    public class MultishipInitError
    {
        public string status { get; set; }
        public string error { get; set; }
    }

    /// <summary>
    /// Multiship Config
    /// </summary>
    public class MultishipConfig
    {
        public string clientId { get; set; }
        public List<string> senders { get; set; }
        public List<string> warehouses { get; set; }
        public List<string> requisites { get; set; }
        public Dictionary<string, string> methodKeys { get; set; }
    }

    public class MultishipAdditionalData
    {
        public int direction { get; set; }
        public int delivery { get; set; }
        public int price { get; set; }
        public int to_ms_warehouse { get; set; }
    }
}