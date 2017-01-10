<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="m_MasterPage.master"
    CodeFile="m_Brand.aspx.cs" Inherits="Admin.m_Brand" ValidateRequest="false" %>

<asp:Content ID="contentCenter" runat="server" ContentPlaceHolderID="cphCenter">
    <div>
        <center>
            <asp:Label ID="lblCustomer" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_Header %>"></asp:Label>
            <br />
            <asp:Label ID="lblCustomerName" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_SubHeader %>"></asp:Label>
        </center>
        <asp:Panel ID="pnlAdd" runat="server" Height="84px" Width="100%">
            <center>
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False"></asp:Label>&nbsp;</center>
            <table border="0" cellpadding="2" cellspacing="0" width="100%">
                <tr>
                    <td style="width: 45%; height: 27px; text-align: right; vertical-align: middle;">
                        <asp:Label ID="Label5" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_Name%>"></asp:Label>: 
                    </td>
                    <td style="height: 27px;">
                        <asp:TextBox ID="txtName" runat="server" Width="300px"></asp:TextBox>
                        <asp:Label ID="Label12" runat="server" Font-Italic="True" ForeColor="DarkGray" Text="<%$ Resources:Resource, Admin_m_Brand_Required %>"></asp:Label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtName"
                            ErrorMessage="<%$ Resources:Resource, Admin_FillChar %>"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr style="background-color: #eff0f1;">
                    <td style="width: 45%; height: 12px; text-align: right">
                        <asp:Label ID="lblStringID" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_Url%>"></asp:Label>&nbsp;
                    </td>
                    <td style="vertical-align: middle; height: 12px">
                        <asp:TextBox ID="txtURL" runat="server" Width="224px"></asp:TextBox>
                        <asp:Label ID="Label6" runat="server" Font-Italic="True" ForeColor="DarkGray" Text="<%$ Resources:Resource, Admin_m_Brand_Required %>"></asp:Label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtURL"
                            ErrorMessage="<%$ Resources:Resource, Admin_FillChar %>"></asp:RequiredFieldValidator>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td style="width: 45%; height: 27px; text-align: right; vertical-align: middle;">
                        <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_Country %>"></asp:Label>&nbsp;
                    </td>
                    <td style="height: 27px;">
                        <asp:DropDownList runat="server" ID="ddlCountry" Width="300px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr style="background-color: #eff0f1;">
                    <td style="width: 45%; text-align: right; height: 29px;">
                        <asp:Label ID="Label7" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_Logo %>"></asp:Label>&nbsp;
                    </td>
                    <td>
                        <asp:Panel ID="pnlLogo" runat="server" Width="100%">
                            &nbsp;<asp:Label ID="Label11" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_CurentLogo %>"></asp:Label>
                            <br />
                            <asp:Image ID="imgLogo" runat="server" />
                            <br />
                            <asp:Button ID="btnDeleteImage" runat="server" Text="<%$  Resources:Resource, Admin_Delete%>"
                                OnClick="btnDeleteLogo_Click" />
                            <br />
                        </asp:Panel>
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="308px" Height="20px" />
                        <asp:Label ID="lblLogo" runat="server" Text="Label" Visible="False"></asp:Label>
                        <br />
                        <asp:Label ID="lblImageInfo" runat="server" Font-Bold="False" Font-Size="Smaller"
                            ForeColor="Gray"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 45%; height: 37px; text-align: right">
                        <asp:Label ID="Label19" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_Enabled%>"></asp:Label>&nbsp;
                    </td>
                    <td style="height: 37px">
                        <asp:CheckBox ID="chkEnabled" runat="server" Checked="True" />
                    </td>
                </tr>
                <tr style="background-color: #eff0f1;">
                    <td style="width: 45%; height: 37px; text-align: right">
                        <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_SortOrder%>"></asp:Label>&nbsp;
                    </td>
                    <td style="height: 37px">
                        <asp:TextBox ID="txtSortOrder" runat="server" Text="0" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 49%; height: 29px; text-align: right">
                        <asp:Label ID="Label4" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_BrandSiteUrl%>"></asp:Label>&nbsp;
                    </td>
                    <td style="vertical-align: middle; height: 29px;">
                        <asp:TextBox ID="txtBrandSiteUrl" runat="server" Text="0" />
                    </td>
                </tr>
            </table>
            <table border="0" cellpadding="2" cellspacing="0" width="100%">
                <tr style="background-color: #eff0f1;">
                    <td style="width: 49%; height: 26px; text-align: right; vertical-align: middle;">
                        <asp:Localize ID="Localize_Admin_m_Product_HeadTitle" runat="server" Text=""></asp:Localize>
                        <br />
                        <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: Resource, Admin_m_Brand_UseGlobalVariables %>"></asp:Literal>
                    </td>
                    <td style="vertical-align: middle; height: 26px;">
                        <asp:TextBox ID="txtHeadTitle" runat="server" Width="354"/>
                    </td>
                </tr>
                <tr>
                    <td style="width: 49%; height: 26px; text-align: right; vertical-align: middle;">
                        H1
                        <br />
                        <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources: Resource, Admin_m_Brand_UseGlobalVariables %>"></asp:Literal>
                    </td>
                    <td style="vertical-align: middle; height: 26px;">
                        <asp:TextBox ID="txtH1" runat="server" Width="354"/>
                    </td>
                </tr>
                <tr style="background-color: #eff0f1;">
                    <td style="width: 49%; height: 29px; text-align: right">
                        <asp:Localize ID="Localize_Admin_m_Product_MetaKeywords" runat="server" Text="<%$ Resources: Resource, Admin_m_Brand_MetaKeywords %>"></asp:Localize>
                        <br />
                        <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, Admin_m_Brand_UseGlobalVariables %>"></asp:Literal>
                    </td>
                    <td style="vertical-align: middle; height: 29px;">
                        <asp:TextBox ID="txtMetaKeys" runat="server" TextMode="MultiLine" Width="354"></asp:TextBox>
                    </td>
                </tr>
                <tr style="background-color: #eff0f1;">
                    <td style="width: 49%; height: 26px; text-align: right; vertical-align: middle;">
                        <asp:Localize ID="Localize_Admin_m_Product_MetaDescription" runat="server" Text="<%$ Resources: Resource, Admin_m_Brand_MetaDescription %>"></asp:Localize>
                        <br />
                        <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources: Resource, Admin_m_Brand_UseGlobalVariables %>"></asp:Literal>
                    </td>
                    <td style="vertical-align: middle; height: 26px;">
                        <asp:TextBox ID="txtMetaDescription" runat="server" TextMode="MultiLine" Width="354"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hfMetaId" runat="server" />
            <!-- editor -->
            <table border="0" id="table1" style="width: 100%; height: 206px;" cellspacing="0"
                cellpadding="0">
                <tr>
                    <td align="center">
                        <br />
                        <asp:Panel ID="Panel4" runat="server" HorizontalAlign="Left" Width="700px" CssClass="all-mar">
                            <asp:Label ID="lblText" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_Description %>"></asp:Label>
                            <CKEditor:CKEditorControl ID="FCKDescription" BasePath="~/ckeditor/" SkinPath="skins/silver/"
                                ToolbarSet="Test" runat="server" Height="500px" Width="700px" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <table border="0" id="table2" style="width: 100%; height: 206px;" cellspacing="0"
                cellpadding="0">
                <tr>
                    <td align="center">
                        <br />
                        <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Left" Width="700px" CssClass="all-mar">
                            <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Resource, Admin_m_Brand_BriefDescription %>"></asp:Label>
                            <CKEditor:CKEditorControl ID="FCKBriefDescription" BasePath="~/ckeditor/" SkinPath="skins/silver/"
                                ToolbarSet="Test" runat="server" Height="500px" Width="700px" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <!-- editor -->
            <br />
            <center>
                <asp:Button ID="btnOK" runat="server" Text="<%$ Resources:Resource, Admin_m_News_Ok %>"
                    Width="110px" OnClick="btnOK_Click" />&nbsp;</center>
            <br />
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="contentScript" runat="server" ContentPlaceHolderID="cphScript">
    <script type="text/javascript">
        function fillUrl() {
            var text = $('#<%=txtName.ClientID %>').val();
            var url = $('#<%=txtURL.ClientID %>').val();
            if ((text != "") && (url == "")) {
                $('#<%=txtURL.ClientID %>').val(translite(text));
            }
        }
        $('#<%=txtURL.ClientID %>').focus(fillUrl);
    </script>
</asp:Content>
