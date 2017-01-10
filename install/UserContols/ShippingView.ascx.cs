using System;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Helpers;
using AdvantShop.Shipping;
using AdvantShop.Trial;

namespace ClientPages
{
    public partial class install_UserContols_ShippingView : AdvantShop.Controls.InstallerStep
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            edostPanel.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityShipping("eDost");
            txteDostPass.Visible = !(Demo.IsDemoEnabled || TrialService.IsTrialEnabled);
            leDostPassword.Visible = Demo.IsDemoEnabled || TrialService.IsTrialEnabled;
            mvShipping.SetActiveView(vNew);
        }

        public new void LoadData()
        {

            var sm = ShippingMethodService.GetShippingMethodByName(chbPickup.Text);
            if (sm != null)
            {
                chbPickup.Checked = true;
            }

            sm = ShippingMethodService.GetShippingMethodByName(chbCourier.Text);
            if (sm != null && sm.Type == ShippingType.FixedRate)
            {
                chbCourier.Checked = true;
                txtCourier.Text = sm.Params.ElementOrDefault(FixeRateShippingTemplate.ShippingPrice);
            }

            sm = ShippingMethodService.GetShippingMethodByName(chbeDost.Text);
            if (sm != null && sm.Type == ShippingType.eDost)
            {
                chbeDost.Checked = true;
                txteDostNumer.Text = sm.Params.ElementOrDefault(EdostTemplate.ShopId);
                txteDostPass.Text = sm.Params.ElementOrDefault(EdostTemplate.Password);

            }
        }

        public new void SaveData()
        {
            var sm = ShippingMethodService.GetShippingMethodByName(chbPickup.Text);
            if (sm != null)
                ShippingMethodService.DeleteShippingMethod(sm.ShippingMethodId);

            if (chbPickup.Checked)
            {
                var method = new ShippingMethod
                {
                    Type = ShippingType.FreeShipping,
                    Name = chbPickup.Text,
                    Description = string.Empty,
                    Enabled = true,
                    SortOrder = 0
                };
                ShippingMethodService.InsertShippingMethod(method);
            }

            sm = ShippingMethodService.GetShippingMethodByName(chbCourier.Text);
            if (sm != null)
                ShippingMethodService.DeleteShippingMethod(sm.ShippingMethodId);
            if (chbCourier.Checked)
            {
                var method = new ShippingMethod
                {
                    Type = ShippingType.FixedRate,
                    Name = chbCourier.Text,
                    Description = string.Empty,
                    Enabled = true,
                    SortOrder = 1
                };
                var id = ShippingMethodService.InsertShippingMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {FixeRateShippingTemplate.ShippingPrice, txtCourier.Text},
                    {FixeRateShippingTemplate.Extracharge, "0"},
                };
                ShippingMethodService.UpdateShippingParams(id, parameters);
            }

            sm = ShippingMethodService.GetShippingMethodByName(chbeDost.Text);
            if (sm != null)
                ShippingMethodService.DeleteShippingMethod(sm.ShippingMethodId);
            if (chbeDost.Checked)
            {
                var method = new ShippingMethod
                {
                    Type = ShippingType.eDost,
                    Name = chbeDost.Text,
                    Description = string.Empty,
                    Enabled = true,
                    SortOrder = 2
                };
                var id = ShippingMethodService.InsertShippingMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {EdostTemplate.ShopId, txteDostNumer.Text},
                    {EdostTemplate.Password, txteDostPass.Text},
                };
                ShippingMethodService.UpdateShippingParams(id, parameters);
            }
        }

        public new bool Validate()
        {
            var validList = new List<ValidElement>();
            if (chbCourier.Checked)
            {
                validList.Add(new ValidElement()
                {
                    Control = txtCourier,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Money,
                    Message = Resources.Resource.Install_UserContols_ShippingView_Err_WrongPrice
                });
            }

            if (chbeDost.Checked)
            {
                validList.Add(new ValidElement()
                {
                    Control = txteDostNumer,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_ShippingView_Err_NeedNum
                });

                if (!(Demo.IsDemoEnabled || TrialService.IsTrialEnabled))
                {
                    validList.Add(new ValidElement()
                    {
                        Control = txteDostPass,
                        ErrContent = ErrContent,
                        ValidType = ValidType.Required,
                        Message = Resources.Resource.Install_UserContols_ShippingView_Err_NeedPass
                    });
                }
            }
            return ValidationHelper.Validate(validList);
        }
    }
}