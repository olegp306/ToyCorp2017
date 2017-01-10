//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;

namespace AdvantShop.Modules
{
    public class BuyInTimeService
    {
        public const string ModuleName = "BuyInTime";
        public const string CacheKey = "BuyInTimeCacheKey";

        public static string PicturePath
        {
            get { return SettingsGeneral.AbsolutePath + "modules/" + ModuleName + "/pictures/"; }
        }

        public enum eShowMode
        {
            None = 0,
            Horizontal = 1,
            Vertical = 2
        }
        public const string CountdownScript = "<div data-plugin=\"countdown\" data-countdown=\"{0}\"></div>";

        #region Install / Uninstall

        public static bool InstallBuyInTimeModule()
        {
            ModulesRepository.ModuleExecuteNonQuery(
            @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Module." + ModuleName + @"') AND type in (N'U'))
                Begin
                    CREATE TABLE Module." + ModuleName + @"
	                    (
	                    Id int NOT NULL IDENTITY (1, 1),
	                    ProductId int NOT NULL,
                        DateStart datetime NOT NULL,
	                    DateExpired datetime NOT NULL,
	                    DiscountInTime float(53) NOT NULL,
                        ActionText nvarchar(MAX) NOT NULL,
	                    ShowMode tinyint NOT NULL,
                        IsRepeat bit NOT NULL,
                        DaysRepeat int NOT NULL,
                        Picture nvarchar(50) NULL
	                    )  ON [PRIMARY]
                    
                    ALTER TABLE Module." + ModuleName + @" ADD CONSTRAINT
	                    PK_BuyInTime PRIMARY KEY CLUSTERED 
	                    (Id) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    
                    ALTER TABLE Module." + ModuleName + @" ADD CONSTRAINT
	                    FK_BuyInTime_Product FOREIGN KEY
	                    (ProductId) REFERENCES Catalog.Product (ProductId) ON UPDATE  NO ACTION ON DELETE  CASCADE
                End",
                CommandType.Text);

            ModuleSettingsProvider.SetSettingValue("BuyInTimeLabel", "<span class=\"buyintime-label\">Успей купить</span>", ModuleName);

            ModuleSettingsProvider.SetSettingValue("BuyInTimeActionTitle",
                "<div class=\"buy-in-time-title\">Успей купить!</div> <div class=\"buy-in-time-action\">До конца распродажи:</div>",
                ModuleName);

            ModuleSettingsProvider.SetSettingValue("BuyInTimeDefaultActionTextMode1",
                "<div class=\"buy-in-time-main-b\"> " +
                    "<div style=\"border-radius: 2px; background-color: #eceaeb; padding: 24px 20px 0 0px;\">" +
			            "<div style=\"padding: 0 15px;display: inline-block;vertical-align: top;\">" +
                            "#ActionTitle# #Countdown# " +
			            "</div>" +
                        "<div style=\"display:inline-block; vertical-align:bottom; padding:0; margin:0 20px 0 0;\">" +
                            "<a href=\"#ProductLink#\">" +
                                "#ProductPicture#" +
                            "</a>" +
                        "</div>" +
                        "<div style=\"display:inline-block; vertical-align:top; position:relative; min-height:125px;width: 270px;\">" +
                            "<div style=\"margin-bottom:5px;\">" +
                                "<a href=\"#ProductLink#\" style=\"font-size:18px; font-weight:bold;text-decoration:none;\">#ProductName#</a>" +
                            "</div>" +
                            "<div style=\"margin-bottom:5px;\"> " +
                                "<span style=\"color: #d40b3e; \">Цена: #OldPrice#</span> Экономия: #DiscountPrice#" +
                            "</div>" +
                            "<div style=\"font-weight:bold; margin-bottom:5px;\"> " +
                                "Цена сегодня: #NewPrice#" +
                            "</div>" +
                            "<div style=\"margin-bottom:20px;\"> " +
                                "<a href=\"#ProductLink#\" class=\"btn btn-big btn-buy\">Купить</a>" +
                            "</div>" +
                            "<div class=\"buy-in-time-discount\"> " +
                                "<div style=\"font-size:24px;\">#DiscountPercent#%</div> " +
                                "<span style=\"font-size:12px;\">скидка</span> " +
                            "</div>" +
                        "</div>" +
                        "<div style=\"display: inline-block; vertical-align: top; border-left: 1px solid #cccbce; margin-left:10px;\">" +
                            "<ul>" +
                                "<li style=\"padding-bottom:10px;\">Самая низкая цена</li>" +
                                "<li style=\"padding-bottom:10px;\">Бесплатная доставка в день заказа</li>" +
                                "<li style=\"padding-bottom:10px;\">Гарантия производителя</li>" +
                                "<li>Бесплатное обучение</li>" +
                            "</ul>" +
                        "</div>" +
                    "</div>" +
                "</div>",
                ModuleName);

            ModuleSettingsProvider.SetSettingValue("BuyInTimeDefaultActionTextMode2",
                "<div class=\"buy-in-time\"> " +
                    "<div class=\"center\"> " +
                        "#ActionTitle# #Countdown# " +
                    "</div> " +
                    "<div class=\"center\"> " +
                        "<div style=\"position:relative; margin:0; padding:10px; display:inline-block;\"> " +
                            "<a href=\"#ProductLink#\"> " +
                                "#ProductPicture# " +
                            "</a> " +
                            "<div class=\"buy-in-time-discount\"> " +
                                "<div style=\"font-size:24px;\">#DiscountPercent#%</div> " +
                                "<span style=\"font-size:12px;\">скидка</span> " +
                            "</div> " +
                        "</div> " +
                    "</div> " +
                    "<div style=\"margin-bottom:5px;\"> " +
                        "<a href=\"#ProductLink#\" style=\"font-size:14px; font-weight:bold;\">#ProductName#</a> " +
                    "</div> " +
                    "<div style=\"margin-bottom:5px;\"> " +
                        "<span style=\"color: #d40b3e;\">Цена: #OldPrice#</span> " +
                    "</div> " +
                    "<div style=\"font-weight:bold; margin-bottom:5px;\">Цена сегодня: #NewPrice#</div> " +
                    "<div style=\"font-weight:bold;\">Экономия: #DiscountPrice#</div> " +
                "</div>",
                ModuleName);

            return true;
        }

        public static bool UninstallBuyInTimeModule()
        {
            ModuleSettingsProvider.RemoveSqlSetting("BuyInTimeActionTitle", ModuleName);
            ModuleSettingsProvider.RemoveSqlSetting("BuyInTimeLabel", ModuleName);
            ModuleSettingsProvider.RemoveSqlSetting("BuyInTimeDefaultActionTextMode1", ModuleName);
            ModuleSettingsProvider.RemoveSqlSetting("BuyInTimeDefaultActionTextMode2", ModuleName);

            return true;
        }

        public static bool UpdateBuyInTimeModule()
        {
            ModulesRepository.ModuleExecuteNonQuery(
                @"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = N'SortOrder' AND [object_id] = OBJECT_ID(N'Module.BuyInTime'))
                BEGIN
	                ALTER TABLE Module.BuyInTime ADD SortOrder int NOT NULL DEFAULT(0)
                END", CommandType.Text);

            return true;
        }

        #endregion

        public static DateTime GetExpireDateTime(DateTime expired, int daysRepeat)
        {
            var now = DateTime.Now;
            var days = Math.Ceiling((double)(now - expired).Days / daysRepeat) *daysRepeat;
            var expiredDateTime = expired.AddDays(days > 0 ? days : daysRepeat);
            if (expiredDateTime <= now)
                expiredDateTime = expiredDateTime.AddDays(daysRepeat);

            return expiredDateTime;
        }
        
        public static ProductDiscount GetByProduct(int productId, DateTime dateTime)
        {
            var action = ModulesRepository.ModuleExecuteReadOne(
                "Select Top(1) * From [Module].[" + ModuleName + "] " +
                "Where ProductId=@ProductId And ((@dateTime between DateStart and DateExpired) Or (IsRepeat = 1)) Order by SortOrder",
                CommandType.Text, GetBuyInTimeProductModelFromReader,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@dateTime", dateTime));

            if (action == null)
                return null;

            if (action.IsRepeat && action.DateExpired < DateTime.Now)
            {
                action.DateExpired = GetExpireDateTime(action.DateExpired, action.DaysRepeat);
            }

            return new ProductDiscount()
            {
                ProductId = productId,
                Discount = action.DiscountInTime,
                DateExpired = action.DateExpired
            };
        }

        public static BuyInTimeProductModel GetByShowMode(int showMode, DateTime dateTime)
        {
            return ModulesRepository.ModuleExecuteReadOne(
                "Select Top(1) * From [Module].[" + ModuleName + "] "+
                "Where ShowMode=@ShowMode And ((@dateTime between DateStart and DateExpired) Or (IsRepeat = 1)) Order by SortOrder",
                CommandType.Text, GetBuyInTimeProductModelFromReader,
                new SqlParameter("@ShowMode", showMode),
                new SqlParameter("@dateTime", dateTime));
        }

        public static List<ProductDiscount> GetProductDiscountsList(DateTime dateTime)
        {
            return ModulesRepository.ModuleExecuteReadList(
                "Select * From [Module].[" + ModuleName + "] Where ((@dateTime between DateStart and DateExpired) Or (IsRepeat = 1))",
                CommandType.Text,
                reader => new ProductDiscount()
                {
                    ProductId = ModulesRepository.ConvertTo<int>(reader, "ProductId"),
                    Discount = ModulesRepository.ConvertTo<float>(reader, "DiscountInTime"),
                    DateExpired = ModulesRepository.ConvertTo<DateTime>(reader, "DateExpired")
                },
                new SqlParameter("@dateTime", dateTime));
        }

        public static DataTable GetProductsTable()
        {
            return ModulesRepository.ModuleExecuteTable(
                "Select Id, [" + ModuleName + "].[ProductId], DateStart, DateExpired, DiscountInTime, Product.Name, IsRepeat, DaysRepeat, SortOrder From [Module].[" + ModuleName + "] " +
                "Left Join Catalog.Product On Product.ProductId = [" + ModuleName + "].[ProductId] Order By SortOrder",
                CommandType.Text);
        }

        #region Get / Add / Update / Delete

        private static BuyInTimeProductModel GetBuyInTimeProductModelFromReader(SqlDataReader reader)
        {
            return new BuyInTimeProductModel
            {
                Id = ModulesRepository.ConvertTo<int>(reader, "Id"),
                ProductId = ModulesRepository.ConvertTo<int>(reader, "ProductId"),
                DateStart = ModulesRepository.ConvertTo<DateTime>(reader, "DateStart"),
                DateExpired = ModulesRepository.ConvertTo<DateTime>(reader, "DateExpired"),
                DiscountInTime = ModulesRepository.ConvertTo<float>(reader, "DiscountInTime"),
                ActionText = ModulesRepository.ConvertTo<string>(reader, "ActionText"),
                ShowMode = ModulesRepository.ConvertTo<int>(reader, "ShowMode"),
                IsRepeat = ModulesRepository.ConvertTo<bool>(reader, "IsRepeat"),
                DaysRepeat = ModulesRepository.ConvertTo<int>(reader, "DaysRepeat"),
                Picture = ModulesRepository.ConvertTo<string>(reader, "Picture"),
                SortOrder = ModulesRepository.ConvertTo<int>(reader, "SortOrder")
            };
        }

        public static BuyInTimeProductModel Get(int id)
        {
            return ModulesRepository.ModuleExecuteReadOne("Select * From [Module].[" + ModuleName + "] Where Id=@Id",
                CommandType.Text, GetBuyInTimeProductModelFromReader, 
                new SqlParameter("@Id", id));
        }

        public static void Add(BuyInTimeProductModel action)
        {
            action.Id = ModulesRepository.ModuleExecuteScalar<int>(
                "Insert Into [Module].[" + ModuleName + "]" +
                " (ProductId,DateStart,DateExpired,DiscountInTime,ActionText,ShowMode,IsRepeat,DaysRepeat,Picture,SortOrder) " +
                "Values (@ProductId,@DateStart,@DateExpired,@DiscountInTime,@ActionText,@ShowMode,@IsRepeat,@DaysRepeat,@Picture,@SortOrder); " +
                "Select scope_identity();",
                CommandType.Text,
                new SqlParameter("@ProductId", action.ProductId),
                new SqlParameter("@DateStart", action.DateStart),
                new SqlParameter("@DateExpired", action.DateExpired),
                new SqlParameter("@DiscountInTime", action.DiscountInTime),
                new SqlParameter("@ActionText", action.ActionText),
                new SqlParameter("@ShowMode", action.ShowMode),
                new SqlParameter("@IsRepeat", action.IsRepeat),
                new SqlParameter("@DaysRepeat", action.DaysRepeat),
                new SqlParameter("@Picture", action.Picture ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", action.SortOrder));

            CacheManager.RemoveByPattern(CacheKey);
        }

        public static void Update(BuyInTimeProductModel action)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Update [Module].[" + ModuleName + "] "+
                "Set ProductId=@ProductId, DateStart=@DateStart, DateExpired=@DateExpired, DiscountInTime=@DiscountInTime, ActionText=@ActionText, ShowMode=@ShowMode, IsRepeat=@IsRepeat, DaysRepeat=@DaysRepeat, Picture=@Picture, SortOrder=@SortOrder " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", action.Id),
                new SqlParameter("@ProductId", action.ProductId),
                new SqlParameter("@DateStart", action.DateStart),
                new SqlParameter("@DateExpired", action.DateExpired),
                new SqlParameter("@DiscountInTime", action.DiscountInTime),
                new SqlParameter("@ActionText", action.ActionText),
                new SqlParameter("@ShowMode", action.ShowMode),
                new SqlParameter("@IsRepeat", action.IsRepeat),
                new SqlParameter("@DaysRepeat", action.DaysRepeat),
                new SqlParameter("@Picture", action.Picture ?? (object)DBNull.Value),
                new SqlParameter("@SortOrder", action.SortOrder));

            CacheManager.RemoveByPattern(CacheKey);
        }

        public static void UpdatePicture(int actionId, string picture)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Update [Module].[" + ModuleName + "] Set Picture=@Picture Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", actionId),
                new SqlParameter("@Picture", picture ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern(CacheKey);
        }

        public static void Delete(int id)
        {
            ModulesRepository.ModuleExecuteNonQuery("Delete From [Module].[" + ModuleName + "] Where Id=@Id",
                CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(CacheKey);
        }

        #endregion
    }
}
