<%@ WebHandler Language="C#" Class="HttpHandlers.Bonuses.ConfirmCardByPhone" %>

using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Localization;
using Newtonsoft.Json;
using Resources;

namespace HttpHandlers.Bonuses
{
    public class ConfirmCardByPhone : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            Culture.InitializeCulture();
            context.Response.ContentType = "application/json";
            
            var phone = context.Request["phone"];
            if (phone.IsNullOrEmpty())
            {
                context.Response.Write(JsonConvert.SerializeObject(new {error = Resource.Client_Bonuses_WrongPhoneNumber}));
                return;
            }

            var customer = CustomerContext.CurrentCustomer;
            if (customer.RegistredUser && customer.BonusCardNumber != null && BonusSystemService.GetCard(customer.BonusCardNumber) != null)
            {
                context.Response.Write(JsonConvert.SerializeObject(new {error = Resource.Client_Bonuses_PhoneNumberExists}));
                return;
            }

            // skipCheck in registration page
            if (context.Request["skipCheck"] == null || context.Request["skipCheck"] != "true")
            {
                var isExistPhone = BonusSystemService.IsPhoneExist(phone);
                if (isExistPhone)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { error = "phone_exist" }));
                    return;
                }
            }

            var bonusCodeResponse = BonusSystemService.GetSmsCode(phone);
            if (bonusCodeResponse != null && bonusCodeResponse.Status == 200)
            {
                context.Session["BonusSmsCode"] = bonusCodeResponse.Data.Code;
                context.Response.Write(JsonConvert.SerializeObject(new {error = ""}));
            }
            else if (bonusCodeResponse != null)
            {
                Debug.LogError(string.Format("ConfirmNewCard Error. msg:{0}, phone: {1}", bonusCodeResponse.Message, phone));
                context.Response.Write(JsonConvert.SerializeObject(new { error = bonusCodeResponse.Message }));
            }
            else
            {
                context.Response.Write(JsonConvert.SerializeObject(new { error = Resource.Client_Bonuses_WrongPhoneNumber }));
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
