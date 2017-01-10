<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShippingMethods.ascx.cs"
    Inherits="UserControls.OrderConfirmation.ShippingMethod" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="AdvantShop.Shipping" %>
<%@ Import Namespace="Resources" %>

<div class="delivery-info">
    <input type="hidden" id="_selectedID" runat="server" value="" />
    <input type="hidden" id="pickpointId" runat="server" value="" />
    <input type="hidden" id="pickpointStringId" runat="server" value="" />
    <input type="hidden" id="pickAddress" runat="server" value="" />
    <input type="hidden" id="pickAdditional" runat="server" value="" />
    <input type="hidden" id="hfDistance" runat="server" value="0" />
    <input type="hidden" id="pickRate" runat="server" value="" />

    <div data-plugin="vis" data-vis-options="{visible:7, textControlShow: '<%= Resource.Client_OrderConfirmation_MoreShipping%>', textControlHide:'<%= Resource.Client_OrderConfirmation_CollapseShipping %>', itemExtraSelector: function(){ return $(this).find('.checkbox input[type=radio]:checked').length > 0 }}">
        <asp:ListView ID="lvShippingRates" runat="server" ItemPlaceholderID="itemPlaceHolder">
            <LayoutTemplate>
                <div class="oc-text-left">Способ доставки</div>
                <div class="shipping-bg">
                <div class="shipping-methods">
                    <div runat="server" id="itemPlaceHolder" />
                </div>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="method-item js-vis-item" data-shipping='<%# Eval("Id") %>'>
                    <div class="checkbox">
                        <input type="radio" name="shippingchk" id="<%# Eval("Id") %>" onclick="shippingSelectChanged(<%# Eval("Id") %>)"/>
                        <label for="<%# Eval("Id") %>"></label>
                    </div>
                    <div class="shipping-img">
                        <img src='<%# ShippingIcons.GetShippingIcon((ShippingType)Eval("Type"), Eval("IconName") as string , SQLDataHelper.GetString(Eval("MethodNameRate"))) %>'
                            <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("MethodNameRate").ToString())) %> />
                    </div>
                    <div class="method-info">
                        <div class="method-name">
                            <%#Eval("MethodNameRate")%>
                            <%# ShippingMethodService.RenderExtend((ShippingItem)Container.DataItem, Distance, pickAddress.Value, false)%>
                        </div>
                        <div class="method-descr-price">
                            <span class="cost"><%# SQLDataHelper.GetFloat(Eval("Rate")) !=0 ? CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Rate"))) : SQLDataHelper.GetString(Eval("ZeroPriceMessage"))%></span> <%# Eval("DeliveryTime")%>
                        </div>
                        <div class="method-descr">
                            <%# Eval("MethodDescription") %>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <SelectedItemTemplate>
                <div class="method-item js-vis-item" data-shipping='<%# Eval("Id") %>'>
                    <div class="checkbox">
                        <input type="radio" name="shippingchk" checked="checked"  id="<%# Eval("Id") %>" onclick="shippingSelectChanged(<%# Eval("Id") %>)"/>
                        <label for="<%# Eval("Id") %>"></label>
                    </div>
                    <div class="shipping-img">
                        <img src='<%# ShippingIcons.GetShippingIcon((ShippingType)Eval("Type"), Eval("IconName") as string , SQLDataHelper.GetString(Eval("MethodNameRate"))) %>'
                            <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("MethodNameRate").ToString())) %> />
                    </div>
                    <div class="method-info">
                        <div class="method-name">
                            <%#Eval("MethodNameRate")%>
                            <%# ShippingMethodService.RenderExtend((ShippingItem)Container.DataItem, Distance, pickAddress.Value, true)%>
                        </div>
                        <div class="method-descr-price">
                            <span class="cost"><%# SQLDataHelper.GetFloat(Eval("Rate")) !=0 ? CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Rate"))) : SQLDataHelper.GetString(Eval("ZeroPriceMessage"))%></span> <%# Eval("DeliveryTime")%>
                        </div>
                        <div class="method-descr">
                            <%# Eval("MethodDescription") %>
                        </div>
                    </div>
                </div>
            </SelectedItemTemplate>
            <EmptyDataTemplate>
                <div class="shipping-methods">
                    <span class="oc-no-way">
                        <asp:Literal ID="lblNoShipping" runat="server" Text="<%$ Resources:Resource, Client_ShippingRates_NoShipping %>" /></span>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>
        <a class="js-vis-control vis-control oc-vis-control" href="javascript:void(0);"><%= Resource.Client_OrderConfirmation_MoreShipping%></a>
    </div>
</div>
<div id="divPickpoint" runat="server" visible="false">
    <script type="text/javascript">
        $(function () {
            jQuery.getScript("http://pickpoint.ru/select/postamat.js");
        });
    </script>
</div>
<div id="divMultiShip" runat="server" visible="false">
    <script type="text/javascript" src="https://api-maps.yandex.ru/2.0/?load=package.standard,package.geoQuery&lang=ru-RU"></script>
    <script type="text/javascript">
         $(function () {
             jQuery.getScript("<%=WidgetCode%>", function() {
             
                mswidget.ready(function() {
                    
                    ms$('body').prepend('<div id="mswidget" class="ms-widget-modal"></div>');

                    mswidget.initCartWidget({
                        'getCity': function() {
                            var geoData = JSON.parse($(".delivery-change").attr("data-delivery"));
                            var city = geoData.city;
                            if (city) {
                                return { value: city };
                            } else {
                                return false;
                            }
                        },
                        
                        //id элемента-контейнера
                        'el': 'mswidget',
                        
                        'itemsDimensions': function () {
                            return [<%= Dimensions%>];
                        },
                        
                        //общий вес товаров в корзине
                        'weight': function() { return <%= Weight%>;},
                        
                        //общая стоимость товаров в корзине
                        'cost': function() { return <%= Cost%>;},
                        
                        //обработка смены варианта доставки
                        'onDeliveryChange': function(delivery) {
                            //если выбран вариант доставки, выводим его описание и закрываем виджет, иначе произошел сброс варианта,
                            //очищаем описание
                            if (delivery) {
                                SetMultiShipAnswer(delivery);
                                mswidget.cartWidget.close();
                            } else {
                                ms$('.address-multiship').text('');
                            }
                        },
                        'onlyPickuppoints': true,
                    });
                });
            });
        });
    </script>
    <div id="mswidget" class="ms-widget-modal"></div>
</div>
