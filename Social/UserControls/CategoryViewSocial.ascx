<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategoryViewSocial.ascx.cs"
    Inherits="Social.UserControls.CategoryView" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<asp:ListView runat="server" ID="lvCategory" GroupItemCount="4">
    <LayoutTemplate>
        <table class="categories">
            <asp:PlaceHolder ID="groupPlaceholder" runat="server" />
        </table>
    </LayoutTemplate>
    <GroupTemplate>
        <tr>
            <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
        </tr>
    </GroupTemplate>
    <GroupSeparatorTemplate>
        <tr class="cat-row-split">
            <td class="cat-split" colspan="7">
            </td>
        </tr>
    </GroupSeparatorTemplate>
    <ItemTemplate>
        <td>
            <%# RenderCategoryImage(Eval("MiniPicture.PhotoName").ToString(), SQLDataHelper.GetInt(Eval("CategoryID")), Eval("UrlPath").ToString(), Eval("Name").ToString())%>
            <div class="cat-name">
                <a href="<%# "social/catalogsocial.aspx?categoryid=" + Eval("CategoryId") %>">
                    <%#Eval("Name") %></a> <span class="cat-count">
                        <%# Eval("ProductsCount")%></span></div>
        </td>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <td class="cat-split">
        </td>
    </ItemSeparatorTemplate>
    <EmptyItemTemplate>
        <td class="cat-empty">
        </td>
    </EmptyItemTemplate>
</asp:ListView>
