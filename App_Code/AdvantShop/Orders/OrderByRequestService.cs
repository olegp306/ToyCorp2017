//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using System.Web;

namespace AdvantShop.Orders
{
    public class OrderByRequestService
    {
        public static OrderByRequest GetOrderByRequest(int orderByRequestId)
        {
            var orderByRequest = SQLDataAccess.ExecuteReadOne<OrderByRequest>("SELECT * FROM [Order].[OrderByRequest] WHERE OrderByRequestId = @OrderByRequestId ", CommandType.Text,
                                                                                         GetOrderByRequestFromReader, new SqlParameter("@OrderByRequestId", orderByRequestId));
            return orderByRequest;
        }

        public static OrderByRequest GetOrderByRequest(string code)
        {
            var orderByRequest = SQLDataAccess.ExecuteReadOne<OrderByRequest>("SELECT TOP(1) * FROM [Order].[OrderByRequest] WHERE Code = @code  ", CommandType.Text,
                                                                                         GetOrderByRequestFromReader, new SqlParameter("@Code", code));
            return orderByRequest;
        }

        public static List<int> GetIdList()
        {
            List<int> idList = SQLDataAccess.ExecuteReadList<int>("SELECT [OrderByRequestId] FROM [Order].[OrderByRequest]", CommandType.Text,
                                                             reader => SQLDataHelper.GetInt(reader, "OrderByRequestId"));
            return idList;
        }

        public static List<OrderByRequest> GetOrderByRequestList()
        {
            var orderByRequestList = SQLDataAccess.ExecuteReadList<OrderByRequest>("SELECT [OrderByRequestId] FROM [Order].[OrderByRequest]", CommandType.Text,
                                                                                    GetOrderByRequestFromReader);
            return orderByRequestList;
        }

        private static OrderByRequest GetOrderByRequestFromReader(SqlDataReader reader)
        {
            return new OrderByRequest
            {
                OrderByRequestId = SQLDataHelper.GetInt(reader, "OrderByRequestId"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                ProductName = SQLDataHelper.GetString(reader, "ProductName"),
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                Quantity = SQLDataHelper.GetFloat(reader, "Quantity"),
                UserName = SQLDataHelper.GetString(reader, "UserName"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Phone = SQLDataHelper.GetString(reader, "Phone"),
                Comment = SQLDataHelper.GetString(reader, "Comment"),
                IsComplete = SQLDataHelper.GetBoolean(reader, "IsComplete"),
                RequestDate = SQLDataHelper.GetDateTime(reader, "RequestDate"),
                Code = SQLDataHelper.GetString(reader, "Code"),
                CodeCreateDate = SQLDataHelper.GetDateTime(reader, "CodeCreateDate", DateTime.MinValue),
                LetterComment = SQLDataHelper.GetString(reader, "LetterComment"),
                Options = SQLDataHelper.GetString(reader, "Options"),
            };
        }

        public static void AddOrderByRequest(OrderByRequest orderByRequest)
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = " INSERT INTO [Order].[OrderByRequest] " +
                                     " ([ProductID], [ProductName], [ArtNo], [Quantity], [UserName], [Email], [Phone], [Comment], [IsComplete], [RequestDate], [OfferID], LetterComment, Options ) " +
                                     " VALUES (@ProductID, @ProductName, @ArtNo, @Quantity, @UserName, @Email, @Phone, @Comment, @IsComplete, @RequestDate, @OfferID, @LetterComment, @Options); SELECT SCOPE_IDENTITY();";
                db.cmd.CommandType = CommandType.Text;
                db.cmd.Parameters.Clear();

                db.cmd.Parameters.AddWithValue("@ProductID", orderByRequest.ProductId);
                db.cmd.Parameters.AddWithValue("@ProductName", orderByRequest.ProductName);
                db.cmd.Parameters.AddWithValue("@ArtNo", orderByRequest.ArtNo);
                db.cmd.Parameters.AddWithValue("@Quantity", orderByRequest.Quantity);
                db.cmd.Parameters.AddWithValue("@UserName", orderByRequest.UserName);
                db.cmd.Parameters.AddWithValue("@Email", orderByRequest.Email);
                db.cmd.Parameters.AddWithValue("@Phone", orderByRequest.Phone);
                db.cmd.Parameters.AddWithValue("@Comment", orderByRequest.Comment);
                db.cmd.Parameters.AddWithValue("@IsComplete", orderByRequest.IsComplete);
                db.cmd.Parameters.AddWithValue("@RequestDate", orderByRequest.RequestDate);
                db.cmd.Parameters.AddWithValue("@OfferID", orderByRequest.OfferId);
                db.cmd.Parameters.AddWithValue("@LetterComment", orderByRequest.LetterComment ?? string.Empty);
                db.cmd.Parameters.AddWithValue("@Options", orderByRequest.Options ?? string.Empty);

                db.cnOpen();
                orderByRequest.OrderByRequestId = SQLDataHelper.GetInt(db.cmd.ExecuteScalar());
                db.cnClose();
            }
        }

        public static void UpdateOrderByRequest(OrderByRequest orderByRequest)
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = " UPDATE [Order].[OrderByRequest] SET [ProductID] = @ProductID, [ProductName] = @ProductName, [ArtNo] = @ArtNo, [Quantity] = @Quantity, [UserName] = @UserName, [Email] = @Email, [Phone] = @Phone, [Comment] = @Comment, [IsComplete] = @IsComplete, [RequestDate] = @RequestDate, [OfferID] = @OfferID, LetterComment=@LetterComment, Options=@Options " +
                                     " WHERE OrderByRequestId = @OrderByRequestId";
                db.cmd.CommandType = CommandType.Text;
                db.cmd.Parameters.Clear();

                db.cmd.Parameters.AddWithValue("@OrderByRequestId", orderByRequest.OrderByRequestId);
                db.cmd.Parameters.AddWithValue("@ProductID", orderByRequest.ProductId);
                db.cmd.Parameters.AddWithValue("@ProductName", orderByRequest.ProductName);
                db.cmd.Parameters.AddWithValue("@ArtNo", orderByRequest.ArtNo);
                db.cmd.Parameters.AddWithValue("@Quantity", orderByRequest.Quantity);
                db.cmd.Parameters.AddWithValue("@UserName", orderByRequest.UserName);
                db.cmd.Parameters.AddWithValue("@Email", orderByRequest.Email);
                db.cmd.Parameters.AddWithValue("@Phone", orderByRequest.Phone);
                db.cmd.Parameters.AddWithValue("@Comment", orderByRequest.Comment);
                db.cmd.Parameters.AddWithValue("@IsComplete", orderByRequest.IsComplete);
                db.cmd.Parameters.AddWithValue("@RequestDate", orderByRequest.RequestDate);
                db.cmd.Parameters.AddWithValue("@OfferID", orderByRequest.OfferId);
                db.cmd.Parameters.AddWithValue("@LetterComment", orderByRequest.LetterComment);
                db.cmd.Parameters.AddWithValue("@Options", orderByRequest.Options ?? string.Empty);

                db.cnOpen();
                db.cmd.ExecuteNonQuery();
                db.cnClose();
            }
        }

        public static void DeleteOrderByRequest(int orderByRequestId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[OrderByRequest] WHERE OrderByRequestId = @OrderByRequestId", CommandType.Text, new SqlParameter("@OrderByRequestId", orderByRequestId));
        }

        public static string CreateCode(int orderByRequestId)
        {
            var code = Guid.NewGuid().ToString();
            SQLDataAccess.ExecuteNonQuery("UPDATE [Order].[OrderByRequest] SET [Code] = @Code, [CodeCreateDate] = GETDATE() WHERE OrderByRequestId = @OrderByRequestId", CommandType.Text,
                                                new SqlParameter("@OrderByRequestId", orderByRequestId), new SqlParameter("@Code", code));
            return code;
        }

        public static void DeleteCode(string code)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Order].[OrderByRequest] SET [Code] = '' WHERE [Code] = @Code", CommandType.Text, new SqlParameter("@Code", code));
        }

        public static void SendConfirmationMessage(int orderByRequestId)
        {
            var orderByRequest = GetOrderByRequest(orderByRequestId);
            var code = CreateCode(orderByRequestId);

            var offer = OfferService.GetOffer(orderByRequest.OfferId);

            IList<EvaluatedCustomOptions> listOptions = null;
            var selectedOptions = orderByRequest.Options.IsNotEmpty() && orderByRequest.Options != "null"
                                                    ? HttpUtility.UrlDecode(orderByRequest.Options)
                                                    : null;
            if (selectedOptions.IsNotEmpty())
            {
                try
                {
                    listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                }
                catch (Exception)
                {
                    listOptions = null;
                }
            }

            string optionsRender = OrderService.RenderSelectedOptions(listOptions);


            var linkByRequestMailTemplate = new LinkByRequestMailTemplate(orderByRequest.OrderByRequestId.ToString(),
                                                                          orderByRequest.ArtNo,
                                                                          orderByRequest.ProductName + " " + optionsRender,
                                                                          orderByRequest.Quantity.ToString(), code,
                                                                          orderByRequest.UserName,
                                                                          orderByRequest.LetterComment,
                                                                          offer != null && offer.Color != null
                                                                              ? offer.Color.ColorName
                                                                              : "",
                                                                          offer != null && offer.Size != null
                                                                              ? offer.Size.SizeName
                                                                              : "");

            linkByRequestMailTemplate.BuildMail();
            SendMail.SendMailNow(orderByRequest.Email, linkByRequestMailTemplate.Subject, linkByRequestMailTemplate.Body, true);
        }

        public static void SendFailureMessage(int orderByRequestId)
        {
            var orderByRequest = GetOrderByRequest(orderByRequestId);
            var offer = OfferService.GetOffer(orderByRequest.OfferId);

            IList<EvaluatedCustomOptions> listOptions = null;
            var selectedOptions = orderByRequest.Options.IsNotEmpty() && orderByRequest.Options != "null"
                                                    ? HttpUtility.UrlDecode(orderByRequest.Options)
                                                    : null;
            if (selectedOptions.IsNotEmpty())
            {
                try
                {
                    listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                }
                catch (Exception)
                {
                    listOptions = null;
                }
            }

            string optionsRender = OrderService.RenderSelectedOptions(listOptions);


            var failureByRequestMail = new FailureByRequestMailTemplate(orderByRequest.OrderByRequestId.ToString(),
                                                                        orderByRequest.ArtNo, orderByRequest.ProductName + " " + optionsRender,
                                                                        orderByRequest.Quantity.ToString(),
                                                                        orderByRequest.UserName,
                                                                        offer != null && offer.Color != null
                                                                            ? offer.Color.ColorName
                                                                            : "",
                                                                        offer != null && offer.Size != null
                                                                            ? offer.Size.SizeName
                                                                            : "");

            failureByRequestMail.BuildMail();
            SendMail.SendMailNow(orderByRequest.Email, failureByRequestMail.Subject, failureByRequestMail.Body, true);
        }
    }
}