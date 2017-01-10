<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SEOSettings.ascx.cs" Inherits="Admin.UserControls.Settings.SEOSettings" %>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_Products%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 240px;">
            <%= Resources.Resource.Admin_m_Default_PageTitle%>
        </td>
        <td>
            <asp:TextBox ID="txtProductsHeadTitle" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #PRODUCT_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            H1
        </td>
        <td>
            <asp:TextBox ID="txtProductsH1" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #PRODUCT_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost rowsPostBig row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaKeywords%>
        </td>
        <td>
            <asp:TextBox ID="txtProductsMetaKeywords" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME# - #PRODUCT_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost rowsPostBig row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaDescription%>
        </td>
        <td>
            <asp:TextBox ID="txtProductsMetaDescription" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME# - #PRODUCT_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost rowsPostBig row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_AdditionalDescription%>
        </td>
        <td>
            <asp:TextBox ID="txtProductsAdditionalDescription" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="" /><br />
            <span style="color: Gray; font-size: 11px; display: block;">
                <%= Resources.Resource.Admin_m_Product_UseGlobalVariables%>
            </span>
        </td>
    </tr>
    <tr class="rowsPost rowsPostBig">
        <td colspan="2">
            <div class="dvSubHelp">
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
                <a href="http://www.advantshop.net/help/pages/settings_of_seo_module" target="_blank">Инструкция. Настройка мета по умолчанию для магазина</a>
            </div>       
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_Categories%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_PageTitle%>
        </td>
        <td>
            <asp:TextBox ID="txtCategoriesHeadTitle" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #CATEGORY_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            H1
        </td>
        <td>
            <asp:TextBox ID="txtCategoriesMetaH1" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #CATEGORY_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaKeywords%>
        </td>
        <td>
            <asp:TextBox ID="txtCategoriesMetaKeywords" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME# - #CATEGORY_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaDescription%>
        </td>
        <td>
            <asp:TextBox ID="txtCategoriesMetaDescription" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME# - #CATEGORY_NAME#" /><br />
            <span style="color: Gray; font-size: 11px; display: block;">
                <%= Resources.Resource.Admin_m_Category_UseGlobalVariables%>
            </span>
        </td>
    </tr>
    <tr class="rowsPost rowsPostBig">
        <td colspan="2">
            <div class="dvSubHelp">
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
                <a href="http://www.advantshop.net/help/pages/settings_of_seo_module" target="_blank">Инструкция. Настройка мета по умолчанию для магазина</a>
            </div>       
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_HeadNewsSEO%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_PageTitle%>
        </td>
        <td>
            <asp:TextBox ID="txtNewsHeadTitle" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #NEWS_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            H1
        </td>
        <td>
            <asp:TextBox ID="txtNewsH1" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #NEWS_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaKeywords%>
        </td>
        <td>
            <asp:TextBox ID="txtNewsMetaKeywords" runat="server" CssClass="niceTextArea textArea2Lines" TextMode="MultiLine" Text="#STORE_NAME# - #NEWS_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaDescription%>
        </td>
        <td>
            <asp:TextBox ID="txtNewsMetaDescription" runat="server" CssClass="niceTextArea textArea2Lines" TextMode="MultiLine" Text="#STORE_NAME# - #NEWS_NAME#" /><br />
            <span style="color: Gray; font-size: 11px; display: block;">
                <%= Resources.Resource.Admin_m_News_UseGlobalVariables%>
            </span>
        </td>
    </tr>
    <tr class="rowsPost rowsPostBig">
        <td colspan="2">
            <div class="dvSubHelp">
                <asp:Image ID="Image5" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
                <a href="http://www.advantshop.net/help/pages/settings_of_seo_module" target="_blank">Инструкция. Настройка мета по умолчанию для магазина</a>
            </div>       
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_StaticPages%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_PageTitle%>
        </td>
        <td>
            <asp:TextBox ID="txtStaticPageHeadTitle" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #PAGENAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            H1
        </td>
        <td>
            <asp:TextBox ID="txtStaticPageH1" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME# - #PAGENAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaKeywords%>
        </td>
        <td>
            <asp:TextBox ID="txtStaticPageMetaKeywords" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME# - #PAGENAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaDescription%>
        </td>
        <td>
            <asp:TextBox ID="txtStaticPageMetaDescription" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME# - #PAGENAME#" /><br />
            <span style="color: Gray; font-size: 11px; display: block;">
                <%= Resources.Resource.Admin_StaticPage_UseGlobalVariables%>
            </span>
        </td>
    </tr>
    <tr class="rowsPost rowsPostBig">
        <td colspan="2">
            <div class="dvSubHelp">
                <asp:Image ID="Image6" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
                <a href="http://www.advantshop.net/help/pages/settings_of_seo_module" target="_blank">Инструкция. Настройка мета по умолчанию для магазина</a>
            </div>       
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_HeadAnotherPages%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_PageTitle%>
        </td>
        <td>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaKeywords%>
        </td>
        <td>
            <asp:TextBox ID="txtMetaKeys" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaDescription%>
        </td>
        <td>
            <asp:TextBox ID="txtMetaDescription" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME#" /><br />
            <span class="subParamNotify">
                <%= Resources.Resource.Admin_UseGlobalVariables%>
            </span>
        </td>
    </tr>
     <tr class="rowsPost rowsPostBig">
        <td colspan="2">
            <div class="dvSubHelp">
                <asp:Image ID="Image7" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
                <a href="http://www.advantshop.net/help/pages/settings_of_seo_module" target="_blank">Инструкция. Настройка мета по умолчанию для магазина</a>
            </div>       
        </td>
    </tr>
    <%--------------------------------------------------------------------------%>
     <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                Страница брэндов
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_PageTitle%>
        </td>
        <td>
            <asp:TextBox ID="txtBrandTitle" runat="server" CssClass="niceTextBox textBoxClass" Text="#STORE_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaKeywords%>
        </td>
        <td>
            <asp:TextBox ID="txtBrandMetaKeywords" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME#" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resources.Resource.Admin_m_Default_MetaDescription%>
        </td>
        <td>
            <asp:TextBox ID="txtBrandMetaDescription" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines" Text="#STORE_NAME#" /><br />
            <span class="subParamNotify">
                <%= Resources.Resource.Admin_UseGlobalVariables%>
            </span>
        </td>
    </tr>
</table>
<div class="dvSubHelp">
    <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
    <a href="http://www.advantshop.net/help/pages/settings_of_seo_module" target="_blank">Инструкция. Настройка мета по умолчанию для магазина</a>
</div>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_OtherSEO%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 240px;">
            <%= Resources.Resource.Admin_SettingsSEO_CustomMetaString%>
        </td>
        <td>
            <asp:TextBox ID="txtCustomMetaString" runat="server" CssClass="niceTextBox textBoxLong" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Тег в Head 
                    </header>
                    <div class="help-content">
                        Дополнительный тег в Head.<br />
                        Используется для подтверждения владения сайтом. <br /><br />
                        Например: <b>&lt;meta name=&quot;generator&quot; content=&quot;AdVantShop.NET&quot;&gt;</b><br />
                        ! Не стоит использовать для вставки счётчиков посещаемости, для этого используйте статические блоки.
                    </div>
                </article>
            </div>
            <br />
            <span class="subParamNotify">
                Не следует использовать данное поле для вставки счетчиков.
            </span>
        </td>
    </tr>
</table>
<div class="dvSubHelp">
    <asp:Image ID="Image2" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
    <a href="http://www.advantshop.net/help/pages/add-tag-head" target="_blank">Инструкция. Дополнительный тег в Head</a>
</div>