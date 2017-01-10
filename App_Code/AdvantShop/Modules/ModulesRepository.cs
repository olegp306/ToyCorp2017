//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Modules.Interfaces;

namespace AdvantShop.Modules
{
    public class ModulesRepository
    {
        /// <summary>
        /// Get Module From SQLDataReader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Module GetModuleFromReader(SqlDataReader reader)
        {
            return new Module
            {
                StringId = SQLDataHelper.GetString(reader, "ModuleStringID"),
                IsInstall = SQLDataHelper.GetBoolean(reader, "IsInstall"),
                DateAdded = SQLDataHelper.GetDateTime(reader, "DateAdded"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified"),
                Version = SQLDataHelper.GetString(reader, "Version"),
                Active = SQLDataHelper.GetBoolean(reader, "Active"),
            };
        }

        /// <summary>
        /// Add module to datebase and set Install
        /// </summary>
        /// <param name="module"></param>
        public static void InstallModuleToDb(Module module)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF (SELECT COUNT([ModuleStringID]) FROM [dbo].[Modules] WHERE [ModuleStringID] = @ModuleStringID) = 0
                    BEGIN
                        INSERT INTO [dbo].[Modules] ([ModuleStringID],[IsInstall],[DateAdded],[DateModified],[Version],[Active]) VALUES (@ModuleStringID,1,@DateAdded,@DateModified,@Version,@Active)
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[Modules] SET [IsInstall] = 1, [DateModified] = @DateModified, [Version] = @Version WHERE [ModuleStringID] = @ModuleStringID
                    END",
                CommandType.Text,
                new SqlParameter("@ModuleStringID", module.StringId),
                new SqlParameter("@DateAdded", module.DateAdded),
                new SqlParameter("@DateModified", module.DateModified),
                new SqlParameter("@Version", module.Version.IsNullOrEmpty() ? DBNull.Value : (object)module.Version),
                new SqlParameter("@Active", module.Active));
        }

        /// <summary>
        /// Get module from datebase
        /// </summary>
        /// <param name="moduleStringId"></param>
        public static Module GetModuleFromDb(string moduleStringId)
        {
            return SQLDataAccess.ExecuteReadOne<Module>(
                @"SELECT * FROM [dbo].[Modules] WHERE [ModuleStringID] = ModuleStringID",
                CommandType.Text,
                GetModuleFromReader,
                new SqlParameter("@ModuleStringID", moduleStringId));
        }

        /// <summary>
        /// Get all module from datebase
        /// </summary>
        public static List<Module> GetModulesFromDb()
        {
            return SQLDataAccess.ExecuteReadList<Module>(
                @"SELECT * FROM [dbo].[Modules]",
                CommandType.Text,
                GetModuleFromReader);
        }

        /// <summary>
        /// Update module in datebase and set Uninstall
        /// </summary>
        /// <param name="moduleStringId"></param>
        public static void UninstallModuleFromDb(string moduleStringId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"DELETE FROM [dbo].[Modules] WHERE [ModuleStringID] = @ModuleStringID;",
                CommandType.Text,
                new SqlParameter("@ModuleStringID", moduleStringId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleStringId"></param>
        /// <returns></returns>
        public static bool IsInstallModule(string moduleStringId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "SELECT [IsInstall] FROM [dbo].[Modules] WHERE [ModuleStringID] = @ModuleStringID",
                CommandType.Text,
                new SqlParameter("@ModuleStringID", moduleStringId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleStringId"></param>
        /// <returns></returns>
        public static bool IsActiveModule(string moduleStringId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "SELECT [Active] FROM [dbo].[Modules] WHERE [ModuleStringID] = @ModuleStringID and [IsInstall] = 1",
                CommandType.Text,
                new SqlParameter("@ModuleStringID", moduleStringId));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleStringId"></param>
        /// <param name="active"></param>
        public static void SetActiveModule(string moduleStringId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [dbo].[Modules] SET [Active] = @Active WHERE [ModuleStringID] = @ModuleStringID",
                CommandType.Text,
                new SqlParameter("@ModuleStringID", moduleStringId),
                new SqlParameter("@Active", active));

            AttachedModules.LoadModules();

            var module = AttachedModules.GetModules().FirstOrDefault(x => x.Name.ToLower() == moduleStringId.ToLower());
            if (module != null && typeof (IModuleChangeActive).IsAssignableFrom(module))
            {
                var instance = (IModuleChangeActive) Activator.CreateInstance(module, null);
                instance.ModuleChangeActive(active);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="da"></param>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsExistsModuleTable(string schema, string tableName)
        {
            return Convert.ToBoolean(SQLDataAccess.ExecuteScalar(
                @"IF((SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName) > 0) Select 1 ELSE Select 0 ",
                CommandType.Text,
                new SqlParameter("@Schema", schema),
                new SqlParameter("@TableName", tableName)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="da"></param>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public static bool IsExistsModuleProcedure(string procedureName)
        {
            return Convert.ToBoolean(SQLDataAccess.ExecuteScalar(
                @"IF(SELECT Count(name) FROM sysobjects WHERE name = @Procedure AND type = 'P') > 0 Select 1 ELSE Select 0 ",
                CommandType.Text,
                new SqlParameter("@Procedure", procedureName)));
        }


        #region SQL Methods

        public static List<TResult> ModuleExecuteReadList<TResult>(string query, CommandType commandType, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            return SQLDataAccess.ExecuteReadList<TResult>(query, commandType, function, parameters);
        }

        public static TResult ModuleExecuteReadOne<TResult>(string query, CommandType commandType, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            return SQLDataAccess.ExecuteReadOne<TResult>(query, commandType, function, parameters);
        }

        public static TResult ModuleExecuteScalar<TResult>(string query, CommandType commandType, params SqlParameter[] parameters) where TResult : IConvertible
        {
            return SQLDataAccess.ExecuteScalar<TResult>(query, commandType, parameters);
        }

        public static DataTable ModuleExecuteTable(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            return SQLDataAccess.ExecuteTable(query, commandType, parameters);
        }

        public static List<TResult> ModuleExecuteReadColumn<TResult>(string query, CommandType commandType, string columnName, params SqlParameter[] parameters) where TResult : IConvertible
        {
            return SQLDataAccess.ExecuteReadColumn<TResult>(query, commandType, columnName, parameters);
        }

        public static void ModuleExecuteNonQuery(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            SQLDataAccess.ExecuteNonQuery(query, commandType, parameters);
        }

        #endregion

        #region Helpers

        private static bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static T ConvertTo<T>(IDataReader reader, string columnName)
        {
#if !DEBUG
            if (!HasColumn(reader, columnName)) return default(T);
#endif
            int index = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(index))
            {
                //return Nullable.GetUnderlyingType(typeof(T)) != null ? null : 
                return default(T);
            }

            //if nullable - take base type
            Type valueType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return (T)Convert.ChangeType(reader[index], valueType);

        }

        public static T ConvertTo<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        #endregion

    }
}