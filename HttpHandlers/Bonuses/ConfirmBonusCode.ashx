<%@ WebHandler Language="C#" Class="HttpHandlers.Bonuses.ConfirmBonusCode" %>

using System;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Repository;
using Newtonsoft.Json;
using Resources;

namespace HttpHandlers.Bonuses
{
    public class ConfirmBonusCode : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            Culture.InitializeCulture();
            context.Response.ContentType = "application/json";

            var code = context.Request["code"];
            var isCheckout = context.Request["isCheckout"] == "true";
            BonusCard bonusCard = null;
            
            if (code.IsNullOrEmpty() || context.Session["BonusSmsCode"] == null)
            {
                context.Response.Write(JsonConvert.SerializeObject(new {error = Resource.Client_Bonuses_WrongCode}));
                return;
            }

            var bonusSmsCode = context.Session["BonusSmsCode"].ToString().Split(new[] { "_" }, StringSplitOptions.None);
            
            if (bonusSmsCode[0] != code)
            {
                context.Response.Write(JsonConvert.SerializeObject(new {error = Resource.Client_Bonuses_WrongCode}));
                return;
            }

            if (bonusSmsCode.Length == 2)
            {
                var cardNumber = bonusSmsCode[1].TryParseLong();
                bonusCard = BonusSystemService.GetCard(cardNumber);

                UpdateCustomer(context, cardNumber, isCheckout);
            }

            if (context.Request["addcart"].IsNotEmpty())
            {
                bonusCard = BonusSystemService.GetCardByPhone(context.Request["phone"]);
                if (bonusCard != null)
                {
                    UpdateCustomer(context, bonusCard.CardNumber, isCheckout);
                }
                else
                {
                    var firstName = context.Request["firstName"];
                    var lastName = context.Request["lastName"].IsNotEmpty() ? context.Request["lastName"] : "-";
                    var secondName = context.Request["secondName"].IsNotEmpty() ? context.Request["secondName"] : "-";

                    var gender = context.Request["gender"] == "1";
                    var birthDay = context.Request["birthDay"];
                    var phone = context.Request["phone"];
                    var email = CustomerContext.CurrentCustomer.RegistredUser
                                    ? CustomerContext.CurrentCustomer.EMail
                                    : (context.Request["email"].IsNotEmpty() ? context.Request["email"] : null);
                    var city = context.Request["city"].IsNotEmpty() ? context.Request["city"].Trim() : IpZoneContext.CurrentZone.City;

                    if (!IsValidCardData(context, firstName, lastName, birthDay, phone))
                        return;

                    bonusCard = new BonusCard()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        SecondName = secondName,
                        Gender = gender,
                        CellPhone = phone,
                        Email = email,
                        CityName = city,
                        DateOfBirth = birthDay.TryParseDateTime()
                    };

                    var cardResponse = BonusSystemService.AddCard(bonusCard);
                    if (cardResponse != null && cardResponse.Status == 200 && cardResponse.Data.CardNumber != 0)
                    {
                        UpdateCustomer(context, cardResponse.Data.CardNumber, isCheckout);
                    }
                    else if (cardResponse != null && cardResponse.Message.IsNotEmpty())
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { error = cardResponse.Message }));
                        return;
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { error = Resource.Client_Bonuses_CheckData }));
                        return;
                    }
                }
            }

            context.Session["BonusSmsCode"] = null;
            context.Response.Write(JsonConvert.SerializeObject(new
            {
                success = "true",
                error = "",
                bonusText = bonusCard != null
                                ? string.Format("{0} ({1} {2} {3})", Resource.Client_StepBonus_ByBonusCard,
                                    Resource.Client_StepBonus_YourBonuses, bonusCard.BonusAmount,
                                    Strings.Numerals(bonusCard.BonusAmount,
                                        Resource.Client_StepBonus_Empty, Resource.Client_StepBonus_1Bonus,
                                        Resource.Client_StepBonus_2Bonus, Resource.Client_StepBonus_5Bonus))
                                : string.Empty
            }));
        }

        private void UpdateCustomer(HttpContext context, long cardNumber, bool isCheckout)
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.RegistredUser)
            {
                customer.BonusCardNumber = cardNumber;
                CustomerService.UpdateCustomer(customer);
            }
            else if (!isCheckout)
            {
                context.Session["bonuscard"] = cardNumber;
            }
            
            if (isCheckout)
            {
                var pageData = OrderConfirmationService.Get(customer.Id);
                if (pageData != null)
                {
                    pageData.OrderConfirmationData.Customer.BonusCardNumber = cardNumber;
                    OrderConfirmationService.Update(pageData);
                }
            }
        }

        private bool IsValidCardData(HttpContext context, string firstName, string lastName, string birthDay, string phone)
        {
            var isValid = firstName.IsNotEmpty() && lastName.IsNotEmpty() && phone.IsNotEmpty() && birthDay.IsNotEmpty();

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

            DateTime dt;
            if (!DateTime.TryParse(birthDay, out dt))
            {
                context.Response.Write(JsonConvert.SerializeObject(new { error = Resource.Client_Bonuses_ErrorBirthDay }));
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