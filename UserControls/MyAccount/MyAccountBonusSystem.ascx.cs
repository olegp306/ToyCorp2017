using System;
using AdvantShop.BonusSystem;
using AdvantShop.Customers;
using Resources;

namespace UserControls.MyAccount
{
    public partial class MyAccountBonusSystem : System.Web.UI.UserControl
    {
        protected BonusCard Card;
        private readonly Customer _customer = CustomerContext.CurrentCustomer;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (_customer.BonusCardNumber != null && (Card = BonusSystemService.GetCard(_customer.BonusCardNumber)) != null)
            {
                txtBonusFirstName.Text = Card.FirstName;
                txtBonusLastName.Text = Card.LastName;
                txtBonusSecondName.Text = Card.SecondName;
                txtBonusDate.Text = Card.DateOfBirth != null
                                        ? ((DateTime) Card.DateOfBirth).ToString("dd.MM.yyyy")
                                        : string.Empty;
                txtBonusDate.Disabled = true;

                txtBonusPhone.Text = Card.CellPhone;
                genderMale.Checked = !Card.Gender;
                genderFemale.Checked = Card.Gender;

                btnMaAddBonusCard.Text = Resource.Client_Bonuses_SaveChanges;
                btnMaAddBonusCard.CssClass = "ma-changecard";
            }
            else
            {
                txtBonusFirstName.Text = _customer.FirstName;
                txtBonusLastName.Text = _customer.LastName;
                txtBonusSecondName.Text = _customer.Patronymic;
                btnMaAddBonusCard.Text = Resource.Client_Bonuses_AddCard;
                btnMaAddBonusCard.CssClass = "ma-addcard";
                txtBonusDate.CssClass += " mask-inp";
                txtBonusPhone.CssClass += " mask-inp";
            }
        }
    }
}