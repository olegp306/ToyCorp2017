<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepShipping.ascx.cs" Inherits="UserControls.OrderConfirmation.StepShipping" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="adv" TagName="ShippingRates" Src="~/UserControls/OrderConfirmation/ShippingMethods.ascx" %>

<div class="oc-block delivery">
    <% if (SettingsDesign.DisplayCityInTopPanel)
       {%>
    <div <%= DisplayDelivery ? "style='margin-bottom: 20px;'" : " style='display:none;'" %>>
    <div class="oc-text-left" style="margin-top: 10px;">Выберите город</div>
    
    <div class="delivery-change js-location-call" data-delivery='<%= DeliveryJson %>' <%= DisplayDelivery ? "" : " style='display:none'" %> title="<%= Resource.Client_OrderConfirmation_ChangeAddress %>">
        <asp:Literal runat="server" ID="liDelivery" />
    </div>
    
    </div>
    <%} %>
    <adv:ShippingRates ID="ShippingRates" runat="server" />
</div>

<div id="divDisplayAddress" runat="server">

    <div class="oc-block delivery-address" style="display: <%=DisplayBlock == "block" || BlockCustomField == "block" ? "block" : "none" %>; padding: 0;">
        <div class="oc-text-left">
            <%= Resources.Resource.Client_OrderConfirmation_ShippingAddress %>
        </div>
        <div class="delivery-address-content">
            <ul class="form form-vr">
                <% if (SettingsOrderConfirmation.IsShowCustomShippingField1)
                   { %>
                <li class="cshipfield" style="display: <%=BlockCustomField%>">
                    <div class="param-name">
                        <label for="txtShippingField1">
                            <%= SettingsOrderConfirmation.CustomShippingField1 %>:</label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtShippingField1" runat="server" />
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowCustomShippingField2)
                   { %>
                <li class="cshipfield" style="display: <%=BlockCustomField%>">
                    <div class="param-name">
                        <label for="txtShippingField2">
                            <%= SettingsOrderConfirmation.CustomShippingField2 %>:</label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtShippingField2" runat="server" />
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowZip)
                   { %>
                <li class="cshipfield" style="display: <%=BlockCustomField%>">
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtZip" runat="server" MaxLength="70" Placeholder="Почтовый индекс" />
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowAddress)
                   { %>
                <li class="deliveryfield" style="display: <%=DisplayBlock%>">
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Required" ID="txtAddress" runat="server" MaxLength="255" TextMode="Multiline" Placeholder="Адрес" />
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowCustomShippingField3)
                   { %>
                <li class="cshipfield" style="display: <%=BlockCustomField%>">
                    <div class="param-name">
                        <label for="txtShippingField3">
                            <%= SettingsOrderConfirmation.CustomShippingField3 %>:</label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtShippingField3" runat="server" />
                    </div>
                </li>
                <% } %>
            </ul>
        </div>

    </div>

</div>

