using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class ColorService
    {

        public static Color GetColor(int? colorID)
        {
            if (!colorID.HasValue)
                return null;

            return SQLDataAccess.ExecuteReadOne<Color>("Select * from Catalog.Color where colorID=@colorId",
                                                       CommandType.Text,
                                                       GetFromReader, new SqlParameter("@colorid", colorID));
        }

        public static List<Color> GetColors()
        {
            return SQLDataAccess.ExecuteReadList<Color>("Select * from Catalog.Color", CommandType.Text, GetFromReader);
        }


        public static Color GetColor(string colorName)
        {
            return SQLDataAccess.ExecuteReadOne<Color>("Select Top 1 * from Catalog.Color where colorName=@colorName order by SortOrder",
                                                       CommandType.Text,
                                                       GetFromReader, new SqlParameter("@colorName", colorName));
        }

        private static Color GetFromReader(SqlDataReader reader)
        {
            return new Color()
                {
                    ColorId = SQLDataHelper.GetInt(reader, "ColorId"),
                    ColorCode = SQLDataHelper.GetString(reader, "ColorCode"),
                    ColorName = SQLDataHelper.GetString(reader, "ColorName"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
                };
        }

        public static int AddColor(Color color)
        {
            if (color == null)
                throw new ArgumentNullException("color");

            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddColor]", CommandType.StoredProcedure,
                                                        new SqlParameter("@ColorName", color.ColorName),
                                                        new SqlParameter("@ColorCode", color.ColorCode),
                                                        new SqlParameter("@SortOrder", color.SortOrder)
                                                        );
        }

        public static void UpdateColor(Color color)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateColor]", CommandType.StoredProcedure,
                                                 new SqlParameter("@ColorId", color.ColorId),
                                                 new SqlParameter("@ColorName", color.ColorName),
                                                 new SqlParameter("@ColorCode", color.ColorCode),
                                                 new SqlParameter("@SortOrder", color.SortOrder)
                                                 );
        }

        public static void DeleteColor(int colorId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.Color WHERE ColorID = @ColorId", CommandType.Text, new SqlParameter("@ColorId", colorId));
        }

        public static List<Color> GetColorsByCategoryID(int categoryId, bool inDepth)
        {
            return SQLDataAccess.ExecuteReadList<Color>("Catalog.sp_GetColorsByCategory", CommandType.StoredProcedure,
                                                         GetFromReader,
                                                        new SqlParameter("@CategoryID", categoryId),
                                                        new SqlParameter("@inDepth", inDepth));
        }

        public static bool IsColorUsed(int colorId)
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar("Select Count(ColorID) from Catalog.Offer where ColorID=@ColorID", CommandType.Text, 
                                                                new SqlParameter("@ColorID", colorId))) > 0;
        }
    }
}