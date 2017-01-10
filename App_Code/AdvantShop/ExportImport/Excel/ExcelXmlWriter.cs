//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Statistic;
using Yogesh.ExcelXml;

namespace AdvantShop.ExportImport.Excel
{
    public class ExcelXmlWriter
    {
        #region Orders saving

        public bool SaveOrdersToXml(string filename, IEnumerable<Order> orders)
        {
            try
            {
                OrdersArrayToWorkbook(orders).Export(filename);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }
        private static string RenderOrderedItems(IEnumerable<OrderItem> items)
        {
            var res = new StringBuilder();
            foreach (OrderItem orderItem in items)
            {
                res.AppendFormat("[{0} - {1} - {2}{3}{4}], ",orderItem.ArtNo, orderItem.Name, orderItem.Amount,
                    Resources.Resource.Admin_ExportOrdersExcel_Pieces, orderItem.SelectedOptions != null && orderItem.SelectedOptions.Count > 0 ? RenderSelectedOptions(orderItem.SelectedOptions, orderItem.Color, orderItem.Size) : string.Empty);
            }
            return res.ToString().TrimEnd(new[] { ',', ' ' });
        }

        private static ExcelXmlWorkbook OrdersArrayToWorkbook(IEnumerable<Order> orders)
        {
            var book = new ExcelXmlWorkbook { Properties = { Author = "AdvantShop.Net", LastAuthor = "AdvantShop.Net" }, DefaultStyle = new XmlStyle { Font = { Name = "Calibri", Size = 11 } } };

            Worksheet sheet = book[0];

            sheet.Name = "Orders";

            sheet.PrintOptions.Orientation = PageOrientation.Landscape;
            sheet.PrintOptions.SetMargins(0.5, 0.4, 0.5, 0.4);

            //XLS columns definition

            sheet.Columns(0).Width = 50;
            sheet.Columns(1).Width = 80;
            sheet.Columns(2).Width = 100;
            sheet.Columns(3).Width = 130;
            sheet.Columns(4).Width = 100;
            sheet.Columns(5).Width = 100;
            sheet.Columns(6).Width = 150;
            sheet.Columns(7).Width = 80;
            sheet.Columns(8).Width = 80;
            sheet.Columns(9).Width = 80;
            sheet.Columns(10).Width = 80;
            sheet.Columns(11).Width = 300;
            sheet.Columns(12).Width = 300;
            sheet.Columns(13).Width = 300;
            sheet.Columns(14).Width = 300;
            sheet.Columns(15).Width = 300;
            sheet.Columns(16).Width = 50;


            sheet[0, 0].Value = "OrderID";
            sheet[1, 0].Value = "Status";
            sheet[2, 0].Value = "OrderDate";
            sheet[3, 0].Value = "FIO";
            sheet[4, 0].Value = "Customer Email";
            sheet[5, 0].Value = "Customer Phone";
            sheet[6, 0].Value = "OrderedItems";
            sheet[7, 0].Value = "Total";
            sheet[8, 0].Value = "Tax";
            sheet[9, 0].Value = "Cost";
            sheet[10, 0].Value = "Profit";
            sheet[11, 0].Value = "Payment";
            sheet[12, 0].Value = "Shipping";
            sheet[13, 0].Value = "Shipping Address";
            sheet[14, 0].Value = "Customer Comment";
            sheet[15, 0].Value = "Admin Comment";
            sheet[16, 0].Value = "Payed";



            var i = 1;
            foreach (Order order in orders)
            {
                if (!CommonStatistic.IsRun) return book;

                CommonStatistic.RowPosition++;
                //Order to XLS row

                sheet[0, i].Value = order.OrderID;
                sheet[1, i].Value = order.OrderStatus != null ? order.OrderStatus.StatusName : "Неизвестный";
                sheet[2, i].Value = order.OrderDate;
                if (order.OrderCustomer != null)
                {
                    sheet[3, i].Value = order.OrderCustomer.LastName + " " + order.OrderCustomer.FirstName;
                    sheet[4, i].Value = order.OrderCustomer.Email ?? string.Empty;
                    sheet[5, i].Value = order.OrderCustomer.MobilePhone ?? string.Empty;
                }
                else
                {
                    sheet[3, i].Value = "Неизвестный";
                    sheet[4, i].Value = string.Empty;
                    sheet[5, i].Value = string.Empty;
                }

                if (order.OrderCurrency != null)
                {
                    sheet[6, i].Value = RenderOrderedItems(order.OrderItems) ?? string.Empty;
                    sheet[7, i].Value = CatalogService.GetStringPrice(order.Sum, order.OrderCurrency);
                    sheet[8, i].Value = CatalogService.GetStringPrice(order.TaxCost, order.OrderCurrency);
                    float totalCost = order.OrderItems.Sum(oi => oi.SupplyPrice * oi.Amount);
                    sheet[9, i].Value = CatalogService.GetStringPrice(totalCost, order.OrderCurrency);
                    sheet[10, i].Value =
                        CatalogService.GetStringPrice(order.Sum - order.ShippingCost - order.TaxCost - totalCost,
                            order.OrderCurrency);
                    sheet[11, i].Value = order.PaymentMethodName;
                    sheet[12, i].Value = order.ArchivedShippingName + " - " +
                                         CatalogService.GetStringPrice(order.ShippingCost, order.OrderCurrency);
                    sheet[13, i].Value = order.ShippingContact != null
                        ? new List<string>
                        {
                            order.ShippingContact.Zip,
                            order.ShippingContact.Country,
                            order.ShippingContact.City,
                            order.ShippingContact.Address,
                            order.ShippingContact.CustomField1,
                            order.ShippingContact.CustomField2,
                            order.ShippingContact.CustomField3
                        }.Where(s => s.IsNotEmpty()).AggregateString(", ")
                        : string.Empty;

                    sheet[14, i].Value = order.CustomerComment ?? string.Empty;
                    sheet[15, i].Value = order.AdminOrderComment ?? string.Empty;
                    sheet[16, i].Value = order.Payed ? " Да" : "Нет";
                }

                i++;
            }

            return book;
        }

        #endregion



        protected static string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist, string color, string size)
        {
            var html = new StringBuilder();
            if (evlist != null && evlist.Count > 0)
            {
                html.Append(" (");

                if (!string.IsNullOrEmpty(color))
                    html.Append(Configuration.SettingsCatalog.ColorsHeader + ": " + color + ",");

                if (!string.IsNullOrEmpty(size))
                    html.Append(Configuration.SettingsCatalog.SizesHeader + ": " + size + ",");

                foreach (EvaluatedCustomOptions ev in evlist)
                {
                    html.Append(string.Format("{0}: {1},", ev.CustomOptionTitle, ev.OptionTitle));
                }

                html.Append(")");
            }
            return html.ToString();
        }

    }
}