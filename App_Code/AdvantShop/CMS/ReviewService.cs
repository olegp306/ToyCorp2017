//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;

namespace AdvantShop.CMS
{
    public class ReviewService
    {
        public static Review GetReview(int reviewId)
        {
            return SQLDataAccess.ExecuteReadOne<Review>("SELECT * FROM [CMS].[Review] WHERE ReviewId = @ReviewId",
                                                                CommandType.Text, GetFromReader,
                                                                new SqlParameter("@ReviewId", reviewId));
        }

        public static IEnumerable<Review> GetReviews(int entityId, EntityType entityType)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Review>("SELECT * FROM [CMS].[Review] WHERE [EntityId] = @EntityId AND [Type] = @entityType",
                                                                      CommandType.Text, GetFromReader,
                                                                      new SqlParameter("@EntityId", entityId),
                                                                      new SqlParameter("@entityType", (int)entityType));
        }

        public static IEnumerable<Review> GetCheckedReviews(int entityId, EntityType entityType)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Review>("SELECT * FROM [CMS].[Review] WHERE [EntityId] = @EntityId AND [Type] = @entityType AND [Checked] = 1",
                                                                      CommandType.Text, GetFromReader,
                                                                      new SqlParameter("@EntityId", entityId),
                                                                      new SqlParameter("@entityType", (int)entityType));
        }

        public static int GetReviewsCount(int entityId, EntityType entityType)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(ReviewID) FROM [CMS].[Review] WHERE [EntityId] = @EntityId AND [Type] = @Type",
                                                                      CommandType.Text,
                                                                      new SqlParameter("@EntityId", entityId),
                                                                      new SqlParameter("@Type", (int)entityType));
        }

        public static int GetCheckedReviewsCount(int entityId, EntityType entityType)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(ReviewID) FROM [CMS].[Review] WHERE [EntityId] = @EntityId AND [Type] = @Type AND [Checked] = 1",
                                                                      CommandType.Text,
                                                                      new SqlParameter("@EntityId", entityId),
                                                                      new SqlParameter("@Type", (int)entityType));
        }

        public static List<Review> GetReviewChildren(int reviewId)
        {
            return SQLDataAccess.ExecuteReadList<Review>("SELECT * FROM [CMS].[Review] WHERE [ParentId] = @ParentId",
                                                                      CommandType.Text, GetFromReader,
                                                                      new SqlParameter("@ParentId", reviewId));
        }

        public static List<int> GetReviewChildrenIds(int reviewId)
        {
            List<int> listIds = SQLDataAccess.ExecuteReadList<int>("SELECT [ReviewId] FROM [CMS].[Review] WHERE [ParentId] = @ParentId",
                                                              CommandType.Text,
                                                              reader => SQLDataHelper.GetInt(reader, "ReviewId"),
                                                              new SqlParameter("@ParentId", reviewId));
            return listIds;
        }

        public static IEnumerable<Review> GetReviewList()
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Review>("SELECT * FROM [CMS].[Review]", CommandType.Text, GetFromReader);
        }

        private static Review GetFromReader(SqlDataReader reader)
        {
            return new Review
            {
                ReviewId = SQLDataHelper.GetInt(reader, "ReviewId"),
                ParentId = SQLDataHelper.GetNullableInt(reader, "ParentId") ?? 0,
                EntityId = SQLDataHelper.GetInt(reader, "EntityId"),
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Text = SQLDataHelper.GetString(reader, "Text"),
                Checked = SQLDataHelper.GetBoolean(reader, "Checked"),
                AddDate = SQLDataHelper.GetDateTime(reader, "AddDate"),
                Ip = SQLDataHelper.GetString(reader, "IP")
            };
        }

        public static void AddReview(Review review)
        {
            review.ReviewId = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("INSERT INTO [CMS].[Review] " +
                                    " ([ParentId], [EntityId], [Type], [CustomerId], [Name], [Email], [Text], [Checked], [AddDate], [IP]) " +
                                    " VALUES (@ParentId, @EntityId, @Type, @CustomerId, @Name, @Email, @Text, @Checked, GETDATE(), @IP); SELECT SCOPE_IDENTITY(); ",
                                    CommandType.Text,
                                    new SqlParameter("@ParentId", review.ParentId),
                                    new SqlParameter("@EntityId", review.EntityId),
                                    new SqlParameter("@Type", (int)review.Type),
                                    new SqlParameter("@CustomerId", review.CustomerId),
                                    new SqlParameter("@Name", review.Name),
                                    new SqlParameter("@Email", review.Email),
                                    new SqlParameter("@Text", review.Text),
                                    new SqlParameter("@Checked", review.Checked),
                                    new SqlParameter("@IP", review.Ip)));
        }

        public static void UpdateReview(Review review)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [CMS].[Review] SET [ParentId] = @ParentId, [EntityId] = @EntityId, [Type] = @Type, [CustomerId] = @CustomerId, [Name] = @Name, [Email] = @Email, [Text] = @Text , [Checked] = @Checked, AddDate=@AddDate  WHERE reviewId = @reviewId",
                CommandType.Text,
                new SqlParameter("@reviewId", review.ReviewId),
                new SqlParameter("@ParentId", review.ParentId),
                new SqlParameter("@EntityId", review.EntityId),
                new SqlParameter("@Type", review.Type),
                new SqlParameter("@CustomerId", review.CustomerId),
                new SqlParameter("@Name", review.Name),
                new SqlParameter("@Email", review.Email),
                new SqlParameter("@Checked", review.Checked),
                new SqlParameter("@Text", review.Text),
                new SqlParameter("@AddDate", review.AddDate)
                );
        }

        public static void CheckReview(int reviewId, bool isChecked)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [CMS].[Review] SET [Checked] = @Checked WHERE reviewId = @reviewId",
                CommandType.Text,
                new SqlParameter("@reviewId", reviewId),
                new SqlParameter("@Checked", isChecked));
        }

        public static void DeleteReview(int reviewId)
        {
            // Список удаляемых коментов
            var deleteIds = new List<int> { reviewId };
            var newIds = new List<int> { reviewId };

            // Пока есть новые коментарии для удаления
            while (newIds.Count > 0)
            {
                var listIds = new List<int>();

                // Берём все дочерние коменты, у всех комметариев с прошлой итерации
                foreach (var newId in newIds)
                {
                    listIds.AddRange(GetReviewChildrenIds(newId));
                }

                // Добавляем в список
                deleteIds.AddRange(listIds);

                newIds.Clear();
                newIds.AddRange(listIds);
            }

            // Удаляем комменты
            foreach (var deleteId in deleteIds)
            {
                DeleteCommentFromDb(deleteId);
            }
        }

        private static void DeleteCommentFromDb(int commentId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [CMS].[Review] WHERE ReviewId = @ReviewId", CommandType.Text, new SqlParameter("@ReviewId", commentId));
        }


        // ********************** Request Methods ********************** //

        public static bool IsExistsEntity(int requestId, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Product:
                    return ProductService.IsExists(requestId);

                default:
                    throw new Exception("AdvantShop.CMS.CommentService\r\npublic static bool IsExistsRequest(int requestId, RequestType requestType)\r\nLogic for RequestType." + entityType + " did not realize");
            }
        }


        public static ReviewEntity GetReviewEntity(int reviewId)
        {
            var type = (EntityType)SQLDataAccess.ExecuteScalar<int>("Select Type From CMS.Review Where ReviewId=@ReviewId", CommandType.Text,
                                                              new SqlParameter() { ParameterName = "@ReviewId", Value = reviewId });
            switch (type)
            {
                case EntityType.Product:
                    return SQLDataAccess.ExecuteReadOne("Select Product.ProductID, Name, Photo.Description, PhotoName From Catalog.Product left join catalog.Photo on Product.ProductID = Photo.ObjId and Type=@type and main = 1 Where Product.ProductID = (Select EntityID From CMS.Review Where ReviewId=@ReviewId )",
                                                        CommandType.Text,
                                                        reader => new ReviewEntity()
                                                                      {
                                                                          Type = EntityType.Product,
                                                                          Name = SQLDataHelper.GetString(reader, "Name"),
                                                                          ReviewEntityId = SQLDataHelper.GetInt(reader, "ProductID"),
                                                                          Photo = SQLDataHelper.GetString(reader, "PhotoName"),
                                                                          PhotoDescription = SQLDataHelper.GetString(reader, "Description")
                                                                      },
                                                                      new SqlParameter() { ParameterName = "@ReviewId", Value = reviewId },
                                                                      new SqlParameter("@type", PhotoType.Product.ToString()));
                default:
                    throw new NotImplementedException();
            }
        }


        public static string GetEntityUrl(int entityId, EntityType type)
        {

            if (!IsExistsEntity(entityId, type))
            {
                return string.Empty;
            }

            switch (type)
            {
                case EntityType.Product:
                    return UrlService.GetAbsoluteLink("/admin/Product.aspx?ProductId=" + entityId);
            }
            return string.Empty;
        }
    }
}