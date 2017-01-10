//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Services;
using System.Xml;
using AdvantShop.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using ICSharpCode.SharpZipLib.Zip;
using Resources;

// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
// <System.Web.Script.Services.ScriptService()> _
[WebService(Namespace = "http://ccc1-2.hosting.parking.ru/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class _1CSvc : WebService
{
    private readonly XmlDocument _sdsLog = new XmlDocument();
    private int _added;
    private int _errors;

    private const string MsgListError = "Error getting list of orders";
    private const string MsgStatusError = "Status {0} not found";
    private const string MsgStatusesError = "Statuses not found";
    private int _updated;

    private bool IsUserValid(string username, string password)
    {
        var customer = CustomerService.GetCustomerByEmailAndPassword(username, password, false);
        return customer != null;
    }

    [WebMethod]
    public XmlDocument ExportOrders(string username, string password)
    {
        if (!IsUserValid(username, password))
            return new XmlDocument();
        var orders = OrderService.GetAllOrders();
        if (orders == null)
            return MsgErr(MsgListError);
        using (var writer = new StringWriter())
        {
            OrderService.SerializeToXml(orders, writer);
            var xml = new XmlDocument();
            xml.Load(new StringReader(writer.ToString()));
            return xml;
        }

    }

    [WebMethod]
    public XmlDocument ExportOrdersByStatusID(string username, string password, int statusId)
    {
        if (!IsUserValid(username, password))
            return new XmlDocument();
        var stats = OrderService.GetOrderStatuses();
        if (stats.All(s => s.StatusID != statusId))
        {
            return MsgErr(string.Format(MsgStatusError, statusId));
        }

        var orders = OrderService.GetOrdersByStatusId(statusId);
        if (orders == null)
            return MsgErr(MsgListError);
        using (var writer = new StringWriter())
        {
            OrderService.SerializeToXml(orders, writer);
            var xml = new XmlDocument();
            xml.Load(new StringReader(writer.ToString()));
            return xml;
        }
    }

    [WebMethod]
    public XmlDocument ExportOrdersByStatusName(string username, string password, string statusName)
    {
        if (!IsUserValid(username, password))
            return new XmlDocument();
        var stats = OrderService.GetOrderStatuses();
        var status = stats.Find(s => s.StatusName != statusName);
        if (status == null)
        {
            return MsgErr(string.Format(MsgStatusError, statusName));
        }
        var orders = OrderService.GetOrdersByStatusId(status.StatusID);
        if (orders == null)
            return MsgErr(MsgListError);
        using (var writer = new StringWriter())
        {
            OrderService.SerializeToXml(orders, writer);
            var xml = new XmlDocument();
            xml.Load(new StringReader(writer.ToString()));
            return xml;
        }
    }

    [WebMethod]
    public XmlDocument GetOrderStatuses(string username, string password)
    {
        if (!IsUserValid(username, password))
            return new XmlDocument();
        var result = new XmlDocument();
        XmlElement root = result.CreateElement("Statuses");
        var stats = OrderService.GetOrderStatuses();
        if (stats == null)
        {
            return MsgErr(MsgStatusesError);
        }
        foreach (var stat in stats)
        {
            XmlElement status = result.CreateElement("Status");
            XmlAttribute index = result.CreateAttribute("ID");
            index.Value = stat.StatusID.ToString();
            status.Attributes.Append(index);
            status.InnerText = stat.StatusName;
            root.AppendChild(status);
        }
        result.AppendChild(root);
        return result;
    }

    [WebMethod]
    public XmlDocument ImportCatalogFromZip(string username, string password)
    {
        if (!IsUserValid(username, password))
            return new XmlDocument();

        try
        {
            var result = new XmlDocument();
            if (!Directory.Exists(Server.MapPath("~/1c_temp")))
            {
                Directory.CreateDirectory(Server.MapPath("~/1c_temp"));
            }
            result.AppendChild(result.CreateElement("ImportLog"));
            string zipfile = Server.MapPath("~/1c_temp/import.zip");
            string impDir = Server.MapPath("~/1c_temp/import/");
            if (!Directory.Exists(impDir))
            {
                Directory.CreateDirectory(impDir);
            }
            ExtractZip(zipfile, impDir);
            foreach (string file in Directory.GetFiles(impDir))
            {
                if (file.EndsWith(".xml"))
                {
                    var doc = new XmlDocument();
                    doc.Load(file);
                    ProcessXml(doc, impDir);
                    if (result.DocumentElement != null)
                    {
                        if (_sdsLog.DocumentElement != null)
                        {
                            result.DocumentElement.InnerXml = result.DocumentElement.InnerXml + _sdsLog.DocumentElement.InnerXml;
                            _sdsLog.DocumentElement.RemoveAll();
                        }
                    }
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            return MsgErr(ex.Message);
        }
    }

    private void ExtractZip(string filename, string directory)
    {
        if (filename.EndsWith(".zip"))
        {
            var zip = new FastZip();
            zip.ExtractZip(filename, directory, null);
        }
    }

    private void PackZip(string filename, string directory)
    {
        var zip = new FastZip();
        zip.CreateZip(filename, directory, true, null);
    }

    [WebMethod]
    public XmlDocument ImportCatalog(string username, string password, XmlDocument xml)
    {
        if (!IsUserValid(username, password))
            return new XmlDocument();
        ProcessXml(xml, null);
        return _sdsLog;
    }

    private XmlDocument MsgErr(string errTxt)
    {
        var result = new XmlDocument();
        XmlElement errorXml = result.CreateElement("Error");
        errorXml.InnerText = errTxt;
        result.AppendChild(errorXml);
        return result;
    }

    private void UpdateInsertProduct(Product product, string parentCategory)
    {
        if (product.ArtNo != string.Empty)
        {
            Product p = ProductService.GetProduct(product.ArtNo);
            if (p == null)
            {
                ProductService.AddProduct(product, false);
                //ProductService.AddProductLink(product.ProductId, categoryId);
                CategoryService.SubParseAndCreateCategory(parentCategory, product.ProductId);
                Log(string.Format(Resource.Admin_Import1C_Added, product.Name, product.ArtNo), "ProductAdded");
            }
            else
            {
                product.ProductId = p.ProductId;
                ProductService.UpdateProduct(product, false);
                //ProductService.AddProductLink(product.ProductId, categoryId);
                CategoryService.SubParseAndCreateCategory(parentCategory, product.ProductId);
                Log(string.Format(Resource.Admin_Import1C_Updated, product.Name, product.ArtNo), "ProductUpdated");
            }
        }
    }

    private void Log(string message, string type)
    {
        if (_sdsLog.DocumentElement == null)
        {
            _sdsLog.AppendChild(_sdsLog.CreateElement("Log"));
        }
        XmlElement el = _sdsLog.CreateElement("LogEntry");
        XmlAttribute attr = _sdsLog.CreateAttribute("Type");
        attr.Value = type;
        el.Attributes.Append(attr);
        attr = _sdsLog.CreateAttribute("Message");
        attr.Value = message;
        el.Attributes.Append(attr);
        if (_sdsLog.DocumentElement != null)
        {
            _sdsLog.DocumentElement.AppendChild(el);
        }

        if (type == "ProductAdded")
        {
            _added++;
        }
        if (type == "ProductUpdated")
        {
            _updated++;
        }
        if (type == "InvalidData")
        {
            _errors++;
        }
    }

    private void ProcessXml(XmlDocument doc, string photoPath)
    {
        try
        {
            //var cats = new Dictionary<string, string>();
            //XmlNodeList categories = doc.GetElementsByTagName("Category");
            //if (categories.Count != 0)
            //{
            //    int i = 0;
            //    foreach (XmlNode categoryXml in categories)
            //    {
            //        if (categoryXml.Attributes != null)
            //        {
            //            Category category = CategoryService.GetCategoryFromDbByCategoryId(int.Parse(categoryXml.Attributes["ID"].InnerText));
            //            if (category == null)
            //            {
            //                try
            //                {
            //                    Category cat = new Category();
            //                    cat.Name = categoryXml.Attributes["Name"].InnerText;
            //                    cat.ParentCategoryId = int.Parse(categoryXml.Attributes["ParentCategory"].InnerText);
            //                    cat.Picture = string.Empty;
            //                    cat.SortOrder = Math.Max(Interlocked.Increment(ref i), i - 1);
            //                    cat.Enabled = true;

            //                    cats.Add(categoryXml.Attributes["ID"].InnerText, CategoryService.AddCategory(cat, true).ToString());
            //                    //cats.Add(categoryXml.Attributes["ID"].InnerText,
            //                    //         CategoryService.AddCategory(categoryXml.Attributes["Name"].InnerText,
            //                    //                                     int.Parse(categoryXml.Attributes["ParentCategory"].InnerText), "",
            //                    //                                     Math.Max(Interlocked.Increment(ref i), i - 1), true,
            //                    //                                     true).ToString());
            //                    CategoryService.UpdateCategory(category, true);
            //                }
            //                catch (Exception ex)
            //                {
            //                    AdvantShop.Diagnostics.Debug.LogError(ex);
            //                }
            //            }
            //            else
            //            {
            //                category.Name = categoryXml.Attributes["Name"].InnerText;
            //                category.ParentCategoryId = int.Parse(categoryXml.Attributes["ParentCategory"].InnerText);
            //                CategoryService.UpdateCategory(category, true);
            //                cats.Add(category.CategoryId.ToString(), category.CategoryId.ToString());
            //            }
            //        }
            //    }
            //}
            if (doc.GetElementsByTagName("Products").Count != 0)
            {
                var products = new Dictionary<string, Product>();
                var productCats = new Dictionary<string, string>();
                var productUnits = new Dictionary<string, string>();
                XmlNode productsXml = doc.GetElementsByTagName("Products")[0];
                foreach (XmlNode prodXml in productsXml.ChildNodes)
                {
                    if (prodXml.Attributes != null)
                    {
                        var product = new Product
                                          {
                                              ArtNo = prodXml.Attributes["SKU"].InnerText,
                                              Name = prodXml.Attributes["Name"].InnerText,
                                              Description = prodXml.Attributes["Description"].InnerText,
                                              Unit = prodXml.Attributes["Unit"].InnerText,
                                              Enabled = true,
                                              UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, prodXml.Attributes["SKU"].InnerText)
                                          };
                        productCats.Add(product.ArtNo, prodXml.Attributes["Category"].InnerText);
                        try
                        {
                            products.Add(product.ArtNo, product);
                        }
                        catch (Exception ex)
                        {
                            Log(ex.Message, "InvalidData");
                        }
                    }
                }

                XmlNodeList offers = doc.GetElementsByTagName("Offer");
                if (offers.Count != 0)
                {
                    foreach (XmlNode offer in offers)
                    {
                        if (offer.Attributes != null)
                        {
                            Product product = products[offer.Attributes["ProductSKU"].InnerText];
                            if (product == null)
                            {
                                break;
                            }
                            var pOffer = new Offer();
                            float price;
                            if (float.TryParse(offer.Attributes["Price"].Value.Replace('.', ','), out price))
                            {
                                pOffer.Price = price;
                            }
                            int amount;
                            if (int.TryParse(offer.Attributes["Amount"].InnerText, out amount))
                            {
                                pOffer.Amount = amount;
                            }

                            if (product.Offers == null)
                            {
                                product.Offers = new List<Offer>();
                            }

                            product.Offers.Add(pOffer);
                        }
                    }
                }

                foreach (Product product in products.Values)
                {
                    UpdateInsertProduct(product, productCats[product.ArtNo]);
                }

                LuceneSearch.CreateAllIndexInBackground();
                ProductService.PreCalcProductParamsMassInBackground();

                if (!string.IsNullOrEmpty(photoPath))
                {
                    XmlNodeList photos = doc.GetElementsByTagName("Photo");
                    if (photos.Count != 0)
                    {
                        FileHelpers.UpdateDirectories();
                        foreach (XmlNode photo in photos)
                        {

                            if (photo.Attributes != null)
                            {
                                var fullfilename = photoPath + photo.Attributes["FileName"].Value;
                                if (File.Exists(fullfilename))
                                {
                                    ProductService.AddProductPhotoByArtNo(photo.Attributes["ProductSKU"].Value, fullfilename, photo.Attributes["Description"].Value, false, null);
                                }
                                File.Delete(fullfilename);
                            }
                        }
                    }
                }
                XmlNodeList props = doc.GetElementsByTagName("Property");
                if (props.Count != 0)
                {
                    foreach (XmlNode prop in props)
                    {
                        if (prop.Attributes != null)
                        {
                            var product = ProductService.GetProduct(prop.Attributes["ProductSKU"].InnerText);
                            if (product.ID == 0)
                            {
                                break;
                            }

                            // TODO use PropertyService
                            //ProductService.AddProperty(product.Id, prop.Attributes["Name"].Value,
                            //                           prop.Attributes["Value"].Value, 0);
                        }
                    }
                }
            }
            Log("Import successfull", "SuccessImport");
        }
        catch (Exception ex)
        {
            Log(ex.Message, "InvalidData");
        }
        CategoryService.RecalculateProductsCountManual();
        CategoryService.ClearCategoryCache();

        XmlElement summary = _sdsLog.CreateElement("LogSummary");
        XmlElement el = _sdsLog.CreateElement("Added");
        el.InnerText = _added.ToString("d");
        summary.AppendChild(el);
        el = _sdsLog.CreateElement("Updated");
        el.InnerText = _updated.ToString("d");
        summary.AppendChild(el);
        el = _sdsLog.CreateElement("Errors");
        el.InnerText = _errors.ToString("d");
        summary.AppendChild(el);
        if (_sdsLog.DocumentElement != null)
            _sdsLog.DocumentElement.AppendChild(summary);
    }

    [WebMethod]
    public XmlDocument ImportCatalogInCsvFromZip(string username, string password)
    {
        if (!IsUserValid(username, password))
            return new XmlDocument();
        try
        {
            var result = new XmlDocument();
            if (!Directory.Exists(Server.MapPath("~/1c_temp")))
            {
                Directory.CreateDirectory(Server.MapPath("~/1c_temp"));
            }
            result.AppendChild(result.CreateElement("ImportLog"));
            var zipfile = Server.MapPath("~/1c_temp/import.zip");
            var impDir = Server.MapPath("~/1c_temp/import/");
            if (!Directory.Exists(impDir))
            {
                Directory.CreateDirectory(impDir);
            }
            ExtractZip(zipfile, impDir);
            foreach (string file in Directory.GetFiles(impDir))
            {
                if (file.EndsWith(".csv") || file.EndsWith(".xml")) continue;
                File.Delete(file.Replace("1c_temp\\import", "upload_images"));
                File.Move(file, file.Replace("1c_temp\\import", "upload_images"));
            }
            foreach (string file in Directory.GetFiles(impDir))
            {
                if (!file.EndsWith(".csv")) continue;
                CsvImport.Factory( file, false).Process();
                if (result.DocumentElement == null) continue;
                if (_sdsLog.DocumentElement == null) continue;
                result.DocumentElement.InnerXml = result.DocumentElement.InnerXml + _sdsLog.DocumentElement.InnerXml;
                _sdsLog.DocumentElement.RemoveAll();
            }
            return result;
        }
        catch (Exception ex)
        {
            return MsgErr(ex.Message);
        }
    }
}