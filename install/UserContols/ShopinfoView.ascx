<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShopinfoView.ascx.cs"
    Inherits="ClientPages.install_UserContols_ShopinfoView" %>
<h1>
    <% =  Resources.Resource .Install_UserContols_ShopinfoView_h1  %></h1>
<asp:Label runat="server" ID="lblError" ForeColor="Red" style="margin-bottom:10px;display:block"></asp:Label>
<div class="group" runat="server" id="divKey">
    <p>
        <%= Resources.Resource.Install_UserContols_ShopinfoView_Key%></p>
    <div class="str">
        <asp:TextBox runat="server" CssClass="txt valid-required valid-compare-source" ID="txtKey"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_ShopName%></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtShopName"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_UrlShop%></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtUrl"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_Logo%></p>
    <div class="container-logo">
        <asp:Image runat="server" ID="imgLogo" />
    </div>
    <div class="str">
        <asp:FileUpload runat="server" ID="fuLogo" CssClass="file-upload" />
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_Favicon%></p>
    <div class="container-logo">
        <asp:Image runat="server" ID="imgFavicon" />
    </div>
    <div class="str">
        <asp:FileUpload runat="server" ID="fuFavicon" CssClass="file-upload" />
    </div>
</div>
<%--<div class="group">
    <p>
        <%= Resources.Resource.Install_UserContols_ShopinfoView_Title%></p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtTitle"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <%= Resources.Resource.Install_UserContols_ShopinfoView_Metadescription%></p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtMetadescription"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_Metakeywords%></p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtMetakeywords"></asp:TextBox>
        <p>
            <%= Resources.Resource.Install_UserContols_ShopinfoView_Remark%></p>
    </div>
</div>--%>
<div class="group">
    <p>
        <% = Resources.Resource. Install_UserContols_ShopinfoView_Country%></p>
    <div class="str">
        <asp:SqlDataSource SelectCommand="SELECT CountryID, CountryName FROM [Customers].[Country]"
            ID="sdsCountry" runat="server" OnInit="sds_Init"></asp:SqlDataSource>
        <asp:DropDownList runat="server" CssClass="textBoxClass" DataSourceID="sdsCountry"
            ID="ddlCountry" DataTextField="CountryName" Width="310px" DataValueField="CountryID">
        </asp:DropDownList>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_Region%></p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtRegion"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_City%></p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtCity"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_Phone%>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPhone"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_ShopinfoView_Director%>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtDirector"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <%= Resources.Resource.Install_UserContols_ShopinfoView_Accountant%>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtAccountant"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <%= Resources.Resource.Install_UserContols_ShopinfoView_Manager%>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt" runat="server" ID="txtManager"></asp:TextBox>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=txtCity.ClientID%>").autocomplete('<%=AdvantShop.Core.UrlRewriter.UrlService.GetAbsoluteLink("/HttpHandlers/GetCities.ashx") %>', {
            delay: 10,
            minChars: 1,
            matchSubset: 1,
            autoFill: true,
            matchContains: 1,
            cacheLength: 10,
            selectFirst: true,
            maxItemsToShow: 10
        });

        $("#<%=txtRegion.ClientID%>").autocomplete('<%=AdvantShop.Core.UrlRewriter.UrlService.GetAbsoluteLink("/HttpHandlers/GetRegions.ashx") %>', {
            delay: 10,
            minChars: 1,
            matchSubset: 1,
            autoFill: true,
            matchContains: 1,
            cacheLength: 10,
            selectFirst: true,
            maxItemsToShow: 10
        });
    });
</script>
