<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BankSettings.ascx.cs" Inherits="Admin.UserControls.Settings.BankSettings" %>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Localize_Admin_CommonSettings_BankingDetails%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 155px;">
            <label class="form-lbl" for="<%= txtCompanyName.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_NameOfOrganization%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtCompanyName" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtINN.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_Inn%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtINN" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtKPP.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_Ppc%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtKPP" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtRS.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_SettelmentAccount%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtRS" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtBankName.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_NameOfBank%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtBankName" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtKS.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_CorrespondentAccount%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtKS" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtBIK.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_Bic%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtBIK" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtAddress.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_Address%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtAddress" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= fuStamp.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_CompanyStamp%></label>
        </td>
        <td>
            <asp:Panel ID="pnlStamp" runat="server" Width="100%">
                <img src='<%= GetImageSource() %>' alt=""/>
                <%--<asp:Image ID="imgStamp" runat="server" Height="80" BorderColor="Gray" BorderWidth="1px" />--%>
                <br />
                <asp:Button ID="btnDeleteStamp" runat="server" Text="<%$ Resources:Resource, Admin_Delete%>" OnClick="DeleteStamp_Click" />
            </asp:Panel>
            <asp:FileUpload ID="fuStamp" runat="server" Height="20px" Width="308px" BackColor="White" />
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Localize_Admin_CommonSettings_Guidance%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 155px;">
            <label class="form-lbl" for="<%= txtDirector.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_Director%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtDirector" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtHeadCounter.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_AccountantGeneral%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtHeadCounter" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtManager.ClientID %>"><%= Resources.Resource.Localize_Admin_CommonSettings_Manager%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" ID="txtManager" runat="server"></asp:TextBox>
        </td>
    </tr>
</table>
