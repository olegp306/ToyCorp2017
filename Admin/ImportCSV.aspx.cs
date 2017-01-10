//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using AdvantShop.Statistic;
using AdvantShop.Trial;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class ImportCSV : AdvantShopAdminPage
    {
        private readonly string _filePath;
        private readonly string _fullPath;
        private bool _hasHeadrs;
        private readonly Dictionary<string, int> _fieldMapping = new Dictionary<string, int>();
        private readonly List<ProductFields> _mustRequiredFfield;
        private const string StrFileName = "importCSV";
        private const string StrFileExt = ".csv";

        protected ImportCSV()
        {
            _filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(_filePath);
            _mustRequiredFfield = new List<ProductFields>();

            _fullPath = Directory.GetFiles(_filePath).Where(f => f.Contains(StrFileName)).OrderByDescending(x => x).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(_fullPath)) return;
            _fullPath = _filePath + (StrFileName + StrFileExt).FileNamePlusDate();
        }

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lblError.Visible = false;
                lblError.Text = string.Empty;
            }
            else
            {
                lblError.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lblError.Visible = true;
            lblError.Text = @"<br/>" + messageText;
        }

        protected void btnAction_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(lblError.Text))
                return;


            if (!_fieldMapping.ContainsKey(ProductFields.Sku.StrName()) && !_fieldMapping.ContainsKey(ProductFields.Name.StrName()))
            {
                MsgErr(Resource.Admin_ImportCsv_SelectNameOrSKU);
                return;
            }

            divAction.Visible = false;
            choseDiv.Visible = false;
            if (!File.Exists(_fullPath)) return;

            if (CommonStatistic.IsRun) return;
            _hasHeadrs = Request["hasheadrs"] == "true";
            CommonStatistic.Init();
            CommonStatistic.CurrentProcess = Request.Url.PathAndQuery;
            CommonStatistic.CurrentProcessName = Resource.Admin_ImportXLS_CatalogUpload;
            linkCancel.Visible = true;
            MsgErr(true);
            lblRes.Text = string.Empty;
            CsvImport.Factory(_fullPath, _hasHeadrs, chboxDisableProducts.Checked, CsvSettings.CsvSeparator, CsvSettings.CsvEnconing, _fieldMapping, CsvSettings.CsvColumSeparator, CsvSettings.CsvPropertySeparator).Process();
            OutDiv.Visible = true;
            TrialService.TrackEvent(TrialEvents.MakeCSVImport, "");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            MsgErr(true);
            divStart.Visible = !CommonStatistic.IsRun && (string.IsNullOrEmpty(Request["action"]));
            divAction.Visible = !CommonStatistic.IsRun && (Request["action"] == "start");

            choseDiv.Visible = !CommonStatistic.IsRun;

            OutDiv.Visible = CommonStatistic.IsRun;
            linkCancel.Visible = CommonStatistic.IsRun;

            if (CommonStatistic.IsRun || (Request["action"] != "start")) return;
            if (!File.Exists(_fullPath)) return;

            var tbl = new Table { ID = "tblValues" };
            var namesRow = new TableRow { ID = "namesRow", BackColor = System.Drawing.ColorTranslator.FromHtml("#0D76B8"), Height=28};
            var firstValRow = new TableRow { ID = "firstValsRow" };
            var ddlRow = new TableRow { ID = "ddlRow" };

            var firstCell = new TableCell { Width = 200, BackColor = System.Drawing.Color.White, };
            firstCell.Controls.Add(new Label { Text = Resource.Admin_ImportCsv_Column, CssClass = "firstColumn" });
            var div1 = new Panel { CssClass = "arrow_left_bg" };
            div1.Controls.Add(new Panel { CssClass = "arrow_right_bg" });
            firstCell.Controls.Add(div1);


            var secondCell = new TableCell { Width = 200 };
            secondCell.Controls.Add(new Label { Text = Resource.Admin_ImportCsv_FistLineInTheFile, CssClass = "firstColumn" });
            var div2 = new Panel { CssClass = "arrow_left_bg_two" };
            div2.Controls.Add(new Panel { CssClass = "arrow_right_bg" });
            secondCell.Controls.Add(div2);

            var firdCell = new TableCell { Width = 200 };
            firdCell.Controls.Add(new Label { Text = Resource.Admin_ImportCsv_DataType, CssClass = "firstColumn" });
            var div3 = new Panel { CssClass = "arrow_left_bg_free" };
            div3.Controls.Add(new Panel { CssClass = "arrow_right_bg" });
            firdCell.Controls.Add(div3);
            var div4 = new Panel { Width = 200 };
            firdCell.Controls.Add(div4);

            namesRow.Cells.Add(firstCell);
            firstValRow.Cells.Add(secondCell);
            ddlRow.Cells.Add(firdCell);

            _hasHeadrs = Request["hasheadrs"].TryParseBool();
            var csvrows = CsvImport.Factory(_fullPath, false, false, CsvSettings.CsvSeparator, CsvSettings.CsvEnconing, null, CsvSettings.CsvColumSeparator, CsvSettings.CsvPropertySeparator).ReadFirst2();

            if (csvrows.Count == 0)
            {
                MsgErr(Resource.Admin_ImportCsv_ErrorReadFille);
                return;
            }

            if (_hasHeadrs && csvrows[0].HasDuplicates())
            {
                var strFileds = string.Empty;
                foreach (var item in csvrows[0].Duplicates())
                {
                    strFileds += "\"" + item + "\",";
                }
                MsgErr(Resource.Admin_ImportCsv_DuplicateHeader + strFileds.Trim(','));
                btnAction.Visible = false;
            }

            for (int i = 0; i < csvrows[0].Length; i++)
            {
                var cell = new TableCell { ID = "cell" + i };
                var lb = new Label();
                bool flagMustReqField = false;
                if (Request["hasheadrs"].ToLower() == "true")
                {
                    var tempCsv = (csvrows[0][i].Length > 50 ? csvrows[0][i].Substring(0, 49) : csvrows[0][i]);
                    if (_mustRequiredFfield.Any(item => item.StrName() == tempCsv.ToLower()))
                    {
                        flagMustReqField = true;
                    }
                    lb.Text = tempCsv;
                }
                else
                {
                    lb.Text = Resource.Admin_ImportCsv_Empty;
                }
                lb.ForeColor = System.Drawing.Color.White;
                cell.Controls.Add(lb);

                if (flagMustReqField)
                {
                    var lbl = new Label
                        {
                            Text = @"*",
                            ForeColor = System.Drawing.Color.Red
                        };
                    cell.Controls.Add(lbl);
                }

                namesRow.Cells.Add(cell);

                cell = new TableCell { Width = 150 };
                var ddl = new DropDownList { ID = "ddlType" + i, Width = 150 };
                ddl.Items.Add(new ListItem(ProductFields.None.ResourceKey(), ProductFields.None.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Sku.ResourceKey(), ProductFields.Sku.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Name.ResourceKey(), ProductFields.Name.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.ParamSynonym.ResourceKey(), ProductFields.ParamSynonym.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Category.ResourceKey(), ProductFields.Category.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Enabled.ResourceKey(), ProductFields.Enabled.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Price.ResourceKey(), ProductFields.Price.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.PurchasePrice.ResourceKey(), ProductFields.PurchasePrice.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Amount.ResourceKey(), ProductFields.Amount.StrName()));

                ddl.Items.Add(new ListItem(ProductFields.MultiOffer.ResourceKey(), ProductFields.MultiOffer.StrName()));

                ddl.Items.Add(new ListItem(ProductFields.Unit.ResourceKey(), ProductFields.Unit.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Discount.ResourceKey(), ProductFields.Discount.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.ShippingPrice.ResourceKey(), ProductFields.ShippingPrice.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Weight.ResourceKey(), ProductFields.Weight.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Size.ResourceKey(), ProductFields.Size.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.BriefDescription.ResourceKey(), ProductFields.BriefDescription.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Description.ResourceKey(), ProductFields.Description.StrName()));

                ddl.Items.Add(new ListItem(ProductFields.Title.ResourceKey(), ProductFields.Title.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.MetaKeywords.ResourceKey(), ProductFields.MetaKeywords.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.MetaDescription.ResourceKey(), ProductFields.MetaDescription.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.H1.ResourceKey(), ProductFields.H1.StrName()));


                ddl.Items.Add(new ListItem(ProductFields.Photos.ResourceKey(), ProductFields.Photos.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Videos.ResourceKey(), ProductFields.Videos.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Markers.ResourceKey(), ProductFields.Markers.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Properties.ResourceKey(), ProductFields.Properties.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Producer.ResourceKey(), ProductFields.Producer.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.OrderByRequest.ResourceKey(), ProductFields.OrderByRequest.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.SalesNotes.ResourceKey(), ProductFields.SalesNotes.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Sorting.ResourceKey(), ProductFields.Sorting.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Related.ResourceKey(), ProductFields.Related.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Alternative.ResourceKey(), ProductFields.Alternative.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.CustomOption.ResourceKey(), ProductFields.CustomOption.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Gtin.ResourceKey(), ProductFields.Gtin.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.GoogleProductCategory.ResourceKey(), ProductFields.GoogleProductCategory.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.Adult.ResourceKey(), ProductFields.Adult.StrName()));
                ddl.Items.Add(new ListItem(ProductFields.ManufacturerWarranty.ResourceKey(), ProductFields.ManufacturerWarranty.StrName()));

                ddl.SelectedValue = lb.Text.Trim().ToLower();
                cell.Controls.Add(ddl);
                ddlRow.Cells.Add(cell);
            }

            var dataRow = csvrows.Count > 1 ? csvrows[1] : csvrows[0];

            if (dataRow != null)
                for (int i = 0; i < dataRow.Length; i++)
                {
                    var cell = new TableCell();
                    if (dataRow[i] == null)
                        cell.Controls.Add(new Label { Text = string.Empty });
                    else
                        cell.Controls.Add(new Label { Text = dataRow[i].Length > 50 ? dataRow[i].Substring(0, 49).HtmlEncode() : dataRow[i].HtmlEncode() });
                    firstValRow.Cells.Add(cell);
                }

            tbl.Rows.Add(namesRow);
            tbl.Rows.Add(firstValRow);
            tbl.Rows.Add(ddlRow);
            choseDiv.Controls.Add(tbl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ImportXLS_Title));

            if ((SaasDataService.IsSaasEnabled) && (!SaasDataService.CurrentSaasData.HaveExcel))
            {
                mainDiv.Visible = false;
                notInTariff.Visible = true;
            }

            if (!IsPostBack)
            {
                if (divStart.Visible)
                {
                    ddlEncoding.Items.Clear();
                    foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
                    {
                        ddlEncoding.Items.Add(new ListItem(enumItem.ToString(), enumItem.StrName()));
                    }
                    ddlEncoding.SelectedValue = CsvSettings.CsvEnconing;

                    ddlSeparetors.Items.Clear();
                    foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
                    {
                        ddlSeparetors.Items.Add(new ListItem(enumItem.ResourceKey(), enumItem.StrName()));
                    }

                    foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
                    {
                        if (CsvSettings.CsvSeparator == enumItem.StrName())
                            ddlSeparetors.SelectedValue = enumItem.StrName();

                        if (CsvSettings.CsvSeparator == SeparatorsEnum.Custom.StrName())
                            txtCustomSeparator.Text = CsvSettings.CsvSeparator;
                    }
   
                    txtColumSeparator.Text = CsvSettings.CsvColumSeparator;
                    txtPropertySeparator.Text = CsvSettings.CsvPropertySeparator;
                }
            }

            if (choseDiv.FindControl("tblValues") != null && IsPostBack)
            {
                short index = 0;
                var cells = ((TableRow)choseDiv.FindControl("ddlRow")).Cells;
                foreach (TableCell item in cells)
                {
                    var element = item.Controls.OfType<DropDownList>().FirstOrDefault();
                    if (element == null) continue;

                    if (element.SelectedValue != ProductFields.None.StrName())
                    {
                        if (!_fieldMapping.ContainsKey(element.SelectedValue))
                            _fieldMapping.Add(element.SelectedValue, index);
                        else
                        {
                            MsgErr(string.Format(Resource.Admin_ImportCsv_DuplicateMessage, element.SelectedItem.Text));
                            return;
                        }
                    }
                    index++;
                }
            }

            if (!btnAction.Visible || !IsPostBack) return;

            foreach (var item in _mustRequiredFfield.Where(item => !_fieldMapping.ContainsKey(item.StrName())))
            {
                MsgErr(string.Format(Resource.Admin_ImportCsv_NotChoice, item.ResourceKey()));
                return;
            }
        }


        protected void linkCancel_Click(object sender, EventArgs e)
        {
            CommonStatistic.IsRun = false;
            hlDownloadImportLog.Attributes.CssStyle["display"] = "inline";
        }

        protected void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (!FileUpload.HasFile) return;
            //delete old
            foreach (var item in Directory.GetFiles(_filePath).Where(f => f.Contains(StrFileName)))
            {
                FileHelpers.DeleteFile(item);
            }

            FileUpload.SaveAs(_fullPath);
            if (!File.Exists(_fullPath)) return;

            CsvSettings.CsvEnconing = ddlEncoding.SelectedValue;

            CsvSettings.CsvSeparator = ddlSeparetors.SelectedValue == SeparatorsEnum.Custom.StrName() ? txtCustomSeparator.Text : ddlSeparetors.SelectedValue;

            CsvSettings.CsvColumSeparator = txtColumSeparator.Text;
            CsvSettings.CsvPropertySeparator = txtPropertySeparator.Text;

            Response.Redirect("ImportCSV.aspx?action=start&hasheadrs=" + chbHasHeadrs.Checked.ToString().ToLower());
        }
    }
}