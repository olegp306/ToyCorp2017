using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using Resources;

namespace Admin.UserControls.ShippingMethods
{
    public partial class MasterControl : System.Web.UI.UserControl
    {
        private bool _valid = false;
        private ParametersControl _ucSpecific;

        private int _methodId;
        protected int ShippingMethodId
        {
            get { return _methodId != 0 ? _methodId : (_methodId = ViewState["MethodID"].ToString().TryParseInt()); }
            set
            {
                ViewState["MethodID"] = value;
                _methodId = value;
            }
        }

        public ShippingMethod Method { get; set; }
        public ShippingType ShippingType { get; set; }


        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Visible)
                return;
            //Dynamic user control load
            var fileName = string.Format("{0}.ascx", ShippingType);
            if (!File.Exists(Server.MapPath("~/Admin/UserControls/ShippingMethods/" + fileName))) return;
            _ucSpecific = (ParametersControl)LoadControl(fileName);
            if (_ucSpecific == null) return;
            _ucSpecific.ID = "ucSpecific";
            pnlSpecific.Controls.Add(_ucSpecific);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Visible)
                return;
            LoadFormData(true);
        }

        private void LoadFormData(bool loadParameters)
        {
            if (Method == null) return;
            if (Method.ShippingMethodId == 1) return;

            ShippingMethodId = Method.ShippingMethodId;
            btnDelete.Attributes["data-confirm"] = string.Format(Resource.Admin_ShippingMethod_DeleteConfirm, Method.Name);
            txtName.Text = Method.Name;
            txtDescription.Text = Method.Description;
            txtZeroPriceMessage.Text = Method.ZeroPriceMessage;
            txtSortOrder.Text = Method.SortOrder.ToString();
            chkEnabled.Checked = Method.Enabled;
            chkDisplayCustomFields.Checked = Method.DisplayCustomFields;
            ckbShowInDetails.Checked = Method.ShowInDetails;

            if (chkEnabled.Checked)
            {
                chkEnabled.Text = Resource.Admin_Checkbox_Enabled;
                chkEnabled.ForeColor = System.Drawing.Color.Blue;
            }
            else
            {
                chkEnabled.Text = Resource.Admin_Checkbox_Disabled;
                chkEnabled.ForeColor = System.Drawing.Color.Red;
            }

            if (Method.IconFileName != null && File.Exists(FoldersHelper.GetPathAbsolut(FolderType.ShippingLogo, Method.IconFileName.PhotoName)))
            {
                imgIcon.ImageUrl = "~/" + FoldersHelper.GetPath(FolderType.ShippingLogo, Method.IconFileName.PhotoName, false);
                imgIcon.Visible = true;
                btnDeleteIcon.Visible = true;
            }
            else
            {
                imgIcon.Visible = false;
                btnDeleteIcon.Visible = false;
            }

            if (_ucSpecific != null && loadParameters)
                _ucSpecific.Parameters = Method.Params;

            rptrPayments.DataSource = ShippingMethodService.GetShippingPayments(ShippingMethodId);
            rptrPayments.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Visible) return;
            if (!ValidateFormData()) return;

            var parameters = _ucSpecific == null ? null : _ucSpecific.Parameters;
            if (parameters == null) return;

            var method = new ShippingMethod
            {
                Type = ShippingType,
                ShippingMethodId = ShippingMethodId,
                Name = txtName.Text,
                Description = txtDescription.Text,
                SortOrder = txtSortOrder.Text.TryParseInt(),
                Enabled = chkEnabled.Checked, //(_ucSpecific == null || _ucSpecific.Parameters != null),
                DisplayCustomFields = chkDisplayCustomFields.Checked,
                ShowInDetails = ckbShowInDetails.Checked,
                ZeroPriceMessage = txtZeroPriceMessage.Text
            };

            if (!ShippingMethodService.UpdateShippingMethod(method)) return;
            if (ShippingType == ShippingType.eDost)
            {
                //COD
                if (SQLDataHelper.GetBoolean(parameters[EdostTemplate.EnabledCOD]))
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[EdostTemplate.ShipIdCOD], out idShip);
                    var payment = PaymentService.GetPaymentMethod(idShip);
                    if (payment == null)
                    {
                        var payMethod = PaymentMethod.Create(PaymentType.CashOnDelivery);
                        payMethod.Name = Resource.CashOnDeliveryName;
                        payMethod.Enabled = true;
                        if (payMethod.Parameters.ContainsKey(CashOnDelivery.ShippingMethodTemplate))
                        {
                            payMethod.Parameters[CashOnDelivery.ShippingMethodTemplate] = ShippingMethodId.ToString();
                        }
                        else
                        {
                            payMethod.Parameters.Add(CashOnDelivery.ShippingMethodTemplate,
                                                     ShippingMethodId.ToString());

                        }

                        var id = PaymentService.AddPaymentMethod(payMethod);
                        parameters[EdostTemplate.ShipIdCOD] = id.ToString();
                    }
                }
                else
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[EdostTemplate.ShipIdCOD], out idShip);
                    PaymentService.DeletePaymentMethod(idShip);
                }

                //PickPoint
                if (SQLDataHelper.GetBoolean(parameters[EdostTemplate.EnabledPickPoint]))
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[EdostTemplate.ShipIdPickPoint], out idShip);
                    var payment = PaymentService.GetPaymentMethod(idShip);
                    if (payment == null)
                    {
                        var payMethod = PaymentMethod.Create(PaymentType.PickPoint);
                        payMethod.Name = Resource.OrderPickPointMessage;
                        payMethod.Enabled = true;
                        if (payMethod.Parameters.ContainsKey(PickPoint.ShippingMethodTemplate))
                        {
                            payMethod.Parameters[PickPoint.ShippingMethodTemplate] = ShippingMethodId.ToString();
                        }
                        else
                        {
                            payMethod.Parameters.Add(PickPoint.ShippingMethodTemplate, ShippingMethodId.ToString());
                        }
                        var id = PaymentService.AddPaymentMethod(payMethod);
                        parameters[EdostTemplate.ShipIdPickPoint] = id.ToString();
                    }
                }
                else
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[EdostTemplate.ShipIdPickPoint], out idShip);
                    PaymentService.DeletePaymentMethod(idShip);
                }
            }

            if (ShippingType == ShippingType.CheckoutRu)
            {
                if (SQLDataHelper.GetBoolean(parameters[ShippingCheckoutRuTemplate.EnabledCOD]))
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[ShippingCheckoutRuTemplate.ShipIdCOD], out idShip);
                    var payment = PaymentService.GetPaymentMethod(idShip);
                    if (payment == null)
                    {
                        var payMethod = PaymentMethod.Create(PaymentType.CashOnDelivery);
                        payMethod.Name = Resource.CashOnDeliveryName;
                        payMethod.Enabled = true;
                        if (payMethod.Parameters.ContainsKey(CashOnDelivery.ShippingMethodTemplate))
                        {
                            payMethod.Parameters[CashOnDelivery.ShippingMethodTemplate] = ShippingMethodId.ToString();
                        }
                        else
                        {
                            payMethod.Parameters.Add(CashOnDelivery.ShippingMethodTemplate,
                                ShippingMethodId.ToString());
                        }

                        var id = PaymentService.AddPaymentMethod(payMethod);
                        parameters[ShippingCheckoutRuTemplate.ShipIdCOD] = id.ToString();
                    }
                }
                else
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[ShippingCheckoutRuTemplate.ShipIdCOD], out idShip);
                    PaymentService.DeletePaymentMethod(idShip);
                }
            }

            if (ShippingType == ShippingType.Cdek)
            {
                if (SQLDataHelper.GetBoolean(parameters[CdekTemplate.EnabledCOD]))
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[CdekTemplate.ShipIdCOD], out idShip);
                    var payment = PaymentService.GetPaymentMethod(idShip);
                    if (payment == null)
                    {
                        var payMethod = PaymentMethod.Create(PaymentType.CashOnDelivery);
                        payMethod.Name = Resource.CashOnDeliveryName;
                        payMethod.Enabled = true;
                        if (payMethod.Parameters.ContainsKey(CashOnDelivery.ShippingMethodTemplate))
                        {
                            payMethod.Parameters[CashOnDelivery.ShippingMethodTemplate] = ShippingMethodId.ToString();
                        }
                        else
                        {
                            payMethod.Parameters.Add(CashOnDelivery.ShippingMethodTemplate,
                                ShippingMethodId.ToString());
                        }

                        var id = PaymentService.AddPaymentMethod(payMethod);
                        parameters[CdekTemplate.ShipIdCOD] = id.ToString();
                    }
                }
                else
                {
                    int idShip = 0;
                    Int32.TryParse(parameters[CdekTemplate.ShipIdCOD], out idShip);
                    PaymentService.DeletePaymentMethod(idShip);
                }
            }
            //if (ShippingType == ShippingType.ShippingNovaPoshta)
            //{
            //    //COD
            //    if (SQLDataHelper.GetBoolean(parameters[NovaPoshtaTemplate.EnabledCOD]))
            //    {
            //        int idShip = 0;
            //        Int32.TryParse(parameters[NovaPoshtaTemplate.ShipIdCOD], out idShip);
            //        var payment = PaymentService.GetPaymentMethod(idShip);
            //        if (payment == null)
            //        {
            //            var payMethod = PaymentMethod.Create(PaymentType.CashOnDelivery);
            //            payMethod.Name = Resource.CashOnDeliveryName;
            //            payMethod.Enabled = true;
            //            if (payMethod.Parameters.ContainsKey(CashOnDelivery.ShippingMethodTemplate))
            //            {
            //                payMethod.Parameters[CashOnDelivery.ShippingMethodTemplate] = ShippingMethodId.ToString();
            //            }
            //            else
            //            {
            //                payMethod.Parameters.Add(CashOnDelivery.ShippingMethodTemplate,
            //                                         ShippingMethodId.ToString());

            //            }

            //            var id = PaymentService.AddPaymentMethod(payMethod);
            //            parameters[NovaPoshtaTemplate.ShipIdCOD] = id.ToString();
            //        }
            //    }
            //    else
            //    {
            //        int idShip = 0;
            //        Int32.TryParse(parameters[NovaPoshtaTemplate.ShipIdCOD], out idShip);
            //        PaymentService.DeletePaymentMethod(idShip);
            //    }
            //}

            var payments = new System.Collections.Generic.List<int>();
            foreach (RepeaterItem item in rptrPayments.Items)
            {
                if (!((CheckBox)item.FindControl("ckbUsePayment")).Checked)
                {
                    payments.Add(SQLDataHelper.GetInt(((HiddenField)item.FindControl("hfPaymentId")).Value));
                }
            }
            ShippingMethodService.UpdateShippingPayments(ShippingMethodId, payments);

            if (ShippingMethodService.UpdateShippingParams(method.ShippingMethodId, parameters))
            {
                Method = ShippingMethodService.GetShippingMethod(method.ShippingMethodId);
                LoadFormData(true);
                OnSaved(new SavedEventArgs { Enabled = method.Enabled, Name = method.Name });
            }
            ShippingCacheRepositiry.Delete(method.ShippingMethodId);
        }

        private void MsgErr(Label lbl, string message)
        {
            if (lbl == null) { _valid = false; return; } lbl.Visible = true;
            lbl.Text = message;
            _valid = false;
        }

        protected bool ValidateFormData()
        {
            _valid = true;
            new[] { txtName, txtSortOrder }
                .Where(textBox => string.IsNullOrEmpty(textBox.Text))
                .ForEach(textBox => MsgErr((Label)FindControl("msg" + textBox.ID.Substring(3)), Resource.Admin_Messages_EnterValue));
            if (!txtSortOrder.Text.IsInt())
                MsgErr(msgSortOrder, Resource.Admin_Messages_IsInt);
            return _valid;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ShippingMethodService.DeleteShippingMethod(ShippingMethodId);
            if (ShippingMethodService.GetShippingMethod(ShippingMethodId) == null)
                Response.Redirect("~/Admin/ShippingMethod.aspx");
        }

        public event Action<object, SavedEventArgs> Saved;

        public void OnSaved(SavedEventArgs args)
        {
            Saved(this, args);
        }

        public class SavedEventArgs : EventArgs
        {
            public bool Enabled { get; set; }
            public string Name { get; set; }
        }

        protected void btnAddCountry_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCountry.Text)) return;
            var country = CountryService.GetCountryByName(txtCountry.Text);
            if (country == null) return;
            if (!ShippingPaymentGeoMaping.IsExistShippingCountry(ShippingMethodId, country.CountryId))
            {
                ShippingPaymentGeoMaping.AddShippingCountry(ShippingMethodId, country.CountryId);
            }
            txtCountry.Text = string.Empty;
        }

        protected void repeaterCountry_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DelCountry")
            {
                var id = SQLDataHelper.GetInt(e.CommandArgument);
                ShippingPaymentGeoMaping.DeleteShippingCountry(ShippingMethodId, id);
            }
        }

        protected void btnAddCity_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCity.Text)) return;
            var city = CityService.GetCityByName(txtCity.Text);
            if (city == null) return;
            if (!ShippingPaymentGeoMaping.IsExistShippingCity(ShippingMethodId, city.CityId))
            {
                ShippingPaymentGeoMaping.AddShippingCity(ShippingMethodId, city.CityId);
            }
            txtCity.Text = string.Empty;
        }


        protected void btnAddCityExcluded_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCityExcluded.Text)) return;
            var city = CityService.GetCityByName(txtCityExcluded.Text);
            if (city == null) return;
            if (!ShippingPaymentGeoMaping.IsExistShippingCityExcluded(ShippingMethodId, city.CityId))
            {
                ShippingPaymentGeoMaping.AddShippingCityExcluded(ShippingMethodId, city.CityId);
            }
            txtCityExcluded.Text = string.Empty;
        }


        protected void btnUploadIcon_Click(object sender, EventArgs e)
        {
            if (!fuIcon.HasFile) return;
            if (!FileHelpers.CheckFileExtension(fuIcon.FileName, FileHelpers.eAdvantShopFileTypes.Image))
            {
                MsgErr(msgSortOrder, Resource.Admin_ErrorMessage_WrongImageExtension);
                return;
            }
            PhotoService.DeletePhotos(ShippingMethodId, PhotoType.Shipping);
            var tempName = PhotoService.AddPhoto(new Photo(0, ShippingMethodId, PhotoType.Shipping) { OriginName = fuIcon.FileName });
            if (string.IsNullOrWhiteSpace(tempName)) return;
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(fuIcon.FileContent))
            {
                FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.ShippingLogo, tempName), SettingsPictureSize.ShippingIconWidth, SettingsPictureSize.ShippingIconHeight, image);
            }
            imgIcon.ImageUrl = FoldersHelper.GetPath(FolderType.ShippingLogo, tempName, true);
            imgIcon.Visible = true;
            btnDeleteIcon.Visible = true;
        }

        protected void btnDeleteIcon_Click(object sender, EventArgs e)
        {
            PhotoService.DeletePhotos(ShippingMethodId, PhotoType.Shipping);
            imgIcon.Visible = false;
            btnDeleteIcon.Visible = false;
        }
    }
}