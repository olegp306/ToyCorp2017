//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Payment
{
    public class PaymentService
    {
        private const string PaymentCacheKey = "PaymentMethods_";
        private const string PaymentCreditCacheKey = PaymentCacheKey + "Credit";

        public enum PageWithPaymentButton
        {
            myaccount,
            orderconfirmation
        }

        public static List<PaymentMethod> GetAllPaymentMethods(bool onlyEnabled)
        {
            var cacheKey = PaymentCacheKey + (onlyEnabled ? "Active" : "All");
            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<List<PaymentMethod>>(cacheKey);

            var payments = SQLDataAccess.ExecuteReadList(
                onlyEnabled
                    ? "SELECT * FROM [Order].[PaymentMethod] left join Catalog.Photo on Photo.ObjId=PaymentMethod.PaymentMethodID and Type=@Type where Enabled=1 ORDER BY [SortOrder]"
                    : "SELECT * FROM [Order].[PaymentMethod] left join Catalog.Photo on Photo.ObjId=PaymentMethod.PaymentMethodID and Type=@Type ORDER BY [SortOrder]",
                CommandType.Text,
                reader => GetPaymentMethodFromReader(reader, true),
                new SqlParameter("@Type", PhotoType.Payment.ToString()));

            CacheManager.Insert(cacheKey, payments);
            return payments;
        }
        
        public static IEnumerable<int> GetAllPaymentMethodIDs()
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("SELECT [PaymentMethodID] FROM [Order].[PaymentMethod]", CommandType.Text, "PaymentMethodID");
        }
        
        public static PaymentMethod GetPaymentMethod(int paymentMethodId)
        {
            var payment =
                SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Order].[PaymentMethod] WHERE [PaymentMethodID] = @PaymentMethodID", CommandType.Text,
                    reader => GetPaymentMethodFromReader(reader),
                    new SqlParameter("@PaymentMethodID", paymentMethodId));

            return payment;
        }

        public static PaymentMethod GetPaymentMethodByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT top(1) * FROM [Order].[PaymentMethod] WHERE [Name] = @Name",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader), new SqlParameter("@Name", name));
        }

        public static PaymentMethod GetPaymentMethodByType(PaymentType type)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT top(1) * FROM [Order].[PaymentMethod] WHERE [PaymentType] = @PaymentType",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader), new SqlParameter("@PaymentType", (int)type));
        }

        public static List<ICreditPaymentMethod> GetCreditPaymentMethods()
        {
            if (CacheManager.Contains(PaymentCreditCacheKey))
                return CacheManager.Get<List<ICreditPaymentMethod>>(PaymentCreditCacheKey);

            var list =
                GetAllPaymentMethods(true)
                    .Where(
                        paymentMethod =>
                            paymentMethod.GetType().GetInterface("AdvantShop.Payment.ICreditPaymentMethod") ==
                            typeof (ICreditPaymentMethod))
                    .Select(x => (ICreditPaymentMethod) x)
                    .ToList();

            CacheManager.Insert(PaymentCreditCacheKey, list);
            return list;
        }

        public static PaymentMethod GetPaymentMethodFromReader(SqlDataReader reader, bool loadPic = false)
        {
            var method = PaymentMethod.Create((PaymentType)SQLDataHelper.GetInt(reader, "PaymentType"));
            method.PaymentMethodId = SQLDataHelper.GetInt(reader, "PaymentMethodID");
            method.Name = SQLDataHelper.GetString(reader, "Name");
            method.Enabled = SQLDataHelper.GetBoolean(reader, "Enabled");
            method.Description = SQLDataHelper.GetString(reader, "Description");
            method.SortOrder = SQLDataHelper.GetInt(reader, "SortOrder");
            method.ExtrachargeType = (ExtrachargeType)SQLDataHelper.GetInt(reader, "ExtrachargeType");
            method.Extracharge = SQLDataHelper.GetFloat(reader, "Extracharge");

            method.Parameters = GetPaymentMethodParameters(method.PaymentMethodId);
            if (loadPic)
                method.IconFileName = new Photo(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"), PhotoType.Payment) { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") };
            return method;
        }

        private static Dictionary<string, string> GetPaymentMethodParameters(int paymentMethodId)
        {
            return
                SQLDataAccess.ExecuteReadDictionary<string, string>(
                    "SELECT Name, Value FROM [Order].[PaymentParam] WHERE [PaymentMethodID] = @PaymentMethodID",
                    CommandType.Text, "Name", "Value", new SqlParameter("@PaymentMethodID", paymentMethodId));
        }

        public static int AddPaymentMethod(PaymentMethod method)
        {
            var id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Order].[PaymentMethod] ([PaymentType],[Name], [Enabled], [Description], [SortOrder],Extracharge,ExtrachargeType) " +
                "VALUES (@PaymentType,@Name, @Enabled, @Description, @SortOrder,@Extracharge,@ExtrachargeType); SELECT scope_identity();",
                CommandType.Text,
                new SqlParameter("@PaymentType", method.Type),
                new SqlParameter("@Name", method.Name ?? string.Empty),
                new SqlParameter("@Enabled", method.Enabled),
                new SqlParameter("@Description", method.Description ?? string.Empty),
                new SqlParameter("@SortOrder", method.SortOrder),
                new SqlParameter("@Extracharge", method.Extracharge),
                new SqlParameter("@ExtrachargeType", (int) method.ExtrachargeType));

            AddPaymentMethodParameters(id, method.Parameters);
            CacheManager.RemoveByPattern(PaymentCacheKey);

            return id;
        }

        private static void AddPaymentMethodParameters(int paymentMethodId, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters.Where(parameter => parameter.Value.IsNotEmpty()))
            {
                SQLDataAccess.ExecuteNonQuery(
                    "INSERT INTO [Order].[PaymentParam] (PaymentMethodID, Name, Value) VALUES (@PaymentMethodID, @Name, @Value)",
                    CommandType.Text,
                    new SqlParameter("@PaymentMethodID", paymentMethodId),
                    new SqlParameter("@Name", parameter.Key),
                    new SqlParameter("@Value", parameter.Value));
            }
        }

        public static void UpdatePaymentMethod(PaymentMethod paymentMethod)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[PaymentMethod] SET [Name] = @Name,[Enabled] = @Enabled,[SortOrder] = @SortOrder,[Description] = @Description,[PaymentType] = @PaymentType, " +
                "Extracharge=@Extracharge, ExtrachargeType=@ExtrachargeType WHERE [PaymentMethodID] = @PaymentMethodID",
                CommandType.Text,
                new SqlParameter("@PaymentMethodID", paymentMethod.PaymentMethodId),
                new SqlParameter("@Name", paymentMethod.Name),
                new SqlParameter("@Enabled", paymentMethod.Enabled),
                new SqlParameter("@SortOrder", paymentMethod.SortOrder),
                new SqlParameter("@Description", paymentMethod.Description),
                new SqlParameter("@PaymentType", (int)paymentMethod.Type),
                new SqlParameter("@Extracharge", paymentMethod.Extracharge),
                new SqlParameter("@ExtrachargeType", (int)paymentMethod.ExtrachargeType));

            UpdatePaymentParams(paymentMethod.PaymentMethodId, paymentMethod.Parameters);
        }

        public static void UpdatePaymentParams(int paymentMethodId, Dictionary<string, string> parameters)
        {
            foreach (var kvp in parameters)
            {
                SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdatePaymentParam]", CommandType.StoredProcedure,
                    new SqlParameter("@PaymentMethodID", paymentMethodId), 
                    new SqlParameter("@Name", kvp.Key),
                    new SqlParameter("@Value", kvp.Value));
            }
            CacheManager.RemoveByPattern(PaymentCacheKey);
        }

        public static void DeletePaymentMethod(int paymentMethodId)
        {
            PhotoService.DeletePhotos(paymentMethodId, PhotoType.Payment);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[PaymentMethod] WHERE [PaymentMethodID] = @PaymentMethodID", CommandType.Text, new SqlParameter("@PaymentMethodID", paymentMethodId));
            CacheManager.RemoveByPattern(PaymentCacheKey);
        }

        public static List<PaymentMethod> GetCertificatePaymentMethods()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[GiftCertificatePayments] INNER JOIN [Order].[PaymentMethod] ON [PaymentMethod].[PaymentMethodID] = [GiftCertificatePayments].[PaymentID] WHERE [Enabled] = 1",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader));
        }

       public static void SaveOrderpaymentInfo(int orderId, int paymentId, string name, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                    "DELETE FROM [Order].[OrderPaymentInfo] WHERE OrderID=@OrderID and PaymentMethodID=@PaymentMethodID and Name=@Name; INSERT INTO [Order].[OrderPaymentInfo] (OrderID, PaymentMethodID, Name, Value) VALUES (@OrderID, @PaymentMethodID, @Name, @Value)",
                    CommandType.Text,
                    new SqlParameter("@OrderID", orderId),
                    new SqlParameter("@PaymentMethodID", paymentId),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Value", value));
        }

        public static PaymentAdditionalInfo GetOrderIdByPaymentIdAndCode(int paymentMethodId, string responseCode)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select * From [Order].[OrderPaymentInfo] Where PaymentMethodID = @PaymentMethodID AND Value = @Code",
                CommandType.Text,
                reader => new PaymentAdditionalInfo
                {
                    PaymentMethodId = SQLDataHelper.GetInt(reader, "PaymentMethodID"),
                    OrderId = SQLDataHelper.GetInt(reader, "OrderID"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    Value = SQLDataHelper.GetString(reader, "Value")
                },
                new SqlParameter("@PaymentMethodID", paymentMethodId),
                new SqlParameter("@Code", responseCode));
        }

        public static List<PaymentMethod> LoadMethods(int shippingMethodId, ShippingOptionEx ext,
            bool displayCertificateMetod, bool hideCashMetod)
        {
            var returnPayment = new List<PaymentMethod>();

            if (displayCertificateMetod)
            {
                var certificateMethod = GetPaymentMethodByType(PaymentType.GiftCertificate);
                if (certificateMethod == null)
                {
                    certificateMethod = new PaymentGiftCertificate
                    {
                        Enabled = true,
                        Name = Resources.Resource.Client_GiftCertificate,
                        Description = Resources.Resource.Payment_GiftCertificateDescription,
                        SortOrder = 0
                    };

                    AddPaymentMethod(certificateMethod);
                }
                returnPayment.Add(certificateMethod);
            }
            else
            {
                foreach (var method in GetAllPaymentMethods(true))
                {
                    if (method.Type == PaymentType.GiftCertificate)
                        continue;

                    if (hideCashMetod && (method.Type == PaymentType.Cash || method.Type == PaymentType.CashOnDelivery))
                        continue;

                    if (shippingMethodId != 0 &&
                        ShippingMethodService.IsPaymentNotUsed(shippingMethodId, method.PaymentMethodId))
                        continue;

                    if (method.Type == PaymentType.Kupivkredit)
                    {
                        var shpCart = ShoppingCartService.CurrentShoppingCart;
                        var kvkMethos = (Kupivkredit) GetPaymentMethod(method.PaymentMethodId);
                        if (shpCart.TotalPrice <= kvkMethos.MinimumPrice)
                            continue;
                    }

                    if (ext == null)
                    {
                        if (method.Type != PaymentType.CashOnDelivery && method.Type != PaymentType.PickPoint)
                            returnPayment.Add(method);
                    }
                    else
                    {
                        switch (method.Type)
                        {
                            case PaymentType.CashOnDelivery:
                                if ((ext.Type.HasFlag(ExtendedType.CashOnDelivery) || ext.Type.HasFlag(ExtendedType.Cdek) || ext.Type.HasFlag(ExtendedType.Checkout))
                                    && ext.ShippingId == int.Parse(method.Parameters[CashOnDelivery.ShippingMethodTemplate]))
                                {
                                    method.Description = CashOnDelivery.GetDecription(ext);
                                    returnPayment.Add(method);
                                }
                                break;
                            case PaymentType.PickPoint:
                                if (ext.Type == ExtendedType.Pickpoint &&
                                    ext.ShippingId == int.Parse(method.Parameters[PickPoint.ShippingMethodTemplate]))
                                {
                                    method.Description = ext.PickpointAddress;
                                    returnPayment.Add(method);
                                }
                                break;
                            default:
                                returnPayment.Add(method);
                                break;
                        }
                    }
                }
            }

            return returnPayment;
        }

        public static List<PaymentMethod> UseGeoMapping(IEnumerable<PaymentMethod> listMethods, string countryName,
            string cityName)
        {
            var items = new List<PaymentMethod>();
            foreach (var elem in listMethods)
            {
                if (ShippingPaymentGeoMaping.IsExistGeoPayment(elem.PaymentMethodId))
                {
                    if (ShippingPaymentGeoMaping.CheckPaymentEnabledGeo(elem.PaymentMethodId, countryName, cityName))
                        items.Add(elem);
                }
                else
                {
                    items.Add(elem);
                }
            }
            return items;
        }
    }
}