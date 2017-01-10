//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Orders
{
    #region  enum

    public enum EnActiveTab
    {
        NoTab = 0,
        DefaultTab = 1,
        UserTab = 2,
        ShippingTab = 3,
        PaymentTab = 4,
        SumTab = 5,
        FinalTab = 6
    }

    public enum EnUserType
    {
        /// <summary>
        /// User without registration
        /// </summary>
        NoUser,

        /// <summary>
        /// User without registration
        /// </summary>
        NewUserWithOutRegistration,

        /// <summary>
        /// User registration on checkout
        /// </summary>
        JustRegistredUser,

        /// <summary>
        /// Registered user
        /// </summary>
        RegisteredUser,

        /// <summary>
        /// Registered user without shipping contact
        /// </summary>
        RegisteredUserWithoutAddress
    }

    #endregion

    public class OrderConfirmationData
    {
        public EnUserType UserType { get; set; }
        public EnActiveTab ActiveTab { get; set; }

        public PaymentItem SelectedPaymentItem { get; set; }
        public ShippingItem SelectedShippingItem { get; set; }

        public bool UseBonuses { get; set; }

        public PaymentDetails PaymentDetails { get; set; }
        public int Distance { get; set; }

        public int CheckSum { get; set; }
        public bool BillingIsShipping { get; set; }
        public CustomerContact ShippingContact { get; set; }

        private CustomerContact _billingContact;
        public CustomerContact BillingContact
        {
            get { return BillingIsShipping ? ShippingContact : _billingContact; }
            set { _billingContact = value; }
        }

        public Customer Customer { get; set; }
        public float TaxesTotal { get; set; }

        public string CustomerComment { get; set; }
        
        public float Sum { get; set; }
        public float BonusPlus { get; set; }

        public OrderConfirmationData()
        {
            SelectedShippingItem = new ShippingItem();
            SelectedPaymentItem = new PaymentItem();
        }
    }

    public class OrderConfirmation
    {
        public Guid CustomerId { get; set; }
        public OrderConfirmationData OrderConfirmationData { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}