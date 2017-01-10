<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShippingRates.ascx.cs"
    Inherits="Admin.UserControls.Order.ShippingRatesControl" %>
<div id="divScripts" runat="server" visible="false">
    <script type="text/javascript" src="http://pickpoint.ru/select/postamat.js"></script>
    <script type="text/javascript">
        function SetPickPointAnswer(result) {
            $('#<%= pickpointId.ClientID  %>').val(result['id']);
            $('#address').html(result['name'] + '<br />' + result['address']);
            $('#<%= pickAddress.ClientID  %>').val(result['name'] + '<br />' + result['address']);
        }
    </script>
</div>
<script type="text/javascript">
    $(function () {
        $("body").on("change", ".shipping-points-cdek, .shipping-points", function (e) {
            var radio = $(this).closest(".radio-shipping").children("input[type=radio]");
            $("._selectedID").val(radio.val());
            radio.attr("checked", "checked");

            $(".pickpointId").val($(this).val());
            $(".pickAddress").val($(this).children("option:selected").text());

            $(".pickAdditional").val($(this).closest(".radio-shipping").find(".hiddenCdekTariff").val());
        });

        $("body").on("click", ".radio-shipping", function () {
            var radio = $(this).children("input[type=radio]");
            $("._selectedID").val(radio.val());

            var select = $(this).find(".shipping-points-cdek, .shipping-points");

            var selectCheckout = $(this).find(".shipping-points-checkout, .shipping-points");

            if (select.length) {
                $(".pickpointId").val(select.val());
                $(".pickAddress").val(select.children("option:selected").text());

                $(".pickAdditional").val($(this).find(".hiddenCdekTariff").val());
            }
            else if (selectCheckout.length || $(this).find(".hiddenCheckoutInfo").length) {
                
                if (selectCheckout.length) {
                    var selectOption = selectCheckout.children("option:selected");

                    $(".pickpointId").val(selectOption.val());
                    $(".pickAddress").val(selectOption.attr("data-checkout-address"));
                    $(".pickAdditional").val(selectOption.attr("data-additional"));
                }
                else if ($(this).find(".hiddenCheckoutInfo").length) {
                    $(".pickAdditional").val($(this).find(".hiddenCheckoutInfo").val());
                }

            } else {
                $(".pickpointId").val("");
                $(".pickAddress").val("");

                $(".pickAdditional").val($(this).find(".hiddenCdekTariff").val());
            }
        });
    });

</script>
<div id="divMultiShip" runat="server" visible="false">
    <script type="text/javascript" src="https://api-maps.yandex.ru/2.0/?load=package.standard,package.geoQuery&lang=ru-RU"></script>
    <script type="text/javascript">
        $(function () {
            jQuery.getScript("<%=WidgetCode%>", function () {

                mswidget.ready(function () {

                    ms$('body').prepend('<div id="mswidget" class="ms-widget-modal"></div>');

                    mswidget.initCartWidget({
                        'getCity': function () {
                            var city = $(".ms-city").val();
                            if (city != "") {
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
                        'weight': function () { return <%= Weight%>; },

                        //общая стоимость товаров в корзине
                        'cost': function () { return <%= Cost%>; },

                        //обработка смены варианта доставки
                        'onDeliveryChange': function (delivery) {
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

            function SetMultiShipAnswer(delivery) {
                $('.pickpointId').val(delivery.pickuppoint);
                $('.pickAddress').val(delivery.delivery_name + ', ' + delivery.address);

                var additionalData = {
                    direction: delivery.direction,
                    delivery: delivery.delivery,
                    price: delivery.cost_with_rules,
                    to_ms_warehouse: delivery.to_ms_warehouse
                };

                $('.pickAdditional').val(JSON.stringify(additionalData));
            }
        });
    </script>
    <div id="mswidget" class="ms-widget-modal"></div>
</div>
<input type="hidden" id="_selectedID" class="_selectedID" runat="server" value="" />
<input type="hidden" id="pickpointId" class="pickpointId" runat="server" value="" />
<input type="hidden" id="pickAddress" class="pickAddress" runat="server" value="" />
<input type="hidden" id="pickAdditional" class="pickAdditional" runat="server" value="" />
<div id="RadioList" runat="server" class="RadioList" visible="true">
</div>
<asp:Label ID="lblNoShipping" runat="server" Style="color: red" Visible="false" Text="<%$ Resources:Resource, Client_ShippingRates_NoShipping %>"></asp:Label>