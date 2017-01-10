//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.SEO
{
    public enum MetaType
    {
        Default,
        Product,
        Category,
        News,
        NewsCategory,
        StaticPage,
        Brand
    }

    public static class MetaInfoService
    {
        public static MetaInfo GetFormatedMetaInfo(MetaInfo meta, string name, string categoryName = null, string brandName = null, string price = null)
        {
            if (meta != null)
            {
                if (string.IsNullOrEmpty(meta.Title))
                {
                    meta.Title = SettingsSEO.GetDefaultTitle(meta.Type) ?? SettingsSEO.DefaultMetaTitle;
                }
                if (string.IsNullOrEmpty(meta.MetaKeywords))
                {
                    meta.MetaKeywords = SettingsSEO.GetDefaultMetaKeywords(meta.Type) ?? SettingsSEO.DefaultMetaKeywords;
                }
                if (string.IsNullOrEmpty(meta.MetaDescription))
                {
                    meta.MetaDescription = SettingsSEO.GetDefaultMetaDescription(meta.Type) ?? SettingsSEO.DefaultMetaKeywords;
                }
                if (string.IsNullOrEmpty(meta.H1))
                {
                    meta.H1 = SettingsSEO.GetDefaultH1(meta.Type) ??SettingsSEO.DefaultH1;
                }

                meta.Title = GlobalStringVariableService.TranslateExpression(meta.Title, meta.Type, name, categoryName, brandName, price);
                meta.MetaKeywords = GlobalStringVariableService.TranslateExpression(meta.MetaKeywords, meta.Type, name, categoryName, brandName, price);
                meta.MetaDescription = GlobalStringVariableService.TranslateExpression(meta.MetaDescription, meta.Type, name, categoryName, brandName, price);
                meta.H1 = GlobalStringVariableService.TranslateExpression(meta.H1, meta.Type, name, categoryName, brandName, price);
            }
            return meta;
        }

        /// <summary>
        /// Get metainfo by metaid and type
        /// </summary>
        /// <param name="metaid"></param>
        /// <returns></returns>
        public static MetaInfo GetMetaInfo(int metaid)
        {

            return SQLDataAccess.ExecuteReadOne<MetaInfo>("Select * from SEO.MetaInfo where MetaID=@MetaID",
                                                          CommandType.Text,
                                                          GetFromReader,
                                                          new SqlParameter("@MetaID", metaid));
        }

        public static MetaInfo GetMetaInfo(int objId, MetaType type)
        {
            return SQLDataAccess.ExecuteReadOne<MetaInfo>(
                "Select * from SEO.MetaInfo where ObjId=@objId and Type=@type", CommandType.Text,
                GetFromReader,
                new SqlParameter {ParameterName = "@objId", Value = objId},
                new SqlParameter {ParameterName = "@type", Value = type.ToString()}
                );
        }

        public static MetaInfo GetFromReader(SqlDataReader reader)
        {
            return new MetaInfo(SQLDataHelper.GetInt(reader, "MetaID"),
                                SQLDataHelper.GetInt(reader, "ObjId"),
                                (MetaType)Enum.Parse(typeof(MetaType), SQLDataHelper.GetString(reader, "Type"), true),
                                SQLDataHelper.GetString(reader, "Title"),
                                SQLDataHelper.GetString(reader, "MetaKeywords"),
                                SQLDataHelper.GetString(reader, "MetaDescription"),
                                SQLDataHelper.GetString(reader, "H1"));
        }

        /// <summary>
        /// Get default metainfo
        /// </summary>
        /// <returns></returns>
        public static MetaInfo GetDefaultMetaInfo()
        {
            return GetDefaultMetaInfo(MetaType.Default, string.Empty);
        }

        public static MetaInfo GetDefaultMetaInfo(MetaType metaType, string h1)
        {
            return new MetaInfo(0, 0, metaType, SettingsSEO.GetDefaultTitle(metaType), SettingsSEO.GetDefaultMetaKeywords(metaType), SettingsSEO.GetDefaultMetaDescription(metaType), SettingsSEO.GetDefaultH1(metaType));
        }


        public static void SetMeta(MetaInfo meta)
        {
            if (IsMetaExist(meta.ObjId, meta.Type))
            {
                UpdateMetaInfo(meta);
            }
            else
            {
                meta.MetaId = InsertMetaInfo(meta);
            }
        }

        public static bool IsMetaExist(int objId, MetaType type)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(MetaID) from [SEO].[MetaInfo] where ObjId=@ObjId and Type=@Type", CommandType.Text,
                                                    new SqlParameter("@ObjId", objId),
                                                    new SqlParameter("@Type", type.ToString())) > 0;
        }

        private static int InsertMetaInfo(MetaInfo meta)
        {
            var id = SQLDataAccess.ExecuteScalar<int>("[SEO].[sp_AddMetaInfo]", CommandType.StoredProcedure,
                                                      new SqlParameter("@Title", meta.Title ?? SettingsSEO.DefaultMetaTitle),
                                                      new SqlParameter("@MetaKeywords", meta.MetaKeywords ?? SettingsSEO.DefaultMetaKeywords),
                                                      new SqlParameter("@MetaDescription", meta.MetaDescription ?? SettingsSEO.DefaultMetaDescription),
                                                      new SqlParameter("@H1", meta.H1 ?? SettingsSEO.DefaultH1 ?? string.Empty),
                                                      new SqlParameter("@ObjId", meta.ObjId),
                                                      new SqlParameter("@Type", meta.Type.ToString()));
            return id;
        }

        private static void UpdateMetaInfo(MetaInfo meta)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [SEO].[MetaInfo] SET [Title] = @Title, [MetaKeywords] = @MetaKeywords,[MetaDescription] = @MetaDescription, [H1]=@H1 where ObjId=@ObjId and Type=@Type",
                CommandType.Text,
                new SqlParameter("@Title", meta.Title ?? SettingsSEO.DefaultMetaTitle),
                new SqlParameter("@MetaKeywords", meta.MetaKeywords ?? SettingsSEO.DefaultMetaKeywords),
                new SqlParameter("@MetaDescription", meta.MetaDescription ?? SettingsSEO.DefaultMetaDescription),
                new SqlParameter("@H1", meta.H1 ?? SettingsSEO.DefaultH1 ?? string.Empty),
                new SqlParameter("@ObjId", meta.ObjId),
                new SqlParameter("@Type", meta.Type.ToString()));
        }

        public static void DeleteMetaInfo(int metaId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [SEO].[MetaInfo] WHERE MetaID=@MetaID", CommandType.Text,
                                          new SqlParameter("@MetaID", metaId));
        }

        public static void DeleteMetaInfo(int objId, MetaType type)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [SEO].[MetaInfo] WHERE ObjId=@objId and Type=@type",
                                          CommandType.Text,
                                          new SqlParameter("@objId", objId),
                                          new SqlParameter("@type", type.ToString()));
        }
    }
}
