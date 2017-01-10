using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class SizeService
    {
        public static Size GetSize(int? sizeID)
        {
            if (!sizeID.HasValue)
                return null;

            return SQLDataAccess.ExecuteReadOne<Size>("Select * from Catalog.Size where sizeID=@sizeID",
                                                       CommandType.Text,
                 
                                                       GetFromReader, new SqlParameter("@sizeID", sizeID));
        }

        public static List<Size> GetSizes()
        {
            return SQLDataAccess.ExecuteReadList<Size>("Select * from Catalog.Size", CommandType.Text, GetFromReader);
        }


        public static Size GetSize(string sizeName)
        {
            return SQLDataAccess.ExecuteReadOne<Size>("Select Top 1 * from Catalog.Size where sizeName=@sizeName order by SortOrder",
                                                       CommandType.Text,
                                                       GetFromReader, new SqlParameter("@sizeName", sizeName));
        }

        private static Size GetFromReader(SqlDataReader reader)
        {
            return new Size()
            {
                SizeId = SQLDataHelper.GetInt(reader, "sizeID"),
                SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static int AddSize(Size size)
        {
            if (size == null)
                throw new ArgumentNullException("size");

            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddSize]", CommandType.StoredProcedure,
                                                        new SqlParameter("@SizeName", size.SizeName),
                                                        new SqlParameter("@SortOrder", size.SortOrder)
                                                        );
        }

        public static void UpdateSize(Size size)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateSize]", CommandType.StoredProcedure,
                                                 new SqlParameter("@SizeId", size.SizeId),
                                                 new SqlParameter("@SizeName", size.SizeName),
                                                 new SqlParameter("@SortOrder", size.SortOrder)
                                                 );
        }

        public static void DeleteSize(int sizeId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.Size WHERE SizeID = @SizeId", CommandType.Text, new SqlParameter("@SizeId", sizeId));
        }

        public static List<Size> GetSizesByCategoryID(int categoryId, bool inDepth)
        {
            return SQLDataAccess.ExecuteReadList<Size>("Catalog.sp_GetSizesByCategory", CommandType.StoredProcedure,
                                                         GetFromReader,
                                                        new SqlParameter("@CategoryID", categoryId),
                                                        new SqlParameter("@inDepth", inDepth));
        }

        public static bool IsSizeUsed(int sizeId)
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar("Select Count(SizeId) From Catalog.Offer WHERE SizeID = @SizeId", CommandType.Text, 
                                                                    new SqlParameter("@SizeId", sizeId))) > 0;
        }
    }
}