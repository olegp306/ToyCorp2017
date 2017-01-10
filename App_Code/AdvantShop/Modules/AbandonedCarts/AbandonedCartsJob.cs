using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Scheduler;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Mails;
using AdvantShop.Orders;
using Quartz;

namespace AdvantShop.Modules
{
    [DisallowConcurrentExecution]
    public class AbandonedCartsJob : IJob
    {
        private List<AbandonedCartLetter> _letters = new List<AbandonedCartLetter>(); 

        public void Execute(IJobExecutionContext context)
        {
            if (!context.CanStart()) return; 
            context.WriteLastRun();

            AbandonedCartsService.DeleteExpiredLetters();
            var templates = AbandonedCartsService.GetTemplates().Where(x => x.Active).ToList();

            if (!templates.Any())
                return;

            var cartsUnReg =
                AbandonedCartsService.GetAbondonedCartsUnReg()
                    .Where(
                        x =>
                            x.OrderConfirmationData != null && x.OrderConfirmationData.Customer != null &&
                            x.OrderConfirmationData.Customer.EMail.IsNotEmpty() &&
                            !x.OrderConfirmationData.Customer.IsAdmin)
                    .ToList();

            var cartsReg = AbandonedCartsService.GetAbondonedCartsReg();

            _letters = AbandonedCartsService.GetAllLetters();
            
            foreach (var template in templates)
            {
                var date = DateTime.Now.AddHours(-template.SendingTime);
                var upDate = DateTime.Now.AddDays(-1).AddHours(-template.SendingTime);

                try
                {
                    foreach (var cart in cartsUnReg.Where(c => c.LastUpdate < date && c.LastUpdate > upDate &&
                                                           _letters.Find(x => x.TemplateId == template.Id && x.CustomerId == c.CustomerId) == null))
                    {
                        SendUnRegUsers(cart, template);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }

                
                try
                {
                    foreach (var cart in cartsReg.Where(c => c.LastUpdate < date && c.LastUpdate > upDate && 
                                                             _letters.Find(x => x.TemplateId == template.Id && x.CustomerId == c.CustomerId) == null))
                    {
                        SendRegUsers(cart, template);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private void SendUnRegUsers(AbandonedCart cart, AbandonedCartTemplate template)
        {

            var customer = cart.OrderConfirmationData.Customer;
            var shoppingCart = ShoppingCartService.GetShoppingCart(ShoppingCartType.ShoppingCart, cart.CustomerId);

            if (_letters.Find(x=> x.Email == customer.EMail) != null)
                return;

            template.Register(customer, shoppingCart);
            template.BuildMail();

            SendMail.SendMailNow(customer.EMail, template.Subject, template.Body, true);
            AbandonedCartsService.LogLetter(new AbandonedCartLetter()
            {
                CustomerId = cart.CustomerId,
                TemplateId = template.Id,
                Email = customer.EMail,
                SendingDate = DateTime.Now
            });
        }

        private void SendRegUsers(AbandonedCart cart, AbandonedCartTemplate template)
        {
            var customer = CustomerService.GetCustomer(cart.CustomerId);
            if (customer == null || customer.IsAdmin || _letters.Find(x => x.Email == customer.EMail) != null)
                return;

            var shoppingCart = ShoppingCartService.GetShoppingCart(ShoppingCartType.ShoppingCart, cart.CustomerId);

            template.Register(customer, shoppingCart);
            template.BuildMail();

            SendMail.SendMailNow(customer.EMail, template.Subject, template.Body, true);
            AbandonedCartsService.LogLetter(new AbandonedCartLetter()
            {
                CustomerId = cart.CustomerId,
                TemplateId = template.Id,
                Email = customer.EMail,
                SendingDate = DateTime.Now
            });
        }
    }
}