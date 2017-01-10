using System.Data;
using AdvantShop.Core.SQL;
using System;
using System.Data.SqlClient;
using AdvantShop.Helpers;

namespace AdvantShop.Shipping
{
    public class ShippingCache
    {
        public int ShippingMethodId { get; set; }
        public int ParamHash { get; set; }
        public string ServerResponse { get; set; }
        public DateTime Created { get; set; }
    }

    public class ShippingCacheRepositiry
    {
        public static ShippingCache Get(int shippingMethodId, int paramHash)
        {
            return SQLDataAccess.ExecuteReadOne<ShippingCache>("Select * from [Order].ShippingCache where ShippingMethodID=@ShippingMethodID and ParamHash=@ParamHash",
                                                                CommandType.Text, reader => new ShippingCache
                                                                {
                                                                    ShippingMethodId = SQLDataHelper.GetInt(reader, "ShippingMethodID"),
                                                                    ParamHash = SQLDataHelper.GetInt(reader, "ParamHash"),
                                                                    ServerResponse = SQLDataHelper.GetString(reader, "Options"),
                                                                    Created = SQLDataHelper.GetDateTime(reader, "Created"),
                                                                },
                                                                new SqlParameter("@ShippingMethodID", shippingMethodId),
                                                                new SqlParameter("@ParamHash", paramHash));
        }

        public static bool Exist(int shippingMethodId, int paramHash)
        {
            return
                SQLDataAccess.ExecuteScalar<int>("Select count(*) from [Order].ShippingCache where ShippingMethodID=@ShippingMethodID and ParamHash=@ParamHash",
                                                CommandType.Text,
                                                new SqlParameter("@ShippingMethodID", shippingMethodId),
                                                new SqlParameter("@ParamHash", paramHash)) > 0;
        }

        public static void Add(ShippingCache model)
        {
            SQLDataAccess.ExecuteNonQuery("Insert into [Order].ShippingCache (ShippingMethodID,ParamHash,Options,Created) VALUES (@ShippingMethodID,@ParamHash,@Options,@Created)",
                CommandType.Text,
                new SqlParameter("@ShippingMethodID", model.ShippingMethodId),
                new SqlParameter("@ParamHash", model.ParamHash),
                new SqlParameter("@Options", model.ServerResponse),
                new SqlParameter("@Created", DateTime.Now));
        }

        public static void Update(ShippingCache model)
        {
            SQLDataAccess.ExecuteNonQuery("Update [Order].ShippingCache set ShippingMethodID=@ShippingMethodID, ParamHash = @ParamHash, Options= @Options",
                CommandType.Text,
                new SqlParameter("@ShippingMethodID", model.ShippingMethodId),
                new SqlParameter("@ParamHash", model.ParamHash),
                new SqlParameter("@Options", model.ServerResponse),
                new SqlParameter("@Created", DateTime.Now));
        }

        public static void Delete()
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].ShippingCache where Created < @Created", CommandType.Text, new SqlParameter("@Created", DateTime.Now.AddDays(-7)));
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].ShippingCache where ShippingMethodID = @ShippingMethodID", CommandType.Text, new SqlParameter("@ShippingMethodID", id));
        }
    }
}