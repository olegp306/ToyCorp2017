var mpAddressOC,
    bonusConfirmCardModal,
    bonusNewCardModal,
    scriptManager = Advantshop.ScriptsManager;


$(function () {

    var bonusConfirmHtml = $('#bonus-confirm').html();

    $(document).on('zone.changeCountry', function (e, data) {

        if ($(".delivery-change").length > 0) {
            setDelivery(data.current.CountryId, data.current.CountryName, data.current.Region, data.current.City);
        }
    });

    $(document).on('zone.changeCity', function (e, data) {

        if ($(".delivery-change").length > 0) {

            setDelivery(data.countryId, data.country, data.region, data.city);

            UpdateShipping();
        }
    });

    var timer;
    $('#txtCity').on('ac.select change', function (e) {
        if (e.type == 'change') {
            timer = setTimeout(function () {
                UpdateShipping();
            }, 300);
        } else {
            clearTimeout(timer);
            UpdateShipping();
        }
    });

    //var timerZip;
    //$('#txtZip').on('change', function (e) {
    //    if (e.type == 'change') {
    //        timerZip = setTimeout(function () {
    //            UpdateShipping();
    //        }, 300);
    //    } else {
    //        clearTimeout(timerZip);
    //        UpdateShipping();
    //    }
    //});

    $(".mask-date").inputmask("d.m.y", { "placeholder": localize("dateFormat"), clearMaskOnLostFocus: true })  // clearMaskOnLostFocus: false - kill validation on ff
        .focusin(function () {
            $(this).removeClass("mask-inp");
        }).focusout(function () {
            if ($(this).val() == "" || !$(this).inputmask("isComplete")) {
                $(this).addClass("mask-inp");
            } else {
                $(this).removeClass("mask-inp");
            }
        });

    if (localize("phoneFormat") != " ") {
        $(".mask-phone").inputmask(localize("phoneFormat"), { clearMaskOnLostFocus: true })
            .focusin(function () {
                $(this).removeClass("mask-inp");
            }).focusout(function () {
                var phone = $(this).val().replace(/\D+/g, '');
                if ($(this).inputmask("isComplete") || (phone.length > 1 && phone.length < 10)) {
                    $(this).removeClass("mask-inp");
                } else {
                    $(this).addClass("mask-inp");
                }
            });
    }


    mpAddressOC = $.advModal({
        control: "a.btn-add-adr-my",
        htmlContent: $("#modal"),
        title: localize("orderConfirnationAddress"),
        beforeClose: function () {
            $("form").data("validator").hideErrors();
        },
        afterOpen: function () { initValidation($("form"), "address"); }
    });

    $.advModal({
        control: ".js-oc-social",
        htmlContent: $(".js-oc-social-content")
    });


    $("#btnAddChangeContactOc").click(function () {
        if ($("form").valid("address")) {
            addUpdateContactForOc();
        } else {
            return false;
        }
    });

    $("#btnAddAddress").click(function () {
        ShowModalAddAddressOCAdd();
    });

    if ($('#chkBillingIsShipping').is(":checked")) {
        $('#pnBilling').hide();
    } else {
        $('#pnBilling').show();
    }

    initValidation($("form"));

    var spinboxTimer;

    $(".delivery-info").on('set.spinbox', ".tDistance", function () {
        var element = $(this);
        $("#hfDistance").val(element.val());

        $.ajax({
            dataType: "text",
            cache: false,
            type: "POST",
            data: { distance: element.val(), shipId: element.attr("data-id") },
            url: "httphandlers/orderconfirmation/GetWeightDistancePrice.ashx",
            success: function (data) {
                if (data != "") {
                    element.parents(".method-info").find(".cost").html(data);
                }
            }
        });

        if (spinboxTimer != null) {
            clearTimeout(spinboxTimer);
        }

        spinboxTimer = setTimeout(function () {
            UpdatePayment();
        }, 800);
    });


    if ($("#chbOldCustomer").is(":checked")) {
        $(".signin").show();
        $(".newcustomer").hide();
        initValidation($("form"));
    }


    $(".usertypes-b input").on("click", function () {
        if ($(this).val() == "0") {
            $(".newcustomer").show();
            $(".signin").hide();
        } else {
            $(".signin").show();
            $(".newcustomer").hide();
            initValidation($("form"));
        }
    });

    $("#chknewcustomer").on("click", function () {
        if ($(this).attr("checked")) {
            $(".newcustomer-hidden").show();
            initValidation($("form"));
        } else {
            $(".newcustomer-hidden").hide();
        }
    });


    if ($('#orderBasketBlock').length > 0) {
        var orderBasket = $('#orderBasketBlock');

        orderBasketFloating(orderBasket);
    }

    $(document).on("city_changed", function () {
        if ($(".delivery-info").length > 0) {
            UpdateShipping();
        }
    });

    if ($(".payments-info").length) {
        UpdatePayment();
    }

    $(".delivery-info").on("click", ".shipping-methods .method-item", function () {

        if ($(this).attr("data-shipping") == $("#_selectedID").val())
            return;

        if ($(this).find(".shipping-points").length > 0) {
            SetShippingPoint($(this).find(".shipping-points option:selected"));
        }

        if ($(this).find(".shipping-points-cdek").length > 0) {
            SetShippingPointCdek($(this).find(".shipping-points-cdek option:selected"));
        }

        if ($(this).find(".shipping-points-checkout").length > 0) {
            SetShippingPointCheckout($(this).find(".shipping-points-checkout option:selected"));
        }

        $("#_selectedID").val($(this).attr("data-shipping"));
        $(this).find(".checkbox input").attr("checked", "checked");

        UpdatePayment();
    });

    $(".delivery-info").on("change", ".shipping-points", function () {
        SetShippingPoint($(this).find("option:selected"));
        UpdatePayment();
    });

    $(".method-item .checkbox").on("change", function () {
        UpdatePayment();
    });

    $(".delivery-info").on("change", ".shipping-points-cdek", function () {
        SetShippingPointCdek($(this).find("option:selected"));
        UpdatePayment();
    });

    $(".delivery-info").on("change", ".shipping-points-checkout", function () {
        SetShippingPointCheckout($(this).find("option:selected"));
        UpdatePayment();
    });

    $(".payments-info").on("click", ".payment-methods .method-item", function () {

        $("#hfPaymentMethodId").val($(this).attr("data-payment"));
        $(this).find(".checkbox input").attr("checked", "checked");

        SetPaymentDetails($(this).attr("data-type"));

        UpdateBasket();
    });

    $("#contactsDivOc").on("click", ".shipping-contact li", function () {
        $(this).find("input").click();
    });


    $("#contactsDivOc").on("click", "input[type=radio]", function (e) {
        e.stopPropagation();

        if ($(this).find("input").is(':checked')) {
            return;
        }

        setContactShippingId($(this).closest("li").attr("data-contactId"), $(this).attr("id"));

        var contact = JSON.parse($(this).closest("li").attr("data-value"));
        setDelivery(contact.countryId, contact.country, contact.region, contact.city);

        UpdateShipping();
    });



    $("#contactsDivOc").on("click", ".address-edit-payment", function (e) {

        var contact = JSON.parse($(this).parent("li").attr("data-value"));

        $("#txtContactNameOc").val(contact.name);
        $("#cboCountryOc").val(contact.countryId);
        $("#txtContactZoneOc").val(contact.region);
        $("#txtContactCityOc").val(contact.city);
        $("#txtContactAddressOc").val(contact.address);
        $("#txtContactZipOc").val(contact.zip);
        $("#hfContactIdOc").val(contact.contactId);
        $("#btnAddChangeContactOc").text(localize("orderConfirnationChange"));

        mpAddressOC.modalShow();
        e.stopPropagation();
    });

    $("#contactsDivOc").on("click", ".link-remove-a", function (e) {

        var contactId = $(this).parents("li").attr("data-contactId");
        var container = $(this).attr("data-container");

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/myaccount/deletecustomercontact.ashx?contactid=" + contactId,
            success: function () {
                getContactsForOC(container);
                UpdateShipping();
            }
        });

        e.stopPropagation();
    });


    // bonuses
    if ($("#btnBonusConfirm").length > 0) {
        bonusConfirmCardModal = $.advModal({ modalId: "bonus-confirm-modal", modalClass: "bonus-modal", clickOut: false });
    }

    if ($("#btnAddBonusCard").length > 0) {
        bonusNewCardModal = $.advModal({ modalId: "bonus-confirm-modal-addcart", modalClass: "bonus-modal", clickOut: false });
    }

    $("#bonuses").on("click", "#chkBonus", function () {
        UpdateBasket();
    });

    $(".bonus-item .bonus-choice input").on("change", function (e) {

        var lbl = $(this).closest('.bonus-choice');

        SetBonusChoose(lbl);
    });

    $("#dvLoginPanel input").on("blur", function () {
        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/orderconfirmation/SaveCustomerData.ashx",
            data: {
                email: $("#txtEmail").val(),
                firstName: $("#txtFirstName").val(),
                lastName: $("#txtLastName").val(),
                patronymic: $("#txtPatronymic").val(),
                phone: $("#txtPhone").val(),
            },
        });
    });

    $("#btnBonusConfirm").on("click", function () {
        var cardnumber = $("#txtCardNumber").val().replace(/\D+/g, '');
        if (cardnumber == "")
            return;

        confirmCardByNumber(cardnumber);
    });

    $("#btnBonusConfirmPhone").on("click", function () {
        var phone = $("#txtPhoneCardNumber").val().replace(/\D+/g, '');
        if (phone == "")
            return;

        confirmCardByNumber(phone);
    });

    function confirmCardByNumber(cardnumber) {
        $("#bonus-confirm-modal .bonus-content-code input").val("");
        $("#bonus-confirm-modal .bonus-confirm-error").text("");

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
                    bonusConfirmCardModal.modalContent("<div class='modal-oc-error'>" + data.error + "</div>");
                    bonusConfirmCardModal.modalSetClickOut(true);
                    bonusConfirmCardModal.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);
                } else {
                    bonusConfirmCardModal.modalContent(bonusConfirmHtml);
                    bonusConfirmCardModal.modalSetClickOut(false);
                    bonusConfirmCardModal.modalBtns([]);
                }
                bonusConfirmCardModal.modalShow();
            },
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            }
        });
    }

    $("body").on("click", "#bonus-confirm-modal .bonus-confirm-code", function () {
        var code = $("#bonus-confirm-modal .bonus-content-code input").val();
        if (code == "")
            return;

        var progress = new scriptManager.Progress.prototype.Init($("#bonus-confirm-modal"));

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
                    $("#bonus-confirm-modal .content").html("<div class='modal-oc-success'>" + localize("bonusCardApplied") + "</div>");
                    bonusConfirmCardModal.modalSetClickOut(true);
                    bonusConfirmCardModal.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);
                    if (data.bonusText != "") {
                        ChangeBonusText(data.bonusText);
                    }
                } else {
                    $("#bonus-confirm-modal .bonus-confirm-error").text(data.error);
                }
            },
            error: function (data) {
                if (data != null && data.error == "") {
                    $("#bonus-confirm-modal .bonus-confirm-error").text(data.error);
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


    $("#btnAddBonusCard").on("click", function () {

        if (!$("#txtBonusPhone").inputmask("isComplete")) {
            $("#txtBonusPhone").val("");
        }
        if (!$("#txtBonusDate").inputmask("isComplete")) {
            $("#txtBonusDate").val("");
        }

        if (!$("#form").valid("mabonus")) {
            return;
        }
        var phone = $("#txtBonusPhone").val().replace(/\D+/g, '');

        $("#bonus-confirm-modal-addcart .bonus-confirm-error").text("");

        var progress = new scriptManager.Progress.prototype.Init($("#bonuses"));

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/bonuses/confirmcardbyphone.ashx",
            data: { phone: phone },
            success: function (data) {
                if (data.error != "") {
                    if (data.error == "phone_exist") {
                        $("#txtPhoneCardNumber").val(phone);
                        $("#card_yes").prop("checked", true);
                        SetBonusChoose($("#card_yes").parents(".bonus-choice"));

                        bonusNewCardModal.modalContent("<div class='modal-oc-error'>" + localize("myaccountBonusPhone") + " " + phone + " " + localize("myaccountBonusPhoneIsExist") + "</div>");
                        bonusNewCardModal.modalBtns([]);
                    } else {
                        bonusNewCardModal.modalContent("<div class='modal-oc-error'>" + data.error + "</div>");
                        bonusNewCardModal.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);
                    }
                    bonusConfirmCardModal.modalSetClickOut(true);
                } else {
                    bonusNewCardModal.modalContent(bonusConfirmHtml);
                    bonusNewCardModal.modalSetClickOut(false);
                }
                bonusNewCardModal.modalShow();
            },
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            }
        });
    });


    $("body").on("click", "#bonus-confirm-modal-addcart .bonus-confirm-code", function () {
        var code = $("#bonus-confirm-modal-addcart  .bonus-content-code input").val();
        if (code == "")
            return;

        var city = "";

        if ($(".delivery-change").length > 0) {
            var delivery = JSON.parse($(".delivery-change").attr("data-delivery"));
            city = delivery.city;
        } else if ($(".shipping-contact").length > 0) {
            var adress = JSON.parse($(".shipping-contact input:checked").parents(".shipping-contact li").attr("data-value"));
            city = adress.city;
        }

        var progress = new scriptManager.Progress.prototype.Init($("#bonus-confirm-modal-addcart"));

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
                email: $("#txtEmail").length > 0 ? $("#txtEmail").val() : "",
                city: city,
                addcart: "true",
                isCheckout: "true"
            },
            success: function (data) {
                if (data.error == "" && data.success == "true") {
                    $("#bonus-confirm-modal-addcart .content").html("<div class='modal-oc-success'>" + localize("bonusCardApplied") + "</div>");
                    bonusNewCardModal.modalSetClickOut(true);
                    bonusNewCardModal.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);
                    if (data.bonusText != "") {
                        ChangeBonusText(data.bonusText);
                    }
                } else {
                    $("#bonus-confirm-modal-addcart .bonus-confirm-error").text(data.error);
                }
            },
            error: function (data) {
                if (data != null && data.error != "") {
                    $("#bonus-confirm-modal-addcart .bonus-confirm-error").text(data.error);
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

    // registration
    $("body").on("click", "#bonus-confirm-modal .bonus-confirm-code-reg", function () {
        var code = $("#bonus-confirm-modal .bonus-content-code input").val();
        if (code == "")
            return;

        var progress = new scriptManager.Progress.prototype.Init($("#bonus-confirm-modal"));

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/bonuses/confirmbonuscode.ashx",
            data: {
                code: code
            },
            success: function (data) {
                if (data.error == "" && data.success == "true") {
                    $("#bonus-confirm-modal .content").html("<div class='modal-oc-success'>" + localize("bonusCardApplied") + "</div>");
                    bonusConfirmCardModal.modalSetClickOut(true);
                    bonusConfirmCardModal.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);

                    $("#bonuses .order-b-content").html(localize("bonusCardApplied"));

                } else {
                    $("#bonus-confirm-modal .bonus-confirm-error").text(data.error);
                }
            },
            error: function (data) {
                if (data != null && data.error == "") {
                    $("#bonus-confirm-modal .bonus-confirm-error").text(data.error);
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

    $("body").on("click", "#bonus-confirm-modal-addcart .bonus-confirm-code-reg", function () {
        var code = $("#bonus-confirm-modal-addcart  .bonus-content-code input").val();
        if (code == "")
            return;

        var progress = new scriptManager.Progress.prototype.Init($("#bonus-confirm-modal-addcart"));

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
                email: $("#txtEmail").length > 0 ? $("#txtEmail").val() : "",
                addcart: "true"
            },
            success: function (data) {
                if (data.error == "" && data.success == "true") {
                    $("#bonus-confirm-modal-addcart .content").html("<div class='modal-oc-success'>" + localize("bonusCardApplied") + "</div>");
                    bonusNewCardModal.modalSetClickOut(true);
                    bonusNewCardModal.modalBtns([{ textBtn: "Ok", isBtnClose: true, classBtn: 'btn-confirm btn-middle' }]);

                    $("#bonuses .order-b-content").html(localize("bonusCardApplied"));
                } else {
                    $("#bonus-confirm-modal-addcart .bonus-confirm-error").text(data.error);
                }
            },
            error: function (data) {
                if (data != null && data.error != "") {
                    $("#bonus-confirm-modal-addcart .bonus-confirm-error").text(data.error);
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

function UpdateShipping() {
    var countryId = "",
        region = "",
        city = $("#txtCity").length ? $("#txtCity").val() : "",
        country = $("#txtCountry").length ? $("#txtCountry").val() : "";

    if ($(".delivery-change").length > 0) {
        var geoData = JSON.parse($(".delivery-change").attr("data-delivery"));

        countryId = geoData.countryId;
        region = geoData.region;
        city = geoData.city;
    }

    var pickpointId = $("#pickpointId").val();
    var pickpointStringId = $("#pickpointStringId").val();
    var pickpointAddress = $("#pickAddress").val();
    var additionalData = $("#pickAdditional").val();
    var distance = $("#hfDistance").val();
    var zip = $("#txtZip").val();

    var contactId = $("#hfOcContactShippingId").length > 0 ? $("#hfOcContactShippingId").val() : "";

    var progress = new scriptManager.Progress.prototype.Init($(".delivery-info"));

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        data: {
            contactId: contactId,
            countryId: countryId,
            country: country,
            region: region,
            city: city,
            pickpointId: pickpointId,
            pickpointStringId: pickpointStringId,
            pickpointAddress: pickpointAddress,
            additionalData: additionalData,
            distance: distance,
            zip: zip
        },
        url: "httphandlers/orderconfirmation/getshipping.ashx",
        success: function (data) {
            var htmlItem = "<div class=\"method-item js-vis-item\" data-shipping=\"{0}\"> \
	                            <div class=\"checkbox\"> <input type=\"radio\" name=\"shippingchk\"{1} id=\"{0}\" onclick=\"{8}\"/> <label for=\"{0}\"></label></div> \
	                            <div class=\"shipping-img\"> {2} </div> \
	                            <div class=\"method-info\"> \
		                            <div class=\"method-name\"> {3} {4} </div> \
                                    <div class=\"method-descr-price\"> <span class='cost'>{5}</span> {6} </div> \
		                            <div class=\"method-descr\"> {7} </div> \
	                            </div> \
                            </div>",
                resulthtml = "",
                shippingBlock = $(".shipping-methods");

            if (data.ShippingRates != null && data.ShippingRates.length > 0) {
                $.each(data.ShippingRates, function (idx, item) {
                    resulthtml += String.Format(htmlItem, item.Id, data.SelectedShippingId == item.Id ? ' checked="checked"' : '',
                        item.Img, item.Name, item.Extanded, item.Price, item.DeliveryTime,
                        item.MethodDescription, item.Id == 1101 ? "shippingSelectChanged(true)" : "shippingSelectChanged(false)");
                });

                shippingBlock.addClass('oc-methods-freeze').css('height', shippingBlock.outerHeight());

                shippingBlock.html(resulthtml);

                var shippingVis = shippingBlock.closest('[data-plugin="vis"]');

                scriptManager.Vis.prototype.Init(shippingVis, Advantshop.Utilities.Eval(shippingVis.attr('data-vis-options')) || {});
                scriptManager.Spinbox.prototype.InitTotal();

                shippingBlock.removeClass('oc-methods-freeze').css('height', 'auto');
                $('#_selectedID').val(data.SelectedShippingId);

            } else {
                shippingBlock.html("<span class=\"oc-no-way\">" + localize('orderConfirnationShippingError') + "</span>");
                $('#_selectedID').val("");
            }

            SetShowShippingField(data.ShowAddress, data.ShowCustomFields);
        },
        beforeSend: function () {
            progress.Show();
        },
        complete: function () {
            progress.Hide();
            UpdatePayment();
        }
    });

}

function UpdatePayment() {

    var selectedCheck = $(".shipping-methods").find(".method-item .checkbox input:checked");

    var methodBlock = selectedCheck.parents(".method-item ");
    if (methodBlock.find(".hiddenCdekTariff").length) {
        SetShippingPointCdek(methodBlock);
    }
    else if (methodBlock.find(".hiddenCheckoutInfo").length) {
        SetShippingPointCheckout(methodBlock);
    }
    else if (methodBlock.find(".shipping-points option:selected").length) {
        SetShippingPoint(methodBlock.find(".shipping-points option:selected"));
    }

    var shippingId = $('#_selectedID').val();
    var paymentId = $('#hfPaymentMethodId').val();

    var contactId = $("#hfOcContactShippingId").length > 0 ? $("#hfOcContactShippingId").val() : "";

    var pickpointStringId = $("#pickpointStringId").val();

    var pickpointId = $('#pickpointId').val();
    var pickpointAddress = $("#pickAddress").val();
    var additionalData = $("#pickAdditional").val();

    var pickRate = $("#pickRate").val();

    var distance = $("#hfDistance").val();



    var progress = new scriptManager.Progress.prototype.Init($(".payments-info"));

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        data: {

            shippingId: shippingId,
            paymentId: paymentId,
            contactId: contactId,

            pickpointId: pickpointId,
            pickpointStringId: pickpointStringId,
            pickpointAddress: pickpointAddress,
            additionalData: additionalData,
            pickRate: pickRate,
            distance: distance
        },
        url: "httphandlers/orderconfirmation/getpayment.ashx",
        success: function (data) {
            if (data != null) {
                var htmlItem = "<div class=\"method-item js-vis-item\" data-payment=\"{0}\" data-type=\"{1}\"> \
	                                <div class=\"checkbox\"> <input type=\"radio\" name=\"paymentchk\"{2} id=\"{0}\"/> <label for=\"{0}\"></label> </div> \
	                                <div class=\"shipping-img\"> {3} </div> \
	                                <div class=\"method-info\"> \
		                                <div class=\"method-name\"> {4} </div> \
		                                <div class=\"method-descr\"> {5} </div> \
	                                </div> \
                                </div>",
                    resulthtml = "",
                    paymentBlock = $(".payment-methods");

                if (data.PaymentMethods != null && data.PaymentMethods.length > 0) {
                    $.each(data.PaymentMethods, function (idx, item) {
                        resulthtml += String.Format(htmlItem, item.Id, item.Type, item.Id == data.SelectedPaymentId ? ' checked="checked"' : '', item.Img, item.Name, item.Description);
                    });

                    paymentBlock.addClass('oc-methods-freeze').css('height', paymentBlock.outerHeight());
                    paymentBlock.html(resulthtml);

                    var paymentVis = paymentBlock.closest('[data-plugin="vis"]');

                    scriptManager.Vis.prototype.Init(paymentVis, Advantshop.Utilities.Eval(paymentVis.attr('data-vis-options')) || {});

                    paymentBlock.removeClass('oc-methods-freeze').css('height', 'auto');
                } else {
                    paymentBlock.html("<span class=\"oc-no-way\">" + localize('orderConfirnationPaymentError') + "</span>");
                }

                $("#hfPaymentMethodId").val(data.SelectedPaymentId);

                SetShowShippingField(data.ShowAddress, data.ShowCustomFields);
                SetPaymentDetails(data.PaymentType);
                SetBtnConfirm(data.IsValid);
            }
        },
        beforeSend: function () {
            progress.Show();
        },
        complete: function () {
            progress.Hide();

            UpdateBasket();
        },
        error: function () {
        }
    });
}

function UpdateBasket() {

    var progress = new scriptManager.Progress.prototype.Init($(".orderbasket"));

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        data: {
            paymentId: $('#hfPaymentMethodId').val(),
            bonuses: $("#chkBonus").is(":checked")
        },
        url: "httphandlers/orderconfirmation/getbasket.ashx",
        success: function (data) {
            if (data != null) {

                var result = "";

                result += "<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + localize('orderConfirnationOrderCost') + ":</div> <div class=\"orderbasket-row-cost\">" + data.TotalOrderPrice + "</div></div></div>";

                if (data.CertificatePrice != "") {
                    result += "<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + localize('orderConfirnationCertificate') + ":</div> <div class=\"orderbasket-row-cost\">" + data.CertificatePrice + "</div></div></div>";
                }

                if (data.CouponPrice != "") {
                    result += "<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + localize('orderConfirnationCoupon') + ":</div> <div class=\"orderbasket-row-cost\">" + data.CouponPrice + "</div></div></div>";
                }

                if (data.Discount != "") {
                    result += "<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + localize('orderConfirnationDiscount') + "(" + data.Discount + ")" + ":</div> <div class=\"orderbasket-row-cost\">-" + data.DiscountSum + "</div></div></div>";
                }

                if (data.Bonuses != "") {
                    result += "<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + localize('orderConfirnationBonuses') + ":</div> <div class=\"orderbasket-row-cost\">-" + data.Bonuses + "</div></div></div>";
                }

                if (data.ShippingPrice != "") {
                    result += "<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + localize('orderConfirnationDeliveryCost') + ":</div> <div class=\"orderbasket-row-cost\">" + (data.ShippingPrice >0 ? "+" : "") + data.ShippingPrice + "</div></div></div>";
                }

                if (data.Taxes != "") {
                    result += data.Taxes;
                }

                if (data.PaymentCost != "") {
                    result += "<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + data.PaymentCost + ":</div> <div class=\"orderbasket-row-cost\">" + data.PaymentExtraCharge + "</div></div></div>";
                }

                $(".js-oc-summary").html(result);
                $(".js-oc-total-sum-basket, .js-oc-total-sum-bottom").html(data.Total);

                $("#bonusPlus").remove();
                $(".oc-bonusplus-bottom").remove();

                if (data.BonusesPlus != "") {
                    $(".orderbasket-row-ex").after("<div id=\"bonusPlus\" class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">" + localize("orderConfirnationBonusPlus") + ":</div><div class=\"orderbasket-row-cost\">+" + data.BonusesPlus + "</div></div></div>");
                    $(".oc-total-price-bottom").after("<div class=\"oc-bonusplus-bottom\">" + localize("orderConfirnationBonusPlusBottom") + ": +<span class=\"oc-bonusplus-price-bottom\">" + data.BonusesPlus + "</span></div>");
                }

                SetBtnConfirm(data.IsValid);
            }
        },
        beforeSend: function () {
            progress.Show();
        },
        complete: function () {
            progress.Hide();
        },
    });
}

function SetShowShippingField(showDeliveryFields, showCustomShippingFields) {
    if (showDeliveryFields) {
        $(".deliveryfield").show();
    } else {
        $(".deliveryfield").hide();
    }

    if (showCustomShippingFields) {
        $(".cshipfield").show();
    } else {
        $(".cshipfield").hide();
    }

    if (!showDeliveryFields && !showCustomShippingFields) {
        $('.delivery-address').hide();
    } else {
        $('.delivery-address').show();
    }

}

function SetBtnConfirm(isValid) {
    if (isValid) {
        $("#btnConfirm").removeClass("btn-disabled");
    } else {
        $("#btnConfirm").addClass("btn-disabled");
    }
}

function SetPaymentDetails(details) {
    $("#pnlInfoForSberBank").hide();
    $("#pnlInfoForBill").hide();
    $("#pnlPhoneForQiwi").hide();

    if (details == "sberbank") {
        $("#pnlInfoForSberBank").show();
    }
    if (details == "bill") {
        $("#pnlInfoForBill").show();
    }
    if (details == "qiwi") {
        $("#pnlPhoneForQiwi").show();
    }
}

function SetPickPointAnswer(result) {
    $('#pickpointId').val(result['id']);
    $('#pickAddress').val(result['name'] + ', ' + result['address']);
    $('#pickAdditional').val("");

    $('.method-item .address-pickpoint').html(result['name'] + ', ' + result['address']);

    UpdatePayment();
}

function SetMultiShipAnswer(delivery) {

    if (typeof (delivery) === 'string') {
        delivery = (new Function("return " + delivery))();
    }

    $('#pickpointId').val(delivery.pickuppoint);
    $('#pickAddress').val(delivery.delivery_name + ', ' + delivery.address);

    var additionalData = {
        direction: delivery.direction,
        delivery: delivery.delivery,
        price: delivery.price,
        to_ms_warehouse: delivery.to_ms_warehouse
    };

    $('#pickAdditional').val(JSON.stringify(additionalData));

    var description = delivery.delivery_name + ', ';
    if (delivery.address != null) {
        description += delivery.address;
    }
    if (delivery.days != null && delivery.days != "") {
        description += ", " + delivery.days + " дн";
    }
    $('.method-item .address-multiship').html(description);
    $('.method-item .address-multiship').parents(".method-item").find(".method-descr-price").text(delivery.cost_with_rules + " руб.");

    UpdatePayment();
}

function SetShippingPoint(el) {
    var description = el.attr("data-description");
    var value = el.val();

    $('#pickpointId').val(value);
    $('#pickAddress').val(el.text());
    $('#pickAdditional').val("");
    $('#pickpointStringId').val("");

    el.parents(".method-info").find(".method-descr").html(description);
    el.parents(".method-info").find(".edost-map").attr("href", "http://www.edost.ru/office.php?c=" + value);
}

function SetShippingPointCdek(el) {
    if (el.find(".hiddenCdekTariff").length) {
        $('#pickAdditional').val(el.find(".hiddenCdekTariff").val());
    }

    var sippingPointsList = (el.find(".shipping-points-cdek option:selected"));
    if (sippingPointsList.length) {
        $('#pickpointId').val(sippingPointsList.val());
        $('#pickpointStringId').val(sippingPointsList.val());
        $('#pickAddress').val(sippingPointsList.text());
    } else {
        $('#pickpointId').val("0");
        $('#pickAddress').val("");
        $('#pickpointStringId').val("");
    }
}

function SetShippingPointCheckout(el) {
    if (el.find(".hiddenCheckoutInfo").length) {
        var additionalInfo = el.find(".hiddenCheckoutInfo").val();
        $('#pickAdditional').val(additionalInfo);
    }

    if (el.find(".hiddenCheckoutExpressRate").length) {
        var expressRate = el.find(".hiddenCheckoutExpressRate").val();
        $('#pickRate').val(expressRate);
    }

    var sippingPointsList = (el.find(".shipping-points-checkout option:selected"));
    if (sippingPointsList.length) {

        $('#pickpointId').val(sippingPointsList.val());
        $('#pickpointStringId').val(sippingPointsList.val());
        $('#pickAddress').val(sippingPointsList.attr("data-checkout-address"));
        $('#pickAdditional').val(sippingPointsList.attr("data-additional"));
        if (sippingPointsList.attr("data-rate").length) {
            $('#pickRate').val(sippingPointsList.attr("data-rate"));
            if (el.find(".cost").length && sippingPointsList.attr("data-full-rate").length) {
                el.find(".cost").html(sippingPointsList.attr("data-full-rate"));
            }

        }

    } else {
        $('#pickpointId').val("0");
        $('#pickAddress').val("");
        $('#pickpointStringId').val("");
    }
}

function ChangeBonusText(bonusText) {
    $("#bonuses .order-b-title").html(localize("orderConfirnationBonusGetDiscount"));
    $("#bonuses .order-b-content").html('<label><input type="checkbox" id="chkBonus" runat="server" /> ' + bonusText + '</label>');

    UpdateBasket();
}

function SetBonusChoose(el) {
    $(".bonus-content").each(function () {
        if (!$(this).hasClass("bonus-hidden")) {
            $(this).addClass("bonus-hidden");
        }
    });

    var item = el.parent(".bonus-item");
    if (item.find("#btnAddBonusCard") != null) {

        if ($("#txtLastName").length > 0 && item.find("#txtBonusLastName").val() == "") {
            item.find("#txtBonusLastName").val($("#txtLastName").val());
        }

        if ($("#txtFirstName").length > 0 && item.find("#txtBonusFirstName").val() == "") {
            item.find("#txtBonusFirstName").val($("#txtFirstName").val());
        }

        if ($("#txtPatronymic").length > 0 && item.find("#txtBonusSecondName").val() == "") {
            item.find("#txtBonusSecondName").val($("#txtPatronymic").val());
        }
        //if ($("#txtPhone").length > 0 && item.find("#txtBonusPhone").val() == "") {
        //    item.find("#txtBonusPhone").val($("#txtPhone").val());
        //}
    }

    el.next(".bonus-content").removeClass("bonus-hidden");
    initValidation($("form"));
}

function showHideBillingPanel(panel) {
    $(panel).toggle();
    initValidation($("form"));
}

function ShowModalAddAddressOCAdd() {
    $("#txtContactNameOc").val("");
    $("#txtContactZoneOc").val("");
    $("#txtContactCityOc").val("");
    $("#txtContactAddressOc").val("");
    $("#txtContactZipOc").val("");
    $("#txtContactPhoneOc").val("");
    $("#hfContactIdOc").val("");
    $("#btnAddChangeContactOc").text(localize("orderConfirnationAdd"));

}

function ShowModalAddAddressOC(fio, country, region, city, address, postcode, contactid) {
    $("#txtContactNameOc").val(fio);
    $("#cboCountryOc").val(country);
    $("#txtContactZoneOc").val(region);
    $("#txtContactCityOc").val(city);
    $("#txtContactAddressOc").val(address);
    $("#txtContactZipOc").val(postcode);
    $("#hfContactIdOc").val(contactid);
    $("#btnAddChangeContactOc").text(localize("orderConfirnationChange"));

    //    event = event || window.event;
    //    event.stopPropagation ? event.stopPropagation() : event.cancelBubble = true;
    mpAddressOC.modalShow();
}

function showHideAdressPayment() {
    var div = $(".adress-payment");
    if (div.is(":hidden")) {
        div.show();
        $("#hfBillingIsShippingOc").val("0");
        if ($('input[name=adrP]').length)
            $('input[name=adrP]').click();
    } else {
        $("#hfBillingIsShippingOc").val("1");
        div.hide();
        $("#hfContactBillingId").val("");
    }
}

function setContactShippingId(id, radioId) {
    $("#hfOcContactShippingId").val(id);
    $("#" + radioId).attr("checked", "checked");
}

function setDelivery(countryId, country, region, city) {
    var delivery = {
        countryId: countryId,
        country: country,
        region: region,
        city: city
    };

    var text = (delivery.country != "" ? delivery.country + ", " : "") +
               (delivery.region != "" && delivery.region != delivery.city ? delivery.region + ", " : "") +
               delivery.city;

    $(".delivery-change").attr("data-delivery", JSON.stringify(delivery));
    $(".delivery-change").text(text);
}

function getContactsForOC(containerId, selectedShippingContact) {
    var itemHtml = "<li data-contactId='{7}' data-value='{13}'> \
                        <div class=\"contact-inp\"><input type=\"radio\" name=\"adr\" id=\"a{8}\" {10}/> </div> \
                        <div class=\"contact-info\"> \
                           <div><span>" + localize('orderConfirnationReceiver') + ":</span> {1}</div> \
                           <div><span>" + localize('orderConfirnationAddress') + ":</span> {0}, {3}, {4}, {5}</div></div> \
                        <a class=\"link-edit-a address-edit-payment\" href=\"javascript:void(0)\">{11}</a> \
                        <a href=\"javascript:void(0)\" class=\"link-remove-a\" data-container=\"{9}\">{12}</a> \
                    </li>";
    var conactsHtml = "";

    var progressMini = new scriptManager.Progress.prototype.Init($(containerId));

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        async: false,
        url: "httphandlers/myaccount/getcustomercontacts.ashx",
        success: function (data) {
            if (data.length > 0) {
                var isFirst = true;
                $.each(data, function (idx, c) {

                    var contactData = {
                        name: c.Name,
                        country: c.Country,
                        countryId: c.CountryId,
                        region: c.RegionName,
                        city: c.City,
                        address: c.Address,
                        zip: c.Zip,
                        contactId: c.CustomerContactID
                    };

                    if (c.CustomerContactID == selectedShippingContact || isFirst) {
                        conactsHtml += String.Format(itemHtml, c.Country, c.Name, c.CountryId, c.RegionName, c.City, c.Address, c.Zip, c.CustomerContactID,
                                                               idx, containerId, "checked=\"checked\"",
                                                               localize("orderConfirnationEdit"), localize("orderConfirnationDelete"),
                                                               JSON.stringify(contactData));
                        isFirst = false;
                        setContactShippingId(c.CustomerContactID, idx);
                    }
                    else {
                        conactsHtml += String.Format(itemHtml, c.Country, c.Name, c.CountryId, c.RegionName, c.City, c.Address, c.Zip, c.CustomerContactID,
                                                               idx, containerId, "",
                                                               localize("orderConfirnationEdit"), localize("orderConfirnationDelete"),
                                                               JSON.stringify(contactData));
                    }
                });

                $(containerId).html(String.Format("<div class=\"order-b-address title\">{1}</div> <ul class=\"list-adress shipping-contact\">{0} </ul>",
                                  conactsHtml, localize("orderConfirnationShippingAddress")));
            }
            else {
                window.location.reload();
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

function addUpdateContactForOc(callback) {
    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        data: {
            fio: htmlEncode($("#txtContactNameOc").val()),
            countryid: htmlEncode($("#cboCountryOc :selected").val()),
            country: htmlEncode($("#cboCountryOc :selected").text()),
            region: htmlEncode($("#txtContactZoneOc").val()),
            city: htmlEncode($("#txtContactCityOc").val()),
            address: htmlEncode($("#txtContactAddressOc").val()),
            zip: htmlEncode($("#txtContactZipOc").val()),
            contactid: htmlEncode($("#hfContactIdOc").val())
        },
        url: "httphandlers/myaccount/addupdatecustomercontact.ashx",
        success: function () {
            mpAddressOC.modalClose();
            if (callback != null) {
                callback("#contactsDivOc");
            } else {
                getContactsForOC("#contactsDivOc");
            }
            UpdateShipping();
        },
        error: function (data) {
            notify(localize("orderConfirnationShippingErrorUpdate") + " status text:" + data.statusText, notifyType.error, true);
            mpAddressOC.modalClose();
        }
    });
}

function getPaymentButton(orderId, containerId) {

    var progress = new scriptManager.Progress.prototype.Init($("#" + containerId));

    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        data: {
            orderid: htmlEncode(orderId)
        },
        url: "httphandlers/orderconfirmation/getpaymentbutton.ashx",
        success: function (data) {
            if (data != null) {
                var formSubmit = $(data.formString);

                if (formSubmit.length != 0) {
                    $('#form').after(formSubmit);
                    if (data.proceedToPayment) {
                        setTimeout(function () {
                            formSubmit[0].submit();
                        }, 4000);
                    } else {
                        $("#" + containerId).html(data.buttonString);
                    }
                } else if (data.buttonString != null && data.buttonString != "") {
                    $("#" + containerId).html(data.buttonString);
                    if (data.proceedToPayment) {
                        try {
                            setTimeout(function() {
                                //$("#" + containerId + " a").click();
                                window.location = $("#" + containerId + " a").attr("href");
                            }, 4000);
                        } catch (e) {}
                    }
                }
            }
        },
        beforeSend: function () {
            progress.Show();
        },
        complete: function () {
            progress.Hide();
        }
    });
}

function formGenerate(form) {
    var container = $("#btnPaymentFunctionality");

    if (!container.length) return;
    container.html(form);
}

function UpdateCustomerEmail() {
    if ($("form").valid("email")) {
        var res = false;
        $.ajax({
            type: "POST",
            url: "httphandlers/myaccount/updatecustomeremail.ashx",
            data: { email: $("#customerEmail").val() },
            dataType: "json",
            async: false,
            cache: false,
            error: function (data) {
                throw new Error(data);
            },
            success: function (data) {
                res = data;
            }
        });
    }
    return res;
}

function orderBasketFloating(obj) {
    var win = $(window),
        scrollTop,
        ocCartBlock = $('.oc-cart-data'),
        ocContent = $(".oc-content"),
        orderBasketOffset = ocCartBlock.offset(),
        staticPosX = orderBasketOffset.left,
        staticPosY = orderBasketOffset.top,
        staticPosEndY = (ocContent.offset().top + ocContent.height()) - obj.height();


    win.on('resize', function () {
        obj.css('left', ocCartBlock.offset().left);
    });

    win.on('scroll', function () {
        staticPosEndY = (ocContent.offset().top + ocContent.height()) - obj.height();
        scrollTop = win.scrollTop();
        staticPosX = ocCartBlock.offset().left;

        if (scrollTop > staticPosY && scrollTop < staticPosEndY) {
            obj.addClass('orderbasket-fixed').css('left', null);
            obj.removeClass('orderbasket-absolute');
        }
        else if (scrollTop > staticPosY && scrollTop > staticPosEndY) {
            obj.removeClass('orderbasket-fixed').css('left', '');
            obj.addClass('orderbasket-absolute');
        }
        else {
            obj.removeClass('orderbasket-fixed').css('left', staticPosX);
            obj.removeClass('orderbasket-absolute');
        }
    });
}

function shippingSelectChanged(param) {
    if (param == 1101) {
        $("#divDisplayAddress").css("display", "none");
    } else {
        $("#divDisplayAddress").css("display", "block");
    }
}
$(document).ready(function () {
    if ($("#1101").attr("checked") == 'checked') {
        $("#divDisplayAddress").css("display", "none");
    } else {
        $("#divDisplayAddress").css("display", "block");
    }
});