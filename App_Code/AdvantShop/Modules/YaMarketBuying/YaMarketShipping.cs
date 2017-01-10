using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvantShop.Modules
{
    public class YaMarketShipping
    {
        public int Id { get; set; }
        public int ShippingMethodId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int MinDate { get; set; }
        public int MaxDate { get; set; }
    }
}