//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.SEO;
using Resources;

namespace ClientPages
{
    public partial class SendRequestOnProduct : AdvantShopClientPage
    {
        private Offer _offer;

        protected Offer offer
        {
            get
            {
                if (_offer == null)
                {
                    int offerId = 0;
                    if (int.TryParse(Request["offerid"], out offerId))
                    {
                        _offer = OfferService.GetOffer(offerId);
                    }
                }

                return _offer;
            }
        }

        protected string Options;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (offer == null)
            {
                Error404();
                return;
            }

            if (!offer.CanOrderByRequest)
            {
                lblMessage.Text = Resource.Client_OrderByRequest_CantBeOrdered;
                MultiView1.SetActiveView(ViewResult);
                return;
            }

            SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_OrderByRequest)),
                    string.Empty);

            if (!IsPostBack)
            {
                MultiView1.SetActiveView(ViewForm);
                float amount = Request["amount"] != null ? Request["amount"].TryParseFloat(1) : 1;
                txtAmount.Text = amount.ToString();
                if (CustomerContext.CurrentCustomer.RegistredUser)
                {
                    txtName.Text = string.Format("{0} {1}", CustomerContext.CurrentCustomer.FirstName,
                                                 CustomerContext.CurrentCustomer.LastName);
                    txtEmail.Text = CustomerContext.CurrentCustomer.EMail;
                    txtPhone.Text = CustomerContext.CurrentCustomer.Phone;
                }


            }

            IList<EvaluatedCustomOptions> listOptions = null;
            Options = Request["options"].IsNotEmpty() && Request["options"] != "null"
                                                ? HttpUtility.UrlDecode(Request["options"])
                                                : null;
            if (Options.IsNotEmpty())
            {
                try
                {
                    listOptions = CustomOptionsService.DeserializeFromXml(Options);
                }
                catch (Exception)
                {
                    listOptions = null;
                }
            }

            lOptions.Text = OrderService.RenderSelectedOptions(listOptions);

            liCaptcha.Visible = SettingsMain.EnableCaptcha;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            bool boolIsValidPast = true;
            boolIsValidPast &= IsValidText(txtName.Text);
            boolIsValidPast &= IsValidText(txtEmail.Text);
            boolIsValidPast &= IsValidText(txtPhone.Text);

            float quantity = 0;
            if (!float.TryParse(txtAmount.Text, out quantity) || (quantity < 0))
            {
                boolIsValidPast = false;
            }

            if (!ValidationHelper.IsValidEmail(txtEmail.Text.Trim()))
            {
                boolIsValidPast = false;
            }

            if (!CaptchaControl1.IsValid())
            {
                CaptchaControl1.TryNew();
                boolIsValidPast = false;
            }

            if (boolIsValidPast == false)
            {
                CaptchaControl1.TryNew();
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Feedback_WrongData);
                return;
            }

            try
            {
                var orderByRequest = new OrderByRequest
                    {
                        OfferId = offer.OfferId,
                        ProductId = offer.Product.ID,
                        ProductName = offer.Product.Name,
                        ArtNo = offer.ArtNo,
                        Quantity = quantity,
                        UserName = txtName.Text,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text,
                        Comment = txtComment.Text,
                        IsComplete = false,
                        RequestDate = DateTime.Now,
                        Options = Options
                    };

                OrderByRequestService.AddOrderByRequest(orderByRequest);


                IList<EvaluatedCustomOptions> listOptions = null;
                if (Options.IsNotEmpty())
                {
                    try
                    {
                        listOptions = CustomOptionsService.DeserializeFromXml(Options);
                    }
                    catch (Exception)
                    {
                        listOptions = null;
                    }
                }

                var mailTemplate =
                    new OrderByRequestMailTemplate(
                        orderByRequest.OrderByRequestId.ToString(CultureInfo.InvariantCulture), offer.ArtNo,
                        offer.Product.Name + " " + OrderService.RenderSelectedOptions(listOptions),
                        quantity.ToString(CultureInfo.InvariantCulture), txtName.Text, txtEmail.Text, txtPhone.Text,
                        txtComment.Text, offer.Color != null ? offer.Color.ColorName : string.Empty,
                        offer.Size != null ? offer.Size.SizeName : string.Empty);

                mailTemplate.BuildMail();

                SendMail.SendMailNow(txtEmail.Text, mailTemplate.Subject, mailTemplate.Body, true);
                SendMail.SendMailNow(SettingsMail.EmailForOrders, mailTemplate.Subject, mailTemplate.Body, true);

                lblMessage.Text = Resource.Client_Feedback_MessageSent;
                MultiView1.SetActiveView(ViewResult);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Feedback_MessageError);
                MultiView1.SetActiveView(ViewResult);
            }
        }

        private static bool IsValidText(String textBox)
        {
            return !string.IsNullOrEmpty(textBox.Trim());
        }

        protected string RenderProductPhoto()
        {
            var productPhotoName = string.Empty;
            if (offer.ColorID != null)
            {
                var productPhoto = PhotoService.GetPhotos(offer.Product.ProductId, PhotoType.Product)
                                               .FirstOrDefault(item => item.ColorID == offer.ColorID);
                if (productPhoto != null)
                {
                    productPhotoName = productPhoto.PhotoName;
                }
            }

            return string.Format(
                "<img src=\"{0}\" alt=\"{1}\" />",
                AdvantShop.FilePath.FoldersHelper.GetImageProductPath(AdvantShop.FilePath.ProductImageType.Middle,
                                                                      string.IsNullOrEmpty(productPhotoName)
                                                                          ? offer.Product.Photo
                                                                          : productPhotoName, false),
                offer.Product.Name);
        }
    }
}