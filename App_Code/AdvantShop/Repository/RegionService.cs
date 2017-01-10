//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Repository
{
    public class RegionService
    {
        public static List<ListItem> GetRegions(string countryId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT [RegionName], [RegionID] FROM [Customers].[Region] WHERE CountryID = @Id ORDER BY [RegionSort]",
                CommandType.Text,
                reader => new ListItem(SQLDataHelper.GetString(reader, "RegionName"),
                                       SQLDataHelper.GetInt(reader, "RegionID").ToString()),
                new SqlParameter("@Id", countryId));
        }

        public static int GetRegionIdByName(string regionName)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT RegionID FROM Customers.Region WHERE RegionName = @name",
                                                      CommandType.Text, new SqlParameter("@name", regionName));
        }

        public static List<int> GetAllRegionIDByCountry(int countryID)
        {
            return
                SQLDataAccess.ExecuteReadList<int>(
                    "SELECT RegionID FROM [Customers].[Region] WHERE [CountryID] = @CountryID",
                    CommandType.Text,
                    reader => SQLDataHelper.GetInt(reader, "RegionID"),
                    new SqlParameter("@CountryID", countryID));
        }

        public static void UpdateRegion(Region region)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Region] set RegionName=@name, RegionCode=@RegionCode, RegionSort=@RegionSort where RegionID = @id",
                CommandType.Text,
                new SqlParameter("@id", region.RegionID),
                new SqlParameter("@name", region.Name),
                new SqlParameter("@RegionCode", region.RegionCode ?? (object) DBNull.Value),
                new SqlParameter("@RegionSort", region.SortOrder));
        }

        public static void InsertRegion(Region region)
        {
            region.RegionID =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Customers].[Region] (RegionName, RegionCode, RegionSort, CountryID) VALUES (@Name, @RegionCode, @Sort, @CountryID);SELECT scope_identity();",
                    CommandType.Text, new SqlParameter("@Name", region.Name),
                    new SqlParameter("@RegionCode", region.RegionCode ?? (object)DBNull.Value),
                    new SqlParameter("@Sort", region.SortOrder),
                    new SqlParameter("@CountryID", region.CountryID));
        }

        public static void DeleteRegion(int regionId)
        {
            if (regionId != SettingsMain.SellerRegionId)
            {
                SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.Region WHERE RegionID = @RegionID",
                                              CommandType.Text,
                                              new SqlParameter("@regionID", regionId));
            }
        }

        public static Region GetRegion(int regionId)
        {
            return
                SQLDataAccess.ExecuteReadOne<Region>("SELECT * FROM [Customers].[Region] WHERE [RegionID] = @regionId",
                    CommandType.Text, ReadRegion,
                    new SqlParameter("@regionId", regionId));
        }

        public static List<string> GetRegionsByName(string name)
        {
            return
                SQLDataAccess.ExecuteReadList<string>(
                    "Select RegionName From [Customers].[Region] WHERE RegionName like @name + '%'",
                    CommandType.Text, reader => SQLDataHelper.GetString(reader, "RegionName"),
                    new SqlParameter("@name", name));
        }

        public static Region ReadRegion(SqlDataReader reader)
        {
            return new Region
            {
                CountryID = SQLDataHelper.GetInt(reader, "CountryID"),
                Name = SQLDataHelper.GetString(reader, "RegionName"),
                RegionCode = SQLDataHelper.GetString(reader, "RegionCode"),
                RegionID = SQLDataHelper.GetInt(reader, "RegionID")
            };
        }
    }
}