using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Repository
{
    public class IpZoneService
    {
        public static IpZone GetZoneByCity(string city, int? countryID)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select Top(1) City.CityID, City.CityName, Region.RegionID, Region.RegionName, Country.CountryID, Country.CountryName " +
                "From Customers.City " +
                "Inner Join Customers.Region On Region.RegionId = City.RegionId " +
                "Inner Join Customers.Country On Country.CountryId = Region.CountryId " +
                "Where @City = LOWER(CityName) and (Country.CountryID=@countryID OR @countryID is null)" +
                "Order by Country.CountryID",
                CommandType.Text,
                reader => new IpZone()
                {
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId"),
                    CountryName = SQLDataHelper.GetString(reader, "CountryName"),
                    RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CityId = SQLDataHelper.GetInt(reader, "CityId"),
                    City = SQLDataHelper.GetString(reader, "CityName"),
                },
                new SqlParameter("@City", city), new SqlParameter("@countryID", (object)countryID ?? DBNull.Value));
        }
    }
}