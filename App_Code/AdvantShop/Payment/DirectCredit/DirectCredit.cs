using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Orders;
using System.Text;
using Newtonsoft.Json;
using AdvantShop.Taxes;
using AdvantShop.Catalog;

namespace AdvantShop.Payment
{
    public class DirectCredit : PaymentMethod, ICreditPaymentMethod
    {
        /*
         * ����������:
         * ������ �� ����� ����� 3000 ���. �� ���������������
         * 
         * �������� ������:
         * Id ��������: 1-178YO4Z
         * API key: 123qwe
         * API secret ($salt ��� "����" ������� ���������): 321ewq
         * ��� ��� ������������� - 1010
         * 
         */

        private const int MinOrderPrice = 1500;
        private const int DefFirstPayment = 25;

        public string PartnerId { get; set; }

        public float MinimumPrice { get; set; }
        public float FirstPayment { get; set; }
        
        public override PaymentType Type
        {
            get { return PaymentType.DirectCredit; }
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
                               {DirectCreditTemplate.PartnerId, PartnerId},
                               {DirectCreditTemplate.MinimumPrice, MinimumPrice.ToString()},
                               {DirectCreditTemplate.FirstPayment, FirstPayment.ToString()}
                              
                           };
            }
            set
            {
                PartnerId = value.ElementOrDefault(DirectCreditTemplate.PartnerId);
                MinimumPrice = value.ElementOrDefault(DirectCreditTemplate.MinimumPrice) != null ? value.ElementOrDefault(DirectCreditTemplate.MinimumPrice).TryParseFloat() : MinOrderPrice;
                FirstPayment = value.ElementOrDefault(DirectCreditTemplate.FirstPayment) != null ? value.ElementOrDefault(DirectCreditTemplate.FirstPayment).TryParseFloat() : DefFirstPayment;
            }
        }

        public override string ProcessJavascript(Order order)
        {
            
            string JSONObjs = GetOrderJson(order, PartnerId);
            
            var sb = new StringBuilder();

            sb.Append("<script type='text/javascript'>");
            sb.Append("(function () {"+
                                "var po = document.createElement('script'); po.type = 'text/javascript'; po.async = false; po.charset='windows-1251';"+
                                "po.src = 'https://api.direct-credit.ru/JsHttpRequest.js';"+
                                "var s = document.getElementsByTagName('head')[0]; s.appendChild(po, s);"+
                                "})();"+

                                "(function () {" +
                                "var po = document.createElement('script'); po.type = 'text/javascript'; po.async = false; po.charset='windows-1251';" +
                                "po.src = 'https://api.direct-credit.ru/script.js';" +
                                "var s = document.getElementsByTagName('head')[0]; s.appendChild(po, s);" +
                                "})();"+
                                "(function () {"+
                                "var po = document.createElement('link');"+
                                "po.rel = 'stylesheet';"+
                                "po.type = 'text/css';"+
                                "po.async = false;"+
                                "po.href = 'https://api.direct-credit.ru/style.css';"+
                                "var s = document.getElementsByTagName('head')[0]; s.appendChild(po, s);"+
                                "})();"
            );

            sb.Append("function DirectCred(){"+
                                
                                "var debug = false;"+
                                "var JSONitems = "+ JSONObjs  +";"+
                                "var arrProducts = [];" +
                                "var results = JSONitems.items;" +
                                "for (var i = 0, len = results.length; i < len; i++) {"+
                                "var result = results[i];"+
                                "arrProducts.push({ id: result.id, name: result.name, price: result.price, type: result.type, count: result.count  });" +
                                "}"+


                                "DCLoans('" + PartnerId + "', 'delProduct', false, function (result) {" +
                                    "if (result.status == true) {"+
                                        "DCLoans('" + PartnerId + "', 'addProduct', { products: arrProducts }, function (result) {" +
                                            "DCLoans('" + PartnerId + "', 'saveOrder', { order: '" + order.OrderID + "', errorText: '��� ������� �������� ������ �� ��������� ������� ��������� ������.���������� �������� ��������.' }, function (result) {}, debug);" +
                                        "}, debug);"+
                                    "}"+
                                "}, debug);"+
                          
                        "};");
            sb.Append("</script>");

            return sb.ToString();
        }

        public override string ProcessJavascriptButton(Order order)
        {
            return "DirectCred();";
        }

     
        /// <summary>
        /// ���������� json � ��������
        /// </summary>
        private string GetOrderJson(Order order, string partnerId)
        {
            var orderItems = order.OrderItems;

            float subtotal = order.OrderItems.Sum(item => item.Amount * item.Price);

            // ����� ������� �� ���������� � ��������� ������
            var taxTotal = TaxServices.GetOrderTaxes(order.OrderID).Where(t => !t.TaxShowInPrice).Sum(t => t.TaxSum);
            var totalDiscount = (float)Math.Round(subtotal / 100 * order.OrderDiscount, 2);

            foreach (var item in orderItems)
            {
                var percent = item.Price * item.Amount * 100 / subtotal;

                item.Price += percent * taxTotal / (100 * item.Amount);
                item.Price -= percent * totalDiscount / (100 * item.Amount);
            }

            var shopCartItems = orderItems.Select(item => item.ProductID != null ? new
            {
                id = item.ArtNo,
                name = item.Name,
                price = (float)Math.Round(item.Price),
                type = ProductService.GetCategoriesByProductId((int)item.ProductID).FirstOrDefault().Name,
                count = item.Amount.ToString("#")
                
            } : null).ToList();

            var array = new
            {
                items = shopCartItems
            };

            

            return JsonConvert.SerializeObject(array);
        }
    }

}