using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop.Catalog;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using System.Web;

namespace UserControls.Details
{
    public partial class ProductPhotoView : UserControl
    {
        #region Fields

        public Product Product { set; get; }

        protected Photo MainPhoto { set; get; }

        protected List<string> Labels = null;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Product.ProductPhotos.Any())
            {

                Offer currentOffer = OfferService.GetMainOffer(Product.Offers, Product.AllowPreOrder, Request["color"].TryParseInt(true), Request["size"].TryParseInt(true));

                if (currentOffer != null)
                {
                    MainPhoto = currentOffer.Photo;
                }
                else
                {
                    MainPhoto =
                        Product.ProductPhotos.OrderByDescending(item => item.Main)
                            .ThenBy(item => item.PhotoSortOrder)
                            .FirstOrDefault(item => item.Main) ?? new Photo(0, Product.ProductId, PhotoType.Product);
                }

                if (MainPhoto == null)
                {
                    //nophoto object
                    MainPhoto = new Photo(0, Product.ProductId, PhotoType.Product)
                    {
                        PhotoName = ""
                    };
                }

                lvPhotos.DataSource = Product.ProductPhotos;
                lvPhotos.DataBind();

                carouselDetails.Visible = lvPhotos.Items.Count > 1;
                pnlPhoto.Visible = true;
                pnlPhoto.Attributes["style"] = "width:" + SettingsPictureSize.MiddleProductImageWidth + "px";
                pnlNoPhoto.Visible = false;
            }
            else
            {
                pnlPhoto.Visible = false;
                pnlNoPhoto.Attributes["style"] = "width:" + SettingsPictureSize.MiddleProductImageWidth + "px";
                pnlNoPhoto.Visible = true;
            }


            LoadModules();
        }

        private void LoadModules()
        {
            var customLabels = new List<ProductLabel>();

            foreach (var labelModule in AttachedModules.GetModules<ILabel>())
            {
                var classInstance = (ILabel)Activator.CreateInstance(labelModule);
                var labelCode = classInstance.GetLabel();
                if (labelCode != null)
                    customLabels.Add(labelCode);
            }

            Labels = customLabels.Where(l => l.ProductIds.Contains(Product.ProductId)).Select(l => l.LabelCode).ToList();
        }

        protected string GetStringEncoded(string value)
        {
            return HttpUtility.HtmlEncode(value.Replace("\'", "\\'"));
        }
    }
}