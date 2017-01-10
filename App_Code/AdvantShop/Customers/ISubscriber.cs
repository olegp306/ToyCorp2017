//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Customers
{
    public interface ISubscriber 
    {
        string Email { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string Phone { get; set; }
    }
}