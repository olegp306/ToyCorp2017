<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAccountCommonInformation.ascx.cs"
    Inherits="UserControls.MyAccount.CommonInformation" %>
<ul class="form form-vr form-adress">
    <li class="title">
        <div class="param-name">
            <%= Resources.Resource.Client_MyAccount_CommonInf %>
        </div>
        <div class="param-value">
        </div>
    </li>
    <li>
        <div class="param-name">
            E-mail:
        </div>
        <div class="param-value">
            <asp:Label ID="lblEmail" runat="server"></asp:Label>
        </div>
    </li>
    <li>
        <div class="param-name">
            <%=Resources.Resource.Client_MyAccount_RegDate%></div>
        <div class="param-value">
            <asp:Label ID="lblRegistrationDate" runat="server"></asp:Label>
        </div>
    </li>
    <li>
        <div class="param-name">
            <label for="txtFirstName">
                <%=Resources.Resource.Client_MyAccount_Name%></label></div>
        <div class="param-value">
            <adv:AdvTextBox ValidationType="Required" ID="txtFirstName" runat="server" DefaultButtonID="btnChangeCommonInfo"></adv:AdvTextBox>
        </div>
    </li>
    <li>
        <div class="param-name">
            <label for="txtLastName">
                <%=Resources.Resource.Client_MyAccount_Surname%></label></div>
        <div class="param-value">
            <adv:AdvTextBox ValidationType="Required" ID="txtLastName" runat="server" DefaultButtonID="btnChangeCommonInfo"></adv:AdvTextBox>
        </div>
    </li>
    <li>
        <div class="param-name">
            <label for="txtContacts">
                <%=Resources.Resource.Client_MyAccount_Phone%></label></div>
        <div class="param-value">
            <adv:AdvTextBox ValidationType="None" ID="txtContacts" runat="server" DefaultButtonID="btnChangeCommonInfo"></adv:AdvTextBox></div>
    </li>
    <li runat="server" id="liCustomerRole">
        <div class="param-name">
            <%=Resources.Resource.Client_MyAccount_CustomerType%></div>
        <div class="param-value">
            <asp:Label runat="server" ID="lCustomerType"></asp:Label></div>
    </li>
    <li runat="server" id="liCustomerGroup">
        <div class="param-name">
            <%=Resources.Resource.Client_MyAccount_CustomerGroup%></div>
        <div class="param-value">
            <asp:Label runat="server" ID="lCustomerGroup" Width="200px"></asp:Label></div>
    </li>
    <li runat="server" id="newsSubscription">
        <div class="param-name"></div>
        <div class="param-value">
            <asp:CheckBox ID="chkSubscribed4News" runat="server" Text="<%$ Resources:Resource,Client_MyAccount_NewsSubscribe%>" /></div> 
    </li>
    <li>
        <div class="param-name">
        </div>
        <div class="param-value">
            <adv:Button Type="Submit" Size="Big" ID="btnChangeCommonInfo" runat="server" Text="<%$ Resources:Resource, Client_MyAccount_Change %>"
                OnClick="btnChangeCommonInfo_Click" />
        </div>
    </li>
</ul>
