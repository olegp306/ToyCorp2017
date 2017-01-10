var mpAddress,
    mpBonusCard,
    bonusAddModalMa,
    bonusConfirmModalMa;

var advantshop = Advantshop;
var scriptManager = advantshop.ScriptsManager;

$(function () {


    var bonusConfirm = $("#bonus-confirm").clone();

    if ($("#contactsDivForm").length) {
        getContactsForm("#contactsDivForm");
    }

    if ($("#modalMyAccountAddress").length) {
        mpAddress = $.advModal({
            title: localize("myaccountShippingAddress"),
            htmlContent: $("#modalMyAccountAddress"),
            control: ".link-edit-a, #btnAddNew",
            clickOut: false,
            afterOpen: function () { initValidation($("form")); }
        });
    }
    if ($("#btnAddChangeContact").length) {
        $("#btnAddChangeContact").click(function () {
            AddContact();
        });
    }

    // bonuses
    if ($(".edit-card-info").length) {
        mpBonusCard = $.advModal({
            title: localize("bonusCard"),
            modalId: "bonus-edit-modal",
            htmlContent: $("#cardinfo"),
            control: ".edit-card-info",
            clickOut: false,
            afterOpen: function () { initValidation($("form")); }
        });
    }

    if ($(".ma-addcard").length > 0) {
        bonusAddModalMa = $.advModal({ modalId: "bonus-ma-confirm-modal-addcart", modalClass: "bonus-modal", clickOut: false });
    }

    if ($(".ma-bonus-confirm").length > 0) {
        bonusConfirmModalMa = $.advModal({ modalId: "bonus-ma-confirm-modal", modalClass: "bonus-modal", clickOut: false });
    }

    $(".ma-addcard").on("click", function () {

        if (!$("#txtBonusPhone").inputmask("isComplete")) {
            $("#txtBonusPhone").val("");
        }

        if (!$("#txtBonusDate").inputmask("isComplete")) {
            $("#txtBonusDate").val("");
        }

        if (!$("form").valid("mabonus")) {
            return;
        }

        var phone = $("#txtBonusPhone").val().replace(/\D+/g, '');

        var progress = new scriptManager.Progress.prototype.Init($("#bonuses"));
        
        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/bonuses/confirmcardbyphone.ashx",
            data: {
                phone: phone
            },
            success: function (data) {
                if (data.error == "phone_exist") {
                    $("#txtPhoneCardNumber").val(phone);
                    $("#card_yes").prop("checked", true);
                    SetBonusChoose($("#card_yes").parents(".bonus-choice"));

                    bonusAddModalMa.modalContent("<div class='modal-oc-error'>" + localize("myaccountBonusPhone") + " " + phone + " " + localize("myaccountBonusPhoneIsExist") + "</div>");
                    bonusAddModalMa.modalSetClickOut(true);
                    bonusAddModalMa.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);
                } else if (data.error != "") {
                    bonusAddModalMa.modalContent("<div class='modal-oc-error'>" + data.error + "</div>");
                    bonusAddModalMa.modalSetClickOut(true);
                    bonusAddModalMa.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);
                } else {
                    bonusAddModalMa.modalContent(bonusConfirm.clone().html());
                    bonusAddModalMa.modalBtns([]);
                }
                bonusAddModalMa.modalShow();
            },
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            }
        });
    });
    
    $("body").on("click", "#bonus-ma-confirm-modal-addcart .bonus-confirm-code", function () {
        var code = $("#bonus-ma-confirm-modal-addcart .bonus-content-code input").val();
        if (code == "")
            return;
        
        $("#bonus-ma-confirm-modal-addcart .bonus-confirm-error").text("");
        
        var progress = new scriptManager.Progress.prototype.Init($("#bonus-ma-confirm-modal-addcart"));

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/bonuses/confirmbonuscode.ashx",
            data: {
                code: code,
                firstName: $("#txtBonusFirstName").val(),
                lastName: $("#txtBonusLastName").val(),
                secondName: $("#txtBonusSecondName").val(),
                gender: $("input[name='BonusGender']:checked").val(),
                birthDay: $("#txtBonusDate").val(),
                phone: $("#txtBonusPhone").val().replace(/\D+/g, ''),
                addcart: "true"
            },
            success: function (data) {
                if (data.error == "" && data.success == "true") {
                    location.reload();
                } else {
                    $("#bonus-ma-confirm-modal-addcart .bonus-confirm-error").text(data.error);
                }
            },
            error: function (data) {
                if (data != null && data.error != "") {
                    $("#bonus-ma-confirm-modal-addcart .bonus-confirm-error").text(data.error);
                }
            },
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            }
        });
    });

    $("#btnBonusConfirmMa").on("click", function () {
        var cardnumber = $("#txtCardNumber").val().replace(/\D+/g, '');
        if (cardnumber == "")
            return;

        confirmCardByNumber(cardnumber);
    });

    $("#btnBonusConfirmPhoneMa").on("click", function () {
        var phone = $("#txtPhoneCardNumber").val().replace(/\D+/g, '');
        if (phone == "")
            return;

        confirmCardByNumber(phone);
    });

    function confirmCardByNumber(cardnumber) {
        $("#bonus-ma-confirm-modal .bonus-content-code input").val("");
        $("#bonus-ma-confirm-modal .bonus-confirm-error").text("");

        var progress = new scriptManager.Progress.prototype.Init($("#bonuses"));

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/bonuses/confirmcardbynumber.ashx",
            data: {
                cardnumber: cardnumber
            },
            success: function (data) {
                if (data.error != "") {
                    bonusConfirmModalMa.modalContent("<div class='modal-oc-error'>" + data.error + "</div>");
                    bonusConfirmModalMa.modalSetClickOut(true);
                    bonusConfirmModalMa.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);
                } else {
                    bonusConfirmModalMa.modalContent(bonusConfirm.clone().html());
                    bonusConfirmModalMa.modalBtns([]);
                }
                bonusConfirmModalMa.modalShow();
            },
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            }
        });
    }

    $("body").on("click", "#bonus-ma-confirm-modal .bonus-confirm-code", function () {
        var code = $("#bonus-ma-confirm-modal .bonus-content-code input").val();
        if (code == "")
            return;

        var progress = new scriptManager.Progress.prototype.Init($("#bonus-ma-confirm-modal"));

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/bonuses/confirmbonuscode.ashx",
            data: {
                code: code,
                isCheckout: "true"
            },
            success: function (data) {
                if (data.error == "" && data.success == "true") {
                    bonusConfirmModalMa.modalSetClickOut(true);
                    $("#bonus-ma-confirm-modal .content").html("<div class='modal-oc-success'>" + localize("bonusCardApplied") + "</div>");
                    location.reload();
                } else {
                    $("#bonus-ma-confirm-modal .bonus-confirm-error").text(data.error);
                }
            },
            error: function (data) {
                if (data != null && data.error == "") {
                    $("#bonus-ma-confirm-modal .bonus-confirm-error").text(data.error);
                }
            },
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            }
        });
    });

    $(".ma-changecard").on("click", function () {

        var phone = $("#txtBonusPhone").val().replace(/\D+/g, '');
        if (phone == "") {
            return;
        }

        var progress = new scriptManager.Progress.prototype.Init(mpBonusCard);

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/bonuses/updatebonuscard.ashx",
            data: {
                firstName: $("#txtBonusFirstName").val(),
                lastName: $("#txtBonusLastName").val(),
                secondName: $("#txtBonusSecondName").val(),
                gender: $(".gender input:checked").val(),
                birthDay: $("#txtBonusDate").val(),
                phone: $("#txtBonusPhone").val().replace(/\D+/g, ''),
            },
            success: function (data) {
                if (data == null)
                    return;
                
                mpBonusCard.modalClose();

                if (data.error == "") {
                    notify(localize("bonusCardSaved"), notifyType.notify);
                } else {
                    notify(data.error, notifyType.error);
                }
            },
            error: function(data) {
                if (data != null && data.error != "") {
                    notify(data.error, notifyType.error);
                }
            },
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            }
        });
    });

});

function AddContact() {
    if ($("form").valid("maAddress")) {
        addUpdateContact(
            $("#txtContactName").val(), $("#cboCountry :selected").val(), $("#cboCountry :selected").text(),
            $("#txtContactZone").val(), $("#txtContactCity").val(), $("#txtContactAddress").val(),
            $("#txtContactZip").val(), $("#hfContactId").val());
    }
}

function ShowModalAddAddress() {
    $("#txtContactName").val("");
    $("#cboCountry").val("171");
    $("#txtContactZone").val("");
    $("#txtContactCity").val("");
    $("#txtContactAddress").val("");
    $("#txtContactZip").val("");
    $("#hfContactId").val("");
    $("#btnAddChangeContact").text(localize("myaccountAdd"));

}

function ShowModalAddress(fio, country, region, city, address, postcode, contactid) {
    $("#txtContactName").val(fio);
    $("#cboCountry").val(country);
    $("#txtContactZone").val(region);
    $("#txtContactCity").val(city);
    $("#txtContactAddress").val(address);
    $("#txtContactZip").val(postcode);
    $("#hfContactId").val(contactid);
    $("#btnAddChangeContact").text(localize("myaccountChange"));
}

function getContactsForm(containerId) {
    var itemHtml = "<li><span for=\"a1\">{0}, {4}, {5}</span><a class=\"link-edit-a\" href=\"javascript:void(0)\" onclick=\"ShowModalAddress('{1}','{2}','{3}','{4}','{5}','{6}','{7}');return false;\">" + localize("myaccountEdit") + "</a><a href=\"javascript:void(0)\" class=\"link-remove-a\" onclick=\"delContact('{7}','{8}'); return false;\">" + localize("myaccountDelete") + "</a></li>";
    var conactsHtml = "";
    var progressMini = new scriptManager.Progress.prototype.Init($(containerId));
    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        url: "httphandlers/myaccount/getcustomercontacts.ashx",
        success: function (data) {
            if (data.length > 0) {
                $.each(data, function (idx, c) {
                    conactsHtml += String.Format(itemHtml, c.Country, c.Name, c.CountryId, c.RegionName, c.City, c.Address, c.Zip, c.CustomerContactID, containerId);
                });
                $(containerId).html("<ul class=\"list-adress\">" + conactsHtml + "</ul>");
            } else {
                $(containerId).html("<span class=\"note\">" + localize("myaccountNoAddress") + "</span>");
            }
        },
        beforeSend: function () {
            progressMini.Show();
        },
        complete: function () {
            progressMini.Hide();
        },
        error: function () {
        }
    });
}

function delContact(id, containerId) {

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        url: "httphandlers/myaccount/deletecustomercontact.ashx",
        data: { contactid: id },
        success: function () {
            getContactsForm(containerId);
        },
        error: function () {
        }
    });
}

function addUpdateContact(fio, countryid, country, region, city, address, postcode, contactid) {
    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        data: {
            fio: htmlEncode(fio),
            countryid: htmlEncode(countryid),
            country: htmlEncode(country),
            region: htmlEncode(region),
            city: htmlEncode(city),
            address: htmlEncode(address),
            zip: htmlEncode(postcode),
            contactid: htmlEncode(contactid)
        },
        url: "httphandlers/myaccount/addupdatecustomercontact.ashx",
        success: function () {
            mpAddress.modalClose();
            getContactsForm("#contactsDivForm");
        },
        error: function () {
            mpAddress.modalClose();
        }
    });
}

function open_printable_version(link) {
    var win = "menubar=no,location=no,resizable=yes,scrollbars=yes";
    var newWin = window.open(link, 'perintableWin', win);
    newWin.focus();
}

function changePayment(containerHistoryId, containerDetailsId, orderNumber) {
    var paymentId = $("#.paymentSelect option:selected").val();
    var paymentName = $("#.paymentSelect option:selected").text();

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        data: {
            orderNumber: orderNumber,
            paymentId: paymentId,
            paymentName: paymentName
        },
        url: "httphandlers/myaccount/ChangePaymentMethod.ashx",
        success: function () {
            showDetails(containerHistoryId, containerDetailsId, orderNumber);
        },
        error: function () {
            showOrdersHistory(containerHistoryId, containerDetailsId);
        }
    });
}

function showOrdersHistory(containerHistoryId, containerDetailsId) {
    $(containerDetailsId).hide();
    $(containerHistoryId).show();
    getOrderHistory(containerHistoryId, containerDetailsId);
}

function showDetails(containerHistoryId, containerDetailsId, orderNumber) {
    var headHtml =
            "<div class=\"order-id\"> \
                " + localize("myaccountOrder") + "{0} \
                <span class=\"status-order\">{1}</span> \
                <br /> <a href=\"#\" class=\"link-back-a\" onclick=\"showOrdersHistory('{3}', '{4}'); return false;\">" + localize("myaccountBackToOrderHistory") + "</a>\
                <a class=\"print\" href=\"javascript:open_printable_version('PrintOrder.aspx?OrderNumber={2}');\"/> \
            </div> \
            <div class=\"status-code\"> \
                <div class=\"status-check-info\"> \
                    " + localize("myaccountOrderStatus") + "<span class=\"code\">{2}</span></div> \
                {5} \
            </div> \
            <br class=\"clear\" />";

    var htmlOrderInf = "<div class=\"order-details\"> \
                        <ul class=\"order-details-list\"> \
                             <li> \
                                <div class=\"param-name\"> \
                                    " + localize("myaccountPayer") + " \
                                </div> \
                                <div class=\"param-value\"> \
                                    <div> \
                                        {0} \
                                    </div> \
                                    <div> \
                                        {1} <a href=\"#\" onclick=\"return false;\">{5}</a> \
                                    </div> \
                                </div> \
                             </li> \
                             <li> \
                                <div class=\"param-name\"> \
                                    " + localize("myaccountOrderShippingAddress") + " \
                                </div> \
                                <div class=\"param-value\"> \
                                    <div> \
                                        {2} \
                                    </div> \
                                </div> \
                             </li> \
                             <li> \
                                <div class=\"param-name\"> \
                                    " + localize("myaccountOrderShippingMethod") + " \
                                </div> \
                                <div class=\"param-value\"> \
                                    <div> \
                                        {3}</div> \
                                </div> \
                             </li> \
                             <li> \
                                <div class=\"param-name\"> \
                                    " + localize("myaccountOrderPaymentMethod") + " </div> \
                                <div class=\"param-value\"> \
                                    {4} \
                                </div> \
                             </li> \
                        </ul> \
                        <br class=\"clear\" /> \
                    </div";
    var addingPrices = "<tr> \
							<td class=\"fullcart-summary\" colspan=\"5\"> \
							<span class=\"fullcart-summary-text\">{0}:</span> \
							<span class=\"per\">{1}</span> \
							<span class=\"price\">{2}</span> \
							</td> \
						</tr>";
    var orderList =
            "<div class=\"order-list\"> \
				<div> \
					<table class=\"fullcart\"> \
						<thead><tr> \
							<th colspan=\"2\" class=\"fullcart-name\">" + localize("myaccountOrderProduct") + "</th> \
							<th class=\"fullcart-price\">" + localize("myaccountOrderPrice") + "</th> \
							<th class=\"fullcart-count\">" + localize("myaccountOrderAmount") + "</th> \
							<th class=\"fullcart-cost\">" + localize("myaccountOrderTotalProductPrice") + "</th> \
						</tr></thead><tbody> \
						{0} \
                        </tbody><tfoot>{1} \
						<tr class=\"footer\"> \
							<td colspan=\"4\"> \
								<div class=\"fullcart-comment-title\"> \
									" + localize("myaccountOrderComment") + "</div> \
								<div class=\"input-wrap order-comment\" data-disabled=\"true\"><textarea class=\"textarea\" disabled=\"disabled\" >{2}</textarea></div> \
							</td> \
							<td class=\"fullcart-summary fullcart-summary-alg\"> \
									<div class=\"fullcart-summary-bg\"> \
										<span class=\"fullcart-summary-text\"> \
										" + localize("myaccountOrderTotalPrice") + "</span> \
										<span class=\"price\">{3}</span> \
                                    </div> \
							</td> \
						</tr> </tfoot>\
					</table> \
				</div> \
			</div>";
    var itemOrderList =
            "<tr> \
				<td class=\"fullcart-photo-data\"> \
					<img src=\"{0}\" alt=\"{1}\" /> \
                </td> \
				<td class=\"fullcart-name-data\"> \
					<div> \
						{6}\
					</div> \
					<div class=\"sku\"> \
                        {2} \
                    </div> \
                    {7} \
				</td> \
				<td class=\"fullcart-price-data\"> \
					<span class=\"price\">{3}</span> \
                </td> \
				<td class=\"fullcart-count-data\"> \
                    {4} \
                </td> \
				<td class=\"fullcart-cost-data\"> \
					<span class=\"price\">{5}</span> \
                </td> \
			</tr>";


    var progressGetOrder = new scriptManager.Progress.prototype.Init($(containerDetailsId));
    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        url: "httphandlers/myaccount/getorderdetails.ashx?ordernumber=" + orderNumber,
        success: function (data) {
            var cancelButton = "<div class=\"order-cancel\"> \
                                    <a href=\"#\" class=\"link-remove-a\" onclick=\"cancelOrder('{0}','{1}','{2}','{3}'); return false;\">" + localize("myaccountCancelOrder") + "</a> \
                                </div> ";
            var resultHtml = String.Format(headHtml,
                data.OrderID,
                data.StatusName + (data.Payed ? " (" + localize("myaccountOrderPayed") + ")" : ""),
                data.Number,
                containerHistoryId,
                containerDetailsId,
                !data.Canceled && !data.Payed
                    ? String.Format(cancelButton, containerHistoryId, containerDetailsId, data.OrderID, data.Number)
                    : "");
            var payments = "";
            if (data.Payments != null && data.Payments.length > 0 && !data.Canceled && !data.Payed) {
                payments += String.Format("<select class=\"paymentSelect\" onchange=\"changePayment('{0}', '{1}', '{2}')\">", containerHistoryId, containerDetailsId, data.Number);
                $.each(data.Payments, function (idx, item) {
                    if (item.id == data.PaymentMethodId) {
                        payments += String.Format("<option value=\"{0}\" name=\"{1}\" selected=\"selected\">{1}</option>", item.id, item.name);
                    }
                    else {
                        payments += String.Format("<option value=\"{0}\" name=\"{1}\">{1}</option>", item.id, item.name);
                    }
                });
                payments += "</select>";
                payments += data.PaymentButton;
            } else {
                payments = data.PaymentMethodName;
            }

            if ($('#form').next().is("form")) {
                $('#form').next().remove();
            }

            $('#form').after(data.PaymentForm);

            resultHtml += String.Format(htmlOrderInf, data.BillingName, data.billingAddress, data.shippingInfo, data.ArchivedShippingName, payments, data.Email);
            var items = "";
            $.each(data.OrderItems, function (idx, item) {
                items += String.Format(itemOrderList,
                    item.Photo,
                    item.Name,
                    item.ArtNo,
                    item.Price,
                    item.Amount,
                    item.TotalPrice,
                    item.Url != ""
                        ? "<a href=\"" + item.Url + "\" class=\"link-pv-name\">" + item.Name + "</a>"
                        : "<span class=\"link-pv-name\">" + item.Name + "</span>",
                    item.CustomOptions);
            });
            var adding = "";
            adding += String.Format(addingPrices, localize("myaccountOrderSum"), "", data.ProductsPrice);
            if (data.TotalDiscount != 0) {
                adding += String.Format(addingPrices, localize("myaccountOrderDiscount"), "-" + data.TotalDiscount + "%", data.TotalDiscountPrice);
            }
            adding += String.Format(addingPrices, localize("myaccountOrderShippingPrice"), "", "+" + data.ShippingPrice);
            

            if (data.couponPrice != "") {
                adding += String.Format(addingPrices, localize("myaccountOrderCoupon"), data.couponPersent + "%", data.couponPrice);
            }
            if (data.Bonus != "") {
                adding += String.Format(addingPrices, localize("myaccountOrderBonus"), "" ,"-" + data.Bonus);
            }

            if (data.CertificatePrice != "") {
                adding += String.Format(addingPrices, localize("myaccountOrderCertificate"), "", data.CertificatePrice);
            }
            
            if (data.TaxesNames != "") {
                adding += String.Format(addingPrices, data.TaxesNames, "", data.TaxesPrice);
            }

            if (data.PaymentPrice != "") {
                adding += String.Format(addingPrices, data.PaymentPriceText, "", data.PaymentPrice);
            }

            if (data)
                resultHtml += String.Format(orderList, items, adding, data.CustomerComment, data.TotalPrice);
            $(containerDetailsId).html(resultHtml);
        },
        beforeSend: function () {
            $(containerDetailsId).show();
            $(containerHistoryId).hide();
            progressGetOrder.Show();
        },
        complete: function () {
            progressGetOrder.Hide();
            $(window).scrollTop(0);
        },
        error: function () {
        }
    });
}

function cancelOrder(containerId, containerDetailsId, orderid, ordernumber) {
    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        url: "httphandlers/myaccount/cancelorder.ashx?ordernumber=" + ordernumber,
        success: function () {
            if (ordernumber == null)
                getOrderHistory(containerId, containerDetailsId);
            else if (ordernumber != null) {
                showDetails(containerId, containerDetailsId, ordernumber);
            }
        },
        beforeSend: function () {

        },
        complete: function () {
        },
        error: function () {
        }
    });
}

function getOrderHistory(containerId, containerDetailsId) {

    var headerTable = "<div class=\"subtitle\">" + localize("myaccountOrderHistory") + "</div><table class=\"order-list\"><tr class=\"header\"><th>" + localize("myaccountOrderHistoryNumber") + "</th><th>" + localize("myaccountOrderHistoryStatus") + "</th><th>" + localize("myaccountOrderHistoryPayment") + "</th><th>" + localize("myaccountOrderHistoryShipping") + "</th><th class=\"price\">" + localize("myaccountOrderHistoryPrice") + "</th><th class=\"time\">" + localize("myaccountOrderHistoryDate") + "</th><th class=\"commands\"></th></tr>";
    var foterTable = "<tr class=\"footer\"> \
                        <td colspan=\"4\"></td> \
                        <td colspan=\"3\" class=\"sum\"> \
                            " + localize("myaccountOrderHistoryTotalPrice") + " \
                            <span class=\"price-result\">{0} <span class=\"note\">*</span></span> \
                            <br/><br/> \
                            <span class=\"block-note\"> <span class=\"note\">*</span> " + localize("myaccountOrderHistoryNote") + " </span>\
                        </td> \
                      </tr> \
                      </table>";
    var itemHtml = "<tr onclick=\"showDetails('{6}','{7}','{8}');\"><td>{0}</td><td><span class=\"state-end\">{1}</span></td><td>{2}</td><td>{3}</td><td class=\"price\">{4}</td><td>{5}<br/><div class=\"sku\">{9}</div></td><td class=\"commands\"> \
        <a href=\"javascript:void(0)\" class=\"order-more\" >" + localize("myaccountOrderHistoryMore") + "</a></td></tr>";
    var ordersHtml = "";

    var progressMini = new scriptManager.Progress.prototype.Init($(containerId));

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        url: "httphandlers/myaccount/getcustomerorderhistory.ashx",
        success: function (data) {
            if (data.Orders.length > 0) {
                $.each(data.Orders, function (idx, ord) {
                    ordersHtml += String.Format(itemHtml,
                        ord.OrderID,
                        ord.Status,
                        ord.ArchivedPaymentName,
                        ord.ShippingMethodName,
                        ord.Sum,
                        ord.OrderDate,
                        containerId,
                        containerDetailsId,
                        ord.OrderNumber,
                        ord.OrderTime
                    );
                });
                $(containerId).html(headerTable + ordersHtml + String.Format(foterTable, data.TotalSum));
            } else {
                $(containerId).html("<div class=\"subtitle\">" + localize("myaccountOrderHistory") + "</div><span class=\"note\">" + localize("myaccountOrderHistoryNoOrders") + "</span>");
            }
        },
        beforeSend: function () {
            progressMini.Show();
        },
        complete: function () {
            progressMini.Hide();
        },
        error: function () {
        }
    });
}
