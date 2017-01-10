

$(document).ready(function () {
    $('#txtPhone').mask('8 999 999 9999');
});

$(document).ready(function () {
    $('div.tabsHeader').on('click', 'div.tabItem', function () {
        $(this).addClass('active').siblings().removeClass('active');
        $('.tabsContent').find('.tabsContentItem').removeClass('activeTab').eq($(this).index()).addClass('activeTab');
        return false;
    });
    //Свернуть и развернуть текст описания страницы
    var descSection = $('.c-briefdescription');
    if (descSection.innerHeight() > 200) {
        descSection.append('<div class="more-separator"><a href="#" class="more-desc">Показать весь текст</a>');
        descSection.addClass('short-text');
    }

    $('.more-desc').click(function () {
        if (descSection.hasClass('short-text')) {
            descSection.removeClass('short-text');
            $(this).text('Свернуть текст');
        }
        else {
            descSection.addClass('short-text');
            $(this).text('Развернуть текст');
        }
        return false;
    });
});
//вкладки
$(document).ready(function () {
    $('div.tabsHeader').on('click', 'div.tabItem', function () {
        $(this).addClass('active').siblings().removeClass('active');
        $('.tabsContent').find('.tabsContentItem').removeClass('activeTab').eq($(this).index()).addClass('activeTab');
        return false;
    });

    $('.promoCodeExpander').click(function () {
        $(".bottomCartSectionHidden").slideToggle('fast');
        return false;
    });
});

function switchTab(tabclass) {

    $('div.tabItem.' + tabclass).click();
    return false;
}

//  кнопки сотритовки в каталоге
$(function () {
    try {
        $(".sortOrderList li a").click(function () {
            if ($(this).hasClass("NoSort")) {
                $(".sortOrderList li a").removeClass("DescSort");
                $(".sortOrderList li a").removeClass("AscSort");
                $(".sortOrderList li a").addClass("NoSort");

                $(this).removeClass("NoSort");

                $(this).addClass("DescSort");
                // Сортировка по цене  -сначала дешевые потом дорогие (this).addClass("DescSort");. А Новизна и популярность в другом порядке (.addClass("AscSort"))
                if($(this).attr("id")=="SortByPrice")
                {
                    $(this).removeClass("DescSort")
                    $(this).addClass("AscSort");
                }


            }
            else if ($(this).hasClass("DescSort")) {
                $(this).removeClass("DescSort");
                $(this).addClass("NoSort");
            }

            else if ($(this).hasClass("AscSort") && $(this).attr("id") == "SortByPrice") {
                $(this).removeClass("AscSort");
                $(this).addClass("DescSort");
            }
            else if ($(this).hasClass("DescSort") && $(this).attr("id") == "SortByPrice") {
                $(this).removeClass("DescSort");
                $(this).addClass("NoSort");
            }
            //выбирем нужный пункт в селекте и кликаем
            var sortFields = ["Новизне", "Цене", "Рэйтингу", "Популярности"];
            var sortValuesInSelect = ["ByAddingDate", "ByPrice", "ByPrice", "ByPopularity"];


            if ($(".DescSort").length == 1) {
                var activeSortField = ($(".DescSort").text());
                var indexSortField = sortFields.indexOf(activeSortField);
                //alert("Desc" + sortValuesInSelect[indexSortField]);
                $("#ddlSort").val("Desc" + sortValuesInSelect[indexSortField]);
            }
            else if ($(".AscSort").length == 1) {
                var activeSortField = ($(".AscSort").text());
                var indexSortField = sortFields.indexOf(activeSortField);

                //alert("Asc" + sortValuesInSelect[indexSortField]);
                $("#ddlSort").val("Asc" + sortValuesInSelect[indexSortField]);
            }
            else {
                $("#ddlSort").val("NoSorting");
            }
            $('#ddlSort').trigger('onchange');
        });
    }
    catch (e)
    { }
});

//выставляем стиль для кнопок сортировки после перзагрузки страницы
$(function () {
    try {
        var valuesInSelectAssociations = {};
        valuesInSelectAssociations.NoSorting = "";
        valuesInSelectAssociations.AscByAddingDate = "SortByAddingDate";
        valuesInSelectAssociations.DescByAddingDate = "SortByAddingDate";
        valuesInSelectAssociations.AscByName = "SortByName";
        valuesInSelectAssociations.DescByName = "SortByName";
        valuesInSelectAssociations.AscByPrice = "SortByPrice";
        valuesInSelectAssociations.DescByPrice = "SortByPrice";
        valuesInSelectAssociations.AscByPopularity = "SortByPopularity";
        valuesInSelectAssociations.DescByPopularity = "SortByPopularity";

        var activeOptionInSelect = $("#ddlSort").val();
        if (activeOptionInSelect != "NoSorting") {

            var activeSelect = $("#" + valuesInSelectAssociations[activeOptionInSelect]);
            if (activeOptionInSelect.indexOf("Desc") + 1) {
                activeSelect.removeClass("NoSort");
                activeSelect.addClass("DescSort");
            }
            else {
                activeSelect.removeClass("NoSort");
                activeSelect.addClass("AscSort");
            }

        }
    }
    catch (e) { }
});
// Для подгрузки катринок каталогов
//$(function () {
//    $(".tree-submenu-category").on("mouseleave", function () {
//        if ($(this).find(".categoryphotoinmenu").length) {
//            $(".categoryphotoinmenu").css("display", "none");
//            $(".categoryphotoinmenu").hide();
//        }
//    });


//    $(".tree-item-link ").on("mouseenter", function () {
//        $(".categoryphotoinmenu").hide();
//    });

//    $(".tree-submenu-column a").on("mouseover", function() {
//        var itemMenu = $(this);
//        if ($(this).parent().parent().find(".categoryphotoinmenu").length) {
//            $(".categoryphotoinmenu").show();
//        } else {
//            $(this).parent().append("<div class='categoryphotoinmenuwrap'>  <img class='categoryphotoinmenu' src='../images/nophoto_xsmall.jpg' alt=''> </div>")
//        }
//        $.ajax({
//            type: "POST",
//            url: 'httphandlers/design/uploadcategorypicture.ashx',
//            data: "categoryname=" + $(this).attr("href").split('/')[1],
//            success: function(data) {
//                itemMenu.parent().parent().find(".categoryphotoinmenu").attr("src", data);

//            }
//        });
//    });


//});


function showSkidka() {
    $(".bottomCartSectionHidden").slideToggle('fast');
    $(".cart-bonus-block").slideToggle('fast');
}



$(function () {

    if (window.DisplayFilter) {
        DisplayFilter(10, 10);
    }

    $("#brand-filter input, #property-filter input[type='checkbox'], #color-filter input, #size-filter input, #extra-filter input").click(function () {
        ApplyFilter(this, false, true, true);
    });

    $(".filterOnMain #property-filter select").change(function () {
        ApplyFilter2(this, false, true, true);
    });
    //$("#property-filter select").change(function () {
    //    ApplyFilter(this, false, true, true);
    //});

    $("#ddlCategory").change(function () {
        ApplyFilter(this, false, true, true);
    });


    if ($("a.zoom span.label-p").length) {
        $("a.zoom span.label-p").click(function () {
            return false;
        });
    }


    $(document.body).on("click", "a.btn.btn-disabled", function (e) {
        e = e || window.event;
        e.preventDefault();
        return false;
    });


    if ($("input[placeholder], textarea[placeholder]").length)
        $("input[placeholder], textarea[placeholder]").placeholder();


    $('div[data-rating-objid^=rating_]').each(function () {
        $(this).raty({
            hints: [localize("ratingAwfull"), localize("ratingBad"), localize("ratingNormal"), localize("ratingGood"), localize("ratingExcelent")],
            readOnly: $(this).hasClass("rating-readonly"),
            score: parseFloat($(this).next("input:hidden[data-rating-objid^=rating_hidden_]").val()),
            click: function (score, evt) {
                var newscore = VoteRating($(this).attr('data-rating-objid').replace("rating_", ""), score);
                if (newscore != 0) {
                    $(this).raty('score', newscore);
                } else {
                    $(this).raty('score', $(this).next("input:hidden[data-rating-objid^=rating_hidden_]").val());
                    notify(localize("ratingAlreadyVote"), notifyType.notify);
                }
                $(this).raty('readOnly', true);
            }
        });
    });


    if ($("ul.carousel-product:visible").length)
        $("ul.carousel-product:visible").jcarousel({ scroll: 1 });

    if ($("[data-plugin=fancybox]").length) {
        $("[data-plugin=fancybox]").fancybox({
            onComplete: function () {
                if ($.browser.msie && $.browser.version < 9) {
                    $('body').addClass('fancybox-videos-hide');
                }
            },
            onClosed: function () {
                if ($.browser.msie && $.browser.version < 9) {
                    $('body').removeClass('fancybox-videos-hide');
                }
            }
        });
    }

    if ($("div.slider").length) {

        $(".slider").each(function () {

            var min = parseFloat($(this).find("input.min").val());
            var max = parseFloat($(this).find("input.max").val());
            var slider = $(this);
            var curMin = parseFloat($(this).data("current-min"));
            var curMax = parseFloat($(this).data("current-max"));

            var step;
            var range = Math.abs(max - min);

            if (range == 0) {
                step = 0;
            } else if (range < 0.001) {
                step = 0.00001;
            }
            else if (range < 0.01) {
                step = 0.0001;
            }
            else if (range < 0.1) {
                step = 0.001;
            }
            else if (range < 1) {
                step = 0.01;
            }
            else if (range < 10) {
                step = 0.1;
            }
            else if (range < 100) {
                step = 1;
            }
            else if (range < 1000) {
                step = 1;
            }
            else if (range < 10000) {
                step = 1;
            }
            else {
                step = 1;
            }

            if (isNaN(min)) min = 0;
            if (isNaN(max)) max = 0;
            if (isNaN(curMin)) curMin = 0;
            if (isNaN(curMax)) curMax = 0;

            slider.data("prices", { from: curMin, to: curMax });

            slider.slider({
                range: true,
                min: min,
                max: max,
                step: step,
                values: [curMin, curMax],
                slide: function (event, ui) {
                    sladeMove.call(this, ui);
                    makeMagic($(this));
                },
                change: function (event, ui) {
                    sladeMove.call(this, ui);
                    ApplyFilter2(this, false, true, false);
                }
            });

            function sladeMove(ui) {
                slider.find("input.min").val(ui.values[0]);
                slider.find("input.max").val(ui.values[1]);
            }

            slider.find("input.min").val(slider.slider("option", "values")[0]);
            slider.find("input.max").val(slider.slider("option", "values")[1]);


            $("input.max, input.min", slider).on('keydown', function (e) {
                var code = e.keyCode,
                    result = true;

                if (e.altKey || e.ctrlKey || e.shiftKey) {
                    return false;
                }
                if (code > 95 && code < 106) { //numpad
                    code = code - 48;
                }
                if (code == 8 || code == 46 || e.keyCode == 110 || e.keyCode == 190 || e.keyCode == 39 || e.keyCode == 40) { //8 - backspace, 46 - delete, 110, 190 - dot, 39 - right, 40 -left, 
                    result = true;
                } else {
                    var symbol = Number(String.fromCharCode(code));
                    result = isNaN(symbol) === false;
                }
                return result;
            });

            $("input.min", slider).on("keyup", function () {
                var newMin = parseFloat($(this).val());
                var newMax = parseFloat($(this).next().val());

                if (newMin < min || newMin > curMax || isNaN(newMin) === true) {
                    return;
                }

                slider.slider({ values: [newMin, newMax] });
                makeMagic(slider);
            });


            $("input.max", slider).on("keyup", function () {
                var newMin = parseFloat($(this).prev().val());
                var newMax = parseFloat($(this).val());

                if (newMax > max || newMax < curMin || isNaN(newMax) === true) {
                    return;
                }

                slider.slider({ values: [newMin, newMax] });
                makeMagic(slider);
            });

            makeMagic(slider);
        });
    }

    if ($("table.avangard").length) {
        var rbs = $("table.avangard input:radio");
        var trs = $("table.avangard tr:not(.header)");
        $("table.avangard").click(function (e) {
            var tr = $(e.target).closest("tr:not(.header,.selected)");

            if (!tr.length) return;

            trs.removeClass("selected");
            tr.addClass("selected");

            var rb = tr.find("input:radio");
            if (!rb.length) return;
            rbs.removeAttr("checked");
            rb.attr("checked", "checked");
        });
    }

    if ($("input:checkbox.adress-payment").length) {
        $("input:checkbox.adress-payment").live("click", function () {
            var listP = $(this).closest("li").find("div.adress-payment");

            if (!listP.length) return false;

            listP.slideToggle();
            return true;
        });
    };

    if ($("table.pv-table").length) {
        var el, cell, tooltip, tooltipImg, tooltipPosition, path;
        var wrapperTooltip = "<div class='tooltip' id='pv-table-tooltip'><img src=\".\" /><div class='declare'></div></div>";

        $("table.pv-table tr:not(.head)").mouseenter(function (e) {

            cell = $(e.target).closest("tr").children("td.icon[abbr]");

            if (!cell.length) return;

            el = cell.find("div.photo");
            path = cell.attr("abbr");

            if (!path) return;

            if (!$("#pv-table-tooltip").length)
                $("body").append(wrapperTooltip);

            tooltip = $("#pv-table-tooltip");

            tooltipImg = tooltip.find("img").attr("src", path);

            tooltip.append(tooltipImg);

            tooltipImg.load(function () {

                tooltip.show();

                tooltipPosition = el.offset();

                tooltip.css({
                    top: tooltipPosition.top - 47,
                    left: tooltipPosition.left - (tooltip.outerWidth() + 13)
                });

            });
        });

        $("table.pv-table tr:not(.head)").mouseleave(function () {
            tooltip = $("#pv-table-tooltip");
            if (tooltip && tooltip.is(":visible")) {
                tooltip.hide();
            }
        });
    }

    $("#check-status a.btn").click(function () {
        var number = $("#check-status input").val();
        if (number.length) {
            var status = CheckOrder(htmlEncode(number.substr(0, 20)));
            if (status) {
                $("#orderStatus").text(localize("checkOrderState") + ": " + status.StatusName);
                if (status.StatusComment) {
                    $("#orderStatus").append("<br />" + localize("checkOrderComent") + ": " + status.StatusComment);
                }
                $("#orderStatus").show();
            } else {
                throw Error(localize("checkOrderError"));
            }
        }
        return false;
    });

    $(".tree-item:not(.tree-item-nosubcat), .tree-item-selected:not(.tree-item-nosubcat) ").hover(function (e) {
        var target = $(this);

        var submenu = target.find(".tree-submenu");
        if (!submenu.length) return true;

        submenu.removeClass("submenu-orientation").css({ left: "" });

        submenu.css('opacity', 0).show().animate({ opacity: 1 }, { duration: 150 });

        var windowWidth = $('body').width(),
            contentOffset = $('#tree').offset().left,
            contentWidth = $('#tree').width(),
            contentFull = contentOffset + contentWidth,
            submenuWidth = submenu.outerWidth(),
            submenuOffset = submenu.offset().left,
            submenuFull = submenuWidth + submenuOffset,
            dimContentSubmenu = submenuOffset - contentOffset,
            left = 0;


        if (windowWidth <= submenuWidth) {
            left = -submenuOffset;
        }
        else if (contentWidth < submenuWidth) {
            left = -(submenuOffset - contentOffset) - ((submenuWidth - contentWidth) / 2);
        }
        else if (contentFull < submenuFull) {
            left = -(submenuFull - contentFull);
        }

        submenu.css({ left: left });
    },
        function (e) {
            var target = $(this);
            var submenu = target.find(".tree-submenu");

            if (!submenu.length) return true;

            submenu.css('opacity', 1).animate({ opacity: 0 }, { duration: 150 });

            setTimeout(function () {
                submenu.hide();
            }, 150);

        });

    $("table.categories").click(function (e) {

        e.cancelBubble = true;
        var target = $(e.target);

        if (!target.is("td:not(cat-split)")) return true;

        var link = target.find("a").attr("href");

        if (!link || !link.length) return true;

        window.location.href = $("base").attr("href") + link;
    });

    var isCtrl = false;

    $(document).keyup(function (e) {
        if (e.keyCode == 17)
            isCtrl = false;
    });

    $(document).keydown(function (e) {
        if (e.keyCode == 17)
            isCtrl = true;

        //left arrow
        if (e.keyCode == 37 && isCtrl == true) {
            if ($("#paging-prev").length)
                document.location = $("#paging-prev").attr("href");
        }

        //right arrow
        if (e.keyCode == 39 && isCtrl == true) {
            if ($("#paging-next").length)
                document.location = $("#paging-next").attr("href");
        }
    });

    if ($(".btn-disabled").length) {
        var btn = $(".btn-disabled");
        if (btn.attr("onClick")) {
            btn.attr("onClickOld", btn.attr("onClick")).attr("onClick", "return false;");
        }
    }

    if ($("input.autocompleteRegion").length) {
        $("input.autocompleteRegion").autocomplete("HttpHandlers/GetRegions.ashx", {
            delay: 300,
            minChars: 1,
            matchSubset: 1,
            autoFill: true,
            matchContains: 1,
            cacheLength: 0,
            selectFirst: true,
            //formatItem: liFormat,
            maxItemsToShow: 10
        });
    }

    if ($("input.autocompleteCity").length) {
        $("input.autocompleteCity").autocomplete('HttpHandlers/GetCities.ashx', {
            delay: 300,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: 0,
            selectFirst: false,
            maxItemsToShow: 10
        });
    }

    if ($("input.autocompleteSearch").length) {
        $("input.autocompleteSearch").autocomplete('HttpHandlers/GetSearch.ashx', {
            delay: 300,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: 0,
            selectFirst: false,
            maxItemsToShow: 10,
            onItemSelect: function (li, $lnk, $input) {

                setTimeout(function () { window.location.assign($('base').attr('href') + $lnk.attr('href')); }, 1);

            }
        });
    }


    if ($("a.trialAdmin").length) {
        $.advModal({
            title: localize("demoMode"),
            control: $("a.trialAdmin"),
            isEnableBackground: true,
            htmlContent: localize("demoCreateTrial"),
            buttons: [
                { textBtn: localize("demoCreateNow"), isBtnClose: true, classBtn: "btn-confirm", func: function () { window.location = localize("trialUrl"); } },
                { textBtn: localize("demoCancel"), isBtnClose: true, classBtn: "btn-action" }
            ]
        });
    }

    var catAlt = $('.catalog-menu-root > li'),
        catAltItems = $('.catalog-menu-items');

    if (catAlt.length > 0 && catAltItems.length > 0) {

        if (catAltItems.closest(catAlt).length > 0) {
            catAlt.on('mouseenter', function () {
                if (catAltItems.is(':visible') !== true) {
                    catAltItems.show();
                }
            });

            catAlt.on('mouseleave', function () {
                if (catAltItems.is(':visible') === true) {
                    catAltItems.hide();
                }
            });

            catAltItems.on('mouseleave', function () {
                if (catAltItems.is(':visible') === true) {
                    catAltItems.hide();
                }
            });
        }



        catAltItems.children('.item.parent').on('mouseenter', function (e) {
            $(this).children('.tree-submenu').show();
        });
        catAltItems.children('.item.parent').on('mouseleave', function (e) {
            $(this).children('.tree-submenu').hide();
        });
    }


    if ($("#discountaction").length) {
        $.advModal({
            modalClass: "disc-actions",
            isEnableBackground: true,
            htmlContent: $("#discountaction")
        }).modalShow();
    }

    //modal for mail auth
    var modalMail = $("#modalMail");
    if (modalMail.length > 0) {
        $.advModal({
            title: localize("LoginEnterEmail"),
            htmlContent: modalMail,
            control: $("#lnkbtnMail"),
            forceShow: true,
            afterOpen: function () {
                $("#txtOauthUserId").focus();
            }
        });
    }
});

$(window).load(function () {
    if (window.notify) {
        notify(null, null, null, { showContainer: true });
    }

    var pvItems = $('.scp .scp-item');

    if (pvItems.length > 0 && $('html').hasClass('touch') !== true) {
        var pvStoragePhotos = {},
            carouselPattern = $('<div />', {
                'class': 'flexslider flexslider-carousel flexslider-pv',
                html: $('<ul />', {
                    'class': 'slides'
                })
            });

        if (pvItems.attr('data-display-previews') === 'true') {
            pvItems.on('mouseenter', function (e) {

                e.stopPropagation();
                e.preventDefault();

                var pvItem = $(this),
                    productid = pvItem.attr('data-productid'),
                    carousel = pvItem.find('.flexslider');

                if (pvItem.hasClass('loading') === true || productid == null || productid.length === 0) {
                    return;
                }

                var deffer = $.Deferred();

                if (pvStoragePhotos[productid] != null) {
                    deffer = generate(pvItem, productid);
                } else {
                    getProductPhotos(productid, pvItem);
                    deffer.resolve();
                }

            });

            pvItems.on('click', '.color-item', function (e) {

                e.stopPropagation();
                e.preventDefault();

                if ($(e.target).closest('.flexslider').length > 0) {
                    return;
                }

                var that = $(this),
                    parent = that.closest('.scp-item'),
                    scp = that.closest('[data-part="sizeColorPickerCatalog"]'),
                    productid = scp.attr('data-productid');

                var deffer = $.Deferred();

                if (pvStoragePhotos[productid] != null) {
                    generate(parent, productid);
                    deffer.resolve();
                } else {
                    deffer = getProductPhotos(productid, parent);

                }

                deffer.done(function () {
                    parent.find('.flexslider .slides li:eq(0)').trigger('click');
                    parent.find('.btn-add').attr('data-color-id', scp.attr('data-color-id'));
                });

            });
        }

        pvItems.find('.scp-img').on('saved.inplace', function (event, inplaceObj, responseInplace) {

            var imgMiddle = $(this),
                placeUpdate = imgMiddle.closest(pvItems),
                colorIdSelected = parseInt(placeUpdate.find('[data-part="sizeColorPickerCatalog"] .color-item.selected').attr('data-color-id')),
                productId = placeUpdate.attr('data-productid'),
                inplaceAttribute = {
                    'options': imgMiddle.attr('data-inplace-options'),
                    'params': imgMiddle.attr('data-inplace-params'),
                    'update': imgMiddle.attr('data-inplace-update'),
                };

            var inplaceParams = inplaceAttribute.params.replace(/'/g, '"');
            inplaceParams = JSON.parse(inplaceParams);

            getProductPhotos(productId, placeUpdate);

            var carouselItems = placeUpdate.find('.flexslider img');

            var img = carouselItems.filter('[data-photo-id="' + inplaceParams.id + '"]');

            var photoId;

            //#region inplace controls
            var inplaceControls = imgMiddle.attr('data-inplace-controls').replace(/'/g, '"');
            inplaceControls = JSON.parse(inplaceControls);

            var dataPrepare = [];

            if (isNaN(colorIdSelected) === false) {
                for (var i = 0, l = pvStoragePhotos[productId].length; i < l; i++) {
                    if (pvStoragePhotos[productId][i].ColorID === colorIdSelected || pvStoragePhotos[productId][i].ColorID == null) {
                        dataPrepare.push(pvStoragePhotos[productId][i]);
                    }
                }
            } else {
                dataPrepare = pvStoragePhotos[productId];
            }

            if (dataPrepare.length > 0 && img.length === 0) {
                //image length === 1

                for (var i = 0, l = dataPrepare.length; i < l; i++) {
                    if (dataPrepare[i].PathSmall == responseInplace) {
                        photoId = dataPrepare[i].PhotoId;
                        break;
                    }
                }

                inplaceControls.update = true;
                inplaceControls["delete"] = true;

            } else if (dataPrepare.length === 0 && img.length === 0) {
                photoId = -1;

                //nophoto
                inplaceControls.update = false;
                inplaceControls["delete"] = false;

            } else if (img.length > 0) {
                //image length > 1
                photoId = img.attr('data-photo-id');

                inplaceControls.update = true;
                inplaceControls["delete"] = true;
            }
            //#endregion

            inplaceControls = JSON.stringify(inplaceControls);
            inplaceControls = inplaceControls.replace(/"/g, '\'');

            imgMiddle.attr('data-inplace-controls', inplaceControls);

            if (imgMiddle.is('[data-plugin="inplace"]') === true) {


                if (inplaceAttribute.params != null) {

                    inplaceParams.id = photoId || productId;
                    inplaceParams.objId = productId;

                    inplaceParams = JSON.stringify(inplaceParams);
                    inplaceParams = inplaceParams.replace(/"/g, '\'');

                    imgMiddle.attr('data-inplace-params', inplaceParams);

                    var oldParams = imgMiddle.data('inplace').params;

                    imgMiddle.data('inplace', $.extend(oldParams, inplaceParams));
                }

                if (inplaceAttribute.options != null && inplaceAttribute.update != null) {

                    var inplaceOptions = inplaceAttribute.options.replace(/'/g, '"');
                    inplaceOptions = JSON.parse(inplaceOptions);

                    inplaceOptions.updateObj = [];

                    inplaceOptions.updateObj.push('ImageProduct' + (photoId || productId));
                    imgMiddle.attr('data-inplace-update', 'ImageProduct' + (photoId || productId));

                    inplaceOptions = JSON.stringify(inplaceOptions);
                    inplaceOptions = inplaceOptions.replace(/"/g, '\'');

                    imgMiddle.attr('data-inplace-options', inplaceOptions);
                }
            }

            Advantshop.ScriptsManager.Inplace.prototype.InitTotal(imgMiddle);
        });

        function getProductPhotos(productid, pvItem) {

            pvItem.addClass('loading');

            return $.ajax({
                url: 'httphandlers/details/getproductphotos.ashx',
                data: { productid: productid },
                dataType: 'json',
                async: false,
                cache: false,
                success: function (data) {
                    pvStoragePhotos[productid] = data;
                    generate(pvItem, productid);
                    pvItem.removeClass('loading');
                },
                error: function (data) {
                    throw Error(data.statusTextfcolor);
                }
            });
        }

        function generate(place, productid) {

            place.addClass('loading');

            var carousel = place.find('.flexslider'),
                data = pvStoragePhotos[productid],
                dataPrepare = [],
                colorIdSelected = parseInt(place.find('[data-part="sizeColorPickerCatalog"] .color-item.selected').attr('data-color-id')),
                colorIdCarousel = parseInt(carousel.attr('data-color-id')),
                imgWrapPattern = $('<div />', { 'class': 'flexslider-pv-wrap' }),
                ImagePreview = place.find('.scp-img'),
                imgPreviewPath = ImagePreview.attr('data-img-type') || 'Small';

            carousel.remove();

            if (isNaN(colorIdSelected) === false) {
                for (var i = 0, l = data.length; i < l; i++) {

                    if (data[i].ColorID === colorIdSelected || data[i].ColorID == null) {
                        dataPrepare.push(data[i]);
                    }
                }
            } else {
                for (var i = 0, l = data.length; i < l; i++) {
                    dataPrepare.push(data[i]);
                }
            }

            if (dataPrepare == null || dataPrepare.length < 2) {
                var imgsrc = 'images/nophoto_small.jpg';
                var imgId = -1;
                if (dataPrepare.length === 1) {
                    if (colorIdSelected != null) {
                        if (dataPrepare[0].ColorID === colorIdSelected) {
                            imgsrc = dataPrepare[0]['Path' + imgPreviewPath];
                            imgId = dataPrepare[0].PhotoId;
                        } else {
                            imgsrc = dataPrepare[0]['Path' + imgPreviewPath];

                            imgId = dataPrepare[0].PhotoId;
                        }
                    }
                }

                updateImage(ImagePreview, imgsrc, imgId);

                return;
            }

            carousel = carouselPattern.clone();

            place.find('[data-part="sizeColorPickerCatalog"]').attr('data-color-id', colorIdSelected);

            place.append(carousel);

            carousel.css({
                width: dataPrepare[0].XSmallProductImageWidth,
                left: -(dataPrepare[0].XSmallProductImageWidth + parseInt(carousel.css('paddingLeft')) + parseInt(carousel.css('paddingRight')) + parseInt(carousel.css('borderLeftWidth')) + parseInt(carousel.css('borderRightWidth')))
            });


            var imgTemp, imgWrapTemp;

            for (var key in dataPrepare) {

                imgTemp = $('<img />', {
                    'src': dataPrepare[key].PathXSmall + "?rnd=" + Math.random(),
                    'data-img-path': dataPrepare[key]['Path' + imgPreviewPath],
                    'data-photo-id': dataPrepare[key].PhotoId,
                    'alt': dataPrepare[key].Description
                });

                imgWrapTemp = imgWrapPattern.clone();

                imgWrapTemp.css({
                    'height': dataPrepare[key].XSmallProductImageHeight,
                    'width': dataPrepare[key].XSmallProductImageWidth
                });

                imgWrapTemp.html(imgTemp);

                if (colorIdSelected != null) {
                    if (colorIdSelected === dataPrepare[key].ColorID || dataPrepare[key].ColorID == null) {
                        carousel.find('.slides').append($('<li />', {
                            html: imgWrapTemp
                        }));
                    }
                }
                else {
                    carousel.find('.slides').append($('<li />', {
                        html: imgWrapTemp
                    }));
                }
            }

            carousel.flexslider({
                controlNav: false,
                animation: 'slide',
                move: 1,
                maxItems: 3,
                animationLoop: false,
                slideshow: false,
                direction: 'vertical',
                useCSS: false
            });

            var liItems = carousel.find('.slides li');

            liItems.first().addClass('selected');

            liItems.on('click touchstart', function (e) {

                e.stopPropagation();
                e.preventDefault();

                carousel.find('.slides li').removeClass('selected');

                $(this).addClass('selected');

                var img = $(this).find('img');

                updateImage(carousel.closest('.scp-item').find('.scp-img'), img.attr('data-img-path'), img.attr('data-photo-id'));

            });

            place.removeClass('loading');

            function updateImage(imgMiddle, newImageMiddlePath, imageMiddlePhotoId) {

                var productId = imgMiddle.closest(pvItems).attr('data-productid');
                var colorIdSelected = parseInt(imgMiddle.closest('.scp-item').find('[data-part="sizeColorPickerCatalog"] .color-item.selected').attr('data-color-id'));
                var photoId;

                imgMiddle.attr({
                    'src': newImageMiddlePath + '?' + Math.random()
                });

                if (imgMiddle.is('[data-plugin="inplace"]') === true) {

                    //#region inplace controls
                    var inplaceControls = imgMiddle.attr('data-inplace-controls').replace(/'/g, '"');
                    inplaceControls = JSON.parse(inplaceControls);

                    var dataPrepare = [];

                    if (isNaN(colorIdSelected) === false) {
                        for (var i = 0, l = pvStoragePhotos[productId].length; i < l; i++) {

                            if (pvStoragePhotos[productId][i].ColorID === colorIdSelected || pvStoragePhotos[productId][i].ColorID == null) {
                                dataPrepare.push(pvStoragePhotos[productId][i]);
                            }
                        }
                    } else {
                        dataPrepare = pvStoragePhotos[productId];
                    }

                    if (dataPrepare.length > 0) { // && img.length === 0
                        //image length === 1

                        photoId = imageMiddlePhotoId;

                        inplaceControls.update = true;
                        inplaceControls["delete"] = true;

                    } else if (dataPrepare.length === 0) { // && img.length === 0
                        //nophoto
                        photoId = -1;

                        inplaceControls.update = false;
                        inplaceControls["delete"] = false;

                    } else { //if (img.length > 0) 
                        //image length > 1
                        photoId = imageMiddlePhotoId;

                        inplaceControls.update = true;
                        inplaceControls["delete"] = true;
                    }
                    //#endregion

                    inplaceControls = JSON.stringify(inplaceControls);
                    inplaceControls = inplaceControls.replace(/"/g, '\'');

                    imgMiddle.attr('data-inplace-controls', inplaceControls);

                    var inplaceAttribute = {
                        'options': imgMiddle.attr('data-inplace-options'),
                        'params': imgMiddle.attr('data-inplace-params'),
                        'update': imgMiddle.attr('data-inplace-update'),
                    };

                    if (inplaceAttribute.params != null) {

                        var inplaceParams = inplaceAttribute.params.replace(/'/g, '"');
                        inplaceParamsObj = JSON.parse(inplaceParams);

                        inplaceParamsObj.id = photoId || productId;
                        inplaceParamsObj.objId = productId;

                        inplaceParams = JSON.stringify(inplaceParamsObj);
                        inplaceParams = inplaceParams.replace(/"/g, '\'');

                        imgMiddle.attr('data-inplace-params', inplaceParams);

                        var oldParams = imgMiddle.data('inplace').params;

                        imgMiddle.data('inplace', $.extend(oldParams, inplaceParamsObj));
                    }

                    if (inplaceAttribute.options != null && inplaceAttribute.update != null) {

                        var inplaceOptions = inplaceAttribute.options.replace(/'/g, '"');
                        inplaceOptions = JSON.parse(inplaceOptions);

                        inplaceOptions.updateObj = [];

                        inplaceOptions.updateObj.push('ImageProduct' + (photoId || productId));
                        imgMiddle.attr('data-inplace-update', 'ImageProduct' + (photoId || productId));

                        inplaceOptions = JSON.stringify(inplaceOptions);
                        inplaceOptions = inplaceOptions.replace(/"/g, '\'');

                        imgMiddle.attr('data-inplace-options', inplaceOptions);
                    }

                    Advantshop.ScriptsManager.Inplace.prototype.InitTotal(imgMiddle);
                }
            }
        }

    }
});

