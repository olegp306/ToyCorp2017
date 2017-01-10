//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.CMS
{
    public static class CarouselService
    {
        public static void DeleteCarousel(int id)
        {
            SQLDataAccess.ExecuteNonQuery("[CMS].[sp_DeleteCarousel]", CommandType.StoredProcedure, new SqlParameter { ParameterName = "@CarouselID", Value = id });
            PhotoService.DeletePhotos(id, PhotoType.Carousel);
        }

        public static int AddCarousel(Carousel carousel)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("[CMS].[sp_InsertCarousel]", CommandType.StoredProcedure,
                new SqlParameter { ParameterName = "@URL", Value = carousel.URL },
                new SqlParameter { ParameterName = "@Enabled", Value = carousel.Enabled },
                new SqlParameter { ParameterName = "@SortOrder", Value = carousel.SortOrder }));
        }

        public static int GetMaxSortOrder()
        {
            return
                SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("Select max(sortorder) from [CMS].[Carousel]",
                                                                 CommandType.Text)) + 10;
        }


        public static int UpdateCarousel(Carousel carousel)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("[CMS].[sp_UpdateCarousel]", CommandType.StoredProcedure,
                new SqlParameter { ParameterName = "@CarouselID", Value = carousel.CarouselID },
                new SqlParameter { ParameterName = "@URL", Value = carousel.URL },
                new SqlParameter { ParameterName = "@SortOrder", Value = carousel.SortOrder },
                new SqlParameter { ParameterName = "@Enabled", Value = carousel.Enabled }
                ));
        }

        private static Carousel GetCarouselFromReader(IDataReader reader)
        {
            return new Carousel
                       {
                           CarouselID = SQLDataHelper.GetInt(reader, "CarouselID"),
                           URL = SQLDataHelper.GetString(reader, "URL"),
                           SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                           Enabled = SQLDataHelper.GetBoolean(reader, "Enabled")
                       };
        }

        public static int GetCarouselsCount()
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("Select Count(CarouselID) From CMS.Carousel Where Enabled=1", CommandType.Text));
        }

        public static IEnumerable<Carousel> GetAllCarouselsMainPage()
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Carousel>("Select CarouselID,PhotoId,ObjId,URL,PhotoName, Description From CMS.Carousel left join Catalog.Photo on Photo.ObjId=Carousel.CarouselID and Type=@Type Where Enabled=1 Order by SortOrder",
                                                                   CommandType.Text,
                                                                   reader => new Carousel
                                                                                {
                                                                                    CarouselID = SQLDataHelper.GetInt(reader, "CarouselID"),
                                                                                    URL = SQLDataHelper.GetString(reader, "URL"),
                                                                                    Picture = new Photo(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"), PhotoType.Carousel) { PhotoName = SQLDataHelper.GetString(reader, "PhotoName"), Description = SQLDataHelper.GetString(reader, "Description") }
                                                                                }, new SqlParameter("@Type", PhotoType.Carousel.ToString()));
        }

        public static IEnumerable<Carousel> GetCarousel(int carouselID)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Carousel>("sp_GetCarousel", CommandType.StoredProcedure, GetCarouselFromReader,
                new SqlParameter { ParameterName = "@CarouselID", Value = carouselID });
        }

        public static void SetActive(int carouselID, bool active)
        {
            SQLDataAccess.ExecuteNonQuery("Update CMS.Carousel set Enabled=@Enabled where CarouselID=@CarouselID", CommandType.Text,
                                        new SqlParameter("@CarouselID", carouselID),
                                        new SqlParameter("@Enabled", active));
        }
    }
}