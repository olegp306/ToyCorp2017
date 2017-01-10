//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.CMS
{
    public class MenuService
    {
        public enum EMenuType
        {
            Top,
            Bottom
        }

        private static readonly Dictionary<EMenuType, string> MenuTypeTables = new Dictionary<EMenuType, string>
                                                                                         {
                                                                                             {EMenuType.Top,"[CMS].[MainMenu]"},
                                                                                             {EMenuType.Bottom,"[CMS].[BottomMenu]"}
                                                                                         };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItem"></param>
        /// <param name="menuType"> </param>
        /// <returns></returns>
        public static int AddMenuItem(AdvMenuItem mItem, EMenuType menuType)
        {
            return SQLDataAccess.ExecuteScalar<int>(string.Format(
                        "INSERT INTO {0} (MenuItemParentID, MenuItemName, MenuItemIcon, MenuItemUrlPath, MenuItemUrlType, SortOrder, ShowMode, Enabled, Blank, NoFollow) VALUES (@MenuItemParentID, @MenuItemName, @MenuItemIcon, @MenuItemUrlPath, @MenuItemUrlType, @SortOrder, @ShowMode, @Enabled, @Blank, @NoFollow); SELECT scope_identity();", MenuTypeTables[menuType]),
                        CommandType.Text,
                        new SqlParameter("@MenuItemParentID", mItem.MenuItemParentID == 0 ? DBNull.Value : (object)mItem.MenuItemParentID),
                        new SqlParameter("@MenuItemName", mItem.MenuItemName),
                        new SqlParameter("@MenuItemIcon", string.IsNullOrEmpty(mItem.MenuItemIcon) ? DBNull.Value : (object)mItem.MenuItemIcon),
                        new SqlParameter("@MenuItemUrlPath", mItem.MenuItemUrlPath),
                        new SqlParameter("@MenuItemUrlType", mItem.MenuItemUrlType),
                        new SqlParameter("@SortOrder", mItem.SortOrder),
                        new SqlParameter("@Blank", mItem.Blank),
                        new SqlParameter("@ShowMode", mItem.ShowMode),
                        new SqlParameter("@Enabled", mItem.Enabled),
                        new SqlParameter("@NoFollow", mItem.NoFollow));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static AdvMenuItem GetMenuItemFromReader(SqlDataReader reader)
        {
            return new AdvMenuItem
                       {
                           MenuItemID = SQLDataHelper.GetInt(reader, "MenuItemID"),
                           MenuItemParentID = SQLDataHelper.GetInt(reader, "MenuItemParentID"),
                           MenuItemName = SQLDataHelper.GetString(reader, "MenuItemName"),
                           MenuItemIcon = SQLDataHelper.GetString(reader, "MenuItemIcon"),
                           MenuItemUrlPath = SQLDataHelper.GetString(reader, "MenuItemUrlPath"),
                           MenuItemUrlType = (EMenuItemUrlType)SQLDataHelper.GetInt(reader, "MenuItemUrlType"),
                           SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                           ShowMode = (EMenuItemShowMode)SQLDataHelper.GetInt(reader, "ShowMode"),
                           Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                           Blank = SQLDataHelper.GetBoolean(reader, "Blank"),
                           NoFollow = SQLDataHelper.GetBoolean(reader, "NoFollow")
                       };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItemId"></param>
        /// <param name="menuType"> </param>
        /// <returns></returns>
        public static AdvMenuItem GetMenuItemById(int mItemId, EMenuType menuType)
        {
            return SQLDataAccess.ExecuteReadOne<AdvMenuItem>(
                   string.Format("SELECT * FROM {0} WHERE MenuItemID = @MenuItemID", MenuTypeTables[menuType]),
                   CommandType.Text,
                   GetMenuItemFromReader,
                   new SqlParameter("@MenuItemID", mItemId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AdvMenuItem> GetMenuItems(EMenuType menuType)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<AdvMenuItem>(
                string.Format("SELECT * FROM {0}", MenuTypeTables[menuType]),
                CommandType.Text,
                GetMenuItemFromReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItemId"></param>
        /// <param name="menuType"> </param>
        /// <returns></returns>
        public static IEnumerable<int> GetParentMenuItems(int mItemId, EMenuType menuType)
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>(
                "[CMS].[sp_GetParentMenuItemsByItemId]",
                CommandType.StoredProcedure,
                "MenuItemID",
                new SqlParameter("@MenuItemID", mItemId),
                new SqlParameter("@MenuType", menuType.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItem"></param>
        /// <param name="menuType"> </param>
        public static void UpdateMenuItem(AdvMenuItem mItem, EMenuType menuType)
        {
            SQLDataAccess.ExecuteNonQuery(
                "[CMS].[sp_UpdateMenuItemByItemId]",
                CommandType.StoredProcedure,
                new SqlParameter("@MenuType", menuType.ToString()),
                new SqlParameter("@MenuItemID", mItem.MenuItemID),
                new SqlParameter("@MenuItemParentID", mItem.MenuItemParentID == 0 ? DBNull.Value : (object)mItem.MenuItemParentID),
                new SqlParameter("@MenuItemName", mItem.MenuItemName),
                new SqlParameter("@MenuItemIcon", string.IsNullOrEmpty(mItem.MenuItemIcon) ? DBNull.Value : (object)mItem.MenuItemIcon),
                new SqlParameter("@MenuItemUrlPath", mItem.MenuItemUrlPath),
                new SqlParameter("@MenuItemUrlType", mItem.MenuItemUrlType),
                new SqlParameter("@SortOrder", mItem.SortOrder),
                new SqlParameter("@ShowMode", mItem.ShowMode),
                new SqlParameter("@Enabled", mItem.Enabled),
                new SqlParameter("@Blank", mItem.Blank),
                new SqlParameter("@NoFollow", mItem.NoFollow));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItemId"></param>
        /// <param name="menuType"> </param>
        public static void DeleteMenuItemById(int mItemId, EMenuType menuType)
        {
            SQLDataAccess.ExecuteNonQuery(
                string.Format("Delete From {0} where MenuItemID = @MenuItemID", MenuTypeTables[menuType]),
                CommandType.Text,
                new SqlParameter("@MenuItemID", mItemId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItemId"></param>
        /// <param name="menuType"> </param>
        public static void DeleteMenuItemIconById(int mItemId, EMenuType menuType, string filePath)
        {
            SQLDataAccess.ExecuteNonQuery(
                string.Format("Update {0} Set MenuItemIcon = @MenuItemIcon Where MenuItemID = @MenuItemID", MenuTypeTables[menuType]),
                CommandType.Text,
                new SqlParameter("@MenuItemID", mItemId),
                new SqlParameter("@MenuItemIcon", DBNull.Value));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="menuType"></param>
        /// <returns></returns>
        public static List<int> GetAllChildIdByParent(int parent, EMenuType menuType)
        {

            return SQLDataAccess.ExecuteReadColumn<int>(
                 "[CMS].[sp_GetChildMenuItemByParent]",
                 CommandType.StoredProcedure,
                 "MenuItemID",
                 new SqlParameter("@MenuType", menuType.ToString()),
                 new SqlParameter("@ParentId", parent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItemParentId"></param>
        /// <param name="menuType"></param>
        /// <returns></returns>
        public static IEnumerable<AdvMenuItem> GetChildMenuItemsByParentId(int mItemParentId, EMenuType menuType)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<AdvMenuItem>(
                 string.Format("SELECT MenuItemID, MenuItemParentID, MenuItemName, MenuItemIcon, MenuItemUrlPath, MenuItemUrlType, SortOrder, ShowMode, Enabled, Blank, NoFollow, (SELECT Count(MenuItemID) FROM {0} AS c WHERE c.MenuItemParentID = p.MenuItemID) as Child_Count FROM {0} as p WHERE  {1} order by [SortOrder]", MenuTypeTables[menuType], mItemParentId == 0 ? "[MenuItemParentID] is Null" : "[MenuItemParentID] = " + mItemParentId),
                  CommandType.Text,
                  (reader) =>
                  {
                      var mItem = GetMenuItemFromReader(reader);
                      mItem.HasChild = SQLDataHelper.GetInt(reader, "Child_Count") > 0;
                      return mItem;
                  });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItemParentId"></param>
        /// <param name="menuType"></param>
        /// <param name="showMode"></param>
        /// <returns></returns>
        public static IEnumerable<AdvMenuItem> GetEnabledChildMenuItemsByParentId(int mItemParentId, EMenuType menuType, EMenuItemShowMode showMode)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<AdvMenuItem>(
                 string.Format("SELECT MenuItemID, MenuItemParentID, MenuItemName, MenuItemIcon, MenuItemUrlPath, MenuItemUrlType, SortOrder, ShowMode, Enabled, Blank, NoFollow, (SELECT Count(MenuItemID) FROM {0} AS c WHERE c.MenuItemParentID = p.MenuItemID) as Child_Count FROM {0} as p WHERE  {1} AND (ShowMode = 0 OR ShowMode = @ShowMode) AND Enabled = 1 order by [SortOrder] ",
                    MenuTypeTables[menuType],
                    mItemParentId == 0 ? "[MenuItemParentID] is Null" : "[MenuItemParentID] = " + mItemParentId),
                  CommandType.Text,
                  (reader) =>
                  {
                      var mItem = GetMenuItemFromReader(reader);
                      mItem.HasChild = SQLDataHelper.GetInt(reader, "Child_Count") > 0;
                      return mItem;
                  },
                  new SqlParameter("@showMode", (int)showMode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mItemParentId"></param>
        /// <param name="menuType"></param>
        /// <param name="showMode"></param>
        /// <returns></returns>
        public static IEnumerable<AdvMenuItem> GetChildMenuItemsByParentId(int mItemParentId, EMenuType menuType, EMenuItemShowMode showMode)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<AdvMenuItem>(
                 string.Format("SELECT MenuItemID, MenuItemParentID, MenuItemName, MenuItemIcon, MenuItemUrlPath, MenuItemUrlType, SortOrder, ShowMode, Enabled, Blank, NoFollow, (SELECT Count(MenuItemID) FROM {0} AS c WHERE c.MenuItemParentID = p.MenuItemID) as Child_Count FROM {0} as p WHERE {1} AND (ShowMode = 0 OR ShowMode = @ShowMode) AND Enabled = 1 order by [SortOrder]", MenuTypeTables[menuType], mItemParentId == 0 ? "[MenuItemParentID] is Null" : "[MenuItemParentID] = " + mItemParentId),
                  CommandType.Text,
                  (reader) =>
                  {
                      var mItem = GetMenuItemFromReader(reader);
                      mItem.HasChild = SQLDataHelper.GetInt(reader, "Child_Count") > 0;
                      return mItem;
                  },
                  new SqlParameter("@showMode", (int)showMode));
        }
    }
}