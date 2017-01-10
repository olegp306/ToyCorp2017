<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderItems.ascx.cs" Inherits="Admin.UserControls.Order.OrderItemsControl" %>
<%@ Register Src="~/Admin/UserControls/PopupTreeView.ascx" TagName="PopupTree" TagPrefix="adv" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<adv:PopupTree runat="server" ID="pTreeProduct" HeaderText="<%$ Resources:Resource, Admin_CatalogLinks_ParentCategory %>"
    Type="CategoryProduct" ExceptId="0" OnTreeNodeSelected="pTreeProduct_NodeSelected"
    OnHiding="pTreeProduct_Hiding" />
<asp:Label runat="server" ID="lblError" Visible="false"></asp:Label>
<table style="width: 100%">
    <tr>
        <td>
            <asp:SqlDataSource runat="server" ID="sdsCurrs" OnInit="sds_Init" SelectCommand="SELECT * FROM [Catalog].[Currency]" />
            <span>
                <asp:Localize runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_ChoosingCurrency%>" />: 
            </span>
            <asp:DropDownList ID="ddlCurrs" runat="server" DataSourceID="sdsCurrs" DataTextField="Name"
                DataValueField="CurrencyIso3" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCurrs_SelectedChanged" Visible="false">
            </asp:DropDownList>
            <asp:Label runat="server" ID="lcurrency" />
            <asp:HiddenField runat="server" ID="hfOldCurrencyValue" />
        </td>
        <td>
            <asp:Label runat="server" ID="lDiscount" Text="<%$ Resources: Resource, Admin_EditOrder_Discount%>"></asp:Label>:
            <asp:TextBox runat="server" ID="txtDiscount" Width="30px" />
            %
        </td>
        <td>
            <div style="float: right;">
                <div style="display: inline-block; vertical-align: top; padding: 3px 0 0 0;">
                    <asp:TextBox runat="server" ID="txtArtNo" CssClass="autocompleteSearch" Width="230px" />
                    <input type="hidden" id="hfOffer" runat="server" class="acsearchhf" />
                </div>
                <div style="display: inline-block;">
                    <asp:Button CssClass="btn btn-middle btn-add" CausesValidation="false" ID="btnAddProductByArtNo" runat="server"
                        OnClick="btnAddProductByArtNo_Click" Text="<%$ Resources: Resource, Admin_OrderSearch_AddProductByArtNoOrName%>" />
                </div>
            </div>
        </td>
        <td>
            <div style="float: right;">
                <asp:Button CssClass="btn btn-middle btn-add" CausesValidation="false" ID="btnAddProduct" runat="server" OnClick="btnAddProduct_Click" Text="<%$ Resources: Resource, Admin_OrderSearch_AddProduct%>" />

            </div>
        </td>
    </tr>
    <tr class="formheaderfooter">
        <td colspan="2"></td>
    </tr>
</table>
<div style="text-align: center;">
    <asp:Repeater ID="DataListOrderItems" runat="server" OnItemCommand="dlItems_ItemCommand"
        OnItemDataBound="dlItems_ItemDataBound">
        <HeaderTemplate>
            <table width="100%" border="0" cellspacing="0" cellpadding="3" class="grid-main">
                <tr class="header">
                    <td>&nbsp;
                    </td>
                    <td>
                        <b>
                            <asp:Label ID="Label4" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ArtNo %>"></asp:Label></b>
                    </td>
                    <td>
                        <b>
                            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemName %>"></asp:Label></b>
                    </td>
                    <td>
                        <b>
                            <asp:Label ID="Label14" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_CustomOptions %>"></asp:Label></b>
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <b>
                            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Size %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <b>
                            <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Color %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 150px; text-align: center;">
                        <b>
                            <asp:Label ID="Label13" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Price %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <b>
                            <asp:Label ID="Label6" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemAmount %>"></asp:Label>
                        </b>
                    </td>
                    <% if (AdvantShop.Configuration.SettingsOrderConfirmation.AmountLimitation)
                       {%>
                    <td class="OrderTableHead" style="width: 100px;">
                        <asp:Localize ID="Localize_Client_ShoppingCart_Available" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_AvailableAmount %>"></asp:Localize>
                    </td>
                    <%
                       }%>
                    <td style="width: 150px; text-align: center;">
                        <b>
                            <asp:Label ID="Label7" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemCost %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 50px"></td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="row1" style="height: 35px;">
                <td>
                    <%# Eval("ProductID") != null ? RenderPicture((int)Eval("ProductID"), SQLDataHelper.GetNullableInt(Eval("PhotoID"))) : ""%>
                </td>
                <td>
                    <asp:Literal runat="server" ID="ltArtNo" Text='<%#Eval("ArtNo")%>'></asp:Literal>
                </td>
                <td>
                    <%# Eval("Name")%>
                </td>
                <td>
                    <%#RenderSelectedOptions((IList<EvaluatedCustomOptions>)Eval("SelectedOptions"))%>
                </td>
                <td>
                    <%# Eval("Size")%>
                </td>
                <td>
                    <%# Eval("Color")%>
                </td>
                <td style="text-align: center;">
                    <%--<%#  CatalogService.GetStringPrice((float)Eval("Price"), 1, CurrencyCode, CurrencyValue)%>--%>
                    <asp:TextBox runat="server" ID="txtPrice" Text='<%#Eval("Price") %>' Width="100px" />
                </td>
                <td style="text-align: center;">
                    <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Eval("Amount") %>' Width="25" />
                </td>
                <% if (AdvantShop.Configuration.SettingsOrderConfirmation.AmountLimitation)
                   {%>
                <td class="OrderTable_td" style="width: 100px;">
                    <asp:Label ID="lbMaxCount" runat="server" Text="Label" ForeColor="Red" CssClass="lbMaxCount" Font-Bold="true" />
                </td>
                <%
                   }%>
                <td style="text-align: center;">
                    <%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Amount")), CurrencyCode, CurrencyValue)%>
                </td>
                <td>
                    <div style="display: inline-block">
                        <asp:ImageButton CausesValidation="false" ID="btnQuantUp" ImageUrl="~/Admin/images/refresh.png"
                            runat="server" CommandArgument='<%# Eval("OrderItemID") %>' CommandName="SaveQuantity" />
                    </div>
                    <div style="display: inline-block">
                        <asp:LinkButton ID="buttonDelete" CssClass="deletebtn showtooltip valid-confirm" 
                            CommandName="Delete" CommandArgument='<%# Eval("OrderItemID") %>'
                            runat="server" Enabled="True" CausesValidation="false" ToolTip='<%$ Resources:Resource, Admin_OrderSearch_Delete%>' />
                    </div>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
