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

namespace AdvantShop.CMS
{
    public static class VoiceService
    {
        #region Answers
        public static Answer GetAnswer(int answerId)
        {
            var answer = SQLDataAccess.ExecuteReadOne<Answer>("SELECT * FROM [Voice].[Answer] WHERE [AnswerID] = @AnswerID",
                                                         CommandType.Text, GetAnswerFromReader, new SqlParameter("@AnswerID", answerId));
            return answer;
        }

        private static Answer GetAnswerFromReader(SqlDataReader reader)
        {
            return new Answer
                    {
                        AnswerId = SQLDataHelper.GetInt(reader["AnswerID"]),
                        FkidTheme = SQLDataHelper.GetInt(reader["FKIDTheme"]),
                        Name = SQLDataHelper.GetString(reader["Name"]).Trim(),
                        CountVoice = SQLDataHelper.GetInt(reader["CountVoice"]),
                        Sort = SQLDataHelper.GetInt(reader["Sort"]),
                        IsVisible = SQLDataHelper.GetBoolean(reader["IsVisible"]),
                        DateAdded = SQLDataHelper.GetDateTime(reader["DateAdded"]),
                        DateModify = SQLDataHelper.GetDateTime(reader["DateModify"])
                    };
        }

        public static List<Answer> GetAllAnswers(int voiceThemeId)
        {
            List<Answer> answers = SQLDataAccess.ExecuteReadList<Answer>("SELECT * FROM [Voice].[Answer] WHERE [FKIDTheme] = @VoiceThemeID ORDER BY [Sort]",
                                                                 CommandType.Text, GetAnswerFromReader, new SqlParameter("@VoiceThemeID", voiceThemeId));
            return answers;
        }

        public static void InsertAnswer(Answer answer)
        {
            answer.AnswerId = SQLDataAccess.ExecuteScalar<int>(@"INSERT INTO [Voice].[Answer] ([FKIDTheme], [Name], [CountVoice], [Sort], [IsVisible], [DateAdded], [DateModify]) VALUES (@FKIDTheme,  @Name,  @CountVoice,  @Sort,  @IsVisible,  @DateAdded,  @DateModify); SELECT scope_identity();",
                                                 CommandType.Text,
                                                 new SqlParameter("@FKIDTheme", answer.FkidTheme),
                                                 new SqlParameter("@Name", answer.Name),
                                                 new SqlParameter("@CountVoice", answer.CountVoice),
                                                 new SqlParameter("@Sort", answer.Sort),
                                                 new SqlParameter("@IsVisible", answer.IsVisible),
                                                 new SqlParameter("@DateAdded", DateTime.Now),
                                                 new SqlParameter("@DateModify", DateTime.Now));
        }

        public static void AddVote(int answerId)
        {
            SQLDataAccess.ExecuteNonQuery(@"UPDATE [Voice].[Answer] SET [CountVoice] = [CountVoice] + 1 WHERE [AnswerID] = @AnswerID",
                                            CommandType.Text, new SqlParameter("@AnswerID", answerId));
        }

        public static void UpdateAnswer(Answer answer)
        {
            SQLDataAccess.ExecuteNonQuery(@"UPDATE [Voice].[Answer] SET [FKIDTheme] = @FKIDTheme,[Name] = @Name,[CountVoice] = @CountVoice, [Sort] = @Sort, [IsVisible] = @IsVisible, [DateModify] = @DateModify WHERE [AnswerID] = @AnswerID",
                                            CommandType.Text,
                                            new SqlParameter("@AnswerID", answer.AnswerId),
                                            new SqlParameter("@FKIDTheme", answer.FkidTheme),
                                            new SqlParameter("@Name", answer.Name),
                                            new SqlParameter("@CountVoice", answer.CountVoice),
                                            new SqlParameter("@Sort", answer.Sort),
                                            new SqlParameter("@IsVisible", answer.IsVisible),
                                            new SqlParameter("@DateModify", DateTime.Now));
        }

        public static void DeleteAnswer(int answerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Voice].[Answer] WHERE [AnswerID] = @AnswerID", CommandType.Text, new SqlParameter("@AnswerID", answerId));
        }
        #endregion Answers

        #region VoiceTheme

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetVoiceThemesCount()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT COUNT([VoiceThemeID]) FROM [Voice].[Answer]", CommandType.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="voiceThemeId"></param>
        /// <returns></returns>
        public static VoiceTheme GetVoiceThemeById(int voiceThemeId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT [VoiceThemeID], [PSYID], [Name], [IsHaveNullVoice], [IsDefault], [IsClose], [DateAdded], [DateModify], (SELECT SUM([CountVoice]) FROM [Voice].[Answer] WHERE (FKIDTheme = [Voice].[VoiceTheme].[VoiceThemeID]) AND (IsVisible = 1)) AS [CountVoice] FROM [Voice].[VoiceTheme] WHERE [VoiceThemeID] = @VoiceThemeID ORDER BY [IsDefault] DESC, [PSYID] ASC",
                CommandType.Text, 
                GetVoiceThemeFromReader, 
                new SqlParameter("@VoiceThemeID", voiceThemeId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static VoiceTheme GetVoiceThemeFromReader(SqlDataReader reader)
        {
            return new VoiceTheme
                       {
                           VoiceThemeId = SQLDataHelper.GetInt(reader["VoiceThemeID"]),
                           PsyId = SQLDataHelper.GetInt(reader["PSYID"]),
                           Name = SQLDataHelper.GetString(reader["Name"]).Trim(),
                           IsHaveNullVoice = SQLDataHelper.GetBoolean(reader["IsHaveNullVoice"]),
                           IsDefault = SQLDataHelper.GetBoolean(reader["IsDefault"]),
                           IsClose = SQLDataHelper.GetBoolean(reader["IsClose"]),
                           DateAdded = SQLDataHelper.GetDateTime(reader["DateAdded"]),
                           DateModify = SQLDataHelper.GetDateTime(reader["DateModify"]),
                           CountVoice = SQLDataHelper.GetInt(reader["CountVoice"])
                       };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static VoiceTheme GetTopTheme()
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP (1) [VoiceThemeID], [PSYID], [Name], [IsHaveNullVoice], [IsDefault], [IsClose], [DateAdded], [DateModify], (SELECT SUM([CountVoice]) FROM [Voice].[Answer] WHERE (FKIDTheme = [Voice].[VoiceTheme].[VoiceThemeID]) AND (IsVisible = 1)) AS [CountVoice] FROM [Voice].[VoiceTheme] ORDER BY [IsDefault] DESC, [PSYID] ASC",
                CommandType.Text, 
                GetVoiceThemeFromReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<int> GetThemeIDs()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [VoiceThemeID] FROM [Voice].[VoiceTheme]",
                CommandType.Text,
                "VoiceThemeID");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<VoiceTheme> GetAllVoiceThemes()
        {
            return SQLDataAccess.ExecuteReadList<VoiceTheme>(
                "SELECT [VoiceThemeID], [PSYID], [Name], [IsHaveNullVoice], [IsDefault], [IsClose], [DateAdded], [DateModify], (SELECT SUM([CountVoice]) FROM [Voice].[Answer] WHERE (FKIDTheme = [Voice].[VoiceTheme].[VoiceThemeID]) AND (IsVisible = 1)) AS [CountVoice] FROM [Voice].[VoiceTheme] ORDER BY [IsDefault] DESC, [PSYID] ASC",
                CommandType.Text, 
                GetVoiceThemeFromReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="voiceTheme"></param>
        public static void AddTheme(VoiceTheme voiceTheme)
        {
            if (voiceTheme.IsDefault)
            {
                ResetIsDefault();
            }
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Voice].[VoiceTheme] ([PsyID], [Name], [IsDefault], [IsHaveNullVoice], [IsClose], [DateAdded], [DateModify]) VALUES ( @PsyID, @Name, @IsDefault, @IsHaveNullVoice, @IsClose, GETDATE(), GETDATE())",
                                            CommandType.Text,
                                            new SqlParameter("@Name", voiceTheme.Name),
                                            new SqlParameter("@PsyID", voiceTheme.PsyId),
                                            new SqlParameter("@IsDefault", voiceTheme.IsDefault),
                                            new SqlParameter("@IsHaveNullVoice", voiceTheme.IsHaveNullVoice),
                                            new SqlParameter("@IsClose", voiceTheme.IsClose)
                                            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="voiceTheme"></param>
        public static void UpdateTheme(VoiceTheme voiceTheme)
        {
            if (voiceTheme.IsDefault)
            {
                ResetIsDefault();
            }
            SQLDataAccess.ExecuteNonQuery("Update [Voice].[VoiceTheme] set [PsyID]=@PsyID, [Name]=@Name, [IsDefault]=@IsDefault, [IsHaveNullVoice]=@IsHaveNullVoice, [IsClose]=@IsClose, [DateModify]=GetDate() where VoiceThemeId = @VoiceThemeId",
                                            CommandType.Text,
                                            new SqlParameter("@PsyID", voiceTheme.PsyId),
                                            new SqlParameter("@Name", voiceTheme.Name),
                                            new SqlParameter("@IsDefault", voiceTheme.IsDefault),
                                            new SqlParameter("@IsHaveNullVoice", voiceTheme.IsHaveNullVoice),
                                            new SqlParameter("@IsClose", voiceTheme.IsClose),
                                            new SqlParameter("@VoiceThemeId", voiceTheme.VoiceThemeId)
                                            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteTheme(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Voice].[VoiceTheme] WHERE [VoiceThemeID] = @VoiceThemeID", 
                CommandType.Text, 
                new SqlParameter("@VoiceThemeID", id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="themeId"></param>
        /// <returns></returns>
        public static string GetVotingName(int themeId)
        {
            return SQLDataAccess.ExecuteScalar<string>(
                "SELECT [Name] FROM [Voice].[VoiceTheme] WHERE [VoiceThemeID] = @Theme", 
                CommandType.Text, 
                new SqlParameter("@Theme", themeId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ResetIsDefault()
        {
            return SQLDataAccess.ExecuteScalar<string>(
                "Update [Voice].[VoiceTheme] SET [IsDefault] = 0 ", 
                CommandType.Text);
        }
        #endregion VoiceTheme
    }
}