<%@ WebHandler Language="C#" Class="HttpHandlers.Bonuses.ConfirmCardByNumber" %>

using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Customers;
using AdvantShop.Localization;
using Newtonsoft.Json;
using Resources;

namespace HttpHandlers.Bonuses
{
    public class ConfirmCardByNumber : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            Culture.InitializeCulture();
            context.Response.ContentType = "application/json";
            
            // Request["cardnumber"] is card number or phone
            var cardNumber = context.Request["cardnumber"].TryParseLong();
            if (cardNumber == 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(new {error = Resource.Client_Bonuses_CardNotExist}));
                return;
            }

            var customer = CustomerContext.CurrentCustomer;
            if (customer.RegistredUser && customer.BonusCardNumber != null && BonusSystemService.GetCard(customer.BonusCardNumber) != null)
            {
                context.Response.Write(JsonConvert.SerializeObject(new {error = Resource.Client_Bonuses_CardNotExist}));
                return;
            }

            GetSmsCodeByCardNumber(context, cardNumber);
        }

        private void GetSmsCodeByCardNumber(HttpContext context, long cardNumber)
        {
            var bonusCodeResponse = BonusSystemService.GetSmsCode(cardNumber);
            if (bonusCodeResponse != null && bonusCodeResponse.Status == 200)
            {
                context.Session["BonusSmsCode"] = bonusCodeResponse.Data.Code + "_" + cardNumber;
                context.Response.Write(JsonConvert.SerializeObject(new {error = ""}));
                return;
            }
            
            GetSmsCodeByPhone(context, cardNumber.ToString());
        }

        private void GetSmsCodeByPhone(HttpContext context, string phone)
        {
            var cardByPhone = BonusSystemService.GetCardByPhone(phone);
            if (cardByPhone != null)
            {
                var bonusCodeByPhoneResponse = BonusSystemService.GetSmsCode(phone);
                if (bonusCodeByPhoneResponse != null && bonusCodeByPhoneResponse.Status == 200)
                {
                    context.Session["BonusSmsCode"] = bonusCodeByPhoneResponse.Data.Code + "_" + cardByPhone.CardNumber;
                    context.Response.Write(JsonConvert.SerializeObject(new {error = ""}));
                    return;
                }
            }

            context.Response.Write(JsonConvert.SerializeObject(new { error = Resource.Client_Bonuses_CardNotFound }));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
