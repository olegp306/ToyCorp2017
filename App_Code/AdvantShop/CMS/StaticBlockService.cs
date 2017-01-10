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
using AdvantShop.Core;
using AdvantShop.Core.Caching;

namespace AdvantShop.CMS
{
    public class StaticBlockService
    {
        public static StaticBlock GetPagePartByKeyWithCache(string key)
        {
            var cacheName = CacheNames.GetStaticBlockCacheObjectName(key);
            if (CacheManager.Contains(cacheName))
                return CacheManager.Get<StaticBlock>(cacheName);
            var item = GetPagePartByKey(key);
            if (item != null)
                CacheManager.Insert(cacheName, item);
            else
                CacheManager.Remove(cacheName);
            return item;
        }

        public static StaticBlock GetPagePartByKey(string key)
        {
            return SQLDataAccess.ExecuteReadOne<StaticBlock>(
                "SELECT [StaticBlockID], [Key], [InnerName], [Content], [Added], [Modified],Enabled FROM [CMS].[StaticBlock] WHERE [Key]=@key",
                CommandType.Text,
                GetPagePartFromReader,
                new SqlParameter("@key", key));
        }

        public static StaticBlock GetPagePart(int id)
        {
            return SQLDataAccess.ExecuteReadOne<StaticBlock>(
                "SELECT [StaticBlockID], [Key], [InnerName], [Content], [Added], [Modified],Enabled FROM [CMS].[StaticBlock] WHERE [StaticBlockID]=@StaticBlockID",
                CommandType.Text,
                GetPagePartFromReader,
                new SqlParameter("@StaticBlockID", id));
        }

        public static bool IsPagePartKeyExist(int id, string key)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(*) FROM [CMS].[StaticBlock] WHERE [Key] = @Key AND [StaticBlockID] <> @StaticBlockID",
                CommandType.Text,
                new[]
                    {
                        new SqlParameter("@Key", key), 
                        new SqlParameter("@StaticBlockID", id)
                    })) > 0;
        }

        private static StaticBlock GetPagePartFromReader(SqlDataReader reader)
        {
            return new StaticBlock(SQLDataHelper.GetInt(reader, "StaticBlockID"))
                       {
                           Key = SQLDataHelper.GetString(reader, "key"),
                           InnerName = SQLDataHelper.GetString(reader, "InnerName"),
                           Content = SQLDataHelper.GetString(reader, "Content"),
                           Added = SQLDataHelper.GetDateTime(reader, "Added"),
                           Modified = SQLDataHelper.GetDateTime(reader, "Modified"),
                           Enabled = SQLDataHelper.GetBoolean(reader, "Enabled")
                       };
        }

        public static int AddStaticBlock(StaticBlock part)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "INSERT INTO [CMS].[StaticBlock] ([Key], [InnerName], [Content], [Added], [Modified],Enabled) VALUES (@Key, @InnerName, @Content, GETDATE(),GETDATE(),@Enabled); SELECT scope_identity();",
                CommandType.Text,
                new[]
                        {
                            new SqlParameter("@Key", part.Key),
                            new SqlParameter("@InnerName", part.InnerName),
                            new SqlParameter("@Content", part.Content),
                            new SqlParameter( "@Enabled", part .Enabled )
                        }
                ));
        }

        public static bool UpdatePagePart(StaticBlock part)
        {
            StaticBlock sb = GetPagePart(part.StaticBlockId);
            var cacheName = CacheNames.GetStaticBlockCacheObjectName(sb.Key);
            if (CacheManager.Contains(cacheName))
                CacheManager.Remove(cacheName);

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [CMS].[StaticBlock] SET [Key] = @Key, [InnerName] = @InnerName, [Content] = @Content, [Modified] = GETDATE(), Enabled =@Enabled WHERE [StaticBlockID] = @StaticBlockID",
                CommandType.Text,
                new[]
                        {
                            new SqlParameter("@StaticBlockID", part.StaticBlockId), 
                            new SqlParameter("@Key", part.Key),
                            new SqlParameter("@InnerName", part.InnerName),
                            new SqlParameter("@Content", part.Content),
                            new SqlParameter("@Enabled", part.Enabled)
                        }
                );

            cacheName = CacheNames.GetStaticBlockCacheObjectName(part.Key);
            CacheManager.Insert(cacheName, part);

            return true;
        }

        public static void DeleteBlock(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [CMS].[StaticBlock] WHERE [StaticBlockID] = @StaticBlockID", CommandType.Text, new SqlParameter("@StaticBlockID", id));
        }

        public static List<string> GetIDs()
        {
            List<string> result = SQLDataAccess.ExecuteReadList<string>("SELECT [StaticBlockID] FROM [CMS].[StaticBlock]", CommandType.Text,
                                                                reader => SQLDataHelper.GetInt(reader, "StaticBlockID").ToString());
            return result;
        }

        public static void SetStaticBlockActivity(int staticBlockId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery("Update [CMS].[StaticBlock] Set Enabled = @Enabled Where [StaticBlockID] = @StaticBlockID",
                                            CommandType.Text, new SqlParameter("@StaticBlockID", staticBlockId), new SqlParameter("@Enabled", active));
        }
    }
}