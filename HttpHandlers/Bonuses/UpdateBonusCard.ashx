<%@ WebHandler Language="C#" Class="HttpHandlers.Bonuses.UpdateBonusCard" %>

using System;
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
    public class UpdateBonusCard : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            Culture.InitializeCulture();
            context.Response.ContentType = "application/json";
            
            var customer = CustomerContext.CurrentCustomer;

            if (!customer.RegistredUser || customer.BonusCardNumber == null)
            {
                return;
            }

            var firstName = context.Request["firstName"];
            var lastName = context.Request["lastName"].IsNotEmpty() ? context.Request["lastName"] : "-";
            var secondName = context.Request["secondName"].IsNotEmpty() ? context.Request["secondName"] : "-";

            var gender = context.Request["gender"] == "1";
            var phone = context.Request["phone"];

            if (!IsValidCardData(context, firstName, lastName, phone))
                return;

            var card = BonusSystemService.GetCard(customer.BonusCardNumber);
            card.Email = customer.EMail;
            card.FirstName = firstName;
            card.LastName = lastName;
            card.SecondName = secondName;
            card.Gender = gender;
            card.CellPhone = phone;

            var cardResponse = BonusSystemService.UpdateCard(card);
            if (cardResponse != null && cardResponse.Status == 200 && cardResponse.Data.CardNumber != 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(new { error = "" }));
            }
            else if (cardResponse != null && cardResponse.Message.IsNotEmpty())
            {
                context.Response.Write(JsonConvert.SerializeObject(new { error = cardResponse.Message }));
            }
            else
            {
                context.Response.Write(JsonConvert.SerializeObject(new { error = "Проверьте данные" }));
            }
        }

        private bool IsValidCardData(HttpContext context, string firstName, string lastName, string phone)
        {
            var isValid = firstName.IsNotEmpty() && lastName.IsNotEmpty() && phone.IsNotEmpty();

            if (!isValid)
            {
                context.Response.Write(JsonConvert.SerializeObject(new { error = Resource.Client_Bonuses_ErrorRequired }));
                return false;
            }

            if (phone.TryParseDecimal() == 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(new { error = Resource.Client_Bonuses_ErrorPhone }));
                return false;
            }

            return true;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}