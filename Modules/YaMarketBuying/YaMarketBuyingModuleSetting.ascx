<%@ Control Language="C#" AutoEventWireup="true" CodeFile="YaMarketBuyingModuleSetting.ascx.cs"
    Inherits="Advantshop.Modules.YaMarketBuyingModuleSetting.Admin_YaMarketBuyingModuleSetting" %>

<style type="text/css">
    .colleft { width: 150px; }
    .colleft2 { width: 290px; }
    .m-hint {padding: 5px 0 10px 0;}
    .yamarketbuying .tbs td {vertical-align: top;padding: 5px 5px 5px 0;}
</style>
<div class="yamarketbuying" style="text-align: center;">
    <table class="tbs" border="0" cellpadding="2" cellspacing="0">
        <tr class="rowsPost">
            <td colspan="2" style="height: 34px;">
                <span class="spanSettCategory">
                    <asp:Localize ID="Localize9" runat="server" Text="Покупка на Яндекс.Маркете" /></span>
                <hr color="#C2C2C4" size="1px" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMessage" runat="server" Visible="False" Style="float: right;"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="colleft">Номер компании:</td>
            <td>
                <asp:TextBox runat="server" ID="txtCampaignId" Text="" Width="300px" />
                <div class="m-hint">
                    В разделе <a href="https://partner.market.yandex.ru/" target="_blank">Маркет для магазинов</a> Яндекса вы увидите надпись вида "№ xx-yyyyyyyy".<br>
                    Номером компании будет являться число yyyyyyyy.
                </div>
            </td>
        </tr>
        <tr>
            <td class="colleft">Авторизационный токен (от Яндекс.Маркета к магазину):</td>
            <td>
                <asp:TextBox runat="server" ID="txtAuth" Text="" Width="300px" />
                <div class="m-hint">
                    Ваш магазин в Яндекс Маркете -> Настройки -> Настройки API покупки
                </div>
            </td>
        </tr>
        <tr>
            <td class="colleft">URL API:</td>
            <td>
                <asp:Label runat="server" ID="lblApiUrl" />
            </td>
        </tr>
        <tr>
            <td class="colleft">Тип авторизации:</td>
            <td>HEADER</td>
        </tr>
        <tr>
            <td class="colleft">Формат данных:</td>
            <td>JSON</td>
        </tr>
        <tr>
            <td class="colleft">Авторизационный токен (от магазина к Яндекс.Маркету):</td>
            <td>
                <asp:TextBox runat="server" ID="txtAuthTokenToYaMarket" Text="" Width="300px" />
                <div class="m-hint">
                    <a href="http://api.yandex.ru/oauth/doc/dg/tasks/get-oauth-token.xml" target="_blank">Алгоритм получения токена</a>
                </div>
            </td>
        </tr>
        <tr>
            <td class="colleft">Идентификатор приложения:</td>
            <td>
                <asp:TextBox runat="server" ID="txtAuthClientId" Text="" Width="300px" />
                <div class="m-hint">
                    Идентификатор приложения, выданный при его регистрации.<br> 
                    Алгоритм получения идентификатора приложения описан в Руководстве разработчика: Регистрация приложения.
                </div>
            </td>
        </tr>
        <tr>
            <td class="colleft">Логин пользователя:</td>
            <td>
                <asp:TextBox runat="server" ID="txtLogin" Text="" Width="300px" />
                <div class="m-hint">
                    Логин, используемый при регистрации магазина в Яндекс.Маркете.
                </div>
            </td>
        </tr>
    </table>
    <div style="padding: 10px 0; font-weight: bold; text-align: left">Способы доставки</div>
    <asp:ListView ID="lvShippings" runat="server" OnItemDataBound="lvShippings_OnItemDataBound" ItemPlaceholderID="trPlaceholderID">
        <LayoutTemplate>
            <table class="table-ui" style="width: 900px">
                <thead>
                    <th>Метод доставки</th>
                    <th style="width: 120px">id метода доставки</th>
                    <th style="width: 200px">Тип</th>
                    <th style="width: 100px">Мин. срок, дни</th>
                    <th style="width: 100px">Макс. срок, дни</th>
                </thead>
                <tbody>
                    <tr id="trPlaceholderID" runat="server"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <%# Eval("Name")%>
                </td>
                <td>
                    <%# Eval("ShippingMethodId")%>
                    <asp:HiddenField runat="server" ID="hfShippingMethodId" Value='<%# Eval("ShippingMethodId")%>' />
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlType">
                        <asp:ListItem Text="Нет" Value="" />
                        <asp:ListItem Text="Курьерская доставка" Value="DELIVERY" />
                        <asp:ListItem Text="Почта" Value="POST" />
                        <asp:ListItem Text="Самовывоз" Value="PICKUP" />
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtMinDate" Text='<%# Eval("MinDate")%>' Width="70px" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtMaxDate" Text='<%# Eval("MaxDate")%>' Width="70px" />
                </td>
            </tr>
        </ItemTemplate>
        <EmptyItemTemplate>
            <tr>
                <td colspan="5">
                    <asp:Localize ID="Localize6" runat="server" Text="Для коректной работы модуля необходимо включить методы доставки" />
                </td>
            </tr>
        </EmptyItemTemplate>
    </asp:ListView>

    <table class="tbs" border="0" cellpadding="2" cellspacing="0">
        <tr>
            <td colspan="2">
                <div style="padding: 10px 0; font-weight: bold">Способы оплаты</div>
            </td>
        </tr>
        <tr>
            <td class="colleft2">
                Оплата при оформлении (только для России)
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlYandex">
                    <asp:ListItem Text="Не использовать" Value="0" />
                    <asp:ListItem Text="Использовать" Value="1" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="colleft2">
                Предоплата напрямую магазину (только для Украины)
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlShopprepaid">
                    <asp:ListItem Text="Не использовать" Value="0" />
                    <asp:ListItem Text="Использовать" Value="1" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="colleft2">
                Наличный расчет при получении заказа
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlCashOnDelivery">
                    <asp:ListItem Text="Не использовать" Value="0" />
                    <asp:ListItem Text="Использовать" Value="1" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="colleft2">
                Оплата банковской картой при получении заказа
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlCardOnDelivery">
                    <asp:ListItem Text="Не использовать" Value="0" />
                    <asp:ListItem Text="Использовать" Value="1" />
                </asp:DropDownList>
            </td>
        </tr>
        
        <tr>
            <td colspan="2">
                <div style="padding: 10px 0; font-weight: bold">Статусы заказа</div>
            </td>
        </tr>
        <tr>
            <td class="colleft2">
                Заказ оформлен, но еще не оплачен (если выбрана оплата при оформлении)
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlUnpaidStatus">
                    <asp:ListItem Text="Нет" Value="0" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="colleft2">
                Заказ можно выполнять
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlProcessingStatus">
                    <asp:ListItem Text="Нет" Value="0" />
                </asp:DropDownList>
            </td>
        </tr>        
        <tr>
            <td class="colleft2">
                Заказ доставлен
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlDeliveredStatus">
                    <asp:ListItem Text="Нет" Value="0" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="colleft2">
                Заказ готов к передаче в службу доставки. (DELIVERY)
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlDeliveryStatus">
                    <asp:ListItem Text="Нет" Value="0" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding: 5px 0">
                <b>Внимание!</b> Заказ должен быть переведен в статус DELIVERY:<br />
                в течение 7 дней, если тип оплаты — YANDEX (оплата при оформлении)<br />
                в течение 21 дня с любым другим типом оплаты<br />
                Если заказ не переведен в статус DELIVERY вовремя, он автоматически отменяется, а магазину выставляется ошибка.<br />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Сохранить" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label runat="server" ID="lblErr" Visible="False"></asp:Label>
            </td>
        </tr>
    </table>
</div>