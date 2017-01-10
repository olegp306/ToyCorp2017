; (function () {
    'use strict';

    $(function () {
        $(".oc-billing-payments").on("click", ".method-item", function () {
            $(this).find(".checkbox input").attr("checked", "checked");
            getPayButton("#btnPay");
        });

        if ($("#btnPay").length > 0) {
            getPayButton("#btnPay");
        }
    });

    function getPayButton(containerId) {

        var orderId = $(".billing-pay").attr("data-order");
        var paymentId = $(".payment-methods .checkbox input:checked").parents(".method-item").attr("data-payment");

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            data: {
                orderid: orderId,
                paymentid: paymentId
            },
            url: "httphandlers/billing/getpaybutton.ashx",
            success: function (data) {
                if (data != null && data != "") {
                    var formSubmit = $(data.formString);
                    if (formSubmit.length != 0) {
                        $('.billing-form').html(formSubmit);
                        $(containerId).html(data.buttonString);
                        $(containerId).show();
                    } else if (data.buttonString != null && data.buttonString != "") {
                        $(containerId).html(data.buttonString);
                        $(containerId).show();
                    }
                    changeSum(data);
                }
            },
            beforeSend: function () {
                $(containerId).hide();
            },
            complete: function () {
            }
        });
    }

    function changeSum(data) {

        var htmlItem = "<div class=\"orderbasket-row\"> <div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">{0}:</div><div class=\"orderbasket-row-cost\">{1}</div></div> </div>";

        var result = "";
        result += String.Format(htmlItem, localize("myaccountOrderSum"), data.ProductsPrice);

        if (data.TotalDiscount != 0) {
            result += String.Format(htmlItem, localize("myaccountOrderDiscount") + "-" + data.TotalDiscount + "%", data.TotalDiscountPrice);
        }

        result += String.Format(htmlItem, localize("myaccountOrderShippingPrice"), "+" + data.ShippingPrice);

        if (data.couponPrice != "") {
            result += String.Format(htmlItem, localize("myaccountOrderCoupon") + " " + data.couponPersent + "%", data.couponPrice);
        }
        if (data.Bonus != "") {
            result += String.Format(htmlItem, localize("myaccountOrderBonus"), "-" + data.Bonus);
        }

        if (data.CertificatePrice != "") {
            result += String.Format(htmlItem, localize("myaccountOrderCertificate"), data.CertificatePrice);
        }

        if (data.TaxesNames != "") {
            result += String.Format(htmlItem, data.TaxesNames, data.TaxesPrice);
        }

        if (data.PaymentPrice != "") {
            result += String.Format(htmlItem, data.PaymentPriceText, data.PaymentPrice);
        }

        $(".js-oc-summary").html(result);
        $(".total-price").html(data.TotalPrice);
    }
})();

function open_printable_version(link) {
    var win = "menubar=no,location=no,resizable=yes,scrollbars=yes";
    var newWin = window.open(link, 'printableWin', win);
    newWin.focus();
}