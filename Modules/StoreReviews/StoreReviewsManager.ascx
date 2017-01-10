<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StoreReviewsManager.ascx.cs"
    Inherits="Advantshop.UserControls.Modules.StoreReviews.Admin_StoreReviewsManager" %>
<%@ Import Namespace="System.Globalization" %>
<style>
    .reviewsTable
    {
        width: 100%;
        border-collapse: collapse;
    }

        .reviewsTable td, .reviewsTable th
        {
            text-align: left;
            border-bottom: 1px solid #000000;
            height: 30px;
        }
</style>
<div>
    <table border="0" cellpadding="2" cellspacing="0" style="width: 100%;">
        <tr class="rowsPost">
            <td colspan="2" style="height: 34px;">
                <span class="spanSettCategory">
                    <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: StoreReviews_ManagerHeader%>" /></span>
                <asp:Label ID="lblMessage" runat="server" Visible="False" Style="float: right;"></asp:Label>
                <hr color="#C2C2C4" size="1px" />
            </td>
        </tr>
    </table>
    <asp:ListView ID="lvReviews" runat="server" ItemPlaceholderID="itemPlaceHolder" OnItemCommand="lvReviewsItemCommand">
        <LayoutTemplate>
            <table class="reviewsTable">
                <tr>
                    <th>
                        <asp:Label ID="lblReview" runat="server" Text='<%$ Resources: StoreReviews_Review%>'></asp:Label>
                    </th>
                    <th style="width: 200px;">
                        <asp:Label ID="lblDate" runat="server" Text='<%$ Resources: StoreReviews_DateAdded%>'></asp:Label>
                    </th>
                    <th style="width: 250px;">
                        <asp:Label ID="Label1" runat="server" Text='<%$ Resources: StoreReviews_ReviewerName%>'></asp:Label>
                    </th>
                    <th style="width: 250px;">
                        <asp:Label ID="lblEmail" runat="server" Text='<%$ Resources: StoreReviews_ReviewerEmail%>'></asp:Label>
                    </th>
                    <th style="width: 100px;">
                        <asp:Label ID="lblModerated" runat="server" Text='<%$ Resources: StoreReviews_Moderated%>'></asp:Label>
                    </th>
                    <th style="width: 80px;">
                        <asp:Label ID="lblRate" runat="server" Text='<%$ Resources: StoreReviews_Rate%>'></asp:Label>
                    </th>
                    <th style="width: 90px;"></th>
                </tr>
                <tr runat="server" id="itemPlaceHolder">
                </tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <%# Eval("Review")%>
                </td>
                <td>
                    <%#Eval("DateAdded")%>
                </td>
                <td>
                    <%#Eval("ReviewerName")%>
                </td>
                <td>
                    <%#Eval("ReviewerEmail")%>
                </td>

                <td>
                    <asp:CheckBox ID="ckbModerated" runat="server" Checked='<%# Eval("Moderated")%>'
                        Enabled="False" />
                </td>
                <td>
                    <%#Eval("Rate")%>
                </td>
                <td>
                    <a href='<%# "javascript:open_window(\"../modules/StoreReviews/editreview.aspx?id=" + Eval("Id") +"\",700,600)"%>'>
                        <asp:Label runat="server" Text='<%$ Resources:StoreReviews_Edit%>'></asp:Label></a>
                    <asp:LinkButton ID="btnDelete" runat="server" Text="<%$ Resources: StoreReviews_Delete%>" OnClientClick="return confirm('Действительно хотите удалить?');"
                        CommandName="deleteReview" CommandArgument='<%#Eval("Id") %>' />
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</div>
