using System;
using AdvantShop;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Security;
using Resources;
using AdvantShop.Controls;

namespace UserControls.OrderConfirmation
{
    public partial class StepLogin : System.Web.UI.UserControl
    {

        public class StepLoginNextEventArgs
        {
            public EnUserType UserType { get; set; }
        }

        public event Action<object, StepLoginNextEventArgs> NextStep;

        protected virtual void OnNextStep(StepLoginNextEventArgs e)
        {
            if (NextStep != null) NextStep(this, e);
        }

        protected void bntGoQReg_Click(object sender, EventArgs e)
        {
            OnNextStep(new StepLoginNextEventArgs { UserType = EnUserType.NewUserWithOutRegistration });
        }

        protected void btnGoWithReg_Click(object sender, EventArgs e)
        {
            OnNextStep(new StepLoginNextEventArgs { UserType = EnUserType.JustRegistredUser });
        }

        protected void btnAuthGO_Click(object sender, EventArgs e)
        {
            if (!ValidateFormData())
            {
                return;
            }

            if (AuthorizeService.SignIn(txtAuthLogin.Text, txtAuthPWD.Text.Trim(), false, true))
            {
                OnNextStep(new StepLoginNextEventArgs { UserType = EnUserType.RegisteredUser });
            }
            else
            {
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_MasterPage_WrongPassword);
                txtAuthPWD.Text = string.Empty; 
            }
        }

        protected bool ValidateFormData()
        {
            bool boolIsValidPast = true;

            if (txtAuthLogin.Text.Trim().IsNullOrEmpty())
            {
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_OrderConfirmation_EnterEmail);
                boolIsValidPast = false;
            }

            if (txtAuthPWD.Text.Trim().IsNullOrEmpty())
            {
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_OrderConfirmation_EnterPassword);
                boolIsValidPast = false;
            }
            return boolIsValidPast;
        }
    }
}