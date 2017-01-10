using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Diagnostics;
using AdvantShop.Shipping;
using Newtonsoft.Json;

namespace Admin.UserControls.ShippingMethods
{
    public partial class MultishipControl : ParametersControl
    {
        protected bool IsActive;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lblStatus.Text = IsActive ? "Активен" : "Не активен";
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                var dictionary = new Dictionary<string, string>();
                var isActive = hfIsActive.Value.TryParseBool();

                dictionary.Add(MultishipTemplate.IsActive, isActive.ToString());

                if (isActive)
                {
                    dictionary.Add(MultishipTemplate.CityFrom, txtCityFrom.Text);

                    dictionary.Add(MultishipTemplate.WeightAvg, txtWeight.Text);
                    dictionary.Add(MultishipTemplate.HeightAvg, txtHeightAvg.Text);
                    dictionary.Add(MultishipTemplate.WidthAvg, txtWidthAvg.Text);
                    dictionary.Add(MultishipTemplate.LengthAvg, txtLengthAvg.Text);

                    if (ddlSenders.Items.Count > 0)
                    {
                        var senderId = ddlSenders.SelectedValue;
                        dictionary.Add(MultishipTemplate.SenderId, senderId);

                        try
                        {
                            var strMsObject = hfMSObject.Value;
                            if (strMsObject.IsNotEmpty())
                            {
                                var msObj = JsonConvert.DeserializeObject<MultishipInitAnswer>(strMsObject);
                                if (msObj != null && msObj.widgetCode.ContainsKey(senderId))
                                {
                                    dictionary.Add(MultishipTemplate.WidgetCode,
                                        string.Format("https://multiship.ru/widget/loader?resource_id={0}&sid={1}&key={2}",
                                                    msObj.config.clientId, senderId, msObj.widgetCode[senderId]));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                }
                else
                {
                    Save(dictionary);
                }

                return dictionary;
            }
            set
            {
                IsActive = value.ElementOrDefault(MultishipTemplate.IsActive).TryParseBool();
                hfIsActive.Value = IsActive.ToString();

                if (IsActive)
                {
                    txtCityFrom.Text = value.ElementOrDefault(MultishipTemplate.CityFrom) ?? "Москва";
                    txtWeight.Text = value.ElementOrDefault(MultishipTemplate.WeightAvg) ?? "5";

                    txtHeightAvg.Text = value.ElementOrDefault(MultishipTemplate.HeightAvg) ?? "10";
                    txtWidthAvg.Text = value.ElementOrDefault(MultishipTemplate.WidthAvg) ?? "10";
                    txtLengthAvg.Text = value.ElementOrDefault(MultishipTemplate.LengthAvg) ?? "10";

                    try
                    {
                        ddlSenders.Items.Clear();

                        var senderId = value.ElementOrDefault(MultishipTemplate.SenderId) ?? string.Empty;
                        var msObject = value.ElementOrDefault(MultishipTemplate.MSobject) ?? string.Empty;

                        if (msObject.IsNotEmpty())
                        {
                            var msObj = JsonConvert.DeserializeObject<MultishipInitAnswer>(msObject);
                            if (msObj != null && msObj.config.senders.Count > 0)
                            {
                                foreach (var sender in msObj.config.senders)
                                {
                                    ddlSenders.Items.Add(new ListItem(sender, sender));
                                }
                            }

                            hfMSObject.Value = msObject;
                        }

                        if (ddlSenders.Items.Count == 0 && senderId.IsNotEmpty())
                        {
                            ddlSenders.Items.Add(new ListItem(senderId, senderId));
                        }

                        if (ddlSenders.Items.FindByValue(senderId) != null)
                            ddlSenders.SelectedValue = senderId;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }

        protected void Save(Dictionary<string, string> dictionary)
        {
            string errorMessage = string.Empty;

            var initObj = MultishipService.Register(txtEmail.Text, txtPassword.Text, txtDomain.Text, ref errorMessage);
            if (initObj == null)
            {
                lError.Text = errorMessage.IsNotEmpty() ? errorMessage : "Ошибка активации";
                lError.Visible = true;
                return;
            }

            dictionary.Add(MultishipTemplate.ClientId, initObj.config.clientId);

            if (initObj.config.methodKeys.ContainsKey("searchDeliveryList"))
            {
                dictionary.Add(MultishipTemplate.SecretKeyDelivery,
                    initObj.config.methodKeys["searchDeliveryList"]);
            }

            if (initObj.config.methodKeys.ContainsKey("createOrder"))
            {
                dictionary.Add(MultishipTemplate.SecretKeyCreateOrder,
                    initObj.config.methodKeys["createOrder"]);
            }

            if (initObj.config.senders.Count > 0)
            {
                var senderId = initObj.config.senders.First();

                dictionary.Add(MultishipTemplate.SenderId, senderId);

                if (initObj.widgetCode != null && initObj.widgetCode.Count > 0)
                {
                    var widgetCode = initObj.widgetCode[senderId];

                    dictionary.Add(MultishipTemplate.WidgetCode,
                        string.Format("https://multiship.ru/widget/loader?resource_id={0}&sid={1}&key={2}",
                                    initObj.config.clientId, senderId, widgetCode));
                }
            }

            if (initObj.config.warehouses.Count > 0)
            {
                dictionary.Add(MultishipTemplate.WarehouseId,
                    initObj.config.warehouses.First());
            }

            if (initObj.config.requisites.Count > 0)
            {
                dictionary.Add(MultishipTemplate.RequisiteId,
                    initObj.config.requisites.First());
            }

            dictionary.Add(MultishipTemplate.MSobject, JsonConvert.SerializeObject(initObj));

            if (dictionary.ContainsKey(MultishipTemplate.IsActive))
                dictionary[MultishipTemplate.IsActive] = true.ToString();
            else
                dictionary.Add(MultishipTemplate.IsActive, true.ToString());

            lError.Visible = false;
        }
    }
}