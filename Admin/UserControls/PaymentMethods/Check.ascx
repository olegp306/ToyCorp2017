<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Check.ascx.cs" Inherits="Admin.UserControls.PaymentMethods.CheckControl" %>
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px; margin-top: 5px;">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
            <h4 style="display: inline; font-size: 12pt;">
                <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethods_HeadSettings %>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_CompanyName %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtCompanyName" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_CompanyName%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_CompanyName_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgCompanyName" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_CompanyPhone %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtPhone" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_CompanyPhone%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_CompanyPhone_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgPhone" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_Country %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtCountry" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_Country%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_Country_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgCountry" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_State %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtState" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_State%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_State_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgState" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_City %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtCity" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_City%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_City_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgCity" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_Address %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtAddress" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_Address%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_Address_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgAddress" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_Fax %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtFax" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_Fax%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_Fax_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgFax" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Check_IntPhone %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtIntPhone" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_IntPhone%>
                    </header>
                    <div class="help-content">
                        <%= Resources.Resource.Admin_PaymentMethod_Check_IntPhone_Description%>
                    </div>
                </article>
            </div>
            <asp:Label runat="server" ID="msgIntPhone" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
</table>
