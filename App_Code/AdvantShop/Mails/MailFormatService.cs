//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Mails
{
    public class MailFormatService
    {
        #region Get/Update/Delete MailFormat

        private static MailFormat GetMailFormatFromReader(SqlDataReader reader)
        {
            return new MailFormat
                {
                    MailFormatID = SQLDataHelper.GetInt(reader, "MailFormatID"),
                    FormatName = SQLDataHelper.GetString(reader, "FormatName"),
                    FormatSubject = SQLDataHelper.GetString(reader, "FormatSubject"),
                    FormatText = SQLDataHelper.GetString(reader, "FormatText"),
                    FormatType = (MailType) SQLDataHelper.GetInt(reader, "FormatType"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    Enable = SQLDataHelper.GetBoolean(reader, "Enable"),
                    AddDate = SQLDataHelper.GetDateTime(reader, "AddDate"),
                    ModifyDate = SQLDataHelper.GetDateTime(reader, "ModifyDate")
                };
        }

        public static MailFormat Get(int mailFormatId)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Settings].[MailFormat] WHERE MailFormatID = @MailFormatID", CommandType.Text,
                                                GetMailFormatFromReader, new SqlParameter("@MailFormatID", mailFormatId));
        }

        public static MailFormat GetByType(int formatType)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT Top(1)* FROM [Settings].[MailFormat] WHERE FormatType = @FormatType And Enable=1",
                    CommandType.Text, GetMailFormatFromReader, new SqlParameter("@FormatType", formatType));
        }

        public static void Update(MailFormat mailFormat)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Settings].[MailFormat] SET FormatName = @FormatName, FormatSubject = @FormatSubject, FormatText = @FormatText, FormatType = @FormatType, SortOrder = @SortOrder, Enable = @Enable, ModifyDate = GETDATE() WHERE (MailFormatID = @MailFormatID)",
                CommandType.Text,
                new SqlParameter("@MailFormatID", mailFormat.MailFormatID),
                new SqlParameter("@FormatName", mailFormat.FormatName),
                new SqlParameter("@FormatSubject", mailFormat.FormatSubject),
                new SqlParameter("@FormatText", mailFormat.FormatText),
                new SqlParameter("@FormatType", (int) mailFormat.FormatType),
                new SqlParameter("@SortOrder", mailFormat.SortOrder),
                new SqlParameter("@Enable", mailFormat.Enable),
                new SqlParameter("@ModifyDate", DateTime.Now));
        }

        public static void Add(MailFormat mailFormat)
        {
            mailFormat.MailFormatID =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Settings].[MailFormat] (FormatName, FormatSubject, FormatText, FormatType, SortOrder, Enable, AddDate, ModifyDate ) VALUES (@FormatName, @FormatSubject, @FormatText, @FormatType, @SortOrder, @Enable, GETDATE(), GETDATE()); SELECT SCOPE_IDENTITY()",
                    CommandType.Text,
                    new SqlParameter("@FormatName", mailFormat.FormatName),
                    new SqlParameter("@FormatSubject", mailFormat.FormatSubject),
                    new SqlParameter("@FormatText", mailFormat.FormatText),
                    new SqlParameter("@FormatType", (int) mailFormat.FormatType),
                    new SqlParameter("@SortOrder", mailFormat.SortOrder),
                    new SqlParameter("@Enable", mailFormat.Enable));
        }

        public static void Delete(int mailFormatId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[MailFormat] WHERE MailFormatID = @MailFormatID",
                                            CommandType.Text,
                                            new SqlParameter("@MailFormatID", mailFormatId));
        }

        #endregion

        #region Get/Update/Delete MailFormatType

        public static MailFormatType GetMailFormatType(int mailFormatTypeId)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Settings].[MailFormatType] WHERE MailFormatTypeID = @MailFormatTypeID",
                                                        CommandType.Text,
                                                        reader => new MailFormatType
                                                        {
                                                            MailFormatTypeID = SQLDataHelper.GetInt(reader, "MailFormatTypeID"),
                                                            TypeName = SQLDataHelper.GetString(reader, "TypeName"),
                                                            SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                                                            Comment = SQLDataHelper.GetString(reader, "Comment"),
                                                        }, 
                                                        new SqlParameter("@MailFormatTypeID", mailFormatTypeId));
        }

        public static void UpdateMailFormatType(MailFormatType mailFormatType)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[MailFormatType] SET TypeName = @TypeName, SortOrder = @SortOrder, Comment = @Comment WHERE (MailFormatTypeID = @MailFormatTypeID)",
                                        CommandType.Text,
                                        new SqlParameter("@MailFormatTypeID", mailFormatType.MailFormatTypeID),
                                        new SqlParameter("@TypeName", mailFormatType.TypeName),
                                        new SqlParameter("@SortOrder", mailFormatType.SortOrder),
                                        new SqlParameter("@Comment", mailFormatType.Comment)
                                        );
        }

        public static void InsertMailFormatType(MailFormatType mailFormatType)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Settings].[MailFormatType] (TypeName, SortOrder, Comment) VALUES (@TypeName, @SortOrder, @Comment); SELECT SCOPE_IDENTITY()",
                                            CommandType.Text,
                                            new SqlParameter("@TypeName", mailFormatType.TypeName),
                                            new SqlParameter("@SortOrder", mailFormatType.SortOrder),
                                            new SqlParameter("@Comment", mailFormatType.Comment));
        }

        public static void DeleteMailFormatType(int mailFormatTypeId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[MailFormatType] WHERE MailFormatTypeID = @MailFormatTypeID",
                                            CommandType.Text, new SqlParameter("@MailFormatTypeID", mailFormatTypeId));
        }

        #endregion
    }
}