<%@ Page Language="C#" MasterPageFile="MasterPage.master" CodeFile="SendRequestOnProduct.aspx.cs"
    Inherits="ClientPages.SendRequestOnProduct" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Register Src="UserControls/Captcha.ascx" TagName="CaptchaControl" TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%=Resources.Resource.Client_SendRequestOnProduct_Header%></h1>
            <div class="form-top">
                <adv:StaticBlock runat="server" SourceKey="requestOnProduct" />
            </div>
            <ul id="ulValidationFailed" runat="server" visible="false" class="ulValidFaild">
            </ul>
            <div class="form-c">
                <asp:MultiView ID="MultiView1" runat="server">
                    <asp:View ID="ViewForm" runat="server">
                        <ul class="form form-vr">
                            <li>
                                <div class="param-name">
                                    <label for="spanName">
                                        <%=Resources.Resource.Client_SendRequestOnProduct_ProductName%>:</label>
                                    <span id="spanName" class="bold">
                                        <%=offer.Product.Name%></span></div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtAmount">
                                        <%=Resources.Resource.Client_SendRequestOnProduct_Amount%>:</label></div>
                                <div class="param-value">
                                    <adv:AdvTextBox ValidationType="Money" ID="txtAmount" runat="server" Text="1" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtName">
                                        <%=Resources.Resource.Client_SendRequestOnProduct_Name%>:</label></div>
                                <div class="param-value">
                                    <adv:AdvTextBox ValidationType="Required" ID="txtName" runat="server" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtEmail">
                                        E-mail:</label></div>
                                <div class="param-value">
                                    <adv:AdvTextBox ValidationType="Email" ID="txtEmail" runat="server" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtPhone">
                                        <%=Resources.Resource.Client_SendRequestOnProduct_Phone%>:</label></div>
                                <div class="param-value">
                                    <adv:AdvTextBox ID="txtPhone" runat="server" ValidationType="Required" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtComment">
                                        <%=Resources.Resource.Client_SendRequestOnProduct_Comment%>:</label></div>
                                <div class="param-value-textarea  param-value">
                                    <adv:AdvTextBox ID="txtComment" TextMode="MultiLine" runat="server"></adv:AdvTextBox>
                                </div>
                            </li>
                            <li runat="server" id="liCaptcha">
                                <div class="param-name">
                                    <label>
                                        <%=Resources.Resource.Client_Details_Code%>:
                                    </label>
                                </div>
                                <div class="param-value">
                                    <adv:CaptchaControl ID="CaptchaControl1" runat="server" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                </div>
                                <div class="param-value">
                                    <adv:Button ID="btnSend" Type="Submit" Size="Middle" runat="server" Text="<%$ Resources:Resource, Client_SendRequestOnProduct_Send%>"
                                        OnClick="btnSend_Click"></adv:Button>
                                </div>
                            </li>
                        </ul>
                    </asp:View>
                    <asp:View ID="ViewResult" runat="server">
                        <div class="sendrequestomproduct-success">
                            <asp:Literal ID="lblMessage" runat="server"></asp:Literal>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
            <div class="form-addon">
                <div class="form-center">
                    <%=RenderProductPhoto()%>
                    <div>
                        <a href="<%= UrlService.GetLink(ParamType.Product, offer.Product.UrlPath, offer.Product.ProductId) %>">
                            <%= offer.Product.Name%></a>
                    </div>
                    <div class="sku">
                        <%= offer.ArtNo%>
                    </div>
                    <div class="sku">
                        <%= offer.Color != null ? AdvantShop.Configuration.SettingsCatalog.ColorsHeader + ": " + offer.Color.ColorName : string.Empty%>
                    </div>
                    <div class="sku">
                        <%= offer.Size != null ? AdvantShop.Configuration.SettingsCatalog.SizesHeader + ": " + offer.Size.SizeName : string.Empty%></div>
                    <div>
                        <%= CatalogService.RenderPrice(offer.Price, offer.Product.CalculableDiscount, true, AdvantShop.Customers.CustomerContext.CurrentCustomer.CustomerGroup, Options)%>
                    </div>
                    <div>
                        <asp:Literal runat="server" ID="lOptions"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
