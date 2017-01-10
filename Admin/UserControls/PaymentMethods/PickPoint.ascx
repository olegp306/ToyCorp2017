<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PickPoint.ascx.cs" Inherits="Admin.UserControls.PaymentMethods.PickPointControl" %>
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px; margin-top: 5px;">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
            <h4 style="display: inline; font-size: 12pt;">
                <%=Resources.Resource.Admin_PaymentMethods_HeadSettings%></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <%=Resources.Resource.Admin_PaymentMethods_LinkedShippingMethod%><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:DropDownList runat="server" ID="ddlShipings" style="display:inline;" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        Привязанный метод доставки
                    </header>
                    <div class="help-content">
                        В этом списке отображаются возможные варианты привязки. <br />
                        <br />
                        Если в списке нет значений, вам нужно добавить метод доставки с типом <b>eDost</b>.
                    </div>
                </article>
            </div>
        </td>
        <td class="columnDescr">
            <div style="background-color: #b2daeb; padding: 10px; width: 530px;">
                <%=Resources.Resource.Admin_PaymentMethod_EdostRequired%>
            </div>
        </td>
    </tr>
</table>