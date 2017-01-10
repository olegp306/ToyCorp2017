//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Resources;

namespace Admin
{
    public partial class EditOrderByRequest : AdvantShopAdminPage
    {
        protected bool AddingNew
        {
            get { return Request["id"] == null || Request["id"].ToLower() == "add"; }
        }

        private int _orderByRequestId;
        public int OrderByRequestId
        {
            get
            {
                return _orderByRequestId == 0 ? Int32.Parse(Request["id"]) : _orderByRequestId;
            }
            set
            {
                _orderByRequestId = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1} {2}", SettingsMain.ShopName, Resource.Admin_OrderByRequest_Header, Request["id"]));

            if (AddingNew)
            {
                Response.Redirect("OrderByRequest.aspx");
                return;
            }

            MsgErr(true);
            lblMessage.Text = "";
            lblMessage.Visible = false;
            lEmailError.Visible = false;
            lPhoneError.Visible = false;
            lUserNameError.Visible = false;
            lQuantityError.Visible = false;

            if (!IsPostBack)
            {
                OrderByRequestId = 0;

                OrderByRequestId = SQLDataHelper.GetInt(Request["id"]);
                btnSave.Text = Resource.Admin_Update;
                lblHead.Text = Resource.Admin_OrderByRequest_Header + " " + OrderByRequestId;
                lblSubHead.Text = Resource.Admin_OrderByRequest_RequestDate;

                LoadOrder();
            }
        }

        private void SaveOrder()
        {
            var orderByRequest = OrderByRequestService.GetOrderByRequest(OrderByRequestId);

            orderByRequest.Quantity = txtQuantity.Text.TryParseFloat();
            orderByRequest.UserName = txtUserName.Text;
            orderByRequest.Email = txtEmail.Text;
            orderByRequest.Phone = txtPhone.Text;
            orderByRequest.Comment = txtComment.Text;
            orderByRequest.IsComplete = chkIsComplete.Checked;
            orderByRequest.LetterComment = txtLetterComment.Text;

            OrderByRequestService.UpdateOrderByRequest(orderByRequest);

            lblMessage.Text = Resource.Admin_OrderByRequest_ChangesSaved;
            lblMessage.Visible = true;
        }

        private void LoadOrder()
        {
            var orderByRequest = OrderByRequestService.GetOrderByRequest(OrderByRequestId);

            IList<EvaluatedCustomOptions> listOptions = null;
            var selectedOptions = orderByRequest.Options.IsNotEmpty() && orderByRequest.Options != "null"
                                                    ? HttpUtility.UrlDecode(orderByRequest.Options)
                                                    : null;
            if (selectedOptions.IsNotEmpty())
            {
                try
                {
                    listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                }
                catch (Exception)
                {
                    listOptions = null;
                }
            }

            string optionsRender = OrderService.RenderSelectedOptions(listOptions);


            lArtNo.Text = orderByRequest.ArtNo;
            lProductName.Text = orderByRequest.ProductName + optionsRender;

            txtQuantity.Text = orderByRequest.Quantity.ToString();
            txtUserName.Text = orderByRequest.UserName;
            txtEmail.Text = orderByRequest.Email;
            txtPhone.Text = orderByRequest.Phone;
            txtComment.Text = orderByRequest.Comment;
            txtLetterComment.Text = orderByRequest.LetterComment;

            chkIsComplete.Checked = orderByRequest.IsComplete;
            lOrderDate.Text = orderByRequest.RequestDate.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            float quantity = 0;
            if ((!float.TryParse(txtQuantity.Text, out quantity)) || (quantity <= 0))
            {
                lQuantityError.Visible = true;
                isValid = false;
            }

            if (txtUserName.Text.Trim().Length == 0)
            {
                lUserNameError.Visible = true;
                isValid = false;
            }

            if (txtEmail.Text.Trim().Length == 0)
            {
                lEmailError.Visible = true;
                isValid = false;
            }

            if (txtPhone.Text.Trim().Length == 0)
            {
                lPhoneError.Visible = true;
                isValid = false;
            }

            if (!isValid)
            {
                return;
            }

            SaveOrder();
            LoadOrder();
        }

        protected void btnDeleteOrder_Click(object sender, EventArgs e)
        {
            OrderByRequestService.DeleteOrderByRequest(OrderByRequestId);
            Response.Redirect("OrderByRequest.aspx");
        }

        protected void btnSendLink_Click(object sender, EventArgs e)
        {
            if (chkCloseAfterConfirmation.Checked)
            {
                chkIsComplete.Checked = true;
            }

            var preOrder = OrderByRequestService.GetOrderByRequest(OrderByRequestId);
            var offer = OfferService.GetOffer(preOrder.OfferId);
            if (offer.Price == 0)
            {
                lblMessage.Text = Resource.Admin_OrderByRequest_ZeroPrice;
                lblMessage.Visible = true;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                SaveOrder();
                LoadOrder();

                OrderByRequestService.SendConfirmationMessage(OrderByRequestId);

                lblMessage.Text = Resource.Admin_OrderByRequest_MailSend;
                lblMessage.Visible = true;
            }
        }

        protected void btnSentFailure_Click(object sender, EventArgs e)
        {
            if (chkCloseAfterFailure.Checked)
            {
                chkIsComplete.Checked = true;
            }

            SaveOrder();
            LoadOrder();

            OrderByRequestService.SendFailureMessage(OrderByRequestId);

            lblMessage.Text = Resource.Admin_OrderByRequest_MailSend;
            lblMessage.Visible = true;
        }

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                Message.Visible = false;
                Message.Text = "";
            }
            else
            {
                Message.Visible = false;
            }
        }
    }
}