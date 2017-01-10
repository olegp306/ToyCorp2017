//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Repository;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace ClientPages
{
    public partial class BrandView : AdvantShopClientPage
    {
        protected Brand brand;
        protected MetaInfo metaInfo;

        private void LoadBrand()
        {
            int brandId = 0;

            if (!int.TryParse(Request["brandid"], out brandId) || (brand = BrandService.GetBrandById(brandId)) == null ||
                !brand.Enabled)
            {
                Error404();
            }
            else
            {
                divBrandSiteUrl.Visible = brand.BrandSiteUrl.IsNotEmpty();

                var sb = new StringBuilder();

                var avalibleCategories = BrandService.GetCategoriesByBrand(brandId);
                foreach (
                    var category in
                        CategoryService.GetChildCategoriesByCategoryId(0, false)
                                       .Where(cat => avalibleCategories.ContainsKey(cat.CategoryId)))
                {
                    sb.AppendFormat("<div class=\"block-uc brand-block\">");
                    string link = UrlService.GetLink(ParamType.Category, category.UrlPath, category.CategoryId);
                    if (link.Contains("?"))
                    {
                        link = QueryHelper.ChangeQueryParam(link, "brand", brandId.ToString());
                        link = QueryHelper.ChangeQueryParam(link, "indepth", "1");
                    }
                    else
                    {
                        link += "?" + QueryHelper.CreateQueryString(new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("brand", brandId.ToString()),
                                new KeyValuePair<string, string>("indepth", "1")
                            });
                    }

                    sb.AppendFormat("<span class=\"title\"><a href='{0}'>{1} ({2})</a></span>", link, category.Name,
                                    avalibleCategories[category.CategoryId].InChildsCategoryCount);

                    sb.AppendFormat("<div class=\"content list-brand-marker\">");

                    foreach (
                        var child in category.ChildCategories.Where(ch => avalibleCategories.ContainsKey(ch.CategoryId))
                        )
                    {
                        string sublink = UrlService.GetLink(ParamType.Category, child.UrlPath, child.CategoryId);
                        if (sublink.Contains("?"))
                        {
                            sublink = QueryHelper.ChangeQueryParam(sublink, "brand", brandId.ToString());
                            sublink = QueryHelper.ChangeQueryParam(sublink, "indepth", "1");
                        }
                        else
                        {
                            sublink += "?" + QueryHelper.CreateQueryString(new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("brand", brandId.ToString()),
                                    new KeyValuePair<string, string>("indepth", "1")
                                });
                        }


                        sb.AppendFormat("<a href='{0}'>{1} ({2})</a>", sublink, child.Name,
                                        avalibleCategories[child.CategoryId].InChildsCategoryCount);
                    }
                    sb.AppendFormat("</div>");
                    sb.AppendFormat("</div>");
                }


                lCategories.Text = sb.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadBrand();
            LoadData();
            metaInfo = SetMeta(brand.Meta, brand.Name);
        }

        private void LoadData()
        {
            var brands = BrandService.GetBrands(false);
            var sb = new StringBuilder();
            var engLetters = BrandService.GetEngBrandChars();
            var rusLetters = BrandService.GetRusBrandChars();

            sb.AppendFormat("<a href=\"manufacturers\" class=\"all-letter{0}\">{1}</a>",
                            Request["letter"] == null ? " selected" : string.Empty, Resources.Resource.Client_Brands_All);

            bool hasNumber = false;
            for (int i = 0; i <= 9; i++)
            {
                if (brands.Find(b => b.Name.ToUpper().StartsWith(i.ToString())) != null)
                {
                    hasNumber = true;
                    break;
                }
            }

            if (hasNumber)
                sb.AppendFormat("<a href='{0}' class='all-letter'>{1}</a> ",
                                UrlService.GetAbsoluteLink("manufacturers?letter=" + "0"), "0-9");
            else
                sb.AppendFormat("<a class='all-letter disabled' href='javascript:void(0);'>{0}</a> ", "0-9");


            foreach (char ch in engLetters)
            {
                if (brands.Find(b => b.Name.ToLower().StartsWith(ch.ToString())) != null)
                {
                    sb.AppendFormat("<a href='{0}'>{1}</a> ",
                                    UrlService.GetAbsoluteLink("manufacturers?letter=" + ch), ch);
                }
                else
                {
                    sb.AppendFormat("<a class='disabled' href='javascript:void(0);'>{0}</a> ", ch);
                }
            }
            lEngLetters.Text = sb.ToString();

            if (SettingsMain.Language == "ru-RU")
            {
                sb.Remove(0, sb.Length);
                foreach (char ch in rusLetters)
                {
                    if (brands.Find(b => b.Name.ToLower().StartsWith(ch.ToString())) != null)
                    {
                        sb.AppendFormat("<a href='{0}'>{1}</a> ",
                                        UrlService.GetAbsoluteLink("manufacturers?letter=" +
                                                                   HttpUtility.UrlEncode(ch.ToString())), ch);
                    }
                    else
                    {
                        sb.AppendFormat("<a class='disabled' href='javascript:void(0);'>{0}</a> ", ch);
                    }
                }
                lRusLetters.Text = sb.ToString();
            }

            ddlCountry.DataSource =
                CountryService.GetAllCountries().Where(c => brands.FindLast(b => b.CountryId == c.CountryId) != null);
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem(Resources.Resource.Client_Brands_AllCoutries, "0"));

            ddlCountry.SelectedValue = brand.CountryId.ToString(CultureInfo.InvariantCulture);
        }
    }
}