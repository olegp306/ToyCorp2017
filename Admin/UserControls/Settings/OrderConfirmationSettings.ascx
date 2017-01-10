<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderConfirmationSettings.ascx.cs" Inherits="Admin.UserControls.Settings.OrderConfirmationSettings" %>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_HeadOrderConfirmation%>
            </span>
            <br />
            <span class="subTitleNotify">
                Настройки, влияющие на логику оформления заказ в магазине
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:345px;">
            <label class="form-lbl" for="<%= cbAmountLimitation.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_UseAmountLimit%></label>
        </td>
        <td>
            <asp:CheckBox ID="cbAmountLimitation" CssClass="checkly-align" runat="server" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Оперировать ли наличием товара при оформлении заказа
                    </header>
                    <div class="help-content">
                        Если опция <b>включена</b>, то нельзя оформить заказ на кол-во большее, чем есть (указано) у товара в графе количество.<br /><br />
                        Если <b>выключена</b>, то количество заказываемого товара может быть любым.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= cbProceedToPayment.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_ProceedToPayment%></label>
        </td>
        <td>
            <asp:CheckBox ID="cbProceedToPayment" CssClass="checkly-align" runat="server" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Переходить к оплате сразу
                    </header>
                    <div class="help-content">
                        Опция определяет вызывать ли сразу переход к системе оплаты. <br /><br />
                        Если опция <b>включена</b>, то на последнем шаге оформления заказа, клиента сразу перенаправит на форму платежной системы.
                        <br /><br />
                        Если опция <b>выключена</b>, то на последнем шаге оформления заказа, клиенту покажется сообщение о завершении оплаты и кнопка "перейти к оплате".
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtMinimalPrice.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_MinimalPrice%></label>
        </td>
        <td>
            <asp:TextBox ID="txtMinimalPrice" runat="server" class="niceTextBox shortTextBoxClass2"></asp:TextBox>
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Минимальная сумма заказа
                    </header>
                    <div class="help-content">
                        Параметр определяет минимальную сумму заказа, ниже которой нельзя оформить заказ.<br /><br />
                        Если сумма заказа в корзине будет ниже, чем указанное значение, клиенту покажется сообщение с уведомлением.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            Поля в оформлении заказа
        </td>
        <td>
            <asp:HyperLink ID="HyperLink2" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_SetOrderFields %>" 
                NavigateUrl="~/Admin/CheckoutFields.aspx" Target="_blank" />
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_BuyInOneclick_Header%>
            </span>
            <br />
            <span class="subTitleNotify">
                Настройки формы упрощённой покупки "в один клик"
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ckbBuyInOneClick.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_BuyInOneclick_Active%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" CssClass="checkly-align" ID="ckbBuyInOneClick" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Включить функцию<br />
                        "Купить в один клик"
                    </header>
                    <div class="help-content">
                        Опция определяет <b>включить</b> или <b>выключить</b> возможность покупки товаров с использованием формы "Купить в один клик".
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ckbBuyInOneClick.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_BuyInOneclick_ActiveInOrderConfirmation%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" CssClass="checkly-align" ID="ckbBuyInOneClickInOrderConfirmation" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Включить функцию<br />
                        "Купить в один клик"<br />
                        при оформлении заказа
                    </header>
                    <div class="help-content">
                        Опция определяет <b>включить</b> или <b>выключить</b> возможность покупки товаров при оформлении заказа с использованием формы "Купить в один клик".
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ckbGoToFinalStep.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_BuyInOneclick_GoToFinalStep%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" CssClass="checkly-align" ID="ckbGoToFinalStep" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Переходить на последний шаг оформления заказа
                    </header>
                    <div class="help-content">
                        Если опция <b>включена</b>, то после нажатия "Ждать звонка" пользователя перенаправит на страницу уведомления о совершении заказа. 
                        Так же в этом случае не будет показан текст из параметра "Текст в финальной форме 'Покупка в один клик'".
                        <br /><br />
                        Если опция <b>выключена</b>, то после нажатия "Ждать звонка" пользователю в этом же окне покажется текст из параметра "Текст в финальной форме".
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtFirstText.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_BuyInOneclick_FirstText%></label>
        </td>
        <td>
            <asp:TextBox ID="txtFirstText" runat="server" TextMode="MultiLine" CssClass="niceTextArea textArea2Lines"></asp:TextBox>
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Текст в первой форме
                    </header>
                    <div class="help-content">
                        Текст, который покажется вверху форму 'Покупка в один клик'
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtSecondText.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_BuyInOneclick_SecondText%></label>
        </td>
        <td>
            <asp:TextBox ID="txtSecondText" runat="server" TextMode="MultiLine" class="niceTextBox textArea2Lines"></asp:TextBox>
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Текст в финальной форме
                    </header>
                    <div class="help-content">
                        Текст, который покажется на этой же форме, уже после совершения покупки.<br /><br />
                        Если опция "Переходить на последний шаг оформления заказа" <b>включена</b>, данный текст не будет показан.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            Поля в оформлении заказа
        </td>
        <td>
            <asp:HyperLink ID="HyperLink1" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_BuyInOneclick_SetFields %>" 
                NavigateUrl="~/Admin/CheckoutFields.aspx" Target="_blank" />
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_GiftCertificates%>
            </span>
            <br />
            <span class="subTitleNotify">
                Настройки работы с подарочными сертификатами и купонами
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ckbEnableGiftCertificateService.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_EnableGiftCertificateService%></label>
        </td>
        <td>
            <asp:CheckBox ID="ckbEnableGiftCertificateService" CssClass="checkly-align" runat="server" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Разрешить использование подарочных сертификатов 
                    </header>
                    <div class="help-content">
                        <b>Включает</b> или <b>выключает</b> возможность покупки и использования подарочных сертификатов.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ckbDisplayPromoTextbox.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_DisplayPromoTextbox%></label>
        </td>
        <td>
            <asp:CheckBox ID="ckbDisplayPromoTextbox" CssClass="checkly-align" runat="server" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Поле ввода
                    </header>
                    <div class="help-content">
                        <b>Включает</b> или <b>выключает</b> поле для ввода купона или сертификата на странице оформления заказа.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtMinimalPriceCertificate.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_MinimalPriceCertificate%></label>
        </td>
        <td>
            <asp:TextBox ID="txtMinimalPriceCertificate" runat="server" class="niceTextBox shortTextBoxClass2"></asp:TextBox>
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Минимальная сумма<br /> сертификата
                    </header>
                    <div class="help-content">
                        Минимально разрешённая сумма сертификата.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtMaximalPricecertificate.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_MaximalPriceCertificate%></label>
        </td>
        <td>
            <asp:TextBox ID="txtMaximalPricecertificate" runat="server" class="niceTextBox shortTextBoxClass2"></asp:TextBox>
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Максимальная сумма<br /> сертификата
                    </header>
                    <div class="help-content">
                        Соответственно, максимально разрешённая сумма сертификата.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <div class="dvSubHelp" style="margin-top:15px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
                <a href="http://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty" target="_blank">Инструкция. Купоны и подарочные сертификаты</a>
            </div>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_PrintOrder_Header%>
            </span>
            <br />
            <span class="subTitleNotify">
                Параметры распечатки заказа администратором из панели работы с заказами
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= chkShowStatusInfo.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_PrintOrder_ShowStatusInfo%></label>
        </td>
        <td>
            <asp:CheckBox ID="chkShowStatusInfo" CssClass="checkly-align" runat="server" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Отображать информацию о статусе заказа
                    </header>
                    <div class="help-content">
                        Опция определяет выводить или нет строчку со статусом заказа на распечатке заказа.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= chkShowMap.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_PrintOrder_ShowMap%></label>
        </td>
        <td>
            <asp:CheckBox ID="chkShowMap" CssClass="checkly-align" runat="server" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Отображать карту c адресом покупателя
                    </header>
                    <div class="help-content">
                        Опция определяет отображать или нет карту по адресу клиента на распечатке заказа.<br /><br />
                        В некоторых случаях карта может быть удобна, например курьеру.
                    </div>
                </article>
            </div>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= rbGoogleMap.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_PrintOrder_UseMapType%></label>
        </td>
        <td>
            <span class="checkly-align">
                <asp:RadioButton ID="rbGoogleMap" runat="server" GroupName="MapType" Checked="true" />    
            </span>
            <span>
                <label class="form-lbl" style="display:inline;" for="<%= rbGoogleMap.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_PrintOrder_MapTypeGoogle%></label>
            </span>
            <span class="checkly-align" style="margin-left:10px;">
                <asp:RadioButton ID="rbYandexMap" runat="server" GroupName="MapType" />
            </span>
            <span>
                <label class="form-lbl" style="display:inline;" for="<%= rbYandexMap.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_PrintOrder_MapTypeYandex%></label>
            </span>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="padding: 15px 0 0 0;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_OrderId%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtOrderId.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_OrderId%></label>
        </td>
        <td>
            <asp:TextBox ID="txtOrderId" runat="server" CssClass="niceTextBox shortTextBoxClass2" />
            <asp:Button runat="server" ID="btnChangeOrderNumber" OnClick="btnChangeOrderNumber_Click"
                Text="<%$ Resources:Resource, Admin_CommonSettings_ChangeOrderIdButton %>" />
            <div style="padding: 5px 0 0 0;">
                <asp:Label runat="server" ID="lblOrderSaveResult" />
            </div>
        </td>
    </tr>
    <tr class="rowsPost">
        <td>
            
        </td>
        <td>
            <div style="width: 580px; padding: 5px 0;">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ChangeOrderIdHint %>" />
            </div>
        </td>
    </tr>
</table>