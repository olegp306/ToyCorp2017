//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{

    public class CustomOptionsService
    {
        #region  Public CustomOption Methods
        public static int AddCustomOption(CustomOption copt)
        {
            var id = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                                                        "[Catalog].[sp_AddCustomOption]",
                                                        CommandType.StoredProcedure,
                                                        new[]
                                                             {
                                                                 new SqlParameter("@Title", copt.Title), 
                                                                 new SqlParameter("@IsRequired", copt.IsRequired), 
                                                                 new SqlParameter("@InputType", copt.InputType), 
                                                                 new SqlParameter("@SortOrder", copt.SortOrder), 
                                                                 new SqlParameter("@ProductID", copt.ProductId)
                                                             }
                                                          ));
            if (id != 0)
            {
                foreach (var optionItem in copt.Options)
                {
                    if (optionItem.Title != null)
                    {
                        AddOption(optionItem, id);
                    }
                }
            }
            return id;
        }

        public static void UpdateCustomOption(CustomOption copt)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateCustomOption]", CommandType.StoredProcedure,
                                            new SqlParameter("@CustomOptionsId", copt.CustomOptionsId),
                                            new SqlParameter("@Title", copt.Title),
                                            new SqlParameter("@IsRequired", copt.IsRequired),
                                            new SqlParameter("@InputType", copt.InputType),
                                            new SqlParameter("@SortOrder", copt.SortOrder),
                                            new SqlParameter("@ProductID", copt.ProductId));
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Options] WHERE [CustomOptionsId] = @CustomOptionsId",
                                            CommandType.Text, new SqlParameter("@CustomOptionsId", copt.CustomOptionsId));
            foreach (var optionItem in copt.Options)
            {
                AddOption(optionItem, copt.CustomOptionsId);
            }
        }

        public static void DeleteCustomOption(int customOptionId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteCustomOption]", CommandType.StoredProcedure, new SqlParameter("@CustomOptionsId", customOptionId));
        }

        public static void DeleteCustomOptionByProduct(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CustomOptions] where [ProductID] = @productId", CommandType.Text, new SqlParameter("@productId", productId));
        }

        public static List<CustomOption> GetCustomOptionsByProductId(int productId)
        {
            var coptions = SQLDataAccess.ExecuteReadList<CustomOption>("[Catalog].[sp_GetCustomOptionsByProductId]", CommandType.StoredProcedure,
                                                                        GetCustomOptionFromReader, new SqlParameter("@ProductId", productId));
            return coptions;
        }

        public static bool DoesProductHaveRequiredCustomOptions(string productId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCustomOptionsIsRequiredByProductId]", CommandType.StoredProcedure, new SqlParameter("@ProductId", productId)) > 0;
        }

        public static bool DoesProductHaveRequiredCustomOptions(int productId)
        {
            return DoesProductHaveRequiredCustomOptions(productId.ToString(CultureInfo.InvariantCulture));
        }

        // Получаем сумму катом опций по цене продукта, и сериализованным опциям
        public static float GetCustomOptionPrice(float price, string attributeXml)
        {
            if (!string.IsNullOrEmpty(attributeXml))
            {
                return GetCustomOptionPrice(price, DeserializeFromXml(attributeXml));
            }

            return 0;
        }

        // Получаем сумму катом опций по цене продукта, и списку катом опций
        public static float GetCustomOptionPrice(float price, IEnumerable<OptionItem> customOptions)
        {
            if (customOptions != null)
            {
                return GetCustomOptionPrice(price, customOptions.Where(p => p != null).Select(p => new EvaluatedCustomOptions
                    {
                        OptionPriceType = p.PriceType,
                        OptionPriceBc = p.PriceBc
                    }));
            }

            return 0;
        }

        // Получаем сумму катом опций по цене продукта, и списку десериализованных катом опций
        public static float GetCustomOptionPrice(float price, IEnumerable<EvaluatedCustomOptions> customOptions)
        {
            float fixedPrice = 0;
            float percentPrice = 0;

            if (customOptions != null)
            {
                foreach (var item in customOptions)
                {
                    switch (item.OptionPriceType)
                    {
                        case OptionPriceType.Fixed:
                            fixedPrice += item.OptionPriceBc;
                            break;

                        case OptionPriceType.Percent:
                            percentPrice += price * item.OptionPriceBc * 0.01F;
                            break;
                    }
                }
            }

            return fixedPrice + percentPrice;
        }

        #endregion

        #region  Public OptionItem Methods
        public static void AddOption(OptionItem opt, int customOptionsId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_AddOption]", CommandType.StoredProcedure,
                                            new SqlParameter("@CustomOptionsId", customOptionsId),
                                            new SqlParameter("@Title", opt.Title),
                                            new SqlParameter("@PriceBC", opt.PriceBc),
                                            new SqlParameter("@PriceType", opt.PriceType),
                                            new SqlParameter("@SortOrder", opt.SortOrder));
        }

        public static void DeleteOption(int optionId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteOption]", CommandType.StoredProcedure, new SqlParameter("@OptionID", optionId));
        }

        public static void UpdateOption(OptionItem opt, int customOptionsId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateOption]", CommandType.StoredProcedure,
                                            new SqlParameter("@Id", opt.OptionId),
                                            new SqlParameter("@CustomOptionsId", customOptionsId),
                                            new SqlParameter("@Title", opt.Title),
                                            new SqlParameter("@PriceBC", opt.PriceBc),
                                            new SqlParameter("@PriceType", opt.PriceType),
                                            new SqlParameter("@SortOrder", opt.SortOrder));
        }

        public static List<OptionItem> GetCustomOptionItems(int customOptionId)
        {
            return SQLDataAccess.ExecuteReadList<OptionItem>("[Catalog].[sp_GetOptionsByCustomOptionId]", CommandType.StoredProcedure, GetOptionItemFromReader, new SqlParameter("@CustomOptionId", customOptionId));
        }

        public static void SubmitCustomOptionsWithSameProductId(int productId, List<CustomOption> list)
        {
            var oldlist = GetCustomOptionsByProductId(productId);
            //Deleting
            foreach (CustomOption copt in oldlist.Where(opt => list.WithId(opt.CustomOptionsId) == null))
            {
                DeleteCustomOption(copt.CustomOptionsId);
            }
            //Updating
            foreach (CustomOption copt in list.Where(copt => oldlist.WithId(copt.CustomOptionsId) != null))
            {
                UpdateCustomOption(copt);
            }
            //Adding
            foreach (CustomOption copt in list.AllWithId(-1))
            {
                AddCustomOption(copt);
            }
        }

        public static string SerializeToXml(List<CustomOption> options, List<OptionItem> values)
        {
            bool noSelectedValues = true;

            var doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Options");
            for (int i = 0; i <= options.Count - 1; i++)
            {
                if (i < values.Count && values[i] != null)
                {
                    XmlElement xopt = doc.CreateElement("Option");
                    XmlElement xel = doc.CreateElement("CustomOptionId");
                    xel.InnerText = options[i].CustomOptionsId.ToString(CultureInfo.InvariantCulture);
                    xopt.AppendChild(xel);
                    xel = doc.CreateElement("CustomOptionTitle");
                    xel.InnerText = options[i].Title;
                    xopt.AppendChild(xel);

                    xel = doc.CreateElement("OptionId");
                    xel.InnerText = values[i].OptionId.ToString(CultureInfo.InvariantCulture);
                    xopt.AppendChild(xel);
                    xel = doc.CreateElement("OptionTitle");
                    xel.InnerText = values[i].Title;
                    xopt.AppendChild(xel);
                    xel = doc.CreateElement("OptionPriceBC");
                    xel.InnerText = values[i].PriceBc.ToString(CultureInfo.InvariantCulture);
                    xopt.AppendChild(xel);
                    xel = doc.CreateElement("OptionPriceType");
                    xel.InnerText = values[i].PriceType.ToString();
                    xopt.AppendChild(xel);
                    root.AppendChild(xopt);

                    noSelectedValues = false;
                }
            }
            doc.AppendChild(root);

            if (noSelectedValues)
            {
                return string.Empty;
            }
            string str;
            using (var memstream = new MemoryStream())
            {

                var wrtr = new XmlTextWriter(memstream, null);
                doc.WriteTo(wrtr);
                wrtr.Close();
                byte[] buff = memstream.GetBuffer();
                int eidx = buff.Length - 1;
                for (int i = buff.Length - 1; i >= 0; i--)
                {
                    if (buff[i] == 0)
                        continue;
                    eidx = i;
                    break;
                }

                str = Encoding.UTF8.GetString(buff, 0, eidx + 1);
                memstream.Close();
            }

            return str;
        }

        public static IList<EvaluatedCustomOptions> DeserializeFromXml(string xml)
        {
            if (xml == null || string.IsNullOrEmpty(xml.Trim()))
            {
                return null;
            }

            var res = new List<EvaluatedCustomOptions>();

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlElement xel in doc.GetElementsByTagName("Option"))
            {
                var xelm = xel.GetElementsByTagName("OptionId")[0];
                if (int.Parse(xelm.InnerText) < 0)
                {
                    continue;
                }
                var evco = new EvaluatedCustomOptions();
                xelm = xel.GetElementsByTagName("CustomOptionId")[0];
                evco.CustomOptionId = int.Parse(xelm.InnerText);

                xelm = xel.GetElementsByTagName("OptionId")[0];
                evco.OptionId = int.Parse(xelm.InnerText);

                xelm = xel.GetElementsByTagName("OptionTitle")[0];
                evco.OptionTitle = xelm.InnerText;

                xelm = xel.GetElementsByTagName("CustomOptionTitle")[0];
                evco.CustomOptionTitle = xelm.InnerText;

                xelm = xel.GetElementsByTagName("OptionPriceBC")[0];
                evco.OptionPriceBc = float.Parse(xelm.InnerText, CultureInfo.InvariantCulture);

                xelm = xel.GetElementsByTagName("OptionPriceType")[0];
                evco.OptionPriceType = (OptionPriceType)Enum.Parse(typeof(OptionPriceType), xelm.InnerText);

                res.Add(evco);
            }
            return res;
        }

        #endregion

        #region  Private Methods
        private static CustomOption GetCustomOptionFromReader(SqlDataReader reader)
        {
            return new CustomOption
            {
                CustomOptionsId = SQLDataHelper.GetInt(reader, "CustomOptionsID"),
                Title = SQLDataHelper.GetString(reader, "Title"),
                InputType = (CustomOptionInputType)SQLDataHelper.GetInt(reader, "InputType"),
                IsRequired = SQLDataHelper.GetBoolean(reader, "IsRequired"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID")
            };
        }

        private static OptionItem GetOptionItemFromReader(SqlDataReader reader)
        {
            return new OptionItem
            {
                OptionId = SQLDataHelper.GetInt(reader, "OptionID"),
                //CustomOptionsId = SQLDataHelper.GetInt(reader, "CustomOptionsId"),
                Title = SQLDataHelper.GetString(reader, "Title"),
                PriceBc = SQLDataHelper.GetFloat(reader, "PriceBC"),
                PriceType = (OptionPriceType)SQLDataHelper.GetInt(reader, "PriceType"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }
        #endregion

        public static string OptionsToString(List<OptionItem> options)
        {
            var res = new StringBuilder();
            for (var i = 0; i < options.Count; i++)
            {
                res.Append("{");
                res.Append(options[i].Title);
                res.Append("|");
                res.Append(options[i].PriceBc.ToString("F2"));
                res.Append("|");
                res.Append(options[i].PriceType);
                res.Append("|");
                res.Append(options[i].SortOrder);
                res.Append("}");
            }
            return res.ToString();
        }

        public static string CustomOptionsToString(List<CustomOption> customOptions)
        {   //[title|type|IsRequired|sortOrder:{value|price|typePrice|sort}...] 
            //[Цвет1|drop|1|0:{Синий|100|f|10}{Красный|10|p|20}],....
            var res = new StringBuilder();
            for (var i = 0; i < customOptions.Count; i++)
            {
                res.Append("[");
                res.Append(customOptions[i].Title);
                res.Append("|");
                res.Append(customOptions[i].InputType);
                res.Append("|");
                res.Append(customOptions[i].IsRequired ? 1 : 0);
                res.Append("|");
                res.Append(customOptions[i].SortOrder);
                res.Append(":");
                res.Append(OptionsToString(customOptions[i].Options));
                res.Append("]");
            }
            return res.ToString();
        }

        public static void CustomOptionsFromString(int productId, string customOptionString)
        {
            try
            {
                DeleteCustomOptionByProduct(productId);
                if (string.IsNullOrEmpty(customOptionString)) return;
                var items = customOptionString.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items)
                {

                    var templateCustomOption = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (templateCustomOption.Length > 2) continue;
                    var customOption = ParseCustomOption(templateCustomOption[0], productId);
                    if (customOption == null) continue;
                    if (templateCustomOption.Length == 2)
                    {
                        customOption.Options = ParseOption(templateCustomOption[1]);
                    }
                    AddCustomOption(customOption);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private static CustomOption ParseCustomOption(string source, int productId)
        {
            var parts = source.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4) return null;
            return new CustomOption
            {
                Title = parts[0],
                InputType = parts[1].TryParseEnume<CustomOptionInputType>(),
                IsRequired = parts[2] == "1",
                SortOrder = parts[3].TryParseInt(),
                ProductId = productId
            };
        }
        private static List<OptionItem> ParseOption(string source)
        {
            var res = new List<OptionItem>();
            var items = source.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parts in items.Select(item => item.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (parts.Length != 4) return null;
                res.Add(new OptionItem
                    {
                        Title = parts[0],
                        PriceBc = parts[1].TryParseFloat(),
                        PriceType = parts[2].TryParseEnume<OptionPriceType>(),
                        SortOrder = parts[3].TryParseInt()
                    });
            }
            return res;
        }
    }
}