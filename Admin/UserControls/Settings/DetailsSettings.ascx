<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DetailsSettings.ascx.cs" Inherits="Admin.UserControls.Settings.DetailsSettings" %>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                Карточка товара
            </span>
            <br />
            <span class="subTitleNotify">
                Настройки, влияющие на логику и внешний вид карточки товара в магазине
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                <%= Resources.Resource.Admin_CommonSettings_DisplayFields%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:280px;">
            <label class="form-lbl" for="<%= chkDisplayWeight.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_DisplayWeight%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkDisplayWeight" CssClass="checkly-align" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Отображать вес
                    </header>
                    <div class="help-content">
                        Отображать или нет графу "вес" в карточке товара.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= chkDisplayDimensions.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_DisplayDimensions%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkDisplayDimensions" CssClass="checkly-align" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Отображать габариты
                    </header>
                    <div class="help-content">
                        Отображать или нет графу "габариты" в карточке товара.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= cbShowStockAvailability.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_ShowStockAvailability%></label>
        </td>
        <td>
            <asp:CheckBox ID="cbShowStockAvailability" runat="server" CssClass="checkly-align" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Отображать остаток товара
                    </header>
                    <div class="help-content">
                        Отображать или нет остаток товара (количество) в графе "наличие" у товара. <br />
                        <br />
                        Например: <br />
                        При включённой опции - В наличии (100 шт.)<br />
                        При отключенной опции - В наличии
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                <%= Resources.Resource.Admin_CommonSettings_ProductPhotos%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= chkCompressBigImage.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_CompressBigImage%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCompressBigImage" CssClass="checkly-align" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Сжимать большую фотографию
                    </header>
                    <div class="help-content">
                        Опция определяет, как поступать с большой фотографией товара. 
                        <br /><br />
                        Если опция <b>включена</b>, то фотография товара будет пережата в соответствии с настройками для (big) большой фотографии.
                        <br /><br />
                        Если опция <b>выключена</b>, то фотография будет загружена "как есть".
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                <%= Resources.Resource.Admin_CommonSettings_Reviews%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= chkAllowReviews.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_AllowReviews%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkAllowReviews" CssClass="checkly-align" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Отзывы к товарам
                    </header>
                    <div class="help-content">
                        Опция определяет, разрешить или нет добавление отзывов к товарам.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ckbModerateReviews.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_ModerateReviews%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="ckbModerateReviews" CssClass="checkly-align" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Модерировать отзывы
                    </header>
                    <div class="help-content">
                        Если опция <b>включена</b>, отзывы сначала попадут на модерирование, и будут отображены в карточке товара только после проверки администратором.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                <%= Resources.Resource.Admin_CommonSettings_ZoomView%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= chkEnableZoom.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_EnableZoom%></label>
        </td>
        <td>
            <asp:CheckBox ID="chkEnableZoom" runat="server" CssClass="checkly-align" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Включить лупу
                    </header>
                    <div class="help-content">
                        Включает отображение лупы для фотографий в карточке товара.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                Доставка в карточке товара
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ddlShowShippingsMethodsInDetails.ClientID %>"><%= Resources.Resource.Admin_DetailsSettings_DisplayShipping%></label>
        </td>
        <td>
            <asp:DropDownList runat="server" ID="ddlShowShippingsMethodsInDetails">
                <asp:ListItem Text="<%$ Resources:Resource, Admin_DetailsSettings_DontDisplay%>" Value="0"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:Resource, Admin_DetailsSettings_DisplayOnClick%>" Value="1"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:Resource, Admin_DetailsSettings_DisplayAlways%>" Value="2"></asp:ListItem>
            </asp:DropDownList>
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Отображение доставки в карточке товара
                    </header>
                    <div class="help-content">
                        Определяет тип отображения вариантов доставки в карточке товара.<br /><br />
                        Также у каждого метода доставки есть опция отключения отображения в этом списке.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtShippingsMethodsInDetailsCount.ClientID %>"><%= Resources.Resource.Admin_DetailsSettings_DisplayShippingCount%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtShippingsMethodsInDetailsCount" CssClass="niceTextBox shortTextBoxClass2" Text="" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Кол-во отображаемых методов доставки
                    </header>
                    <div class="help-content">
                        Определяет количество разрешённых методов доставки для отображения в карточки товара.<br /><br />
                        Также у каждого метода доставки есть опция отключения отображения в этом списке.
                    </div>
                </article>
            </div>
        </td>
    </tr>
</table>
