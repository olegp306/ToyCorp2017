<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShippingMethods.ascx.cs"
    Inherits="UserControls.OrderConfirmation.ShippingMethod" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="AdvantShop.Shipping" %>

<input type="hidden" id="_selectedID" class="HiddenField" runat="server" value="" />
<input type="hidden" id="pickpointId" class="HiddenField" runat="server" value="" />
<input type="hidden" id="pickAddress" class="HiddenField" runat="server" value="" />
<input type="hidden" id="hfDistance" class="HiddenField" runat="server" value="0" />
<div id="RadioList" runat="server" class="RadioList" visible="true">
</div>
<asp:ListView ID="lvShippingRates" runat="server" ItemPlaceholderID="itemPlaceHolder">
    <LayoutTemplate>
        <table class="avangard">
            <tr class="header">
                <th colspan="3">
                </th>
                <th class="cost">
                    <asp:Literal runat="server" Text="<%$Resources:Resource, Client_OrderConfirmation_ShippingPrice %>" />
                </th>
                <th class="shipping-time">
                    <asp:Literal runat="server" Text="<%$Resources:Resource, Client_OrderConfirmation_ShippingTerm %>" />
                </th>
                <th class="note">
                    <asp:Literal runat="server" Text="<%$Resources:Resource, Client_OrderConfirmation_Note %>" />
                </th>
                <th>
                </th>
            </tr>
            <tr runat="server" id="itemPlaceHolder">
            </tr>
        </table>
    </LayoutTemplate>
    <ItemTemplate>
        <tr onclick="setValueShipping(this);" id='<%# PefiksId +  Eval("Id") %>'>
            <td class="checkbox">
                <input type="radio" name="gr" />
            </td>
            <td class="shipping-img">
                <img src='<%# ShippingIcons.GetShippingIcon((ShippingType)Eval("Type"), Eval("IconName") as string , SQLDataHelper.GetString(Eval("MethodNameRate"))) %>'
                    <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("MethodNameRate").ToString())) %> />
            </td>
            <td>
                <%#Eval("MethodNameRate") + RenderPickPoint((ShippingOptionEx)Eval("Ext"))%>
                <%# RenderExtend((int)Eval("MethodId"),(ShippingType)Eval("Type"), (Dictionary<string,string>)Eval("Params")) %>
            </td>
            <td class="cost">
                <%# AdvantShop.Catalog.CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Rate")))%>
            </td>
            <td class="shipping-time">
                <%# Eval("DeliveryTime")%>
            </td>
            <td class="note">
                <%# Eval("MethodDescription") %>
            </td>
            <td>
            </td>
        </tr>
    </ItemTemplate>
    <SelectedItemTemplate>
        <tr class="selected" id='<%# PefiksId +  Eval("Id ") %>' onclick="setValueShipping(this);">
            <td class="checkbox">
                <input type="radio" name="gr" checked="checked" />
            </td>
            <td class="shipping-img">
                <img src='<%# ShippingIcons.GetShippingIcon((ShippingType)Eval("Type"), Eval("IconName") as string , SQLDataHelper.GetString(Eval("MethodNameRate"))) %>'
                    <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("MethodNameRate").ToString())) %> />
            </td>
            <td>
                <%#Eval("MethodNameRate") + RenderPickPoint((ShippingOptionEx)Eval("Ext"))%>
                <%# RenderExtend((int)Eval("MethodId"),(ShippingType)Eval("Type"), (Dictionary<string,string>)Eval("Params")) %>
            </td>
            <td class="cost">
                <%# AdvantShop.Catalog.CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Rate")))%>
            </td>
            <td class="shipping-time">
                <%# Eval("DeliveryTime")%>
            </td>
            <td class="note">
                <%# Eval("MethodDescription") %>
            </td>
            <td>
            </td>
        </tr>
    </SelectedItemTemplate>
    <EmptyDataTemplate>
        <table class="avangard">
            <tr class="header">
                <th style="text-align: center;">
                    <br />
                    <asp:Label ID="lblNoShipping" runat="server" Style="color: red" Text="<%$ Resources:Resource, Client_ShippingRates_NoShipping %>"></asp:Label>
                </th>
            </tr>
        </table>
    </EmptyDataTemplate>
</asp:ListView>

<div id="divScripts" runat="server" visible="false">
    <script type="text/javascript">
        $(function () {
            if ($("tr.selected").first().find("a.pickpoint").length && $("#address").html() == "") {
                $("#btnNextFromShipPay").addClass("btn-disabled");
            } else {
                $("#btnNextFromShipPay").removeClass("btn-disabled");
            }
            jQuery.getScript("http://pickpoint.ru/select/postamat.js");
        });
    </script>
</div>
