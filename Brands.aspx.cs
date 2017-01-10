//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Repository;
using AdvantShop.SEO;
using Resources;

namespace ClientPages
{
    public partial class Brands : AdvantShopClientPage
    {
        private const int BrandsPerPage = 8;

        private List<Brand> _dataSource;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            LoadData();
            var nmeta = SetMeta(
                new MetaInfo
                {
                    Type = MetaType.Brand,
                    Title = SettingsSEO.BrandMetaTitle,
                    MetaKeywords = SettingsSEO.BrandMetaKeywords,
                    MetaDescription = SettingsSEO.BrandMetaDescription,
                    H1 = SettingsSEO.BrandMetaTitle
                }, page: paging.CurrentPage);

            header.Text = nmeta.H1;
        }

        private void LoadData()
        {
            var brands = BrandService.GetBrands(false);
            var sb = new StringBuilder();
            var engLetters = BrandService.GetEngBrandChars();
            var rusLetters = BrandService.GetRusBrandChars();

            ddlCountry.DataSource =
                CountryService.GetAllCountries().Where(c => brands.FindLast(b => b.CountryId == c.CountryId) != null);
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem(Resource.Client_Brands_AllCoutries, "0"));

            if (Request["letter"] != null)
            {
                var selectedLetter = new char();
                char.TryParse(Request["letter"].ToLower(), out selectedLetter);
                if (!engLetters.Contains(selectedLetter) && !rusLetters.Contains(selectedLetter) &&
                    selectedLetter != '0')
                {
                    Error404();
                }
                else if (selectedLetter == '0')
                {
                    _dataSource = brands.Where(b => char.IsDigit(b.Name[0])).ToList();
                }
                else
                {
                    _dataSource = brands.Where(b => b.Name.ToLower().StartsWith(selectedLetter.ToString())).ToList();
                }

            }
            else if (Request["country"] != null)
            {
                ListItem item;
                string country = HttpUtility.HtmlDecode(Request["country"]);
                if ((item = ddlCountry.Items.FindByText(country)) == null)
                {
                    Error404();
                }
                else
                {
                    ddlCountry.SelectedValue = item.Value;
                    _dataSource = brands.Where(b => b.CountryId.ToString() == item.Value).ToList();
                }
            }
            else
            {
                _dataSource = brands;
            }


            sb.AppendFormat("<a href=\"manufacturers\" class=\"all-letter{0}\">{1}</a>",
                            Request["letter"] == null ? " simbol-selected" : string.Empty,
                            Resource.Client_Brands_All);

            if (brands.Any(b => char.IsDigit(b.Name[0])))
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
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (_dataSource == null) return;
            int page = paging.CurrentPage;
            paging.TotalPages = Convert.ToInt32(Math.Ceiling((double)_dataSource.Count() / BrandsPerPage));
            if (paging.TotalPages < paging.CurrentPage || paging.CurrentPage <= 0)
            {
                Error404();
                return;
            }
            lvBrands.DataSource = _dataSource.Skip((page - 1) * BrandsPerPage).Take(BrandsPerPage).ToList();
            lvBrands.DataBind();
        }
    }
}