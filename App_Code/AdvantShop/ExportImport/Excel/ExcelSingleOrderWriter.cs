//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using CarlosAg.ExcelXmlWriter;
using Resources;

namespace AdvantShop.ExportImport.Excel
{
    public class ExcelSingleOrderWriter
    {
        public void Generate(string filename, Order order)
        {
            var book = new Workbook();
            // -----------------------------------------------
            //  Properties
            // -----------------------------------------------
            book.Properties.Author = "AdVantShop.Net";
            book.Properties.LastAuthor = "AdVantShop.Net";
            book.Properties.Created = DateTime.Now;
            book.Properties.Version = "14.00";
            book.ExcelWorkbook.WindowHeight = 12330;
            book.ExcelWorkbook.WindowWidth = 24915;
            book.ExcelWorkbook.WindowTopX = 120;
            book.ExcelWorkbook.WindowTopY = 90;
            book.ExcelWorkbook.ProtectWindows = false;
            book.ExcelWorkbook.ProtectStructure = false;
            // -----------------------------------------------
            //  Generate Styles
            // -----------------------------------------------
            GenerateStyles(book.Styles);
            // -----------------------------------------------
            //  Generate Eeno1 Worksheet
            // -----------------------------------------------
            GenerateWorksheetSheet1(book.Worksheets, order);
            // -----------------------------------------------
            book.Save(filename);
        }

        private void GenerateStyles(WorksheetStyleCollection styles)
        {
            // -----------------------------------------------
            //  Default
            // -----------------------------------------------
            WorksheetStyle Default = styles.Add("Default");
            Default.Name = "Normal";
            Default.Font.FontName = "Calibri";
            Default.Font.Size = 11;
            Default.Font.Color = "#000000";
            Default.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  m51494400
            // -----------------------------------------------
            WorksheetStyle m51494400 = styles.Add("m51494400");
            m51494400.Font.FontName = "Verdana";
            m51494400.Font.Color = "#000000";
            m51494400.Interior.Color = "#ECECEC";
            m51494400.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494400.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494400.Alignment.WrapText = true;
            m51494400.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 3, "#959595");
            m51494400.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 3, "#959595");
            m51494400.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494400.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494420
            // -----------------------------------------------
            WorksheetStyle m51494420 = styles.Add("m51494420");
            m51494420.Font.FontName = "Verdana";
            m51494420.Font.Color = "#000000";
            m51494420.Interior.Color = "#ECECEC";
            m51494420.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494420.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494420.Alignment.WrapText = true;
            m51494420.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 3, "#959595");
            m51494420.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494420.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494420.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494440
            // -----------------------------------------------
            WorksheetStyle m51494440 = styles.Add("m51494440");
            m51494440.Font.FontName = "Verdana";
            m51494440.Font.Color = "#000000";
            m51494440.Interior.Color = "#ECECEC";
            m51494440.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494440.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            m51494440.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494440.Alignment.WrapText = true;
            m51494440.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 3, "#959595");
            m51494440.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494440.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494440.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494460
            // -----------------------------------------------
            WorksheetStyle m51494460 = styles.Add("m51494460");
            m51494460.Font.FontName = "Verdana";
            m51494460.Font.Color = "#000000";
            m51494460.Interior.Color = "#ECECEC";
            m51494460.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494460.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            m51494460.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494460.Alignment.WrapText = true;
            m51494460.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 3, "#959595");
            m51494460.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494460.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494460.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494480
            // -----------------------------------------------
            WorksheetStyle m51494480 = styles.Add("m51494480");
            m51494480.Font.FontName = "Verdana";
            m51494480.Font.Color = "#000000";
            m51494480.Interior.Color = "#ECECEC";
            m51494480.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494480.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494480.Alignment.WrapText = true;
            m51494480.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 3, "#959595");
            m51494480.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494480.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 3, "#959595");
            m51494480.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494176
            // -----------------------------------------------
            WorksheetStyle m51494176 = styles.Add("m51494176");
            m51494176.Font.FontName = "Verdana";
            m51494176.Font.Color = "#000000";
            m51494176.Interior.Color = "#ECECEC";
            m51494176.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494176.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494176.Alignment.WrapText = true;
            m51494176.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494176.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 3, "#959595");
            m51494176.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494176.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494196
            // -----------------------------------------------
            WorksheetStyle m51494196 = styles.Add("m51494196");
            m51494196.Font.FontName = "Verdana";
            m51494196.Font.Color = "#000000";
            m51494196.Interior.Color = "#ECECEC";
            m51494196.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494196.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494196.Alignment.WrapText = true;
            m51494196.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494196.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494196.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494196.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494216
            // -----------------------------------------------
            WorksheetStyle m51494216 = styles.Add("m51494216");
            m51494216.Font.FontName = "Verdana";
            m51494216.Font.Color = "#000000";
            m51494216.Interior.Color = "#ECECEC";
            m51494216.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494216.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            m51494216.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494216.Alignment.WrapText = true;
            m51494216.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494216.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494216.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494216.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494236
            // -----------------------------------------------
            WorksheetStyle m51494236 = styles.Add("m51494236");
            m51494236.Font.FontName = "Verdana";
            m51494236.Font.Color = "#000000";
            m51494236.Interior.Color = "#ECECEC";
            m51494236.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494236.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            m51494236.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494236.Alignment.WrapText = true;
            m51494236.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494236.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494236.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494236.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494256
            // -----------------------------------------------
            WorksheetStyle m51494256 = styles.Add("m51494256");
            m51494256.Font.FontName = "Verdana";
            m51494256.Font.Color = "#000000";
            m51494256.Interior.Color = "#ECECEC";
            m51494256.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494256.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494256.Alignment.WrapText = true;
            m51494256.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494256.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494256.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 3, "#959595");
            m51494256.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494276
            // -----------------------------------------------
            WorksheetStyle m51494276 = styles.Add("m51494276");
            m51494276.Font.FontName = "Verdana";
            m51494276.Font.Color = "#000000";
            m51494276.Interior.Color = "#FFFFFF";
            m51494276.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494276.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494276.Alignment.WrapText = true;
            m51494276.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494276.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 3, "#959595");
            m51494276.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494276.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494296
            // -----------------------------------------------
            WorksheetStyle m51494296 = styles.Add("m51494296");
            m51494296.Font.FontName = "Verdana";
            m51494296.Font.Color = "#000000";
            m51494296.Interior.Color = "#FFFFFF";
            m51494296.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494296.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494296.Alignment.WrapText = true;
            m51494296.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494296.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494296.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494296.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494316
            // -----------------------------------------------
            WorksheetStyle m51494316 = styles.Add("m51494316");
            m51494316.Font.FontName = "Verdana";
            m51494316.Font.Color = "#000000";
            m51494316.Interior.Color = "#FFFFFF";
            m51494316.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494316.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            m51494316.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494316.Alignment.WrapText = true;
            m51494316.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494316.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494316.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494316.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494336
            // -----------------------------------------------
            WorksheetStyle m51494336 = styles.Add("m51494336");
            m51494336.Font.FontName = "Verdana";
            m51494336.Font.Color = "#000000";
            m51494336.Interior.Color = "#FFFFFF";
            m51494336.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494336.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            m51494336.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494336.Alignment.WrapText = true;
            m51494336.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494336.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494336.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            m51494336.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  m51494356
            // -----------------------------------------------
            WorksheetStyle m51494356 = styles.Add("m51494356");
            m51494356.Font.FontName = "Verdana";
            m51494356.Font.Color = "#000000";
            m51494356.Interior.Color = "#FFFFFF";
            m51494356.Interior.Pattern = StyleInteriorPattern.Solid;
            m51494356.Alignment.Vertical = StyleVerticalAlignment.Center;
            m51494356.Alignment.WrapText = true;
            m51494356.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "#959595");
            m51494356.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "#959595");
            m51494356.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 3, "#959595");
            m51494356.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "#959595");
            // -----------------------------------------------
            //  s70
            // -----------------------------------------------
            WorksheetStyle s70 = styles.Add("s70");
            s70.Font.FontName = "Verdana";
            s70.Font.Color = "#000000";
            s70.Alignment.Vertical = StyleVerticalAlignment.Top;
            s70.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s83
            // -----------------------------------------------
            WorksheetStyle s83 = styles.Add("s83");
            s83.Font.FontName = "Verdana";
            s83.Font.Color = "#000000";
            s83.Interior.Color = "#FFFFFF";
            s83.Interior.Pattern = StyleInteriorPattern.Solid;
            s83.Alignment.Vertical = StyleVerticalAlignment.Center;
            s83.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s89
            // -----------------------------------------------
            WorksheetStyle s89 = styles.Add("s89");
            s89.Font.Bold = true;
            s89.Font.FontName = "Verdana";
            s89.Font.Color = "#000000";
            s89.Interior.Color = "#C2C2C2";
            s89.Interior.Pattern = StyleInteriorPattern.Solid;
            s89.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s89.Alignment.Vertical = StyleVerticalAlignment.Center;
            s89.Alignment.WrapText = true;
            s89.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 3, "#959595");
            s89.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            s89.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 3, "#959595");
            // -----------------------------------------------
            //  s90
            // -----------------------------------------------
            WorksheetStyle s90 = styles.Add("s90");
            s90.Font.Bold = true;
            s90.Font.FontName = "Verdana";
            s90.Font.Color = "#000000";
            s90.Interior.Color = "#C2C2C2";
            s90.Interior.Pattern = StyleInteriorPattern.Solid;
            s90.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s90.Alignment.Vertical = StyleVerticalAlignment.Center;
            s90.Alignment.WrapText = true;
            s90.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "#959595");
            s90.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 3, "#959595");
            // -----------------------------------------------
            //  s91
            // -----------------------------------------------
            WorksheetStyle s91 = styles.Add("s91");
            s91.Font.Bold = true;
            s91.Font.FontName = "Verdana";
            s91.Font.Color = "#000000";
            s91.Interior.Color = "#C2C2C2";
            s91.Interior.Pattern = StyleInteriorPattern.Solid;
            s91.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s91.Alignment.Vertical = StyleVerticalAlignment.Center;
            s91.Alignment.WrapText = true;
            s91.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 3, "#959595");
            s91.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 3, "#959595");
            // -----------------------------------------------
            //  s129
            // -----------------------------------------------
            WorksheetStyle s129 = styles.Add("s129");
            s129.Font.FontName = "Verdana";
            s129.Font.Color = "#000000";
            s129.Interior.Color = "#FFFFFF";
            s129.Interior.Pattern = StyleInteriorPattern.Solid;
            s129.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s129.Alignment.Vertical = StyleVerticalAlignment.Center;
            s129.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s130
            // -----------------------------------------------
            WorksheetStyle s130 = styles.Add("s130");
            s130.Font.Bold = true;
            s130.Font.FontName = "Verdana";
            s130.Font.Color = "#000000";
            s130.Interior.Color = "#FFFFFF";
            s130.Interior.Pattern = StyleInteriorPattern.Solid;
            s130.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s130.Alignment.Vertical = StyleVerticalAlignment.Center;
            s130.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s131
            // -----------------------------------------------
            WorksheetStyle s131 = styles.Add("s131");
            s131.Font.Bold = true;
            s131.Font.FontName = "Verdana";
            s131.Font.Color = "#000000";
            s131.Interior.Color = "#FFFFFF";
            s131.Interior.Pattern = StyleInteriorPattern.Solid;
            s131.Alignment.Vertical = StyleVerticalAlignment.Center;
            s131.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s132
            // -----------------------------------------------
            WorksheetStyle s132 = styles.Add("s132");
            s132.Font.FontName = "Verdana";
            s132.Font.Size = 18;
            s132.Font.Color = "#000000";
            s132.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s132.Alignment.Vertical = StyleVerticalAlignment.Center;
            s132.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s133
            // -----------------------------------------------
            WorksheetStyle s133 = styles.Add("s133");
            s133.Font.FontName = "Verdana";
            s133.Font.Color = "#666666";
            s133.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s133.Alignment.Vertical = StyleVerticalAlignment.Center;
            s133.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s134
            // -----------------------------------------------
            WorksheetStyle s134 = styles.Add("s134");
            s134.Alignment.Vertical = StyleVerticalAlignment.Center;
            s134.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s135
            // -----------------------------------------------
            WorksheetStyle s135 = styles.Add("s135");
            s135.Font.Bold = true;
            s135.Font.FontName = "Verdana";
            s135.Font.Color = "#000000";
            s135.Alignment.Vertical = StyleVerticalAlignment.Center;
            s135.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s136
            // -----------------------------------------------
            WorksheetStyle s136 = styles.Add("s136");
            s136.Font.FontName = "Verdana";
            s136.Font.Color = "#000000";
            s136.Alignment.Vertical = StyleVerticalAlignment.Center;
            s136.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s137
            // -----------------------------------------------
            WorksheetStyle s137 = styles.Add("s137");
            s137.Font.FontName = "Verdana";
            s137.Font.Color = "#000000";
            s137.Alignment.Vertical = StyleVerticalAlignment.Center;
            s137.Alignment.WrapText = true;
            s137.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 3, "#959595");
            // -----------------------------------------------
            //  s141
            // -----------------------------------------------
            WorksheetStyle s141 = styles.Add("s141");
            s141.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s141.Alignment.Vertical = StyleVerticalAlignment.Top;
        }

        private void GenerateWorksheetSheet1(WorksheetCollection sheets, Order order)
        {
            Culture.InitializeCulture();
            Worksheet sheet = sheets.Add(Resource.Admin_ViewOrder_ItemNum + order.OrderID);
            sheet.Table.DefaultRowHeight = 15F;
            sheet.Table.ExpandedColumnCount = 5;
            List<OrderTax> taxedItems = TaxServices.GetOrderTaxes(order.OrderID);
            sheet.Table.ExpandedRowCount = 42 + order.OrderItems.Count * 2 + taxedItems.Count;
            sheet.Table.FullColumns = 1;
            sheet.Table.FullRows = 1;
            WorksheetColumn column0 = sheet.Table.Columns.Add();
            column0.Width = 186;
            column0.Span = 1;
            WorksheetColumn column1 = sheet.Table.Columns.Add();
            column1.Index = 3;
            column1.Width = 156;
            sheet.Table.Columns.Add(120);
            sheet.Table.Columns.Add(89);
            // Order ID-----------------------------------------------
            WorksheetRow Row0 = sheet.Table.Rows.Add();
            Row0.Height = 22;
            WorksheetCell cell;
            cell = Row0.Cells.Add();
            cell.StyleID = "s132";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_ItemNum + order.OrderID;
            cell.MergeAcross = 4;
            // Status -----------------------------------------------
            WorksheetRow Row1 = sheet.Table.Rows.Add();
            cell = Row1.Cells.Add();
            cell.StyleID = "s133";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "(" + order.OrderStatus.StatusName + ")";
            cell.MergeAcross = 4;
            //  -----------------------------------------------
            WorksheetRow Row2 = sheet.Table.Rows.Add();
            cell = Row2.Cells.Add();
            cell.StyleID = "s134";
            cell.MergeAcross = 4;
            // Date -----------------------------------------------
            WorksheetRow Row3 = sheet.Table.Rows.Add();
            cell = Row3.Cells.Add();
            cell.StyleID = "s135";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_Date;
            cell = Row3.Cells.Add();
            cell.StyleID = "s70";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Culture.ConvertDate(order.OrderDate);
            cell.MergeAcross = 3;
            // NUmber to status check -----------------------------------------------
            WorksheetRow Row4 = sheet.Table.Rows.Add();
            cell = Row4.Cells.Add();
            cell.StyleID = "s135";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_Number;
            cell = Row4.Cells.Add();
            cell.StyleID = "s70";
            cell.Data.Type = DataType.String;
            cell.Data.Text = order.Number;
            cell.MergeAcross = 3;
            // Status comment -----------------------------------------------
            WorksheetRow Row5 = sheet.Table.Rows.Add();
            cell = Row5.Cells.Add();
            cell.StyleID = "s135";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_StatusComment;
            cell = Row5.Cells.Add();
            cell.StyleID = "s70";
            cell.Data.Type = DataType.String;
            cell.Data.Text = order.StatusComment;
            cell.MergeAcross = 3;


            WorksheetRow RowEmail = sheet.Table.Rows.Add();
            cell = RowEmail.Cells.Add();
            cell.StyleID = "s135";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewCustomer_Email;
            cell = RowEmail.Cells.Add();
            cell.StyleID = "s70";
            cell.Data.Type = DataType.String;
            cell.Data.Text = order.OrderCustomer.Email;
            cell.MergeAcross = 3;

            WorksheetRow RowPhone = sheet.Table.Rows.Add();
            cell = RowPhone.Cells.Add();
            cell.StyleID = "s135";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_CommonSettings_Phone;
            cell = RowPhone.Cells.Add();
            cell.StyleID = "s70";
            cell.Data.Type = DataType.String;
            cell.Data.Text = order.OrderCustomer.MobilePhone;
            cell.MergeAcross = 3;

            // -----------------------------------------------
            WorksheetRow Row6 = sheet.Table.Rows.Add();
            cell = Row6.Cells.Add();
            cell.StyleID = "s136";
            cell.MergeAcross = 4;
            // Headers -----------------------------------------------
            WorksheetRow Row7 = sheet.Table.Rows.Add();
            Row7.Cells.Add(Resource.Admin_ViewOrder_Billing, DataType.String, "s70");
            Row7.Cells.Add(Resource.Admin_ViewOrder_Shipping, DataType.String, "s70");
            Row7.Cells.Add(Resource.Admin_ViewOrder_ShippingMethod, DataType.String, "s70");
            // Names -----------------------------------------------
            WorksheetRow Row8 = sheet.Table.Rows.Add();
            Row8.Cells.Add("     " + Resource.Admin_ViewOrder_Name + order.BillingContact.Name, DataType.String, "s70");

            Row8.Cells.Add("     " + Resource.Admin_ViewOrder_Name + order.ShippingContact.Name, DataType.String, "s70");
            var shippingMethodName = order.ArchivedShippingName;
            if (order.OrderPickPoint != null)
                shippingMethodName += order.OrderPickPoint.PickPointAddress.Replace("<br/>", " ");

            Row8.Cells.Add("     " + shippingMethodName, DataType.String, "s70");
            // Countries -----------------------------------------------
            WorksheetRow Row9 = sheet.Table.Rows.Add();
            Row9.Cells.Add("     " + Resource.Admin_ViewOrder_Country + order.BillingContact.Country, DataType.String,
                           "s70");
            Row9.Cells.Add("     " + Resource.Admin_ViewOrder_Country + order.ShippingContact.Country, DataType.String,
                           "s70");
            Row9.Cells.Add(Resource.Admin_ViewOrder_PaymentType, DataType.String, "s70");
            // Cities -----------------------------------------------
            WorksheetRow Row10 = sheet.Table.Rows.Add();
            Row10.Cells.Add("     " + Resource.Admin_ViewOrder_City + order.BillingContact.City, DataType.String, "s70");
            Row10.Cells.Add("     " + Resource.Admin_ViewOrder_City + order.ShippingContact.City, DataType.String, "s70");
            Row10.Cells.Add("     " + order.PaymentMethodName, DataType.String, "s70");
            // Zones -----------------------------------------------
            WorksheetRow Row11 = sheet.Table.Rows.Add();
            Row11.Cells.Add("     " + Resource.Admin_ViewOrder_Zone + order.BillingContact.Zone, DataType.String, "s70");
            Row11.Cells.Add("     " + Resource.Admin_ViewOrder_Zone + order.ShippingContact.Zone, DataType.String, "s70");
            cell = Row11.Cells.Add();
            cell.StyleID = "s70";
            // Zips -----------------------------------------------
            WorksheetRow Row12 = sheet.Table.Rows.Add();
            Row12.Cells.Add("     " + Resource.Admin_ViewOrder_Zip + order.BillingContact.Zip, DataType.String, "s70");
            Row12.Cells.Add("     " + Resource.Admin_ViewOrder_Zip + order.ShippingContact.Zip, DataType.String, "s70");
            cell = Row12.Cells.Add();
            cell.StyleID = "s70";
            // Adresses -----------------------------------------------
            WorksheetRow Row13 = sheet.Table.Rows.Add();
            Row13.Cells.Add("     " + Resource.Admin_ViewOrder_Address + order.BillingContact.Address, DataType.String,
                            "s70");
            Row13.Cells.Add("     " + Resource.Admin_ViewOrder_Address + order.ShippingContact.Address, DataType.String,
                            "s70");
            cell = Row13.Cells.Add();
            cell.StyleID = "s70";
            // -----------------------------------------------
            WorksheetRow Row17 = sheet.Table.Rows.Add();
            cell = Row17.Cells.Add();
            cell.StyleID = "s134";
            cell.MergeAcross = 4;
            // Orders -----------------------------------------------
            WorksheetRow Row18 = sheet.Table.Rows.Add();
            cell = Row18.Cells.Add();
            cell.StyleID = "s136";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_OrderItem;
            cell.MergeAcross = 4;
            // -----------------------------------------------
            WorksheetRow Row19 = sheet.Table.Rows.Add();
            Row19.Height = 15;
            cell = Row19.Cells.Add();
            cell.StyleID = "s137";
            cell.MergeAcross = 4;
            // Order items table header -----------------------------------------------
            WorksheetRow Row20 = sheet.Table.Rows.Add();
            Row20.Height = 16;
            Row20.Cells.Add(Resource.Admin_ViewOrder_ItemName, DataType.String, "s89");
            Row20.Cells.Add(Resource.Admin_ViewOrder_CustomOptions, DataType.String, "s90");
            Row20.Cells.Add(Resource.Admin_ViewOrder_Price, DataType.String, "s90");
            Row20.Cells.Add(Resource.Admin_ViewOrder_ItemAmount, DataType.String, "s90");
            Row20.Cells.Add(Resource.Admin_ViewOrder_ItemCost, DataType.String, "s91");
            // Order items -----------------------------------------------
            foreach (OrderItem item in order.OrderItems)
            {
                WorksheetRow Row = sheet.Table.Rows.Add();
                cell = Row.Cells.Add();
                cell.StyleID = "m51494176";
                cell.Data.Type = DataType.String;
                cell.Data.Text = item.ArtNo + ", " + item.Name;
                cell.MergeDown = 1;
                cell = Row.Cells.Add();
                cell.StyleID = "m51494196";
                var html = new StringBuilder();

                if (!string.IsNullOrEmpty(item.Color))
                    html.Append(Configuration.SettingsCatalog.ColorsHeader + ": " + item.Color + " \n");

                if (!string.IsNullOrEmpty(item.Size))
                    html.Append(Configuration.SettingsCatalog.SizesHeader + ": " + item.Size + " \n");

                foreach (EvaluatedCustomOptions ev in item.SelectedOptions)
                {
                    html.Append(string.Format("- {0}: {1} \n", ev.CustomOptionTitle, ev.OptionTitle));
                }

                cell.Data.Text = html.ToString();
                cell.MergeDown = 1;
                cell = Row.Cells.Add();
                cell.StyleID = "m51494216";
                cell.Data.Type = DataType.String;

                cell.Data.Text = CatalogService.GetStringPrice(item.Price, order.OrderCurrency.CurrencyValue,
                                                               order.OrderCurrency.CurrencyCode);
                cell.MergeDown = 1;
                cell = Row.Cells.Add();
                cell.StyleID = "m51494236";
                cell.Data.Type = DataType.String;
                cell.Data.Text = item.Amount.ToString();
                cell.MergeDown = 1;
                cell = Row.Cells.Add();
                cell.StyleID = "m51494256";
                cell.Data.Type = DataType.String;

                cell.Data.Text = CatalogService.GetStringPrice(item.Price * item.Amount,
                                                               order.OrderCurrency.CurrencyValue,
                                                               order.OrderCurrency.CurrencyCode);
                cell.MergeDown = 1;
                // -----------------------------------------------
                WorksheetRow RowSep = sheet.Table.Rows.Add();
                RowSep.Height = 15;
            }
            // -----------------------------------------------
            WorksheetRow Row27 = sheet.Table.Rows.Add();
            Row27.Height = 15;
            cell = Row27.Cells.Add();
            cell.StyleID = "s70";
            cell = Row27.Cells.Add();
            cell.StyleID = "s70";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_ItemCost2;
            cell.Index = 4;

            var ordCurrency = order.OrderCurrency;
            float productPrice = order.OrderItems.Sum(item => item.Amount * item.Price);

            Row27.Cells.Add(
                CatalogService.GetStringPrice(productPrice, ordCurrency.CurrencyValue, ordCurrency.CurrencyCode),
                DataType.String, "s70");


            //-----------------------------------------------

            if (order.Coupon != null)
            {

                WorksheetRow Row28 = sheet.Table.Rows.Add();
                cell = Row28.Cells.Add();
                cell.StyleID = "s70";
                cell = Row28.Cells.Add();
                cell.StyleID = "s70";
                cell.Data.Type = DataType.String;
                cell.Data.Text = Resource.Admin_ViewOrder_Coupon;
                cell.Index = 4;

                var productsWithCoupon = order.OrderItems.Where(item => item.IsCouponApplied).Sum(item => item.Price * item.Amount);

                switch (order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        Row28.Cells.Add(String.Format("-{0} ({1})",
                                                       CatalogService.GetStringPrice(order.Coupon.Value, ordCurrency.CurrencyValue, ordCurrency.CurrencyCode),
                                                       order.Coupon.Code), DataType.String, "s70");
                        break;
                    case CouponType.Percent:
                        Row28.Cells.Add(String.Format("-{0} ({1}%) ({2})",
                                                      CatalogService.GetStringPrice(productsWithCoupon * order.Coupon.Value / 100, ordCurrency.CurrencyValue, ordCurrency.CurrencyCode),
                                                      CatalogService.FormatPriceInvariant(order.Coupon.Value),
                                                      order.Coupon.Code), DataType.String, "s70");
                        break;
                }

            }

            // -----------------------------------------------
            float totalDiscount = order.OrderDiscount;
            if (totalDiscount > 0)
            {
                WorksheetRow Row28 = sheet.Table.Rows.Add();
                cell = Row28.Cells.Add();
                cell.StyleID = "s70";
                cell = Row28.Cells.Add();
                cell.StyleID = "s70";
                cell.Data.Type = DataType.String;
                cell.Data.Text = Resource.Admin_ViewOrder_ItemDiscount;
                cell.Index = 4;

                Row28.Cells.Add(
                    "-" +
                    CatalogService.GetStringDiscountPercent(productPrice, totalDiscount,
                                                            ordCurrency.CurrencyValue, ordCurrency.CurrencySymbol,
                                                            ordCurrency.IsCodeBefore,
                                                            CurrencyService.CurrentCurrency.PriceFormat, false),
                    DataType.String, "s70");
            }

            // -------------------------------------------------------

            if (order.Certificate != null)
            {
                WorksheetRow Row28 = sheet.Table.Rows.Add();
                cell = Row28.Cells.Add();
                cell.StyleID = "s70";
                cell = Row28.Cells.Add();
                cell.StyleID = "s70";
                cell.Data.Type = DataType.String;
                cell.Data.Text = Resource.Admin_ViewOrder_ItemDiscount;
                cell.Index = 4;

                Row28.Cells.Add(
                    "-" +
                    string.Format("-{0}", CatalogService.GetStringPrice(order.Certificate.Price, ordCurrency.CurrencyValue, ordCurrency.CurrencyCode)),
                    DataType.String, "s70");
            }

            // -----------------------------------------------

            float bonusPrice = order.BonusCost;
            if (bonusPrice > 0)
            {
                WorksheetRow Row28_5 = sheet.Table.Rows.Add();
                cell = Row28_5.Cells.Add();
                cell.StyleID = "s70";
                cell = Row28_5.Cells.Add();
                cell.StyleID = "s70";
                cell.Data.Type = DataType.String;
                cell.Data.Text = Resource.Admin_ViewOrder_Bonuses;
                cell.Index = 4;

                Row28_5.Cells.Add("-" + CatalogService.GetStringPrice(bonusPrice), DataType.String, "s70");
            }
            // -----------------------------------------------
            WorksheetRow Row29 = sheet.Table.Rows.Add();
            cell = Row29.Cells.Add();
            cell.StyleID = "s70";
            cell = Row29.Cells.Add();
            cell.StyleID = "s70";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_ShippingPrice;
            cell.Index = 4;

            Row29.Cells.Add(
                "+" +
                CatalogService.GetStringPrice(order.ShippingCost, order.OrderCurrency.CurrencyValue,
                                              order.OrderCurrency.CurrencyCode), DataType.String, "s70");

            if (taxedItems.Count > 0)
                foreach (OrderTax tax in taxedItems)
                {
                    WorksheetRow Row = sheet.Table.Rows.Add();
                    cell = Row.Cells.Add();
                    cell.StyleID = "s70";
                    cell = Row.Cells.Add();
                    cell.StyleID = "s70";
                    cell.Data.Type = DataType.String;
                    cell.Data.Text = (tax.TaxShowInPrice ? Resource.Core_TaxServices_Include_Tax + " " : "") +
                                     tax.TaxName + ":";
                    cell.Index = 4;
                    Row.Cells.Add(
                        (tax.TaxShowInPrice ? "" : "+") +
                        CatalogService.GetStringPrice(tax.TaxSum, order.OrderCurrency.CurrencyValue,
                                                      order.OrderCurrency.CurrencyCode), DataType.String, "s70");
                }
            else
            {
                WorksheetRow Row30a = sheet.Table.Rows.Add();
                cell = Row30a.Cells.Add();
                cell.StyleID = "s70";
                cell = Row30a.Cells.Add();
                cell.StyleID = "s70";
                cell.Data.Type = DataType.String;
                cell.Data.Text = Resource.Admin_ViewOrder_Taxes;
                cell.Index = 4;

                Row30a.Cells.Add(
                    "+" +
                    CatalogService.GetStringPrice(0, order.OrderCurrency.CurrencyValue, order.OrderCurrency.CurrencyCode),
                    DataType.String, "s70");
            }
            // -----------------------------------------------

            if (order.PaymentCost > 0)
            {
                WorksheetRow Row291 = sheet.Table.Rows.Add();
                cell = Row291.Cells.Add();
                cell.StyleID = "s70";
                cell = Row291.Cells.Add();
                cell.StyleID = "s70";
                cell.Data.Type = DataType.String;
                cell.Data.Text = Resource.Admin_ViewOrder_PaymentExtracharge;
                cell.Index = 4;

                Row291.Cells.Add(
                    "+" +
                    CatalogService.GetStringPrice(order.PaymentCost, order.OrderCurrency.CurrencyValue,
                                                  order.OrderCurrency.CurrencyCode), DataType.String, "s70");
            }

            // -----------------------------------------------

         WorksheetRow Row30 = sheet.Table.Rows.Add();
            cell = Row30.Cells.Add();
            cell.StyleID = "s70";
            cell = Row30.Cells.Add();
            cell.StyleID = "s135";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Admin_ViewOrder_TotalPrice;
            cell.Index = 4;

            cell =
                Row30.Cells.Add(
                    CatalogService.GetStringPrice(order.Sum, order.OrderCurrency.CurrencyValue,
                                                  order.OrderCurrency.CurrencyCode), DataType.String, "s135");

            // -----------------------------------------------
            WorksheetRow Row31 = sheet.Table.Rows.Add();
            cell = Row31.Cells.Add();
            cell.StyleID = "s135";
            cell.Data.Type = DataType.String;
            cell.Data.Text = Resource.Client_PrintOrder_YourComment;
            cell.MergeAcross = 4;
            // -----------------------------------------------
            WorksheetRow Row32 = sheet.Table.Rows.Add();
            cell = Row32.Cells.Add();
            cell.StyleID = "s141";
            cell.Data.Type = DataType.String;
            cell.Data.Text = order.CustomerComment;
            cell.MergeAcross = 4;
            cell.MergeDown = 1;
            // -----------------------------------------------
            //  Options
            // -----------------------------------------------
            sheet.Options.Selected = true;
            sheet.Options.ProtectObjects = false;
            sheet.Options.ProtectScenarios = false;
            sheet.Options.PageSetup.Header.Margin = 0.3F;
            sheet.Options.PageSetup.Footer.Margin = 0.3F;
            sheet.Options.PageSetup.PageMargins.Bottom = 0.75F;
            sheet.Options.PageSetup.PageMargins.Left = 0.7F;
            sheet.Options.PageSetup.PageMargins.Right = 0.7F;
            sheet.Options.PageSetup.PageMargins.Top = 0.75F;
            sheet.Options.Print.PaperSizeIndex = 9;
            sheet.Options.Print.VerticalResolution = 0;
            sheet.Options.Print.ValidPrinterInfo = true;
        }
    }
}