//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------


using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using Resources;

namespace AdvantShop.Taxes
{

    public class TaxServices
    {
        public static void CreateTax(TaxElement t)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Catalog].[Tax]([Name], [Enabled], [ShowInPrice], [Rate]) VALUES (@name, @enabled, @showInPrice, @Rate)",
                CommandType.Text,
                new SqlParameter("@name", t.Name),
                new SqlParameter("@enabled", t.Enabled),
                new SqlParameter("@showInPrice", t.ShowInPrice),
                new SqlParameter("@Rate", t.Rate));
        }

        public static List<TaxElement> GetTaxes()
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Catalog].[Tax]", CommandType.Text, ReadTax);
        }

        public static TaxElement GetTax(int id)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Catalog].[Tax] WHERE [TaxId] = @id", CommandType.Text, ReadTax, new SqlParameter("@id", id));
        }

        public static void UpdateTax(TaxElement t)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[Tax] SET [Name] = @name, [Enabled] = @enabled,  [ShowInPrice] = @showInPrice, [Rate] = @Rate WHERE [TaxId] = @TaxId",
                CommandType.Text,
                new SqlParameter("@TaxId", t.TaxId),
                new SqlParameter("@name", t.Name),
                new SqlParameter("@enabled", t.Enabled),
                new SqlParameter("@showInPrice", t.ShowInPrice),
                new SqlParameter("@Rate", t.Rate));
        }

        public static void DeleteTax(int taxId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Tax] WHERE [TaxId] = @TaxId",
                                            CommandType.Text,
                                            new SqlParameter("@TaxId", taxId));
        }

        public static List<int> GetAllTaxesIDs()
        {
            List<int> result = SQLDataAccess.ExecuteReadList("select [TaxId] from [Catalog].[Tax]", CommandType.Text,
                                                             reader => SQLDataHelper.GetInt(reader, "TaxId"));
            return result;
        }
        
        public static Dictionary<TaxElement, float> CalculateTaxes(float price)
        {
            return GetTaxes().Where(tax=>tax.Enabled).ToDictionary(tax => tax, tax => CalculateTax(price, tax));
        }


        private static float CalculateTax(float price, TaxElement tax)
        {
            float returnTax = tax.Rate;

            if (tax.ShowInPrice)
            {
                returnTax = returnTax * price / (100.0F + returnTax);
            }
            else
            {
                returnTax = returnTax * price / 100.0F;
            }
            return returnTax;
        }

        private static TaxElement ReadTax(SqlDataReader reader)
        {
            var t = new TaxElement
            {
                TaxId = SQLDataHelper.GetInt(reader, "TaxId"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Rate = SQLDataHelper.GetFloat(reader, "Rate"),
                ShowInPrice = SQLDataHelper.GetBoolean(reader, "ShowInPrice")
            };
            return t;
        }


        public static List<OrderTax> GetOrderTaxes(int orderid)
        {
            List<OrderTax> returnList = SQLDataAccess.ExecuteReadList("select taxId, taxName, taxSum, taxShowInPrice, TaxRate from [Order].[OrderTax] where orderid=@orderid",
                                                            CommandType.Text, read => new OrderTax
                                                                                          {
                                                                                              TaxID = SQLDataHelper.GetInt(read, "taxId"),
                                                                                              TaxName = SQLDataHelper.GetString(read, "taxName"),
                                                                                              TaxSum = SQLDataHelper.GetFloat(read, "taxSum"),
                                                                                              TaxShowInPrice = SQLDataHelper.GetBoolean(read, "taxShowInPrice"),
                                                                                              TaxRate = SQLDataHelper.GetFloat(read, "TaxRate"),
                                                                                          },
                                                            new SqlParameter("@orderid", orderid));
            return returnList;
        }

        public static void SetOrderTaxes(int orderId, List<OrderTax> taxValues)
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "insert into [Order].[OrderTax] (TaxID, TaxName, TaxSum, TaxRate, TaxShowInPrice, OrderId) values (@TaxID, @TaxName, @TaxSum, @TaxRate, @TaxShowInPrice, @OrderId)";
                db.cmd.CommandType = CommandType.Text;
                db.cnOpen();
                foreach (var taxValue in taxValues)
                {
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.AddWithValue("@OrderId", orderId);
                    db.cmd.Parameters.AddWithValue("@TaxID", taxValue.TaxID);
                    db.cmd.Parameters.AddWithValue("@TaxName", taxValue.TaxName);
                    db.cmd.Parameters.AddWithValue("@TaxSum", taxValue.TaxSum);
                    db.cmd.Parameters.AddWithValue("@TaxRate", taxValue.TaxRate);
                    db.cmd.Parameters.AddWithValue("@TaxShowInPrice", taxValue.TaxShowInPrice);
                    db.cmd.ExecuteNonQuery();
                }
                db.cnClose();
            }
        }
        public static void ClearOrderTaxes(int orderId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [Order].[OrderTax] where [OrderId] = @OrderId",
                                            CommandType.Text,
                                            new SqlParameter("@OrderId", orderId));
        }

        public static string BuildTaxTable(List<OrderTax> taxes, float currentCurrencyRate, string currentCurrencyIso3, string message)
        {
            var sb = new StringBuilder();
            if (!taxes.Any())
            {
                sb.Append("<tr><td style=\"background-color: #FFFFFF; text-align: right\">");
                sb.Append(message);
                sb.Append("&nbsp;</td><td style=\"background-color: #FFFFFF; width: 150px\">");
                sb.Append(CatalogService.GetStringPrice(0, currentCurrencyRate, currentCurrencyIso3));
                sb.Append("</td></tr>");
            }
            else
                foreach (OrderTax tax in taxes)
                {
                    sb.Append("<tr><td style=\"background-color: #FFFFFF; text-align: right\">");
                    sb.Append((tax.TaxShowInPrice ? Resource.Core_TaxServices_Include_Tax : "") + " " + tax.TaxName);
                    sb.Append(":&nbsp</td><td style=\"background-color: #FFFFFF; width: 150px\">" + (tax.TaxShowInPrice ? "" : "+"));
                    sb.Append(CatalogService.GetStringPrice(tax.TaxSum, currentCurrencyRate, currentCurrencyIso3));
                    sb.Append("</td></tr>");
                }
            return sb.ToString();
        }

        #region Certificates
        public static void DeleteCertificateTaxes()
        {
            SQLDataAccess.ExecuteScalar("DELETE FROM [Settings].[GiftCertificateTaxes]", CommandType.Text);
        }

        public static void SaveCertificateTaxes(List<int> taxIds)
        {
            DeleteCertificateTaxes();

            foreach (var taxId in taxIds)
            {
                SQLDataAccess.ExecuteScalar(
                    "INSERT INTO [Settings].[GiftCertificateTaxes] ([TaxID]) VALUES (@TaxID)",
                    CommandType.Text,
                    new SqlParameter("@TaxID", taxId));
            }
        }

        public static IEnumerable<TaxElement> GetCertificateTaxes()
        {
            return SQLDataAccess.ExecuteReadList<TaxElement>(
                "SELECT * FROM [Settings].[GiftCertificateTaxes] inner join Catalog.Tax on GiftCertificateTaxes.TaxID = Tax.TaxID",
                CommandType.Text, ReadTax);


        }

        public static Dictionary<TaxElement, float> CalculateCertificateTaxes(float price)
        {
            return GetCertificateTaxes().Where(tax => tax.Enabled).ToDictionary(tax => tax, tax => CalculateTax(price, tax));
        }

        #endregion 
    }
}