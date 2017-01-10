<%@ Page Language="C#" AutoEventWireup="true" CodeFile="m_StaticBlock.aspx.cs" Inherits="Admin.m_StaticBlock"
    MasterPageFile="m_MasterPage.master" ValidateRequest="false" %>


<asp:Content ID="contentCenter" ContentPlaceHolderID="cphCenter" runat="server">
    <div style="padding-top: 5px;">
        <div style="text-align: center;" >
            <asp:Label ID="lblBigHead" runat="server" CssClass="AdminHead" Text="<%$ Resources:Resource, Admin_PagePart_lblMain %>"></asp:Label>
            <br/>
            <asp:Label ID="lblSubHead" runat="server" CssClass="AdminSubHead" Text="<%$ Resources:Resource, Admin_PagePart_Create %>"></asp:Label><br/>
            <asp:Label ID="lblRestrict" runat="server" Text="Label" Font-Bold="True" Visible="False"
                       ForeColor="Red"></asp:Label>
        </div>
        <br />
        <table border="0" cellpadding="2" cellspacing="0" width="100%" id="TABLE2" class="catalog_link">
            <tr style="background-color: #eff0f1;">
                <td style="width: 49%; height: 33px; text-align: right">
                    <asp:Literal ID="Literal1" Text="<%$ Resources:Resource, Admin_PageParts_Key %>"
                        runat="server"></asp:Literal>
                </td>
                <td style="vertical-align: middle; height: 33px;">
                    <asp:TextBox ID="txtKey" runat="server" Width="230px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 49%; height: 33px; text-align: right">
                    <asp:Literal ID="Literal2" Text="<%$ Resources:Resource, Admin_PageParts_Title %>"
                        runat="server"></asp:Literal>
                </td>
                <td style="vertical-align: middle; height: 33px;">
                    <asp:TextBox ID="txtPageTitle" runat="server" Width="230px"></asp:TextBox>
                </td>
            </tr>
            <tr style="background-color: #eff0f1;">
                <td style="width: 49%; height: 33px; text-align: right">
                    <asp:Literal ID="Literal3" Text="<%$ Resources:Resource, Admin_PageParts_Enabled %>"
                        runat="server"></asp:Literal>
                </td>
                <td style="vertical-align: middle; height: 33px;">
                    <asp:CheckBox ID="chbEnabled" runat="server" Checked="true" />
                </td>
            </tr>
        </table>
        <br />
        <table border="0" id="table1" style="width: 100%; height: 206px;" cellspacing="0"
            cellpadding="0">
            <tr>
                <td align="center">
                    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Left" Width="700px" CssClass="all-mar">
                        <asp:Label ID="Label8" runat="server" Text="<%$ Resources:Resource, Admin_PageParts_Text %>"></asp:Label>
                        <br />
                        <CKEditor:CKEditorControl ID="CKEditorControl1" BasePath="~/ckeditor/" runat="server" Height="300px" Width="700px" />
                    </asp:Panel>
                    &nbsp;
                </td>
            </tr>
        </table>
        <center>
            <%--<asp:ValidationSummary runat="server" ID="vSumm" ValidationGroup="vGroup" ForeColor="Red" />--%>
            &nbsp;<asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False"></asp:Label>
            <asp:HiddenField ID="hfMetaId" runat="server" />
        </center>
        <center>
            <br />
            <asp:Button ID="btnAdd" runat="server" Text="Add" Width="103px" ValidationGroup="vGroup"
                OnClick="btnAdd_Click" />&nbsp;
        </center>
        <br />
        <br />
    </div>
</asp:Content>
