//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AdvantShop.Modules
{
    public class StoreReviewRepository
    {
        public static bool InstallStoreReviewsModule()
        {
            if (!ModulesRepository.IsExistsModuleTable("Module", "StoreReview"))
            {
                ModulesRepository.ModuleExecuteNonQuery(
                    @"CREATE TABLE Module.StoreReview
                    (  ID int NOT NULL IDENTITY (1, 1),
	                        ParentID int NULL,
	                        ReviewerEmail nvarchar(50) NOT NULL,
                            ReviewerName nvarchar(100) NOT NULL,
	                        Review nvarchar(MAX) NOT NULL,
	                        DateAdded datetime NOT NULL,
                            Moderated bit NOT NULL,
	                        Rate int NULL
	                        )  ON [PRIMARY]
	                            TEXTIMAGE_ON [PRIMARY]                                        
                        ALTER TABLE Module.StoreReview ADD CONSTRAINT
	                        PK_StoreReview PRIMARY KEY CLUSTERED 
	                        (
	                        ID
	                        ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]                                        
                        ALTER TABLE Module.StoreReview SET (LOCK_ESCALATION = TABLE)
                        SET IDENTITY_INSERT Module.StoreReview ON",
                    CommandType.Text);
            }
            ModuleSettingsProvider.SetSettingValue("PageSize", "20", "StoreReviews");
            return ModulesRepository.IsExistsModuleTable("Module", "StoreReview");
        }

        public static bool UninstallStoreReviewsModule()
        {
            if (ModulesRepository.IsExistsModuleTable("Module", "StoreReview"))
            {
                ModulesRepository.ModuleExecuteNonQuery(@"DROP TABLE Module.StoreReview", CommandType.Text);
            }

            return ModulesRepository.IsExistsModuleTable("Module", "StoreReview");
        }

        public static bool IsAliveStoreReviewsModule()
        {
            return ModulesRepository.IsExistsModuleTable("Module", "StoreReview");
        }

        private static StoreReview GetStoreReviewFromReader(SqlDataReader reader)
        {
            return new StoreReview
                {
                    Id = ModulesRepository.ConvertTo<int>(reader, "ID"),
                    ParentId = ModulesRepository.ConvertTo<int>(reader, "ParentID"),
                    Rate = ModulesRepository.ConvertTo<int>(reader, "Rate"),
                    Review = ModulesRepository.ConvertTo<string>(reader, "Review"),
                    ReviewerEmail = ModulesRepository.ConvertTo<string>(reader, "ReviewerEmail"),
                    ReviewerName = ModulesRepository.ConvertTo<string>(reader, "ReviewerName"),
                    DateAdded = ModulesRepository.ConvertTo<DateTime>(reader, "DateAdded"),
                    Moderated = ModulesRepository.ConvertTo<bool>(reader, "Moderated"),
                    HasChild = ModulesRepository.ConvertTo<int>(reader, "ChildsCount") > 0
                };
        }

        public static List<StoreReview> GetStoreReviewsByParentId(int parentId)
        {
            return GetStoreReviewsByParentId(parentId, false);
        }
        
        public static DataTable GetStoreReviews()
        {
            return ModulesRepository.ModuleExecuteTable(
                "SELECT * FROM [Module].[StoreReview] ORDER BY [DateAdded] DESC",
                CommandType.Text
            );
        }

        public static List<StoreReview> GetStoreReviewsByParentId(int parentId, bool isModerated)
        {
            return ModulesRepository.ModuleExecuteReadList<StoreReview>(
                "SELECT *, (SELECT Count(ID) FROM [Module].[StoreReview] WHERE [ParentID] = ParentReview.ID) as ChildsCount FROM [Module].[StoreReview] as ParentReview WHERE "
                + (parentId == 0 ? "[ParentID] is NULL" : "[ParentID] = " + parentId)
                + (isModerated ? " AND [Moderated] = 1" : string.Empty) + " ORDER BY [DateAdded]"
                + (parentId == 0 ? "DESC" : "ASC"),
                CommandType.Text,
                (reader) =>
                {
                    var review = GetStoreReviewFromReader(reader);
                    review.ChildrenReviews = ModulesRepository.ConvertTo<int>(reader, "ChildsCount") > 0
                                                 ? GetStoreReviewsByParentId(
                                                     ModulesRepository.ConvertTo<int>(reader, "ID"), isModerated)
                                                 : new List<StoreReview>();
                    return review;
                }
            );
        }

        public static StoreReview GetStoreReview(int id)
        {
            return ModulesRepository.ModuleExecuteReadOne<StoreReview>(
                "SELECT *, (SELECT Count(ID) FROM [Module].[StoreReview] WHERE [ParentID] = ParentReview.ID) as ChildsCount FROM [Module].[StoreReview] as ParentReview WHERE [ID] = @ID",
                CommandType.Text,
                (reader) =>
                {
                    var review = GetStoreReviewFromReader(reader);
                    review.ChildrenReviews = ModulesRepository.ConvertTo<int>(reader, "ChildsCount") > 0
                                                 ? GetStoreReviewsByParentId(
                                                     ModulesRepository.ConvertTo<int>(reader, "ParentID"))
                                                 : new List<StoreReview>();
                    return review;
                },
                new SqlParameter("@ID", id));
        }

        public static void DeleteStoreReviewsById(int id)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "DELETE FROM [Module].[StoreReview] WHERE [ID] = @ID",
                CommandType.Text,
                new SqlParameter("@ID", id));
        }

        public static void AddStoreReview(StoreReview storeReview)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "INSERT INTO [Module].[StoreReview] ([ParentID],[Rate],[Review],[ReviewerEmail],[ReviewerName],[DateAdded],[Moderated]) VALUES (@ParentID,@Rate,@Review,@ReviewerEmail,@ReviewerName,GETDATE(),@Moderated)",
                CommandType.Text,
                new SqlParameter("@ParentID", storeReview.ParentId == 0 ? DBNull.Value : (object)storeReview.ParentId),
                new SqlParameter("@Rate", storeReview.Rate),
                new SqlParameter("@Review", storeReview.Review),
                new SqlParameter("@ReviewerEmail", storeReview.ReviewerEmail),
                new SqlParameter("@ReviewerName", storeReview.ReviewerName),
                new SqlParameter("@Moderated", storeReview.Moderated));

            if (ModuleSettingsProvider.GetSettingValue<bool>("EnableSendMails", "StoreReviews"))
            {
                var message = ModuleSettingsProvider.GetSettingValue<string>("Format", "StoreReviews");
                message = message.Replace("#NAME#", storeReview.ReviewerName);
                message = message.Replace("#EMAIL#", storeReview.ReviewerEmail);
                message = message.Replace("#REVIEW#", storeReview.Review);

                ModulesService.SendModuleMail(
                    ModuleSettingsProvider.GetSettingValue<string>("Subject", "StoreReviews"),
                    message,
                    ModuleSettingsProvider.GetSettingValue<string>("Email", "StoreReviews"),
                    true);
            }
        }

        public static void UpdateStoreReview(StoreReview storeReview)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "UPDATE [Module].[StoreReview] SET [ParentID]=@ParentID,[Rate]=@Rate,[Review]=@Review,[ReviewerEmail]=@ReviewerEmail,[ReviewerName]=@ReviewerName, [Moderated]=@Moderated, [DateAdded]=@DateAdded WHERE [ID]=@ID",
                CommandType.Text,
                new SqlParameter("@ID", storeReview.Id),
                new SqlParameter("@ParentID", storeReview.ParentId == 0 ? DBNull.Value : (object)storeReview.ParentId),
                new SqlParameter("@Rate", storeReview.Rate),
                new SqlParameter("@Review", storeReview.Review),
                new SqlParameter("@ReviewerEmail", storeReview.ReviewerEmail),
                new SqlParameter("@ReviewerName", storeReview.ReviewerName),
                new SqlParameter("@Moderated", storeReview.Moderated),
                new SqlParameter("@DateAdded", storeReview.DateAdded));
        }


    }
}
