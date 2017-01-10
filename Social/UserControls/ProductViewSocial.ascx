<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductViewSocial.ascx.cs"
    Inherits="Social.UserControls.ProductView" EnableViewState="false" %>
<%@ Register Src="~/UserControls/Rating.ascx" TagName="Rating" TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="SizeColorPickerCatalog" Src="~/UserControls/Catalog/SizeColorPickerCatalog.ascx" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<asp:MultiView runat="server" ID="mvProducts">
    <Views>
        <asp:View runat="server" ID="viewTile">
            <asp:ListView runat="server" ID="lvTile" ItemPlaceholderID="tilePlaceHolder">
                <LayoutTemplate>
                    <div class="pv-tile scp">
                        <div runat="server" id="tilePlaceHolder">
                        </div>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="pv-item scp-item" data-productid="<%# Eval("ProductId") %>" style='width: <%= ImageWidth%>px;'>
                        <table class="p-table">
                            <tr>
                                <td class="img-middle" style='height: <%= ImageHeightSmall%>px;'>
                                    <figure>
                                        <div class="pv-photo">
                                            <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("Name")), "social/detailssocial.aspx?productId=" + Eval("ProductId"))%>
                                        </div>
                                        <%# CatalogService.RenderLabels(Convert.ToBoolean(Eval("Recomended")), Convert.ToBoolean(Eval("OnSale")), Convert.ToBoolean(Eval("Bestseller")), Convert.ToBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                    </figure>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="pv-div-link">
                                        <a href="<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>" class="link-pv-name">
                                            <%# Eval("Name") %></a>
                                    </div>
                                    <%# SQLDataHelper.GetString(Eval("Name")).Length < 20 ? string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) : string.Empty %>
                                    <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>'
                                        Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>'
                                        ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%#ColorImageWidth %>"  />

                                    <adv:Rating ID="rating1" runat="server" ProductId='<%# Convert.ToInt32(Eval("ProductID")) %>'
                                        ShowRating='<%# EnableRating %>' Rating='<%# Convert.ToDouble(Eval("Ratio")) %>'
                                        ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                    <div class="price-container">
                                        <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")), SQLDataHelper.GetFloat(Eval("MultiPrices")))%>
                                    </div>
                                    <div class="pv-btns">
                                        <adv:Button ID="btnAdd" CssSpan="btn-add-icon" CssClass="no-pie" runat="server" Type="Add" Size="Middle"
                                            Text='<%$ Resources:Resource, Client_Catalog_Add %>' Href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>'
                                            Visible='<%# SQLDataHelper.GetDecimal(Eval("Price")) > 0 && SQLDataHelper.GetDecimal(Eval("Amount")) > 0 %>'
                                            data-cart-add='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' />
                                        <adv:Button ID="btnOrderByRequest" runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_Catalog_OrderByRequest %>'
                                            Href='<%# "sendrequestonproduct.aspx?offerID=" + Eval("OfferID") %>' Visible='<%# SQLDataHelper.GetInt(Eval("Amount")) <= 0 && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>'
                                            Target="_blank" />
                                        <adv:Button ID="btnBuy" runat="server" Type="Buy" Size="Middle" Text='<%$ Resources:Resource, Client_More %>'
                                            Href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>' />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ItemTemplate>
                <EmptyItemTemplate>
                    <div class="no-items">
                        <%= Resources.Resource.Client_Catalog_NoItemsFound  %>
                    </div>
                </EmptyItemTemplate>
            </asp:ListView>
        </asp:View>
        <asp:View runat="server" ID="viewList">
            <asp:ListView runat="server" ID="lvList" ItemPlaceholderID="listPlaceholder">
                <LayoutTemplate>
                    <div class="pv-list scp">
                        <div runat="server" id="listPlaceholder">
                        </div>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="pv-item scp-item" data-productid="<%# Eval("ProductId") %>" <%# Convert.ToInt32(Eval("CountPhoto")) >= 3 ? "style=\"min-height:" + 3 * (ImageHeightXSmall + 17) + "px;\"" : "" %>>
                        <div class="pv-photo-c">
                            <figure>
                                <div class="pv-photo">

                                    <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("Name")), "social/detailssocial.aspx?productId=" + Eval("ProductId"))%>
                                </div>
                                <%# CatalogService.RenderLabels(Convert.ToBoolean(Eval("Recomended")), Convert.ToBoolean(Eval("OnSale")), Convert.ToBoolean(Eval("Bestseller")), Convert.ToBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                            </figure>
                        </div>
                        <div class="pv-info">
                            <a href="<%# "social/detailssocial.aspx?productId=" + Eval("ProductId")%>" class="link-pv-name">
                                <%# Eval("Name") %></a>
                            <div class="sku">
                                <%# Eval("ArtNo") %>
                            </div>
                             <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>'
                                        Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' 
                                 ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%#ColorImageWidth %>" />
                            <adv:Rating ID="rating1" runat="server" ProductId='<%# Convert.ToInt32(Eval("ProductID")) %>'
                                ShowRating='<%# EnableRating %>' Rating='<%# Convert.ToDouble(Eval("Ratio")) %>'
                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                            <%# !string.IsNullOrWhiteSpace(Eval("BriefDescription").ToString()) ? "<div class=\"descr\">" + Eval("BriefDescription") + "</div>" : string.Empty %>
                            <div class="price-container">
                                <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")), SQLDataHelper.GetFloat(Eval("MultiPrices")))%>
                            </div>
                            <div class="pv-btns">
                                <adv:Button ID="btnAdd" CssClass="no-pie" CssSpan="btn-add-icon" runat="server" Type="Add" Size="Middle"
                                    Text='<%$ Resources:Resource, Client_Catalog_Add %>' Href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>'
                                    Visible='<%# SQLDataHelper.GetDecimal(Eval("Price")) > 0 && SQLDataHelper.GetDecimal(Eval("Amount")) > 0 %>'
                                    data-cart-add='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' />
                                <adv:Button ID="btnOrderByRequest" runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_Catalog_OrderByRequest %>'
                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("offerID") %>' Visible='<%# SQLDataHelper.GetInt(Eval("Amount")) <= 0 && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>'
                                    Target="_blank" />
                                <adv:Button ID="btnBuy" runat="server" Type="Buy" Size="Middle" Text='<%$ Resources:Resource, Client_More %>'
                                    Href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>' />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="no-items">
                        <%= Resources.Resource.Client_Catalog_NoItemsFound  %>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </asp:View>
        <asp:View runat="server" ID="viewTable">
            <asp:ListView runat="server" ID="lvTable" ItemPlaceholderID="tablePlaceHolder">
                <LayoutTemplate>
                    <table class="pv-table">
                        <tr class="head">
                            <th class="p-name">
                                <asp:Literal runat="server" Text="<%$ Resources:Resource, Client_UserControls_ProductView_Name%>" />
                            </th>
                            <th class="rating"></th>
                            <th class="pv-price">
                                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Resource, Client_UserControls_ProductView_Price%>" />
                            </th>
                            <th class="btns"></th>
                        </tr>
                        <tr runat="server" id="tablePlaceHolder" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr class="pv-item">
                        <td>
                            <a href="<%# "social/detailssocial.aspx?productId=" + Eval("ProductId")%>"
                                class="link-pv-name">
                                <%# Eval("Name") %></a>
                            <div class="sku">
                                <%# Eval("ArtNo") %>
                            </div>
                        </td>

                        <td class="rating">
                             <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>'
                                        Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' 
                                 ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%#ColorImageWidth %>" />
                            <adv:Rating ID="rating1" runat="server" ProductId='<%# Convert.ToInt32(Eval("ProductID")) %>'
                                ShowRating='<%# EnableRating %>' Rating='<%# Convert.ToDouble(Eval("Ratio")) %>'
                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                        </td>
                        <td class="pv-price">
                            <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")), SQLDataHelper.GetFloat(Eval("MultiPrices")))%>
                            <%# CatalogService.RenderLabels(Convert.ToBoolean(Eval("Recomended")), Convert.ToBoolean(Eval("OnSale")), Convert.ToBoolean(Eval("Bestseller")), Convert.ToBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")), 1)%>
                        </td>
                        <td class="btns">
                            <adv:Button ID="btnAdd" CssClass="no-pie" CssSpan="btn-add-icon" runat="server" Type="Add" Size="Middle"
                                Text='<%$ Resources:Resource, Client_Catalog_Add %>' Href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>'
                                Visible='<%# SQLDataHelper.GetDecimal(Eval("Price")) > 0 && SQLDataHelper.GetDecimal(Eval("Amount")) > 0 %>'
                                data-cart-add='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' />
                            <adv:Button ID="btnOrderByRequest" runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_Catalog_OrderByRequest %>'
                                Href='<%# "sendrequestonproduct.aspx?offerID=" + Eval("offerID") %>' Visible='<%# SQLDataHelper.GetInt(Eval("Amount")) <= 0 && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>'
                                Target="_blank" />
                            <adv:Button ID="btnBuy" runat="server" Type="Buy" Size="Middle" Text='<%$ Resources:Resource, Client_More %>'
                                Href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyItemTemplate>
                    <div class="no-items">
                        <%= Resources.Resource.Client_Catalog_NoItemsFound  %>
                    </div>
                </EmptyItemTemplate>
            </asp:ListView>
        </asp:View>
    </Views>
</asp:MultiView>
