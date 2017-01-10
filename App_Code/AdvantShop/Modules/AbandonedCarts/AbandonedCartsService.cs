﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Modules
{
    public class AbandonedCartsService
    {
        public const string ModuleName = "AbandonedCarts";

        #region Abondoned cart

        private static AbandonedCart GetOrderConfirmationFromReader(SqlDataReader reader)
        {
            return new AbandonedCart
            {
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                OrderConfirmationData =
                    JsonConvert.DeserializeObject<OrderConfirmationData>(SQLDataHelper.GetString(reader, "OrderConfirmationData")),
                LastUpdate = SQLDataHelper.GetDateTime(reader, "LastUpdate", DateTime.MinValue),
                SendingCount = SQLDataHelper.GetInt(reader, "SendingCount"),
                SendingDate = SQLDataHelper.GetNullableDateTime(reader, "SendingDate")
            };
        }

        public static List<AbandonedCart> GetAbondonedCartsReg()
        {
            return
                ModulesRepository.ModuleExecuteReadList(
                    "Select Distinct sc.[CustomerId], null as OrderConfirmationData, " + 
                    "(Select top(1) UpdatedOn From [Catalog].[ShoppingCart] Where [ShoppingCart].[CustomerID] = sc.CustomerId and ShoppingCartType = @CartType Order By UpdatedOn Desc) as LastUpdate, " +
                    "(Select Count(*) From [Module].[AbandonedCartLetter] Where AbandonedCartLetter.CustomerId = sc.CustomerId) as SendingCount, " +
                    "(Select Top(1)SendingDate From [Module].[AbandonedCartLetter] Where AbandonedCartLetter.CustomerId = sc.CustomerId Order By SendingDate Desc) as SendingDate " +
                    "From [Catalog].[ShoppingCart] as sc " + 
                    "Inner Join [Customers].[Customer] On sc.[CustomerId] = [Customer].[CustomerID] " +
                    "Where ShoppingCartType = @CartType " +
                    "Order By LastUpdate Desc",
                    CommandType.Text, 
                    GetOrderConfirmationFromReader,
                    new SqlParameter("@CartType", ((int)ShoppingCartType.ShoppingCart).ToString()));
        }

        public static List<AbandonedCart> GetAbondonedCartsUnReg()
        {
            return
                ModulesRepository.ModuleExecuteReadList(
                    "Select OrderConfirmation.CustomerId, OrderConfirmationData, LastUpdate, " +
                    "(Select Count(*) From [Module].[AbandonedCartLetter] Where AbandonedCartLetter.CustomerId = OrderConfirmation.CustomerId) as SendingCount, " +
                    "(Select Top(1)SendingDate From [Module].[AbandonedCartLetter] Where AbandonedCartLetter.CustomerId = OrderConfirmation.CustomerId Order By SendingDate Desc) as SendingDate " +
                    "From [Order].[OrderConfirmation] " +
                    "Left Join [Customers].[Customer] On [OrderConfirmation].[CustomerId] = [Customer].[CustomerId] " +
                    "Where [Customer].[CustomerId] is null " +
                    "Order By LastUpdate Desc",
                    CommandType.Text,
                    GetOrderConfirmationFromReader);
        }

        public static AbandonedCart GetAbondonedCart(Guid customerId)
        {
            return
                ModulesRepository.ModuleExecuteReadOne(
                    "Select *, " +
                    "(Select Count(*) From [Module].[AbandonedCartLetter] Where AbandonedCartLetter.CustomerId = OrderConfirmation.CustomerId) as SendingCount, " +
                    "(Select Top(1)SendingDate From [Module].[AbandonedCartLetter] Where AbandonedCartLetter.CustomerId = OrderConfirmation.CustomerId Order By SendingDate Desc) as SendingDate " +
                    "From [Order].[OrderConfirmation] Where CustomerId = @CustomerId",
                    CommandType.Text,
                    GetOrderConfirmationFromReader, 
                    new SqlParameter("@CustomerId", customerId));
        }

        #endregion

        #region Abondoned template

        private static AbandonedCartTemplate GetTemplateFromReader(SqlDataReader reader)
        {
            return new AbandonedCartTemplate
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Subject = SQLDataHelper.GetString(reader, "Subject"),
                Body = SQLDataHelper.GetString(reader, "Body"),
                SendingTime = SQLDataHelper.GetInt(reader, "SendingTime"),
                Active = SQLDataHelper.GetBoolean(reader, "Active"),
            };
        }

        public static List<AbandonedCartTemplate> GetTemplates()
        {
            return
                ModulesRepository.ModuleExecuteReadList("Select * From [Module].[AbandonedCartTemplate]", CommandType.Text,
                    GetTemplateFromReader);
        }

        public static AbandonedCartTemplate GetTemplate(int id)
        {
            return
                ModulesRepository.ModuleExecuteReadOne(
                    "Select * From [Module].[AbandonedCartTemplate] Where Id = @Id", CommandType.Text,
                    GetTemplateFromReader,
                    new SqlParameter("@Id", id));
        }

        public static void AddTemplate(AbandonedCartTemplate template)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Insert Into [Module].[AbandonedCartTemplate] (Name,Subject,Body,SendingTime,Active) Values(@Name,@Subject,@Body,@SendingTime,@Active)",
                CommandType.Text,
                new SqlParameter("@Name", template.Name),
                new SqlParameter("@Subject", template.Subject),
                new SqlParameter("@Body", template.Body),
                new SqlParameter("@SendingTime", template.SendingTime),
                new SqlParameter("@Active", template.Active));
        }

        public static void UpdateTemplate(AbandonedCartTemplate template)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Update [Module].[AbandonedCartTemplate] Set Name = @Name, Subject = @Subject,Body = @Body,SendingTime = @SendingTime, Active = @Active Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Name", template.Name),
                new SqlParameter("@Subject", template.Subject),
                new SqlParameter("@Body", template.Body),
                new SqlParameter("@SendingTime", template.SendingTime),
                new SqlParameter("@Active", template.Active),
                new SqlParameter("@Id", template.Id));
        }

        public static void DeleteTemplate(int id)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Delete From [Module].[AbandonedCartTemplate] Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        #endregion

        #region Letter logs

        public static List<AbandonedCartLetter> GetAllLetters()
        {
            var list = ModulesRepository.ModuleExecuteReadList(
                "Select * From [Module].[AbandonedCartLetter]", CommandType.Text,
                reader => new AbandonedCartLetter()
                {
                    CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                    TemplateId = SQLDataHelper.GetInt(reader, "TemplateId"),
                    Email = SQLDataHelper.GetString(reader, "Email"),
                    SendingDate = SQLDataHelper.GetDateTime(reader, "SendingDate")
                });

            return list ?? new List<AbandonedCartLetter>();
        }

        public static List<AbandonedCartLetter> GetLetters(Guid customerId)
        {
            return
                ModulesRepository.ModuleExecuteReadList(
                    "Select * From [Module].[AbandonedCartLetter] Where CustomerId = @CustomerId", CommandType.Text,
                    reader => new AbandonedCartLetter()
                    {
                        CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                        TemplateId = SQLDataHelper.GetInt(reader, "TemplateId"),
                        Email = SQLDataHelper.GetString(reader, "Email"),
                        SendingDate = SQLDataHelper.GetDateTime(reader, "SendingDate")
                    },
                    new SqlParameter("CustomerId", customerId));
        }

        public static void LogLetter(AbandonedCartLetter letter)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Insert Into [Module].[AbandonedCartLetter] (TemplateId,CustomerId,SendingDate,Email) Values(@TemplateId,@CustomerId,@SendingDate,@Email)",
                CommandType.Text,
                new SqlParameter("@TemplateId", letter.TemplateId),
                new SqlParameter("@CustomerId", letter.CustomerId),
                new SqlParameter("@Email", letter.Email),
                new SqlParameter("@SendingDate", letter.SendingDate));
        }

        public static void DeleteExpiredLetters()
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Delete From [Module].[AbandonedCartLetter] Where SendingDate < @SendingDate",
                CommandType.Text,
                new SqlParameter("@SendingDate", DateTime.Now.AddMonths(-48)));
        }

        #endregion

        #region Sending

        public static int SendMessageUnReg(AbandonedCartTemplate template, List<Guid> cartIds)
        {
            int count = 0;

            foreach (var cartId in cartIds)
            {
                var cart = GetAbondonedCart(cartId);
                if (cart != null && cart.OrderConfirmationData != null && cart.OrderConfirmationData.Customer != null &&
                    cart.OrderConfirmationData.Customer.EMail.IsNotEmpty() &&
                    !cart.OrderConfirmationData.Customer.IsAdmin)
                {
                    var customer = cart.OrderConfirmationData.Customer;
                    var shoppingCart = ShoppingCartService.GetShoppingCart(ShoppingCartType.ShoppingCart, cart.CustomerId);
                    
                    template.Register(customer, shoppingCart);
                    template.BuildMail();

                    SendMail.SendMailNow(customer.EMail, template.Subject, template.Body, true);
                    LogLetter(new AbandonedCartLetter()
                    {
                        CustomerId = cart.CustomerId,
                        TemplateId = template.Id,
                        Email = customer.EMail,
                        SendingDate = DateTime.Now
                    });

                    count++;
                }
            }

            return count;
        }

        public static int SendMessageReg(AbandonedCartTemplate template, List<Guid> customerIds)
        {
            int count = 0;

            foreach (var customerId in customerIds)
            {
                var customer = CustomerService.GetCustomer(customerId);
                if (customer == null || customer.IsAdmin)
                    continue;

                var shoppingCart = ShoppingCartService.GetShoppingCart(ShoppingCartType.ShoppingCart, customerId);
                if (shoppingCart == null || !shoppingCart.Any())
                    continue;

                template.Register(customer, shoppingCart);
                template.BuildMail();

                SendMail.SendMailNow(customer.EMail, template.Subject, template.Body, true);
                LogLetter(new AbandonedCartLetter()
                {
                    CustomerId = customerId,
                    TemplateId = template.Id,
                    Email = customer.EMail,
                    SendingDate = DateTime.Now
                });

                count++;
            }

            return count;
        }

        #endregion

        #region Install/Uninstall

        public static bool InstallModule()
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Module.AbandonedCartTemplate') AND type in (N'U'))" +
                @"Begin
                    CREATE TABLE Module.AbandonedCartTemplate
	                    (
	                    Id int NOT NULL IDENTITY (1, 1),
	                    Name nvarchar(MAX) NOT NULL,
                        Subject nvarchar(MAX) NOT NULL,
	                    Body nvarchar(MAX) NOT NULL,
                        SendingTime int NOT NULL,
                        Active bit NOT NULL
	                    )  ON [PRIMARY]                    
                End",
                CommandType.Text);

            ModulesRepository.ModuleExecuteNonQuery(
                "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Module.AbandonedCartLetter') AND type in (N'U'))" +
                @"Begin 
                 CREATE TABLE Module.AbandonedCartLetter(
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [TemplateId] [int] NOT NULL,
	                [CustomerId] [uniqueidentifier] NOT NULL,
	                [SendingDate] [datetime] NOT NULL,
                    [Email] nvarchar(max) NULL,
                   CONSTRAINT [PK_AbandonedCartLetter] PRIMARY KEY CLUSTERED ([Id] ASC)
                 ) ON [PRIMARY]
                End",
                CommandType.Text);

            ModulesRepository.ModuleExecuteNonQuery(
                @"IF ((Select Count(*) From Module.AbandonedCartTemplate) = 0) 
                Begin  
                    Insert Into Module.AbandonedCartTemplate (Name,Subject,Body,SendingTime,Active)  
                    Values ('Первое письмо', 'Мы обнаружили, что Вы не завершили свой заказ на сайте #SHOPNAME#', '<p>Здравствуйте, уважаемый клиент!</p>    <p>Мы обратили внимание, что Вы начинали оформлять заказ на сайте <a href=""#SHOPURL#"">#SHOPNAME#</a>, но не завершили его.</p>    <p>Цена на товар, который Вы положили в корзину, снизилась и теперь Вы можете оформить заказ по выгодной цене:</p>    <p>#PRODUCTS#</p>    <p>Для того, чтобы закончить <strong>заказ со скидкой</strong>, нажмите <a href=""#BASKETURL#""\><strong>Оформить заказ</strong></a></p>    <p>Приятных покупок!</p>    <p>Наш магазин работает круглосуточно, без выходных. Если у вас появились вопросы, свяжитесь с нами по телефону: 8 (8888) 88-88-88</p> ', 1, 0)  
                End",
                CommandType.Text);

            ModulesRepository.ModuleExecuteNonQuery("DELETE FROM Catalog.ShoppingCart WHERE CreatedOn<@olderThan", CommandType.Text, 
                new SqlParameter("@olderThan", DateTime.Now.AddMonths(-2)));

            return true;
        }

        #endregion
    }
}