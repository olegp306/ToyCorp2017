<%@ Page Language="C#" MasterPageFile="MasterPage.master" CodeFile="ShoppingCart.aspx.cs"
    Inherits="ClientPages.ShoppingCart_Page" %>

<%@ Register TagPrefix="adv" TagName="BuyInOneClick" Src="~/UserControls/BuyInOneClick.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <div class="full-cart-wrap">
                <h1 class="mainContantTitle">
                    <%= Resources.Resource.Client_ShoppingCart_ShoppingCart %></h1>
                <div class="cartSection">
                    <asp:Panel ID="pnlTopContent" runat="server">
                    </asp:Panel>
                    <asp:Literal ID="ltrlTopContent" runat="server"></asp:Literal>
                    <div id="cartWrapper" class="cart-wrapper">
                        <div id="dvOrderMerged" runat="server" visible="false" class="ShoppingCart_MergedOrder">
                            <asp:Localize ID="Localize_Client_ShoppingCart_ProductsInBasket" runat="server" Text="<%$ Resources:Resource, Client_ShoppingCart_ProductsInBasket %>"></asp:Localize>
                        </div>
                        <div data-plugin="cart">
                        </div>
                    </div>

                    <adv:StaticBlock runat="server" SourceKey="shoppingcart" />
                    <asp:Panel ID="pnlBottomContent" runat="server">
                    </asp:Panel>
                    <asp:Literal ID="ltrlBottomContent" runat="server"></asp:Literal>
                    <asp:Label ID="lDemoWarning" runat="server" CssClass="warn" Text="<%$ Resources:Resource, Client_ShoppingCart_FakeShop %>" />
                </div>
            </div>
        </div>
    </div>
    </div>
    <div class="orderingCartbuttonWrapper">
    <div class="btn-cart-confirm">
                        <adv:BuyInOneClick ID="BuyInOneClick" runat="server" />
                        <adv:Button ID="aCheckOut" runat="server" Type="Confirm" Size="Big" Text="<%$ Resources:Resource, Client_ShoppingCart_DrawUp %>"
                            DisableValidation="True" OnClick="btnConfirmOrder_Click" CssClass="greenButton"/>
    </div>
		</div>

    <asp:Label ID="lblEmpty" runat="server" Text="" Visible="False" />
</asp:Content>
