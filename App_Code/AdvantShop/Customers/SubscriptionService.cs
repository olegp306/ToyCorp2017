//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;

namespace AdvantShop.Customers
{
    public class SubscriptionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subcription"></param>
        public static void AddSubscription(Subscription subcription)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Customers].[Subscription] (Email,Subscribe,SubscribeDate,UnsubscribeDate,UnsubscribeReason) VALUES (@Email,@Subscribe,GETDATE(),NULL,@UnsubscribeReason)",
              CommandType.Text,
              new SqlParameter("@Email", subcription.Email),
              new SqlParameter("@Subscribe", subcription.Subscribe),
              new SqlParameter("@UnsubscribeReason", subcription.UnsubscribeReason));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subcription"></param>
        public static void UpdateSubscription(Subscription subcription)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Customers].[Subscription] SET Email=@Email, Subscribe=@Subscribe, UnsubscribeReason=@UnsubscribeReason WHERE [Id]=@Id",
              CommandType.Text,
              new SqlParameter("@Email", subcription.Email),
              new SqlParameter("@Subscribe", subcription.Subscribe),
              new SqlParameter("@UnsubscribeReason", subcription.UnsubscribeReason),
              new SqlParameter("@Id", subcription.Id));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsExistsSubscription(string email)
        {
            return SQLDataAccess.ExecuteScalar<bool>("IF(SELECT Count(Id) FROM [Customers].[Subscription] WHERE [Email] = @Email) > 0 BEGIN SELECT 1 END ELSE BEGIN SELECT 0 END",
                CommandType.Text,
                new SqlParameter("@Email", email));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public static bool IsSubscribe(string email)
        {
            return SQLDataAccess.ExecuteScalar<bool>("IF(SELECT COUNT(Id) FROM [Customers].[Subscription] WHERE [Email] = @Email AND [Subscribe] = 1) > 0 BEGIN SELECT 1 END ELSE BEGIN SELECT 0 END",
                CommandType.Text,
                new SqlParameter("@Email", email));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public static void Subscribe(string email)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(Id) FROM [Customers].[Subscription] WHERE [Email] = @Email) > 0" +
                "BEGIN UPDATE [Customers].[Subscription] SET [Subscribe] = 1, [SubscribeDate] = GETDATE() WHERE [Email] = @Email END " +
                "ELSE " +
                "BEGIN INSERT INTO [Customers].[Subscription] ([Email],[Subscribe],[SubscribeDate],[UnsubscribeDate],[UnsubscribeReason]) VALUES (@Email,1,GETDATE(),NULL,NULL) END",
                CommandType.Text,
                new SqlParameter("@Email", email));

            var customer = CustomerService.GetCustomerByEmail(email);
            var subscription = new Subscription { Email = email };
            if (customer != null)
            {
                subscription.FirstName = customer.FirstName;
                subscription.LastName = customer.LastName;
                subscription.Phone = customer.Phone;
            }

            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                moduleObject.SubscribeEmail(subscription);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static void Subscribe(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Customers].[Subscription] SET [Subscribe] = 1, [SubscribeDate] = GETDATE(), [UnsubscribeDate] = NULL WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
            //todo: лишние запросы получения кастомера и подписки <Sckeef>
            var subscription = GetSubscription(id);
            var customer = CustomerService.GetCustomerByEmail(subscription.Email);
            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                moduleObject.SubscribeEmail(new Subscription { Email = subscription.Email, FirstName = customer.FirstName, LastName = customer.LastName, Phone = customer.Phone });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="unsubscribeReason"></param>
        public static void Unsubscribe(string email, string unsubscribeReason)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Customers].[Subscription] SET [Subscribe] = 0, [UnsubscribeDate] = GETDATE(), [UnsubscribeReason] = @UnsubscribeReason WHERE [Email] = @Email",
                CommandType.Text,
                new SqlParameter("@Email", email),
                new SqlParameter("@UnsubscribeReason", string.IsNullOrEmpty(unsubscribeReason) ? (object)DBNull.Value : unsubscribeReason));

            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                moduleObject.UnsubscribeEmail(email);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public static void Unsubscribe(string email)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Customers].[Subscription] SET [Subscribe] = 0, [UnsubscribeDate] = GETDATE() WHERE [Email] = @Email",
                CommandType.Text,
                new SqlParameter("@Email", email));

            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                moduleObject.UnsubscribeEmail(email);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static void Unsubscribe(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Customers].[Subscription] SET [Subscribe] = 0, [UnsubscribeDate] = GETDATE() WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));

            var subscription = GetSubscription(id);

            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                moduleObject.UnsubscribeEmail(subscription.Email);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Subscription> GetSubscriptions()
        {
            return SQLDataAccess.ExecuteReadList<Subscription>(
                "SELECT * FROM [Customers].[Subscription]",
                CommandType.Text,
                GetSubscriptionFromReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Subscription GetSubscription(int id)
        {
            return SQLDataAccess.ExecuteReadOne<Subscription>(
                "SELECT * FROM [Customers].[Subscription] WHERE Id = @Id",
                CommandType.Text,
                GetSubscriptionFromReader,
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static Subscription GetSubscription(string email)
        {
            return SQLDataAccess.ExecuteReadOne<Subscription>(
                "SELECT * FROM [Customers].[Subscription] WHERE [Email] = @Email",
                CommandType.Text,
                GetSubscriptionFromReader,
                new SqlParameter("@Email", email));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<ISubscriber> GetSubscribedEmails()
        {
            return SQLDataAccess.ExecuteReadList<ISubscriber>(
              "SELECT [Subscription].[Email], [Customer].[FirstName], [Customer].[LastName], [Customer].[Phone] FROM [Customers].[Subscription] LEFT JOIN [Customers].[Customer] ON [Subscription].[Email] = [Customer].[Email] WHERE [Subscribe] = 1",
              CommandType.Text,
              (reader) =>
              {
                  return new Subscription
                      {
                          Email = SQLDataHelper.GetString(reader, "Email"),
                          FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                          LastName = SQLDataHelper.GetString(reader, "LastName"),
                          Phone = SQLDataHelper.GetString(reader, "Phone"),
                      };
              });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteSubscription(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Customers].[Subscription] WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public static void DeleteSubscription(string email)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Customers].[Subscription] WHERE [Email] = @Email",
                CommandType.Text,
                new SqlParameter("@Email", email));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Subscription GetSubscriptionFromReader(SqlDataReader reader)
        {
            return new Subscription
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Subscribe = SQLDataHelper.GetBoolean(reader, "Subscribe"),
                SubscribeDate = SQLDataHelper.GetDateTime(reader, "SubscribeDate"),
                UnsubscribeDate = SQLDataHelper.GetDateTime(reader, "UnsubscribeDate"),
                UnsubscribeReason = SQLDataHelper.GetString(reader, "UnsubscribeReason")
            };
        }
    }
}