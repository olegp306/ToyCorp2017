<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindCustomers.ascx.cs"
    Inherits="Admin.UserControls.FindCustomers" %>
<asp:UpdatePanel ID="upCustomers" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
    <ContentTemplate>
        <div>
            <div class="rightPanelHeader">
                <div style="width: 30px; float: left">
                    <img src="images/folder.gif" />
                </div>
                <div style="width: 164px; float: left">
                    <asp:Label ID="lblBigHead" runat="server" Text="<%$ Resources:Resource, Admin_ViewCustomer_Header %>"
                        Font-Bold="true" />
                </div>
            </div>
            <div style="clear: both">
                <center>
                    <table border="0" cellpadding="0" cellspacing="0" class="customersearch">
                        <tr>
                            <td class="firsttd">
                                <asp:Literal ID="Literal1" runat="server" Text="Email" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtSEmail" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="firsttd">
                                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Resource, Admin_ViewCustomer_Name %>" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtSFirstName" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="firsttd">
                                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Resource, Admin_ViewCustomer_LastName %>" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtSLastName" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <asp:Button ID="btnClear" runat="server" Text="<%$ Resources:Resource, Admin_ViewCustomer_Clear %>"
                                    CssClass="customerfind" OnClick="btnClear_Click" />
                                <asp:Button ID="btnFindCustomer" runat="server" Text="<%$ Resources:Resource, Admin_ViewCustomer_Find %>"
                                    CssClass="customerfind" OnClick="btnFindCustomer_Click" />
                            </td>
                        </tr>
                    </table>
                    </center>
                    <div style="width: 100%; height: 1px; border-top: 1px solid #B8B8B8">
                    </div>
                    <div class="admin_product_categoryListBlock">
                        <asp:HiddenField ID="currentPage" runat="server" Value="1" />
                        <asp:Repeater runat="server" ID="rCustomers" OnItemCommand="rCustomers_ItemCommand">
                            <HeaderTemplate>
                                <table style="width: 100%">
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td style="width: 14px;">
                                        <img src="images/blank.gif" />
                                    </td>
                                    <td>
                                        <asp:HyperLink ID="LinkButton2" CssClass="blueLink" runat="server" Text='<%# Eval("FirstName") + " " + Eval("LastName")%>'
                                            NavigateUrl='<%# "~/admin/ViewCustomer.aspx?CustomerID=" + Eval("ID")%>'
                                            Font-Bold='<%# Eval("ID").ToString()  == Request["CustomerID"] %>' />
                                    </td>
                                    <td style="width: 14px;">
                                        <asp:LinkButton runat="server" ID="ibDelete" CssClass="showtooltip valid-confirm"
                                            CommandName="DeleteCustomer" CommandArgument='<%# Eval("ID")%>' Visible='<%# (string)Eval("EMail") != "admin" %>'
                                            data-confirm="<%$ Resources:Resource, Admin_ViewCustomer_DeleteCustomerConfirmation %>" 
                                            ToolTip="<%$Resources:Resource, Admin_ViewCustomer_DeleteCustomer%>">
                                            <img src="images/gcross.gif" alt="" />
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody> </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                    <%if (_paging.PageCount > 1)
                      { %>
                      <center>
                    <table id="pager" style="background-color: #EFF0F1; text-align: center;">
                        <tr>
                            <td>
                                <asp:LinkButton ID="lbPreviousPage" CssClass="Link" runat="server" Enabled="false"
                                    EnableViewState="false" OnClientClick="removeunloadhandler()" Text="<%$Resources:Resource, Admin_Product_Prev%>"
                                    OnClick="lbPreviousPage_Click"> </asp:LinkButton>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlCurrentPage" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCurrentPage_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbNextPage" CssClass="Link" runat="server" Enabled="false" EnableViewState="false"
                                    OnClientClick="removeunloadhandler()" Text="<%$Resources:Resource, Admin_Product_Next %>"
                                    OnClick="lbNextPage_Click">
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                    </center>
                    <%}%>
                
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
