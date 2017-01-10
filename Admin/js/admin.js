﻿function clickButton(e, buttonid) {
    var evt = e ? e : window.event;
    var bt = document.getElementById(buttonid);

    if (bt) {
        if (evt.keyCode == 13) {
            bt.click();
            return false;
        }
    }
}

function keyboard_navigation(D) {
    D = D || window.event;
    var A = D.keyCode;
    if (D.ctrlKey) {
        if (A == 13) {
            //Enter
        }
        var E = (D.target || D.srcElement).tagName;
        if (E != "INPUT" && E != "TEXTAREA") {
            var C;
            if (A == 37) {
                C = document.getElementById("prevpage");
            }
            if (A == 39) {
                C = document.getElementById("nextpage");
            }
            if (C && C != undefined && C.href != undefined) {
                //C.onclick();
                location.href = C.href;
            }
            if ((A == 38) && B.text) {
                B.text.focus();
                B.text.select();
            }
        }
    }
}

function open_window(link, w, h) {
    var xOffset = screen.availWidth / 2 - w / 2;
    var yOffset = screen.availHeight / 2 - h / 2; //(opener.outerHeight *2)/10;

    xOffset = xOffset > 0 ? xOffset : 0;
    yOffset = yOffset > 0 ? yOffset : 0;

    var win = "width=" + w + ",height=" + h + ",menubar=no,location=no,resizable=yes,scrollbars=yes,left=" + xOffset + ",top=" + yOffset;
    wishWin = window.open(link, 'wishWin', win);
    wishWin.focus();
}

function open_printable_version(link) { var win = "menubar=no,location=no,resizable=yes,scrollbars=yes"; newWin = window.open(link, 'printableWin', win); newWin.focus(); }
//function TextBoxKeyPress() {
//    if (document.all) {
//        if (event.keyCode == 13) {
//            event.returnValue = false;
//            event.cancel = true;
//            var btn;
//            var btns = $("input.btn");
//            for (var i = 0; i < btns.length; i++)
//                if (btns[i].value == "Filter") {
//                btn = btns[i];
//                break;
//            }
//            btn.click();
//        }
//    }
//}

function translite(src) {
    var res = "";
    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        async: false,
        data: {
            source: src
        },
        url: "httphandlers/translit.ashx",
        success: function (data) {
            res = data;
        },
        error: function (data) {
            alert("can't translite '" + src + "'");
        }
    });
    return res;
}

$(function () {
    tabInit();

    decodeFormData();


    $(document.body).on('click', '.js-ajax-button', function () {

        var btn = $(this);

        if (btn.hasClass('btn-disabled')) {
            return;
        }

        btn.siblings('.js-ajax-inline').removeClass('ajax-inline-hidden');
        btn.addClass('btn-disabled');

    });


    var messageError = $('.js-error-message');

    if (messageError.length > 0 && messageError.text().length > 0) {
        setTimeout(function () { messageError.fadeOut(700, function () { messageError.empty(); }) }, 5000);
    }

    if ($("input.autocompleteRegion").length) {
        $("input.autocompleteRegion").autocomplete("../HttpHandlers/GetRegions.ashx", {
            delay: 300,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: 10,
            selectFirst: false,
            maxItemsToShow: 10
        });
    }

    if ($("input.autocompleteCity").length) {
        $("input.autocompleteCity").autocomplete('../HttpHandlers/GetCities.ashx', {
            delay: 300,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: 10,
            selectFirst: false,
            maxItemsToShow: 10
        });
    }

    $(".main-menu > ul > li.main-menu-item").each(function () {

        var show_parent = false;

        $(this).children("ul").children(".m-hide").each(function () {

            if ($(this).children("ul").children(".m-item").length > 0) {

                $(this).removeClass("m-hide");
                show_parent = true;
            }
        });

        if (!show_parent) {
            show_parent = $(this).children("ul").children(".m-item").length > 0;
        }

        if (show_parent && $(this).hasClass("m-hide")) {
            $(this).removeClass("m-hide");
        }
    });



    $(document).on('keydown.pagenumber', function (e) {
        //37 - left arrow
        //39 - right arrow
        if (e.ctrlKey === true && e.keyCode === 37) {
            if ($("#paging-prev").length)
                document.location = $("#paging-prev").attr("href");
        } else if (e.ctrlKey === true && e.keyCode === 39) {
            if ($("#paging-next").length)
                document.location = $("#paging-next").attr("href");
        }
    });


    $(".tpl-save-btn").click(function () {

        var settings = [];

        $(".tpl-settings-control").each(function () {
            var control = $(this).children();

            if (control.is("input[type='checkbox']")) {
                settings.push(control.attr("id") + "~" + (control.is(":checked") ? "True" : "False"));
            } else {
                settings.push(control.attr("id") + "~" + control.val());
            }
        });

        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { settings: settings },
            url: "httphandlers/settings/savetemplatesettings.ashx",
            success: function (data) {
                $(".tpl-settings-result").html(data.result);
            },
            error: function (data) {
                $(".tpl-settings-result").html(data.result);
                this.location = this.location;
            }
        });
    });

    $(".lnk-del-order").on("click", function (e) {
        var result = confirm($(this).attr("data-message"));
        if (result === false) {
            e.preventDefault();
        }
        e.stopPropagation();
    });
    //#region product properties

    $("body").on("click", ".dp input, .dp .icon-calendar", function (e) {
        var datepickerInput = $(this).is("input") ? $(this) : $(this).prev("input"),
            datePicker = datepickerInput.data("datepicker");

        if (!datePicker) {
            datePicker = datepickerInput.datepicker().data("datepicker");
        }

        datePicker.show();

        //  datePicker.on('changeDate', function () {
        //     $(this).datepicker('hide');
        //  });
    });
    

    $("body").on("click", ".change-group-lnk", function (e) {
        $(".btn-change-group").click();
        $("#SelectedIds").val("");
        e.stopPropagation();
    });

    $("body").on("change", "#change-group-modal select", function () {
        $(".hfgroupId").val($(this).val());
    });

    $("body").on("click", ".property-item-values input", function() {

        var selectedvals = $(".hfpropselected").val().split(","),
            value = $(this).val(),
            index = selectedvals.indexOf(value),
            checked = $(this).is(':checked');
        
        if (index >= 0) {
            if (!checked) {
                selectedvals.splice(index, 1);
            }
        } else if (checked) {
            selectedvals.push(value);
        }

        $(".hfpropselected").val(selectedvals.join(","));
    });

    $("body").on("click", ".expander-lnk", function () {

        if (!$(this).hasClass("expanded")) {
            
            if (!$(this).hasClass("loaded")) {

                var propertyId = $(this).parent(".property-item-values").attr("data-property");
                loadProperties(propertyId, $(this));

            } else {
                $(this).parent().children(".propval-item").removeClass("propval-hidden");
            }

            $(this).text(localize("Collapse"));
            $(this).addClass("expanded");


        } else {

            $(this).text(localize("Expande"));
            $(this).removeClass("expanded");
            var items = $(this).parent().children(".propval-item");
            if (items.length > 10) {
                items.slice(10).addClass("propval-hidden");
            }
        }
    });

    $("body").on("click", ".valid-confirm", function (e) {

        e.stopPropagation();

        var confirmText = $(this).attr("data-confirm");
        if (confirmText == null) {
            confirmText = localize('Admin_QDelete');
        }

        if (!confirm(confirmText)) {
            e.preventDefault();
            return false;
        }
    });

    //#endregion
});

function loadProperties(propertyId, expandEl) {
    $.ajax({
        dataType: "text",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: {
            propertyId: propertyId,
            productId: $(".hfpropproductId").val()
        },
        url: "httphandlers/product/loadproperties.ashx",
        success: function (data) {
            if (data != null) {
                expandEl.before(data);
                expandEl.addClass("loaded");
            }
        }
    });
}

var modalGroups;
function showModalGroups(e) {
    if (modalGroups == null) {
        modalGroups = $.advModal({
            modalId: "change-group-modal",
            title: localize('ChangePropertyGroup'),
            htmlContent: $(".change-group-b").html(),
            buttons: [{ textBtn: localize('Save'), isBtnClose: false, classBtn: 'btn-action change-group-lnk' },
                      { textBtn: localize('Close'), isBtnClose: true, classBtn: 'btn-action' }]
        });
    } else {
        modalGroups.modalContent($(".change-group-b").html());
    }
    modalGroups.modalShow();
    e.stopPropagation();
}

function tabInit() {
    if ($("#tabs").length) {
        $("#tabs").advTabs({
            headers: "#tabs-headers > li",
            contents: "#tabs-contents  div.tab-content"
        });

        $("select, input, textarea", "#tabs-contents").change(function (e) {
            e = e || window.event;
            var tabContent = $(e.target).closest("div.tab-content");
            var tabdata = $("#tabs").data("tabs");
            var idx = $(tabdata.contents).index(tabContent);

            var tabHeader;
            if (idx != -1) {
                tabHeader = $(tabdata.headers[idx]);
                tabHeader.find("img.floppy").fadeIn();
            }
        });
    }
}

function isNumber(n) {
    n = n.replace(/,/, ".");
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function saveProperties(event) {

    var values = [];
    var selectedvals = $(".hfpropselected").val().split(",");

    for (var i = 0; i < selectedvals.length; i++) {
        values.push(selectedvals[i]);
    }

    $(".hfpropresult").val(values.join(","));


    var hasErrors = false,
        newValues = [];

    $(".propval-newitem input").each(function () {

        $(this).removeClass("novalid");

        var propertyValue = {
            PropertyId: $(this).parents(".property-item-values").attr("data-property"),
            Value: $(this).val().trim()
        };

        if (propertyValue.Value != "" && $(this).attr("class") == "valtype-number") {
            if (!isNumber(propertyValue.Value)) {
                hasErrors = true;
                $(this).addClass("notvalid");
            }
        }

        if (!hasErrors && propertyValue.Value != "") {
            newValues.push(propertyValue);
        }
    });

    $(".hfnewpropresult").val(JSON.stringify(newValues));

    if (hasErrors) {

        // show message and cancel saving
        alert(localize("PropertyValueNumberError"));

        event = event || window.event;

        if (event.preventDefault) {
            event.preventDefault();
        } else {
            event.returnValue = false;
        }
    }
}

function copyProduct(event) {
    if (!confirm("Вы действительно хотите создать копию товара?")) {
        return;
    }

    event = event || window.event;

    if (event.preventDefault) {
        event.preventDefault();
    } else {
        event.returnValue = false;
    }

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { productId: getURLParameter("ProductID") },
        url: "httphandlers/catalog/CopyProduct.ashx",
        success: function (data) {
            if (data.Ok == true) {
                window.location.assign(data.Link);
                //notify(String.Format(localize("Admin_Product_CopySuccess"), data.Link), notifyType.notify);
            } else {
                notify(localize("Admin_Product_CopyError"), notifyType.error);
            }
        },
        error: function (data) {
            notify(localize("Admin_Product_CopyError"), notifyType.error);
        }
    });

    event.stopPropagation();
    return false;
}

function getURLParameter(name) {
    return decodeURI(
        (RegExp(name.toLowerCase() + '=' + '(.+?)(&|$)').exec(location.search.toLowerCase()) || [, null])[1]
    );
}

function SearchProduct() {
    if ($("input.autocompleteSearch").length) {
        $("input.autocompleteSearch").autocomplete('httphandlers/order/productssearch.ashx', {
            delay: 15,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: null,
            selectFirst: false,
            maxItemsToShow: 30,
            onItemSelect: function (li, $lnk, $input) {
                if ($(".acsearchhf").length > 0) {
                    var indexOf = li.innerHTML.indexOf("<span");
                    var offer = $(li).find("span").attr("data-offerid");

                    if (offer != null) {
                        $(".acsearchhf").val(offer);
                    }
                    $input.val(li.innerHTML.substring(0, indexOf));
                }
            }
        });
    }
}