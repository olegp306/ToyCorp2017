//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using AdvantShop.Statistic;
using AdvantShop.Trial;
using Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class ExportCsv : AdvantShopAdminPage
    {
        private readonly string _strFilePath;
        private string _strFullPath;
        public string NotDoPost = string.Empty;
        protected List<ProductFields> FieldMapping = new List<ProductFields>();
        private const string StrFileName = "products";
        private const string StrFileExt = ".csv";
        protected string ExtStrFileName;
        protected const string CategorySort = "categorySort";

        public ExportCsv()
        {
            _strFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(_strFilePath);
            var firstFile = Directory.GetFiles(_strFilePath).FirstOrDefault(f => f.Contains(StrFileName));
            _strFullPath = firstFile;
            ExtStrFileName = Path.GetFileName(firstFile);

            if (!string.IsNullOrWhiteSpace(_strFullPath)) return;
            ExtStrFileName = (StrFileName + StrFileExt).FileNamePlusDate();
            _strFullPath = _strFilePath + ExtStrFileName;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CommonHelper.DisableBrowserCache();
            choseDiv.Visible = !CommonStatistic.IsRun;
            divAction.Visible = !CommonStatistic.IsRun;
            divbtnAction.Visible = !CommonStatistic.IsRun;

            LoadFirstProduct();
            var tbl = new Table { ID = "tblValues" };
            var ddlRow = new TableRow { ID = "ddlRow" };
            var lblRow = new TableRow { ID = "lblRow", BackColor = ColorTranslator.FromHtml("#EFF0F2") };
            var cellM = new TableCell { ID = "cellM" };
            cellM.Attributes.Add("style", "vertical-align:top; width:150px");
            cellM.Controls.Add(new Label { Text = Resource.Admin_ExportCsv_Column });
            ddlRow.Cells.Add(cellM);

            var cellL = new TableCell { ID = "cellL" };
            cellL.Attributes.Add("style", "vertical-align:top; width:150px");
            cellL.Controls.Add(new Label { Text = Resource.Admin_ExportCsv_SampleOfData });
            var div4 = new Panel { Width = 110 };
            cellL.Controls.Add(div4);
            lblRow.Cells.Add(cellL);

            foreach (var item in Enum.GetValues(typeof(ProductFields)))
            {
                var enumTemp = (ProductFields)item;
                var enumIntStringTemp = enumTemp.ConvertIntString();
                // none and photo in export by default no need
                if (enumTemp == ProductFields.None || enumTemp == ProductFields.Sorting) continue;
                var cell = new TableCell { ID = "cell" + enumIntStringTemp };
                cell.Attributes.Add("style", "vertical-align:top");
                var ddl = new DropDownList { ID = "ddlType" + ((int)item), Width = 150 };
                ddl.Items.Add(new ListItem(ProductFields.None.ResourceKey(), ProductFields.None.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Sku.ResourceKey(), ProductFields.Sku.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Name.ResourceKey(), ProductFields.Name.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.ParamSynonym.ResourceKey(), ProductFields.ParamSynonym.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Category.ResourceKey(), ProductFields.Category.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Sorting.ResourceKey(), ProductFields.Sorting.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.Enabled.ResourceKey(), ProductFields.Enabled.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Price.ResourceKey(), ProductFields.Price.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.PurchasePrice.ResourceKey(), ProductFields.PurchasePrice.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Amount.ResourceKey(), ProductFields.Amount.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.MultiOffer.ResourceKey(), ProductFields.MultiOffer.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.Unit.ResourceKey(), ProductFields.Unit.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Discount.ResourceKey(), ProductFields.Discount.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.ShippingPrice.ResourceKey(), ProductFields.ShippingPrice.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Weight.ResourceKey(), ProductFields.Weight.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Size.ResourceKey(), ProductFields.Size.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.BriefDescription.ResourceKey(), ProductFields.BriefDescription.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Description.ResourceKey(), ProductFields.Description.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.Title.ResourceKey(), ProductFields.Title.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.MetaKeywords.ResourceKey(), ProductFields.MetaKeywords.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.MetaDescription.ResourceKey(), ProductFields.MetaDescription.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.H1.ResourceKey(), ProductFields.H1.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.Photos.ResourceKey(), ProductFields.Photos.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Videos.ResourceKey(), ProductFields.Videos.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Markers.ResourceKey(), ProductFields.Markers.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Properties.ResourceKey(), ProductFields.Properties.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Producer.ResourceKey(), ProductFields.Producer.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.OrderByRequest.ResourceKey(), ProductFields.OrderByRequest.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.SalesNotes.ResourceKey(), ProductFields.SalesNotes.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.Related.ResourceKey(), ProductFields.Related.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Alternative.ResourceKey(), ProductFields.Alternative.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.CustomOption.ResourceKey(), ProductFields.CustomOption.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Gtin.ResourceKey(), ProductFields.Gtin.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GoogleProductCategory.ResourceKey(), ProductFields.GoogleProductCategory.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.Adult.ResourceKey(), ProductFields.Adult.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.ManufacturerWarranty.ResourceKey(), ProductFields.ManufacturerWarranty.ConvertIntString()));

                if (!string.IsNullOrEmpty(Request["state"]) && Request["state"].ToLower() == "select")
                    ddl.SelectedValue = enumIntStringTemp;
                var lb = new Label
                    {
                        ID = "lbProduct" + ((int)item),
                        Text = ddlProduct.Items.Count > 0 && Request["state"] == "select"
                                   ? ddlProduct.Items.FindByValue(enumIntStringTemp).Text
                                   : string.Empty
                    };
                lb.Attributes.Add("style", "display:block");
                ddl.Attributes.Add("onchange", string.Format("Change(this)"));
                cell.Controls.Add(ddl);
                ddlRow.Cells.Add(cell);
                var cellLbl = new TableCell { ID = "cellLbl" + enumIntStringTemp };
                cellLbl.Controls.Add(lb);
                lblRow.Cells.Add(cellLbl);
            }

            tbl.Rows.Add(ddlRow);
            tbl.Rows.Add(lblRow);
            choseDiv.Controls.Add(tbl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveExcel)
            {
                mainDiv.Visible = false;
                notInTariff.Visible = true;
            }

            lblCategoriesCount.Text = ExportFeedService.CheckCategory("CsvExport", 0)
                                          ? Resource.Admin_ExportCsv_AllCategories
                                          : ExportFeedService.GetCsvCategoriesCount("CsvExport") + " " + Resource.Admin_ExportCsv_Categories;


            //hrefAgaint.Visible = false;
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ExportExcel_Title));

            if (!IsPostBack)
            {
                ChbCategorySort.Checked = Request[CategorySort].TryParseBool();

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
                chbCsvExportNoInCategory.Checked = CsvSettings.CsvExportNoInCategory;

            }

            if (choseDiv.FindControl("tblValues") != null && IsPostBack)
            {
                var cells = ((TableRow)choseDiv.FindControl("ddlRow")).Cells;
                foreach (TableCell item in cells)
                {
                    var element = item.Controls.OfType<DropDownList>().FirstOrDefault();
                    if (element == null) continue;

                    if (item.Controls.OfType<DropDownList>().First().SelectedValue == ProductFields.None.ConvertIntString()) continue;

                    if (!FieldMapping.Contains((ProductFields)SQLDataHelper.GetInt(item.Controls.OfType<DropDownList>().First().SelectedValue)))
                        FieldMapping.Add((ProductFields)SQLDataHelper.GetInt((item.Controls.OfType<DropDownList>().First().SelectedValue)));//, cells.GetCellIndex(item));
                    else
                    {
                        MsgErr(string.Format(Resource.Admin_ImportCsv_DuplicateMessage, item.Controls.OfType<DropDownList>().First().SelectedItem.Text));
                        return;
                    }
                }

                if (ChbCategorySort.Checked)
                {
                    var ind = FieldMapping.IndexOf(ProductFields.Category);
                    if (ind > 0)
                        FieldMapping.Insert(ind + 1, ProductFields.Sorting);
                    else
                        FieldMapping.Add(ProductFields.Sorting);
                }
            }
            if (FieldMapping.Count == 0 && IsPostBack)
            {
                MsgErr(Resource.Admin_ExportCsv_ListEmpty);
                return;
            }
            MsgErr(true);
            OutDiv.Visible = CommonStatistic.IsRun;
            linkCancel.Visible = CommonStatistic.IsRun;
        }

        private void LoadFirstProduct()
        {
            var columSeparator = txtColumSeparator.Text;
            var propertySeparator = txtPropertySeparator.Text;

            var product = ProductService.GetFirstProduct();
            if (product == null) return;
            foreach (var item in Enum.GetValues(typeof(ProductFields)))
            {
                if ((ProductFields)item == ProductFields.None)
                    ddlProduct.Items.Add(new ListItem { Text = @"-", Value = ProductFields.None.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Sku)
                    ddlProduct.Items.Add(new ListItem { Text = product.ArtNo, Value = ProductFields.Sku.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Name)
                    ddlProduct.Items.Add(new ListItem { Text = product.Name.HtmlEncode(), Value = ProductFields.Name.ConvertIntString() });

                if ((ProductFields)item == ProductFields.ParamSynonym)
                    ddlProduct.Items.Add(new ListItem { Text = product.UrlPath, Value = ProductFields.ParamSynonym.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Category)
                    ddlProduct.Items.Add(new ListItem { Text = CategoryService.GetCategoryStringByProductId(product.ProductId, columSeparator), Value = ProductFields.Category.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Sorting)
                    ddlProduct.Items.Add(new ListItem { Text = CategoryService.GetCategoryStringByProductId(product.ProductId, columSeparator, true), Value = ProductFields.Sorting.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Enabled)
                    ddlProduct.Items.Add(new ListItem { Text = product.Enabled ? "+" : "-", Value = ProductFields.Enabled.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Price)
                    ddlProduct.Items.Add(new ListItem { Text = product.Offers.Select(x => x.Price).FirstOrDefault().ToString("F2"), Value = ProductFields.Price.ConvertIntString() });

                if ((ProductFields)item == ProductFields.PurchasePrice)
                    ddlProduct.Items.Add(new ListItem { Text = product.Offers.Select(x=>x.SupplyPrice).FirstOrDefault().ToString("F2"), Value = ProductFields.PurchasePrice.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Amount)
                    ddlProduct.Items.Add(new ListItem { Text = product.Offers.Select(x => x.Amount).FirstOrDefault().ToString("F2"), Value = ProductFields.Amount.ConvertIntString() });

                if ((ProductFields)item == ProductFields.MultiOffer)
                {
                    ddlProduct.Items.Add(new ListItem
                        {
                            Text = OfferService.OffersToString(product.Offers, columSeparator, propertySeparator),
                            Value = ProductFields.MultiOffer.ConvertIntString()
                        });
                }

                if ((ProductFields)item == ProductFields.Unit)
                    ddlProduct.Items.Add(new ListItem { Text = product.Unit, Value = ProductFields.Unit.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Discount)
                    ddlProduct.Items.Add(new ListItem { Text = product.Discount.ToString("F2"), Value = ProductFields.Discount.ConvertIntString() });

                if ((ProductFields)item == ProductFields.ShippingPrice)
                    ddlProduct.Items.Add(new ListItem { Text = product.ShippingPrice.ToString("F2"), Value = ProductFields.ShippingPrice.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Weight)
                    ddlProduct.Items.Add(new ListItem { Text = product.Weight.ToString("F2"), Value = ProductFields.Weight.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Size)
                    ddlProduct.Items.Add(new ListItem { Text = product.Size.Replace("|", " x "), Value = ProductFields.Size.ConvertIntString() });

                if ((ProductFields)item == ProductFields.BriefDescription)
                    ddlProduct.Items.Add(new ListItem { Text = product.BriefDescription.Reduce(20).HtmlEncode(), Value = ProductFields.BriefDescription.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Description)
                    ddlProduct.Items.Add(new ListItem { Text = product.Description.Reduce(20).HtmlEncode(), Value = ProductFields.Description.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Title)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.Title.Reduce(20), Value = ProductFields.Title.ConvertIntString() });

                if ((ProductFields)item == ProductFields.H1)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.H1.Reduce(20), Value = ProductFields.H1.ConvertIntString() });

                if ((ProductFields)item == ProductFields.MetaKeywords)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.MetaKeywords.Reduce(20), Value = ProductFields.MetaKeywords.ConvertIntString() });

                if ((ProductFields)item == ProductFields.MetaDescription)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.MetaDescription.Reduce(20), Value = ProductFields.MetaDescription.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Photos)
                    ddlProduct.Items.Add(new ListItem { Text = PhotoService.PhotoToString(product.ProductPhotos, columSeparator, propertySeparator), Value = ProductFields.Photos.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Videos)
                    ddlProduct.Items.Add(new ListItem { Text = ProductVideoService.VideoToString(product.ProductVideos, columSeparator).Reduce(20).HtmlEncode(), Value = ProductFields.Videos.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Markers)
                    ddlProduct.Items.Add(new ListItem { Text = ProductService.MarkersToString(product, columSeparator), Value = ProductFields.Markers.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Properties)
                    ddlProduct.Items.Add(new ListItem { Text = PropertyService.PropertiesToString(product.ProductPropertyValues, columSeparator, propertySeparator).HtmlEncode(), Value = ProductFields.Properties.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Producer)
                    ddlProduct.Items.Add(new ListItem { Text = BrandService.BrandToString(product.BrandId), Value = ProductFields.Producer.ConvertIntString() });

                if ((ProductFields)item == ProductFields.OrderByRequest)
                    ddlProduct.Items.Add(new ListItem { Text = product.AllowPreOrder ? "+" : "-", Value = ProductFields.OrderByRequest.ConvertIntString() });

                if ((ProductFields)item == ProductFields.SalesNotes)
                    ddlProduct.Items.Add(new ListItem { Text = product.SalesNote, Value = ProductFields.SalesNotes.ConvertIntString() });

                if ((ProductFields)item == ProductFields.Related)
                    ddlProduct.Items.Add(new ListItem(ProductService.LinkedProductToString(product.ProductId, RelatedType.Related, columSeparator), ProductFields.Related.ConvertIntString()));

                if ((ProductFields)item == ProductFields.Alternative)
                    ddlProduct.Items.Add(new ListItem(ProductService.LinkedProductToString(product.ProductId, RelatedType.Alternative, columSeparator), ProductFields.Alternative.ConvertIntString()));

                if ((ProductFields)item == ProductFields.CustomOption)
                    ddlProduct.Items.Add(new ListItem(CustomOptionsService.CustomOptionsToString(CustomOptionsService.GetCustomOptionsByProductId(product.ProductId)), ProductFields.CustomOption.ConvertIntString()));

                if ((ProductFields)item == ProductFields.Gtin)
                    ddlProduct.Items.Add(new ListItem(product.Gtin, ProductFields.Gtin.ConvertIntString()));

                if ((ProductFields)item == ProductFields.GoogleProductCategory)
                    ddlProduct.Items.Add(new ListItem(product.GoogleProductCategory, ProductFields.GoogleProductCategory.ConvertIntString()));

                if ((ProductFields)item == ProductFields.Adult)
                    ddlProduct.Items.Add(new ListItem { Text = product.Adult ? "+" : "-", Value = ProductFields.Adult.ConvertIntString() });

                if ((ProductFields)item == ProductFields.ManufacturerWarranty)
                    ddlProduct.Items.Add(new ListItem { Text = product.ManufacturerWarranty ? "+" : "-", Value = ProductFields.ManufacturerWarranty.ConvertIntString() });
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            if (lError.Visible) return;
            if (CommonStatistic.IsRun) return;

            //delete old
            foreach (var item in Directory.GetFiles(_strFilePath).Where(f => f.Contains(StrFileName)))
            {
                FileHelpers.DeleteFile(item);
            }

            divAction.Visible = false;
            divbtnAction.Visible = false;
            choseDiv.Visible = false;

            CsvSettings.CsvEnconing = ddlEncoding.SelectedValue;
            CsvSettings.CsvSeparator = ddlSeparetors.SelectedValue == SeparatorsEnum.Custom.StrName() ? txtCustomSeparator.Text : ddlSeparetors.SelectedValue;

            CsvSettings.CsvColumSeparator = txtColumSeparator.Text;
            CsvSettings.CsvPropertySeparator = txtPropertySeparator.Text;
            CsvSettings.CsvExportNoInCategory = chbCsvExportNoInCategory.Checked;

            CommonStatistic.Init();
            CommonStatistic.CurrentProcess = Request.Url.PathAndQuery;
            CommonStatistic.CurrentProcessName = Resource.Admin_ExportExcel_CatalogDownload;
            linkCancel.Visible = true;
            OutDiv.Visible = true;
            btnDownload.Visible = false;

            // Directory
            foreach (var file in Directory.GetFiles(_strFilePath).Where(f => f.Contains(StrFileName)).ToList())
            {
                FileHelpers.DeleteFile(file);
            }

            ExtStrFileName = (StrFileName + StrFileExt).FileNamePlusDate();
            _strFullPath = _strFilePath + ExtStrFileName;
            FileHelpers.CreateDirectory(_strFilePath);
            CsvExport.Factory(_strFullPath, CsvSettings.CsvEnconing, CsvSettings.CsvSeparator, CsvSettings.CsvColumSeparator, CsvSettings.CsvPropertySeparator, FieldMapping, CsvSettings.CsvExportNoInCategory).Process();

            TrialService.TrackEvent(TrialEvents.MakeCSVExport, "");
        }

        protected void linkCancel_Click(object sender, EventArgs e)
        {
            CommonStatistic.IsRun = false;
            CommonStatistic.Init();
            linkCancel.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            divSomeMessage.Visible = !CommonStatistic.IsRun;

            if (CommonStatistic.IsRun)
            {
                ltLink.Text = string.Empty;
                return;
            }
            if (File.Exists(_strFullPath))
            {
                var f = new FileInfo(_strFullPath);
                const double size = 0;
                double sizeM = (double)f.Length / 1048576; //1024 * 1024

                string sizeMesage;
                if ((int)sizeM > 0)
                {
                    sizeMesage = ((int)sizeM) + " MB";
                }
                else
                {
                    double sizeK = (double)f.Length / 1024;
                    if ((int)sizeK > 0)
                    {
                        sizeMesage = ((int)sizeK) + " KB";
                    }
                    else
                    {
                        sizeMesage = ((int)size) + " B";
                    }
                }

                var temp = @"<a href='" + UrlService.GetAbsoluteLink("price_temp/" + ExtStrFileName) + @"' {0}>" +
                           Resource.Admin_ExportExcel_DownloadFile + @"</a>";

                //spanMessage.Text
                var t = @"<span> " + Resource.Admin_ExportExcel_FileSize + @": " + sizeMesage + @"</span>" + @"<span>, " + AdvantShop.Localization.Culture.ConvertDate(File.GetLastWriteTime(_strFullPath)) + @"</span>";
                ltLink.Text = string.Format(temp, "") + t;
            }
            else
            {
                ltLink.Text = @"<span>" + Resource.Admin_ExportExcel_NotExistDownloadFile + @"</span>";
            }
        }

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lError.Visible = false;
                lError.Text = string.Empty;
            }
            else
            {
                lError.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lError.Visible = true;
            lError.Text = @"<br/>" + messageText;
        }
    }
}