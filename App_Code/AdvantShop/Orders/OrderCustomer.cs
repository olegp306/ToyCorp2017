//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Orders
{
    [Serializable]
    public class OrderCustomer : IOrderCustomer
    {
        public int OrderID { get; set; }
        public Guid CustomerID { get; set; }
        public string CustomerIP { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
    }
}