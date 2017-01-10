//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Security;
using Resources;

namespace AdvantShop.Shipping
{
    public class ShippingMethodService
    {
        public static ShippingMethod GetShippingMethodFromReader(SqlDataReader reader, bool loadPic = false)
        {
            return new ShippingMethod
            {
                ShippingMethodId = SQLDataHelper.GetInt(reader, "ShippingMethodID"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Type = (ShippingType)SQLDataHelper.GetInt(reader, "ShippingType"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                DisplayCustomFields = SQLDataHelper.GetBoolean(reader, "DisplayCustomFields"),
                ShowInDetails = SQLDataHelper.GetBoolean(reader, "ShowInDetails"),
                IconFileName = loadPic
                    ? new Photo(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"),
                        PhotoType.Payment) { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") }
                    : null,
                ZeroPriceMessage = SQLDataHelper.GetString(reader, "ZeroPriceMessage")
            };
        }

        /// <summary>
        /// return shipping service by his id
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <returns>ShippingMethod</returns>
        public static DataTable GetShippingPayments(int shippingMethodId)
        {
            return
                SQLDataAccess.ExecuteTable(
                    "SELECT [PaymentMethod].[PaymentMethodID], [PaymentMethod].[Name], (Select Count(PaymentMethodID) From [Order].[ShippingPayments] Where PaymentMethodID = [PaymentMethod].[PaymentMethodID] AND ShippingMethodID = @ShippingMethodID) as [Use] FROM [Order].[PaymentMethod]",
                    CommandType.Text,
                    new SqlParameter("@ShippingMethodID", shippingMethodId));
        }

        /// <summary>
        /// return shipping service by his id
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <returns>ShippingMethod</returns>
        public static ShippingMethod GetShippingMethod(int shippingMethodId)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Order].[ShippingMethod] WHERE ShippingMethodID = @ShippingMethodID",
                    CommandType.Text,
                    reader => GetShippingMethodFromReader(reader),
                    new SqlParameter("@ShippingMethodID", shippingMethodId));
        }


        public static bool IsPaymentNotUsed(int shippingMethodId, int paymentMethodId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count(PaymentMethodID) FROM [Order].[ShippingPayments] WHERE ShippingMethodID = @ShippingMethodID AND PaymentMethodID = @PaymentMethodID",
                CommandType.Text, new SqlParameter("@ShippingMethodID", shippingMethodId),
                new SqlParameter("@PaymentMethodID", paymentMethodId)) > 0;
        }

        public static ShippingMethod GetShippingMethodByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Order].[ShippingMethod] WHERE Name = @Name",
                CommandType.Text,
                reader => GetShippingMethodFromReader(reader),
                new SqlParameter("@Name", name));
        }

        /// <summary>
        /// get all enabled shipping services
        /// </summary>
        /// <returns>List of ShippingMethod</returns>
        public static List<ShippingMethod> GetAllShippingMethods(bool enabled)
        {
            return
                SQLDataAccess.ExecuteReadList(
                    "SELECT ShippingMethodID,Name,ShippingType,Name,ShippingMethod.Description,Enabled,SortOrder,DisplayCustomFields,ObjId,PhotoId,PhotoName,ShowInDetails,ZeroPriceMessage FROM [Order].[ShippingMethod] " +
                    " left join Catalog.Photo on Photo.objId=ShippingMethod.ShippingMethodID and Type=@Type" +
                    " WHERE Enabled = @Enabled order by sortOrder",
                    CommandType.Text, reader => GetShippingMethodFromReader(reader, true),
                    new SqlParameter("@Enabled", enabled), new SqlParameter("@Type", PhotoType.Shipping.ToString()));
        }

        /// <summary>
        /// get all enabled shipping services
        /// </summary>
        /// <returns>List of ShippingMethod</returns>
        public static List<ShippingMethod> GetAllShippingMethods()
        {
            return
                SQLDataAccess.ExecuteReadList(
                    "SELECT ShippingMethodID,Name,ShippingType,Name,ShippingMethod.Description,Enabled,SortOrder,DisplayCustomFields,ObjId,PhotoId,PhotoName,ShowInDetails,ZeroPriceMessage FROM [Order].[ShippingMethod] left join Catalog.Photo on Photo.objId=ShippingMethod.ShippingMethodID and Type=@Type order by sortOrder",
                    CommandType.Text, reader => GetShippingMethodFromReader(reader, true),
                    new SqlParameter("@Type", PhotoType.Shipping.ToString()));
        }

        public static IEnumerable<int> GetAllShippingMethodIds()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT ShippingMethodID  FROM [Order].[ShippingMethod]", CommandType.Text, "ShippingMethodID");
        }

        public static int InsertShippingMethod(ShippingMethod item)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Order].[ShippingMethod] ([ShippingType],[Name],[Description],[Enabled],[SortOrder],[DisplayCustomFields],[ShowInDetails], ZeroPriceMessage) " +
                    "VALUES (@ShippingType,@Name,@Description,@Enabled,@SortOrder,@DisplayCustomFields,@ShowInDetails, @ZeroPriceMessage); SELECT scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@ShippingType", (int)item.Type),
                    new SqlParameter("@Name", item.Name),
                    new SqlParameter("@Description", item.Description),
                    new SqlParameter("@Enabled", item.Enabled),
                    new SqlParameter("@SortOrder", item.SortOrder),
                    new SqlParameter("@DisplayCustomFields", item.DisplayCustomFields),
                    new SqlParameter("@ShowInDetails", item.ShowInDetails),
                    new SqlParameter("@ZeroPriceMessage", item.ZeroPriceMessage ?? ""));
        }

        public static bool UpdateShippingPayments(int shippingMethodId, List<int> payments)
        {
            var deleteCmd = "Delete From [Order].[ShippingPayments] Where [ShippingMethodID] = @shippingMethodId;";
            var insertCmd = payments.Aggregate(string.Empty,
                (current, paymentId) => current + string.Format("INSERT INTO [Order].[ShippingPayments] ([ShippingMethodID], [PaymentMethodID]) VALUES ({0}, {1});", shippingMethodId, paymentId));
            SQLDataAccess.ExecuteNonQuery(deleteCmd + insertCmd, CommandType.Text, new SqlParameter("shippingMethodId", shippingMethodId));
            return true;
        }

        public static bool UpdateShippingMethod(ShippingMethod item)
        {
            SQLDataAccess.ExecuteNonQuery(
                item.Type == ShippingType.None
                    ? "UPDATE [Order].[ShippingMethod] SET [Name] = @Name,[Description] = @Description,[Enabled] = @Enabled,[SortOrder] = @SortOrder, DisplayCustomFields=@DisplayCustomFields, ShowInDetails=@ShowInDetails, ZeroPriceMessage=@ZeroPriceMessage WHERE ShippingMethodID=@ShippingMethodID"
                    : "UPDATE [Order].[ShippingMethod] SET [ShippingType] = @ShippingType,[Name] = @Name,[Description] = @Description,[Enabled] = @Enabled,[SortOrder] = @SortOrder, DisplayCustomFields=@DisplayCustomFields, ShowInDetails=@ShowInDetails, ZeroPriceMessage=@ZeroPriceMessage WHERE ShippingMethodID=@ShippingMethodID",
                CommandType.Text,
                new SqlParameter("@ShippingType", (int)item.Type),
                new SqlParameter("@Name", item.Name),
                new SqlParameter("@Description", item.Description),
                new SqlParameter("@Enabled", item.Enabled),
                new SqlParameter("@SortOrder", item.SortOrder),
                new SqlParameter("@ShippingMethodID", item.ShippingMethodId),
                new SqlParameter("@DisplayCustomFields", item.DisplayCustomFields),
                new SqlParameter("@ShowInDetails", item.ShowInDetails),
                new SqlParameter("@ZeroPriceMessage", item.ZeroPriceMessage ?? "")
                );

            return true;
        }

        public static void DeleteShippingMethod(int shippingId)
        {
            PhotoService.DeletePhotos(shippingId, PhotoType.Shipping);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[ShippingMethod] WHERE ShippingMethodID = @shippingId",
                CommandType.Text, new SqlParameter("@shippingId", shippingId));
        }

        /// <summary>
        /// gets list of shippingMethod by type and enabled
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ShippingMethod> GetShippingMethodByType(ShippingType type)
        {
            return
                SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Order].[ShippingMethod] WHERE ShippingType = @ShippingType and enabled=1",
                    CommandType.Text, reader => GetShippingMethodFromReader(reader),
                    new SqlParameter("@ShippingType", (int)type));
        }

        public static Dictionary<string, string> GetShippingParams(int shippingMethodId)
        {
            return
                SQLDataAccess.ExecuteReadDictionary<string, string>(
                    "SELECT ParamName,ParamValue FROM [Order].[ShippingParam] WHERE ShippingMethodID = @ShippingMethodID",
                    CommandType.Text, "ParamName", "ParamValue", new SqlParameter("@ShippingMethodID", shippingMethodId));
        }

        public static void InsertShippingParams(int shippingMethodId, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                SQLDataAccess.ExecuteNonQuery(
                    "INSERT INTO [Order].[ShippingParam] ([ShippingMethodID],[ParamName],[ParamValue]) VALUES (@ShippingMethodID,@ParamName,@ParamValue)",
                    CommandType.Text,
                    new SqlParameter("@ShippingMethodID", shippingMethodId),
                    new SqlParameter("@ParamName", parameter.Key),
                    new SqlParameter("@ParamValue", parameter.Value));
            }
        }

        public static bool UpdateShippingParams(int shippingMethodId, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                SQLDataAccess.ExecuteNonQuery(
                    @"if (SELECT COUNT(*) FROM [Order].[ShippingParam] WHERE [ShippingMethodID] = @ShippingMethodID AND [ParamName] = @ParamName) = 0
		                INSERT INTO [Order].[ShippingParam] ([ShippingMethodID], [ParamName], [ParamValue]) VALUES (@ShippingMethodID, @ParamName, @ParamValue)
	                else
		                UPDATE [Order].[ShippingParam] SET [ParamValue] = @ParamValue WHERE [ShippingMethodID] = @ShippingMethodID AND [ParamName] = @ParamName",
                    CommandType.Text,
                    new SqlParameter("@ShippingMethodID", shippingMethodId),
                    new SqlParameter("@ParamName", parameter.Key),
                    new SqlParameter("@ParamValue", parameter.Value));
            }
            return true;
        }


        public static string RenderExtend(ShippingItem shippingItem, int distance, string pickupAddress, bool isSelected)
        {
            var result = string.Empty;

            switch (shippingItem.Type)
            {
                case ShippingType.eDost:
                    if (shippingItem.Ext != null && shippingItem.Ext.Type == ExtendedType.Pickpoint)
                    {
                        var temp = shippingItem.Ext.Pickpointmap.IsNotEmpty()
                            ? string.Format(",{{city:'{0}', ids:null}}", shippingItem.Ext.Pickpointmap)
                            : string.Empty;

                        result =
                            string.Format(
                                "<br/><div class=\"address-pickpoint\">{0}</div>" +
                                "<a href=\"javascript:void(0);\" class='pickpoint' onclick=\"PickPoint.open(SetPickPointAnswer{1});\">{2}</a><br />",
                                isSelected ? pickupAddress : string.Empty,
                                temp, Resource.ShippingRates_PickpointSelect);
                    }
                    else if (shippingItem.ShippingPoints != null && shippingItem.ShippingPoints.Any())
                    {
                        result = "<div class=\"shipping-points-b\"> <select class=\"shipping-points\">";
                        foreach (var point in shippingItem.ShippingPoints)
                        {
                            result += string.Format("<option data-description=\"{0}\" value=\"{1}\" {3}>{2}</option>",
                                HttpUtility.HtmlEncode(point.Description), point.Id, point.Address,
                                shippingItem.Ext != null && point.Id.ToString() == shippingItem.Ext.PickpointId
                                    ? "selected='selected'"
                                    : string.Empty);
                        }
                        result += "</select> ";
                        result +=
                            string.Format(
                                "<a class=\"edost-map\" target=\"_blank\" href=\"http://www.edost.ru/office.php?c={0}\">{1}</a>",
                                shippingItem.ShippingPoints.First().Id, Resource.ShippingRates_ShowOnMap);

                        result += "</div>";
                    }
                    break;

                case ShippingType.Multiship:
                    if (shippingItem.Ext != null)
                    {
                        result =
                            string.Format(
                                "<div class=\"address-multiship\">{0}</div>" +
                                "<span class=\"btn-c\"><a href=\"javascript:void(0);\" class=\"btn btn-confirm btn-middle multiship-choose\" data-mswidget-open>{1}</a></span>",
                                isSelected ? pickupAddress : string.Empty,
                                Resource.ShippingRates_PickpointSelect);
                    }
                    break;

                case ShippingType.ShippingByRangeWeightAndDistance:
                    if (shippingItem.Params != null &&
                        shippingItem.Params.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.UseDistance).TryParseBool())
                    {
                        result =
                            string.Format(
                                " <input data-plugin=\"spinbox\" data-spinbox-options=\"{{min:0,max:100000,step:1}}\" type=\"text\" class=\"tDistance\" value=\"{0}\"  data-id='{1}'/>",
                                distance, shippingItem.MethodId);
                    }
                    break;

                case ShippingType.Cdek:
                    result = "<input type=\"hidden\" class=\"hiddenCdekTariff\" value=\"" + shippingItem.Ext.AdditionalData + "\">";
                    if (shippingItem.ShippingPoints != null)
                    {
                        result += "<div class=\"shipping-points-b\"> <select class=\"shipping-points-cdek\">";
                        foreach (var point in shippingItem.ShippingPoints)
                        {
                            result += string.Format("<option data-description=\"{0}\" data-tariffid=\"{1}\" value=\"{2}\">{3}</option>",
                                                        HttpUtility.HtmlEncode(point.Description), shippingItem.Ext.AdditionalData, point.Code, point.Address);
                        }
                        result += "</select></div>";
                    }
                    break;
                case ShippingType.CheckoutRu:

                    result = "<input type=\"hidden\" class=\"hiddenCheckoutInfo\" value=\"" + shippingItem.Ext.AdditionalData + "\">";
                    if (shippingItem.ShippingPoints != null)
                    {
                        result += "<div class=\"shipping-points-b\"> <select class=\"shipping-points-checkout\">";
                        foreach (var point in shippingItem.ShippingPoints)
                        {
                            result +=
                                string.Format(
                                    "<option data-description=\"{0}\" data-additional=\"{1}\" data-rate=\"{2}\" data-full-rate=\"{3}\" value=\"{4}\" data-checkout-address=\"{6}\">{5} - {3}</option>",
                                    HttpUtility.HtmlEncode(point.Description), point.AdditionalData, point.Rate,
                                    CatalogService.GetStringPrice(point.Rate), point.Code, point.Address,
                                    HttpUtility.HtmlEncode(point.Address));
                        }
                        result += "</select></div>";
                    }
                    else
                    {
                        result += "<input type=\"hidden\" class=\"hiddenCheckoutExpressRate\" value=\"" + shippingItem.Rate + "\">";
                    }
                    break;
            }

            return result;
        }

        public static bool ShowAddressField(EnUserType type, ShippingItem shippingItem)
        {
            //доставка до двери сдеком
            if (type != EnUserType.RegisteredUser && shippingItem != null && shippingItem.Type == ShippingType.Cdek && shippingItem.Ext != null)
            {
                var tariff = Cdek.tariffs.FirstOrDefault(item => string.Equals(item.tariffId.ToString(), shippingItem.Ext.AdditionalData));
                return tariff != null && tariff.mode.EndsWith("Д");
            }

            return type != EnUserType.RegisteredUser &&
                   shippingItem != null && shippingItem.Type != ShippingType.SelfDelivery &&
                   (shippingItem.Ext == null || (shippingItem.Ext != null && shippingItem.Ext.Type != ExtendedType.Pickpoint)) &&
                   (shippingItem.ShippingPoints == null || shippingItem.ShippingPoints.Count == 0);


        }

        public static bool ShowCustomField(EnUserType type, ShippingItem shippingItem)
        {
            return ShowAddressField(type, shippingItem) &&
                   shippingItem.DisplayCustomFields;
        }

        /// <summary>
        /// Get shippping item with rules
        /// </summary>
        /// <param name="shippingItem">base shippingItem</param>
        /// <param name="pickupId">pickupId (pickpoint id)</param>
        /// <param name="pickupAddress">pickupAddress (pickpoint address)</param>
        /// <param name="additionalData">additional pickup data</param>
        /// <param name="cityToDelivery">City to delivery</param>
        /// <returns></returns>
        public static ShippingItem GetListShippingItem(ShippingItem shippingItem, int pickupId,
            string pickupAddress, string additionalData, string cityToDelivery)
        {
            if (shippingItem.Ext != null &&
                (shippingItem.Ext.Type == ExtendedType.Pickpoint || shippingItem.Ext.Type == ExtendedType.CashOnDelivery || shippingItem.Ext.Type == ExtendedType.Boxberry || shippingItem.Ext.Type == ExtendedType.Cdek))
            {
                shippingItem.Ext.PickpointId = pickupId.ToString();
                shippingItem.Ext.PickpointAddress = pickupAddress;
                shippingItem.Ext.AdditionalData = additionalData;

                if (shippingItem.Type == ShippingType.Multiship)
                {
                    var cart = ShoppingCartService.CurrentShoppingCart;
                    var multiship = new Multiship(GetShippingParams(shippingItem.MethodId))
                    {
                        CityTo = cityToDelivery,
                        ShoppingCart = cart,
                        TotalPrice = cart.TotalPrice - cart.TotalDiscount,
                    };

                    var multishipOption =
                        multiship.GetOptions(false)
                            .Find(x => x.Extend != null && x.Extend.PickpointId == pickupId.ToString());

                    if (multishipOption != null)
                    {
                        shippingItem.Rate = multishipOption.Rate;
                    }
                }
            }
            return shippingItem;
        }
    }
}