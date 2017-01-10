
$(function () {

    if ($(".ddlViewOrderStatus").length) {
        var dll = $(".ddlViewOrderStatus");

        $(".ddlViewOrderStatus").change(function () {
            setOrderStatus(dll.val(), dll.attr("data-orderid"));
        });
    }

    if ($(".useIn1c").length) {
        $(".useIn1c").change(function() {
            changeUseIn1C($(".useIn1c input").is(":checked"), $(".useIn1c").attr("data-orderid"));
        });
    }

    if ($(".editableTextBoxInViewOrder").length) {
        var editableTextBox, progress;
        $(".editableTextBoxInViewOrder").focusout(function () {
            editableTextBox = $(this);
            progress = new Advantshop.ScriptsManager.Progress.prototype.Init(editableTextBox);
            progress.Show();
            updateOrderField(editableTextBox.attr("data-orderid"), editableTextBox.val(), editableTextBox.attr("data-field-type"));
            progress.Hide();
        });
    }

    $(".multiship-create-order").on("click", function () {
        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { orderid: $(this).attr("data-value") },
            url: "httphandlers/order/createordermultiship.ashx",
            success: function (data) {
                if (data != null && data.result != "error") {
                    notify(localize("Admin_ViewOrder_CreateMultishipOrder"), notifyType.notify);
                } else {
                    notify(localize("Admin_ViewOrder_ErrorCreateMultishipOrder"), notifyType.error);
                }
            },
            error: function () {
                notify(localize("Admin_ViewOrder_ErrorCreateMultishipOrder"), notifyType.error);
            }
        });
    });

    $(".send-billing-link").on("click", function () {
        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { orderid: $(this).attr("data-value") },
            url: "httphandlers/order/sendbillinglink.ashx",
            success: function (data) {
                if (data != null && data.result != "error") {
                    notify(localize("Admin_ViewOrder_SendBillingLinkSuccess"), notifyType.notify);
                } else {
                    notify(localize("Admin_ViewOrder_SendBillingLinkError"), notifyType.error);
                }
            },
            error: function () {
                notify(localize("Admin_ViewOrder_SendBillingLinkError"), notifyType.error);
            }
        });
    });
    
    $(".cdek-send-order").on("click", function () {
        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { orderid: $(this).attr("data-value"), tariff: $(this).attr("data-tariff"), pickpoint: $(this).attr("data-pickpoint") },
            url: "httphandlers/order/cdeksendorder.ashx",
            success: function (data) {
                notify(data.result.message, data.result.status ? notifyType.notify : notifyType.error);
            },
            error: function () {
                notify(localize("Admin_ViewOrder_ErrorCreateMultishipOrder"), notifyType.error);
            }
        });
    });

    $(".cdek-delete-order").on("click", function () {
        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { orderid: $(this).attr("data-value") },
            url: "httphandlers/order/cdekdeleteorder.ashx",
            success: function (data) {
                notify(data.result.message, data.result.status ? notifyType.notify : notifyType.error);
            },
            error: function () {
                notify(localize("Admin_ViewOrder_ErrorCreateMultishipOrder"), notifyType.error);
            }
        });
    });

    $(".checkout-send-order").on("click", function () {
        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { orderid: $(this).attr("data-value")},
            url: "httphandlers/order/checkoutsendorder.ashx",
            success: function (data) {
                notify(data.result.message, data.result.error ? notifyType.error : notifyType.notify);
            },
            error: function () {
                notify(localize("Admin_ViewOrder_ErrorCreateCheckoutOrder"), notifyType.error);
            }
        });
    });


    $(".cdek-callcustomer-order").on("click", function () {
        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { orderid: $(this).attr("data-value") },
            url: "httphandlers/order/cdekcallcustomer.ashx",
            success: function (data) {
                notify(data.result.message, data.result.status ? notifyType.notify : notifyType.error);
            },
            error: function () {
                notify(localize("Admin_ViewOrder_ErrorCreateMultishipOrder"), notifyType.error);
            }
        });
    });

    $(".cdek-callcourier-order").on("click", function () {
        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { orderid: $(this).attr("data-value") },
            url: "httphandlers/order/cdekcallcourier.ashx",
            success: function (data) {
                notify(data.result.message, data.result.status ? notifyType.notify : notifyType.error);
            },
            error: function () {
                notify(localize("Admin_ViewOrder_ErrorCreateMultishipOrder"), notifyType.error);
            }
        });
    });
});

function getCdekOrderPrintform(orderid) {
    window.location = "httphandlers/order/cdekorderprintform.ashx?orderid=" + orderid;
    return true;
}

function getCdekOrderReportStatus(orderid) {
    window.location = "httphandlers/order/cdekreportorderstatus.ashx?orderid=" + orderid;
    return true;
}

function getCdekOrderReportInfo(orderid) {
    window.location = "httphandlers/order/cdekreportorderinfo.ashx?orderid=" + orderid;
    return true;
}

function setOrderPaid(paid, orderid) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid, paid: paid },
        url: "httphandlers/order/setorderpaid.ashx",
        success: function () {
            if ($(".orders-list-name[data-current-order=1]").length) {
                if (paid == 1) {
                    $(".orders-list-name[data-current-order=1]").removeClass("orders-list").addClass("orders-list-ok");
                } else if (paid == 0) {
                    $(".orders-list-name[data-current-order=1]").removeClass("orders-list-ok").addClass("orders-list");
                }
            }
            notify(localize("Admin_ViewOrder_SaveOrderPayStatus"), notifyType.notify);
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSaveOrderPayStatus"), notifyType.error);
        }
    });
}


function setOrderStatus(statusid, orderid) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid, statusid: statusid },
        url: "httphandlers/order/setorderstatus.ashx",
        success: function (data) {
            if ($(".order-main").length) {
                $(".order-main").first().css("border-left-color", "#" + data.Color);
            }
            if ($(".orders-list-row[data-current-order=1]").length) {
                $(".orders-list-row[data-current-order=1]").css("border-left-color", "#" + data.Color);
            }
            notify(localize("Admin_ViewOrder_SaveOrderStatus"), notifyType.notify);

            if (data.IsNotifyUser) {
                $("#lnkSendMail").show();
            }
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSaveOrderStatus"), notifyType.error);
        },
        complete: function () {
        }
    });
}

function changeUseIn1C(use, orderid) {

    $.ajax({
        dataType: "text",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid, use: use },
        url: "httphandlers/order/changeusein1c.ashx",
        success: function (data) {
            if (data == "true") {
                notify("Изменения сохранены", notifyType.notify);
            } else {
                notify(localize("Admin_ViewOrder_ErrorOnSaveOrderStatus"), notifyType.error);
            }
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSaveOrderStatus"), notifyType.error);
        }
    });
}

function updateOrderField(orderid, text, field) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid, text: text, field: field },
        url: "httphandlers/order/updateorderfield.ashx",
        success: function () {
            notify(localize("Admin_ViewOrder_SaveComment"), notifyType.notify);
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSaveComment"), notifyType.error);
        }
    });
}

function sendMailOrderStatus(orderid) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid },
        url: "httphandlers/order/sendmailorderstatus.ashx",
        success: function (data) {
            if ($(".order-main").length) {
                $(".order-main").first().css("border-left-color", "#" + data.result);
            }
            if ($(".orders-list-row[data-current-order=1]").length) {
                $(".orders-list-row[data-current-order=1]").css("border-left-color", "#" + data.result);
            }
            notify(localize("Admin_ViewOrder_SendMailStatusOrder"), notifyType.notify);
            $("#lnkSendMail").hide();
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSendMailStatusOrder"), notifyType.error);
        },
        complete: function () {
        }
    });
}