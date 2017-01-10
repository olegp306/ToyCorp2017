<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShippingView.ascx.cs"
    Inherits="ClientPages.install_UserContols_ShippingView" %>
<h1>
    <% = Resources.Resource.Install_UserContols_ShippingView_h1%></h1>
<asp:MultiView runat="server" ID="mvShipping">
    <asp:View runat="server" ID="vNew">
        <fieldset class="group simple">
            <legend>
                <asp:CheckBox runat="server" ID="chbPickup" Text="<%$ Resources:Resource, Install_UserContols_ShippingView_Chb_Self %>" /></legend>
        </fieldset>
        <fieldset class="group">
            <legend>
                <asp:CheckBox runat="server" ID="chbCourier" Text="<%$ Resources:Resource, Install_UserContols_ShippingView_Chb_Courier %>" />
            </legend>
            <div class="block-options">
                <p>
                    <% = Resources.Resource.Install_UserContols_ShippingView_ShippingPrice%>
                </p>
                <div class="str">
                    <asp:TextBox runat="server" CssClass="txt valid-money" ID="txtCourier"></asp:TextBox>
                </div>
            </div>
        </fieldset>
        <fieldset class="group" runat="server" id="edostPanel">
            <legend>
                <asp:CheckBox runat="server" ID="chbeDost" Text="<%$ Resources:Resource,Install_UserContols_ShippingView_eDost %>" />
            </legend>
            <div class="block-options">
                <p style="font-size: 12px; padding-bottom: 15px;">
                    <%= Resources.Resource.Install_UserContols_ShippingView_eDostDesc%>
                </p>
                <p>
                    <%= Resources.Resource.Install_UserContols_ShippingView_eDostNum%></p>
                <div class="str">
                    <asp:TextBox runat="server" CssClass="txt valid-required" ID="txteDostNumer"></asp:TextBox>
                </div>
                <p>
                    <%= Resources.Resource.Install_UserContols_ShippingView_eDostPass%></p>
                <div class="str">
                    <asp:TextBox runat="server" CssClass="txt valid-required" ID="txteDostPass"></asp:TextBox>
                    <asp:Literal runat="server" ID="leDostPassword" Text="******"/>
                </div>
            </div>
        </fieldset>
        <p>
            <% = Resources.Resource.Install_UserContols_ShippingView_Warning%>
        </p>
    </asp:View>
    <asp:View runat="server" ID="vExistShippings">
        <div class="group">
            <% = Resources.Resource.Install_UserContols_ShippingView_ShippingExist%>
        </div>
    </asp:View>
</asp:MultiView>
