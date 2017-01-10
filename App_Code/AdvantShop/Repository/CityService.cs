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


namespace AdvantShop.Repository
{
    public class CityService
    {
        #region Get /  Add / Update / Delete 

        public static City GetCity(int cityId)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[City] WHERE [CityID] = @CityId",
                                        CommandType.Text, GetCityFromReader, new SqlParameter("@CityId", cityId));
        }

        public static string GetPhone() {
            City currentCity = CityService.GetCity(IpZoneContext.CurrentZone.CityId);

            return currentCity != null && currentCity.PhoneNumber.IsNotEmpty()
                               ? currentCity.PhoneNumber
                               : AdvantShop.Configuration.SettingsMain.Phone;
        }

        public static City GetCityFromReader(SqlDataReader reader)
        {
            return new City
            {
                CityId = SQLDataHelper.GetInt(reader, "CityID"),
                Name = SQLDataHelper.GetString(reader, "CityName"),
                RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                CitySort = SQLDataHelper.GetInt(reader, "CitySort"),
                DisplayInPopup = SQLDataHelper.GetBoolean(reader, "DisplayInPopup"),
                PhoneNumber = SQLDataHelper.GetString(reader, "PhoneNumber")
            };
        }

        public static void Update(City city)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[City] set CityName=@name, CitySort=@CitySort, RegionId=@RegionId, DisplayInPopup=@DisplayInPopup, PhoneNumber=@PhoneNumber  Where CityID = @id",
                CommandType.Text,
                new SqlParameter("@id", city.CityId),
                new SqlParameter("@name", city.Name),
                new SqlParameter("@CitySort", city.CitySort),
                new SqlParameter("@RegionID", city.RegionId),
                new SqlParameter("@DisplayInPopup", city.DisplayInPopup),
                new SqlParameter("@PhoneNumber", city.PhoneNumber ?? (object)DBNull.Value)
                );
        }

        public static void UpdatePhone(int cityId, string phone)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[City] set PhoneNumber=@PhoneNumber Where CityID = @id",
                CommandType.Text,
                new SqlParameter("@id", cityId),
                new SqlParameter("@PhoneNumber", phone)
                );
        }

        public static void Add(City city)
        {
            city.CityId =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert into [Customers].[City] (CityName, RegionID, CitySort, DisplayInPopup, PhoneNumber) Values (@Name, @RegionID, @CitySort, @DisplayInPopup, @PhoneNumber);SELECT scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Name", city.Name),
                    new SqlParameter("@CitySort", city.CitySort),
                    new SqlParameter("@RegionID", city.RegionId),
                    new SqlParameter("@DisplayInPopup", city.DisplayInPopup),
                    new SqlParameter("@PhoneNumber", city.PhoneNumber ?? (object)DBNull.Value));
        }

        public static void Delete(int cityId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from Customers.City Where CityID=@CityID", CommandType.Text, new SqlParameter("@CityID", cityId));
        }

        #endregion
        
        public static List<string> GetCitiesByName(string name)
        {
            string translitRu = StringHelper.TranslitToRus(name);
            string translitKeyboard = StringHelper.TranslitToRusKeyboard(name);

            return SQLDataAccess.ExecuteReadList("Select Distinct Top (10) CityName From Customers.City WHERE CityName like @name + '%' OR CityName like @translitRu + '%' OR CityName like @translitKeyboard + '%' order by CityName",
                                                    CommandType.Text, reader => SQLDataHelper.GetString(reader, "CityName"),
                                                    new SqlParameter("@name", name),
                                                    new SqlParameter("@translitRu", translitRu),
                                                    new SqlParameter("@translitKeyboard", translitKeyboard));
        }

        public static City GetCityByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne("Select * from Customers.City where lower(CityName)=@CityName", CommandType.Text, GetCityFromReader,
                                                      new SqlParameter { ParameterName = "@CityName", Value = name.ToLower() });
        }

        public static bool IsCityInCountry(int cityId, int countryId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(*) From Customers.City where RegionID in (SELECT RegionID FROM [Customers].[Region] WHERE [CountryID] = @countryID) and CityID=@CityID",
                CommandType.Text,
                new SqlParameter("@CityID", cityId),
                new SqlParameter("@countryID", countryId) ) > 0;
        }

        public static List<City> GetCitiesByDisplayInPopup()
        {
            return SQLDataAccess.ExecuteReadList("Select top 40 * From Customers.City Where DisplayInPopup=1 order by CitySort desc, CityName asc", CommandType.Text,
                GetCityFromReader);
        }

        public static List<City> GetCitiesByCountryInPopup(int countryId)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select top 40 * From Customers.City Where RegionID in (SELECT RegionID FROM [Customers].[Region] WHERE [CountryID] = @CountryId) and DisplayInPopup=1 order by CitySort desc, CityName asc",
                CommandType.Text,
                GetCityFromReader, new SqlParameter("@CountryId", countryId));
        }
    }
}