using System.Collections.Generic;
using System;

namespace AdvantShop.Payment
{
    public class BillUa : PaymentMethod
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyEssentials { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Credit { get; set; }
        public float CurrencyValue { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.BillUa; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.Javascript; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {BillUaTemplate.CompanyName, CompanyName},
                               {BillUaTemplate.CompanyCode, CompanyCode},
                               {BillUaTemplate.CompanyEssentials, CompanyEssentials},
                               {BillUaTemplate.BankName, BankName},
                               {BillUaTemplate.BankCode, BankCode},
                               {BillUaTemplate.Credit, Credit},
                               {BillUaTemplate.CurrencyValue, CurrencyValue.ToString()}
                           };
            }
            set
            {
                CompanyName = value.ElementOrDefault(BillUaTemplate.CompanyName);
                CompanyCode = value.ElementOrDefault(BillUaTemplate.CompanyCode);
                CompanyEssentials = value.ElementOrDefault(BillUaTemplate.CompanyEssentials);
                BankName = value.ElementOrDefault(BillUaTemplate.BankName);
                BankCode = value.ElementOrDefault(BillUaTemplate.BankCode);
                Credit = value.ElementOrDefault(BillUaTemplate.Credit);
                float decVal;
                CurrencyValue = value.ContainsKey(BillUaTemplate.CurrencyValue) &&
                                float.TryParse(value[BillUaTemplate.CurrencyValue], out decVal)
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