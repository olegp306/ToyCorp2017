//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for SberBank
    /// </summary>
    public class SberBank : PaymentMethod
    {

        public float CurrencyValue { get; set; }
        public string CompanyName { get; set; }
        public string TransAccount { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string BankName { get; set; }
        public string CorAccount { get; set; }
        public string BIK { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.SberBank; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.Javascript; }
        }
        public override Dictionary<string,string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {SberBankTemplate.BankName, BankName},
                               {SberBankTemplate.CompanyName, CompanyName},
                               {SberBankTemplate.TransAccount, TransAccount},
                               {SberBankTemplate.INN, INN},
                               {SberBankTemplate.KPP, KPP},
                               {SberBankTemplate.CorAccount, CorAccount},
                               {SberBankTemplate.BIK, BIK},
                               {SberBankTemplate.CurrencyValue, CurrencyValue.ToString()}
                           };
            }
            set 
            {
                BankName = value.ElementOrDefault(SberBankTemplate.BankName);
                CompanyName = value.ElementOrDefault(SberBankTemplate.CompanyName);
                TransAccount = value.ElementOrDefault(SberBankTemplate.TransAccount);
                INN = value.ElementOrDefault(SberBankTemplate.INN);
                KPP = value.ElementOrDefault(SberBankTemplate.KPP);
                BIK = value.ElementOrDefault(SberBankTemplate.BIK);
                CorAccount = value.ElementOrDefault(SberBankTemplate.CorAccount);
                float decVal;
                CurrencyValue = value.ContainsKey(SberBankTemplate.CurrencyValue) &&
                                float.TryParse(value[SberBankTemplate.CurrencyValue], out decVal)
                                    ? decVal
                                    : 1;
            }
        }

        public override string ProcessJavascriptButton(Orders.Order order)
        {
            return String.Format("javascript:open_printable_version('Check_{0}.aspx?ordernumber={1}&methodid={2}');", Type.ToString(), order.Number, PaymentMethodId);
        }
    }
}