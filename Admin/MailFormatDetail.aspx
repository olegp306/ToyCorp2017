<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="MailFormatDetail.aspx.cs" Inherits="Admin.MailFormatDetail" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div style="margin-left: 10px;">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tbody>
                <tr>
                    <td style="width: 72px;">
                        <img src="images/orders_ico.gif" alt="" />
                    </td>
                    <td>
                        <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_MailFormatDetail_Header %>"></asp:Label><br />
                        <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_MailFormatDetail_Create %>"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:hyperlink ID="Hyperlink1" NavigateUrl="~/Admin/MailFormat.aspx" Text='<%$ Resources: Resource, Admin_Back %>' runat="server" CssClass="Link"></asp:hyperlink>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Blue" Visible="false"></asp:Label>
        <asp:Label ID="Message" runat="server" ForeColor="Red" Visible="False"></asp:Label>
        
        <table cellspacing="0" cellpadding="4" border="0" style="width: 1000px; border-collapse: collapse;">
            <tr>
                <td style="width:120px; font-weight: bold;">
                    <asp:Label ID="lblMailFormatName" runat="server" Text="<%$ Resources:Resource, Admin_MailFormat_Name %>" />
                </td>
                <td>
                    <asp:TextBox ID="txtName" runat="server" Width="800px" />
                </td>
            </tr>
            <tr>
                <td style="width:120px; font-weight: bold;">
                    <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_MailFormat_Subject%>" />
                </td>
                <td>
                    <asp:TextBox ID="txtSubject" runat="server" Width="800px" />
                </td>
            </tr>
            <tr>
                <td style="width:120px; font-weight: bold;">
                    <asp:Label ID="lblMailFormatText" runat="server" Text="<%$ Resources:Resource, Admin_MailFormat_Text %>" />
                </td>
                <td>
                    <CKEditor:CKEditorControl ID="CKEditorControl1" BasePath="~/ckeditor/" runat="server" Height="450px" Width="800px" />
                </td>
            </tr>
        </table>

        <asp:UpdatePanel ID="ff" runat ="server" UpdateMode ="Conditional" >
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlTypes" EventName="selectedindexchanged"/>
            </Triggers>
            <ContentTemplate>
                <table cellspacing="0" cellpadding="4" border="0" style="width: 1000px; border-collapse: collapse;">
                    <tr>
                        <td style="width:120px; font-weight: bold;">
                            <asp:Label ID="lblMailFormatDescription" runat="server" Text="<%$ Resources:Resource, Admin_MailFormat_Description %>" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescription" runat="server" Width="800px" Height="54px" ReadOnly="true" TextMode="MultiLine" />
                        </td>
                    </tr>       
                    <tr>
                        <td style="width:120px; font-weight: bold;">
                            <asp:Label ID="lblMailFormatType" runat="server" Text="<%$ Resources:Resource, Admin_MailFormat_Type %>" />
                        </td>
                        <td>        
                            <asp:DropDownList ID="ddlTypes" runat="server" DataSourceID="SqlDataSource1" CausesValidation="false" AutoPostBack="true"
                                DataTextField="TypeName" DataValueField="MailFormatTypeID" onselectedindexchanged="ddlTypes_SelectedIndexChanged" />
                        </td>
                    </tr>      
                </table>           
            </ContentTemplate>
        </asp:UpdatePanel>    
        <table cellspacing="0" cellpadding="4" border="0" style="width: 1000px; border-collapse: collapse;">   
            <tr>
                <td style="width:120px; font-weight: bold;">
                    <asp:Label ID="lblMailFormatActive" runat="server" Text="<%$ Resources:Resource, Admin_MailFormat_Active %>" />
                </td>
                <td>
                    <asp:CheckBox ID="chkActive" runat="server" Checked="true"/>
                </td>
            </tr>
            <tr>
                <td style="width:120px; font-weight: bold;">
                    <asp:Label ID="lblMailFormatSort" runat="server" Text="<%$ Resources:Resource, Admin_MailFormat_Sort %>" />
                </td>
                <td>
                    <asp:TextBox ID="txtSortOrder" runat="server" Width="250px" />
                </td>
            </tr>
            <tr style="background-color: White;">
                <td colspan="2">
                    <asp:Button CssClass="btn btn-middle btn-add" CausesValidation="false" ID="btnSave" runat="server" 
                        Text='<%$ Resources:Resource, Admin_Insert %>' onclick="btnSave_Click" />
                </td>
            </tr>
        </table>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" SelectCommand="SELECT [MailFormatTypeID], [TypeName] FROM [Settings].[MailFormatType] ORDER BY [SortOrder]"
            OnInit="SqlDataSource1_Init"></asp:SqlDataSource>
    </div>
</asp:Content>
