using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Helpers;
using AdvantShop.Modules;

namespace Advantshop.Modules.UserControls.BuyInTime
{
    public partial class BuyInTimeModuleAddEdit : System.Web.UI.Page
    {
        protected int Id;

        private const string ModuleName = "BuyInTime";
        private BuyInTimeProductModel _action;

        
        protected void Page_Load(object sender, EventArgs e)
        {
            Id = Request["Id"].TryParseInt();

            lBase.Text = string.Format("<base href='{0}'/>",
                           Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath +
                           (!Request.ApplicationPath.EndsWith("/") ? "/" : string.Empty) + "modules/BuyInTime/");

            ckeActionText.BaseHref = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath +
                                  (!Request.ApplicationPath.EndsWith("/") ? "/" : string.Empty);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            if (Id != 0)
            {
                LoadAction();
            }
            else
            {
                var now = DateTime.Now;
                txtDateStart.Text = now.ToString();
                txtDateExpired.Text = now.Date.AddDays(5).ToString();
                ckeActionText.Text = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeDefaultActionTextMode1", ModuleName);
                txtSortOrder.Text = "0";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Id != 0)
                SaveAction();
            else
                CreateAction();

            if (!lblMessage.Visible)
            {
                var jScript = new StringBuilder();
                jScript.Append("<script type=\'text/javascript\' language=\'javascript\'> ");
                jScript.Append("window.opener.location.reload(true); ");
                jScript.Append("self.close();");
                jScript.Append("</script>");
                Type csType = this.GetType();
                ClientScriptManager clScriptMng = this.ClientScript;
                clScriptMng.RegisterClientScriptBlock(csType, "Close_window", jScript.ToString());
            }
        }

        protected void ddlShowMode_OnChanged(object sender, EventArgs e)
        {
            if (ddlShowMode.SelectedValue == "1")
            {
                ckeActionText.Text = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeDefaultActionTextMode1",
                    ModuleName);
            }
            else if (ddlShowMode.SelectedValue == "2")
            {
                ckeActionText.Text = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeDefaultActionTextMode2",
                    ModuleName);
            }
        }

        protected void DeletePicture_Click(object sender, EventArgs e)
        {
            if (_action == null)
                _action = BuyInTimeService.Get(Id);

            var filepath = BuyInTimeService.PicturePath + _action.Picture;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                BuyInTimeService.UpdatePicture(_action.Id, null);

                pnlPicture.Visible = false;
                fuPucture.Visible = true;
            }
        }

        #region Private methods

        private void LoadAction()
        {
            _action = BuyInTimeService.Get(Id);

            if (_action != null)
            {
                var product = ProductService.GetProduct(_action.ProductId);

                txtProductArtNo.Text = product.ArtNo;
                txtDiscount.Text = _action.DiscountInTime.ToString();
                txtDateStart.Text = _action.DateStart.ToString();
                txtDateExpired.Text = _action.DateExpired.ToString();
                ddlShowMode.SelectedValue = _action.ShowMode.ToString();
                ckeActionText.Text = _action.ActionText;
                chkIsRepeat.Checked = _action.IsRepeat;
                txtDaysRepeat.Text = _action.DaysRepeat.ToString();
                txtSortOrder.Text = _action.SortOrder.ToString();

                if (_action.Picture.IsNotEmpty())
                {
                    pnlPicture.Visible = true;
                    fuPucture.Visible = false;
                    liPicture.Text = string.Format("<img src='pictures/{0}'/>", _action.Picture);
                }
                else
                {
                    pnlPicture.Visible = false;
                    fuPucture.Visible = true;
                }
            }
        }

        private void SaveAction()
        {
            if (!IsValidData())
                return;

            var productId = ProductService.GetProductId(txtProductArtNo.Text);

            try
            {
                _action = BuyInTimeService.Get(Id);

                _action.ProductId = productId;
                _action.DiscountInTime = txtDiscount.Text.TryParseFloat();
                _action.DateStart = txtDateStart.Text.TryParseDateTime();
                _action.DateExpired = txtDateExpired.Text.TryParseDateTime();
                _action.ShowMode = ddlShowMode.SelectedValue.TryParseInt();
                _action.ActionText = ckeActionText.Text;
                _action.IsRepeat = chkIsRepeat.Checked;
                _action.DaysRepeat = txtDaysRepeat.Text.TryParseInt(1);
                _action.SortOrder = txtSortOrder.Text.TryParseInt(0);

                BuyInTimeService.Update(_action);

                txtProductArtNo.Text = txtDiscount.Text = txtDateExpired.Text = txtDateStart.Text = string.Empty;

                if (fuPucture.HasFile)
                {
                    if (!FileHelpers.CheckFileExtension(fuPucture.FileName, FileHelpers.eAdvantShopFileTypes.Image))
                    {
                        MsgErr("Неправильный формат");
                        return;
                    }

                    var fileName = _action.Id + Path.GetExtension(fuPucture.FileName);

                    if (!Directory.Exists(BuyInTimeService.PicturePath))
                        Directory.CreateDirectory(BuyInTimeService.PicturePath);

                    fuPucture.SaveAs(BuyInTimeService.PicturePath + fileName);
                    BuyInTimeService.UpdatePicture(_action.Id, fileName);
                }
            }
            catch (Exception ex)
            {
                MsgErr("cant add" + ex);
            }

            txtProductArtNo.Text = string.Empty;
        }

        private void CreateAction()
        {
            if (!IsValidData())
                return;

            var productId = ProductService.GetProductId(txtProductArtNo.Text);

            try
            {
                var action = new BuyInTimeProductModel()
                {
                    ProductId = productId,
                    DiscountInTime = txtDiscount.Text.TryParseFloat(),
                    DateStart = txtDateStart.Text.TryParseDateTime(),
                    DateExpired = txtDateExpired.Text.TryParseDateTime(),
                    ShowMode = ddlShowMode.SelectedValue.TryParseInt(),
                    ActionText = ckeActionText.Text,
                    IsRepeat = chkIsRepeat.Checked,
                    DaysRepeat = txtDaysRepeat.Text.TryParseInt(1),
                    SortOrder = txtSortOrder.Text.TryParseInt(0)
                };

                BuyInTimeService.Add(action);

                txtProductArtNo.Text = txtDiscount.Text = txtDateExpired.Text = txtDateStart.Text = string.Empty;

                if (fuPucture.HasFile)
                {
                    if (!FileHelpers.CheckFileExtension(fuPucture.FileName, FileHelpers.eAdvantShopFileTypes.Image))
                    {
                        MsgErr("Неправильный формат");
                        return;
                    }

                    var fileName = action.Id + Path.GetExtension(fuPucture.FileName);

                    if (!Directory.Exists(BuyInTimeService.PicturePath))
                        Directory.CreateDirectory(BuyInTimeService.PicturePath);

                    fuPucture.SaveAs(BuyInTimeService.PicturePath + fileName);
                    BuyInTimeService.UpdatePicture(action.Id, fileName);
                }
            }
            catch (Exception ex)
            {
                MsgErr("cant add" + ex);
            }

            txtProductArtNo.Text = string.Empty;
        }

        private bool IsValidData()
        {
            var valid = true;

            lblMessage.Visible = false;

            if (txtProductArtNo.Text.IsNullOrEmpty() || txtDiscount.Text.IsNullOrEmpty() ||
                txtDateExpired.Text.IsNullOrEmpty() || txtDateStart.Text.IsNullOrEmpty())
            {
                MsgErr("error");
                valid = false;
            }

            var productId = ProductService.GetProductId(txtProductArtNo.Text);
            if (productId == 0)
            {
                MsgErr("artno not exist");
                valid = false;
            }

            return valid;
        }

        private void MsgErr(string msg)
        {
            lblMessage.Visible = true;
            lblMessage.Text = msg;
        }


        #endregion
    }
}