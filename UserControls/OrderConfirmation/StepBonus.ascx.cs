using System;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Orders;
using Resources;

namespace UserControls.OrderConfirmation
{
    public partial class StepBonus : System.Web.UI.UserControl
    {
        #region Fields

        public OrderConfirmationData PageData { get; set; }

        protected BonusCard Card;

        #endregion
        
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (PageData == null)
                return;

            if (!BonusSystem.IsActive)
            {
                this.Visible = false;
                return;
            }

            var customer = CustomerContext.CurrentCustomer;
            var cardNumber = customer.RegistredUser ? customer.BonusCardNumber : PageData.Customer.BonusCardNumber;

            Card = BonusSystemService.GetCard(cardNumber);
            if (Card != null)
            {
                liBonusAmount.Text = string.Format("({0} {1} {2})",
                    Resource.Client_StepBonus_YourBonuses,
                    Card.BonusAmount,
                    Strings.Numerals(Card.BonusAmount, Resource.Client_StepBonus_Empty,
                        Resource.Client_StepBonus_1Bonus,
                        Resource.Client_StepBonus_2Bonus,
                        Resource.Client_StepBonus_5Bonus));

                chkBonus.Checked = PageData.UseBonuses;
            }
            
            if (customer.RegistredUser)
            {
                txtBonusLastName.Text = customer.LastName;
                txtBonusFirstName.Text = customer.FirstName;
                txtBonusSecondName.Text = customer.Patronymic;
                if (customer.Phone.IsNotEmpty())
                {
                    txtBonusPhone.Text = customer.Phone;
                }
            }

            if (BonusSystem.BonusesForNewCard != 0)
            {
                liBonusesForNewCard.Text = string.Format(Resource.Client_StepBonus_NewCardBonuses,
                    CatalogService.GetStringPrice(BonusSystem.BonusesForNewCard));
                liBonusesForNewCard.Visible = true;
            }
        }
    }
}