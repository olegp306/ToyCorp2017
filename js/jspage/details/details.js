/*carousel details*/
(function ($) {

    var carouselDetails = new function () {
        var instance;

        function carouselDetails(el, storageOffers) {
            if (!instance) {

                this.$obj = $(el);
                this.storageOffers = storageOffers;
                this.Init();
                this.Filter();
                instance = this;

                return instance;
            } else {
                return instance;
            }
        }

        carouselDetails.prototype.Init = function () {
            var carouselDetailsObj = this;

            carouselDetailsObj.carouselData = carouselDetailsObj.$obj.data('flexslider');

            if (carouselDetailsObj.carouselData == null) {
                var opts = Advantshop.Utilities.Eval(carouselDetailsObj.$obj.attr('data-flexslider-options')) || {};
                carouselDetailsObj.carouselData = carouselDetailsObj.$obj.flexslider(opts).data('flexslider');
            }

            if (carouselDetailsObj.carouselData != null && carouselDetailsObj.carouselData.repository == null) {

                carouselDetailsObj.carouselData.repository = [];

                var itemTemp, attrtemp, objTemp = {};
                for (var i = 0, l = carouselDetailsObj.carouselData.slides.length; i < l; i += 1) {

                    itemTemp = carouselDetailsObj.carouselData.slides.eq(i);
                    attrtemp = itemTemp.attr('data-color-id');

                    if (attrtemp == null || attrtemp.length === 0) {
                        objTemp = { type: 'any', obj: itemTemp };
                    } else {
                        objTemp = { type: 'color', obj: itemTemp };
                    }

                    carouselDetailsObj.carouselData.repository.push(objTemp);
                }

                carouselDetailsObj.$obj.data('flexslider', carouselDetailsObj.carouselData);
            }
        };

        carouselDetails.prototype.Filter = function () {
            var carouselDetailsObj = this, itemTemp;

            if (carouselDetailsObj.storageOffers.Offers.length === 0) {
                return;
            }


            carouselDetailsObj.carouselData.clear();

            /*add items carousel*/

            for (var a = 0, arrAddLength = carouselDetailsObj.carouselData.repository.length; a < arrAddLength; a += 1) {
                itemTemp = carouselDetailsObj.carouselData.repository[a];
                if (itemTemp.type === 'any') {
                    carouselDetailsObj.carouselData.addSlide(itemTemp.obj);
                } else if (itemTemp.type === 'color' && carouselDetailsObj.storageOffers.ColorIdSelected === parseInt(itemTemp.obj.attr('data-color-id'))) {
                    carouselDetailsObj.carouselData.addSlide(itemTemp.obj);
                }
            }

            carouselDetailsObj.$obj.trigger('carouselDetailsFilter', [carouselDetailsObj]);
        };

        carouselDetails.prototype.Update = function (storageOffers) {

        };

        return carouselDetails;
    };

    Advantshop.NamespaceRequire('Advantshop.Details.Part');
    Advantshop.Details.Part.CarouselDetails = carouselDetails;

})(jQuery);

/*init page*/
(function ($, $doc) {
    $(function () {
        $('#zoom').removeClass('cloud-zoom-progress');

        var hfProductId = document.getElementById('hfProductId');

        if (hfProductId == null || hfProductId.getAttribute('data-page') !== 'details' || isNaN(hfProductId.value) === true) {
            return;
        }

        var offers = new Advantshop.Offers(hfProductId.value),
            sizeColorPicker,
            zoom = $('#zoom'),
            imagePreview = $('#preview-img');
        hasPhoto = Advantshop.Utilities.Eval(zoom.attr('data-has-photo')),
        fancyboxControls = $('#icon-zoom, #link-fancybox');

        if (offers.storageOffers != null && offers.storageOffers.Offers.length > 0) {
            sizeColorPicker = Advantshop.Details.Part.SizeColorPickerDetails.prototype.Init(offers.storageOffers, $('[data-part="sizeColorPickerDetails"]:first'));
        }


        if (zoom.hasClass('clood-zoom') !== true && hasPhoto === true) {
            fancyboxControls = fancyboxControls.add(zoom);
        }

        //#region sizeColorPicker
        if (sizeColorPicker != null && offers.storageOffers != null && offers.storageOffers.Offers.length > 0) {

            //offers.UpdateProduct();

            sizeColorPicker.$obj.on('changeSize', '.size-item', function (event, obj) {
                offers.storageOffers = obj.storageOffers;//в соц сетях приходит старые значения
                offers.UpdateProduct();
            });

            sizeColorPicker.$obj.on('changeColor', '.color-item', function (event, obj) {
                offers.storageOffers = obj.storageOffers;//в соц сетях приходит старые значения
                offers.UpdateProduct();
            });

            sizeColorPicker.$obj.on('sizeColorFilter', function () {
                if (carouselDetails != null) {
                    carouselDetails.Filter();
                }
            });
        }
        //#endregion sizeColorPicker

        //#region Carousel
        if (document.getElementById('flexsliderDetails') != null) {
            var carouselDetails = new Advantshop.Details.Part.CarouselDetails(document.getElementById('flexsliderDetails'), offers.storageOffers);

            if (carouselDetails.carouselData.slides.length === 0) {
                fancyboxControls.hide();
                imagePreview.attr('src', 'images/nophoto.jpg');
                zoom.attr('href', 'javascript:void(0);');
                if (zoom.data('zoom') != null) {
                    zoom.data('zoom').destroy();
                }
            } else {
                var cloudZoomObj = $('.cloud-zoom');
                if (cloudZoomObj.CloudZoom != null) {
                    cloudZoomObj.CloudZoom();
                }

                fancyboxControls.show();
            }

            carouselDetails.$obj.on('carouselDetailsFilter', function (e, carouselDetailsObj) {

                var isNoPhoto = false;

                if (zoom.data('zoom') != null) {
                    zoom.data('zoom').destroy();
                }

                if (carouselDetailsObj.carouselData.slides.length === 0) {
                    $('#icon-zoom, #link-fancybox').hide();
                    imagePreview.attr('src', 'images/nophoto.jpg');
                    zoom.attr('href', 'javascript:void(0);');
                    isNoPhoto = true;
                } else {
                    $('#icon-zoom, #link-fancybox').show();

                    carouselDetailsObj.carouselData.slides.eq(0).find('[data-imagePicker-caller]').click();
                    if (zoom.hasClass('cloud-zoom') === true && zoom.CloudZoom != null) {
                        zoom.CloudZoom();
                    }
                    isNoPhoto = false;
                }

                if (Advantshop.ScriptsManager.Inplace != null && Advantshop.ScriptsManager.Inplace.prototype.checkEnabled()) {

                    var inplaceControls = imagePreview.attr('data-inplace-controls').replace(/'/g, '"');
                    inplaceControls = JSON.parse(inplaceControls);

                    if (isNoPhoto) {
                        inplaceControls["update"] = false;
                        inplaceControls["delete"] = false;
                    } else {
                        inplaceControls["update"] = true;
                        inplaceControls["delete"] = true;
                    }

                    inplaceControls = JSON.stringify(inplaceControls);
                    inplaceControls = inplaceControls.replace(/"/g, '\'');

                    imagePreview.attr('data-inplace-controls', inplaceControls);

                    Advantshop.ScriptsManager.Inplace.prototype.InitTotal(carouselDetails.$obj.find('[data-plugin="inplace"]'));
                }
            });

            if (Advantshop.ScriptsManager.Inplace != null) {
                carouselDetails.$obj.on('click', '.slides > li', function () {
                    var photoId = this.getAttribute('data-photo-id');

                    if (photoId != null && imagePreview.attr('data-inplace-params') != null) {
                        var inplaceParams = imagePreview.attr('data-inplace-params').replace(/'/g, '"');
                        inplaceParams = JSON.parse(inplaceParams);

                        inplaceParams.id = photoId;

                        inplaceParams = JSON.stringify(inplaceParams);
                        inplaceParams = inplaceParams.replace(/"/g, '\'');

                        imagePreview.attr('data-inplace-params', inplaceParams);

                        Advantshop.ScriptsManager.Inplace.prototype.InitTotal(imagePreview);

                    }
                });
            }


        } else if (hasPhoto === true && zoom.CloudZoom != null) {
            zoom.CloudZoom();
        }


        $('#availability').on('saved.inplace', function (event, inplaceObj, responseInplace) {
            if (inplaceObj.params.type === 'Offer' && inplaceObj.params.prop === 'Amount') {
                inplaceObj.$obj.html(data.amountString);

                var hfProductId = document.getElementById('hfProductId');
                var offers = new Advantshop.Offers(hfProductId.value);

                if (offers != null && offers.storageOffers != null && offers.storageOffers.offerSelected != null) {
                    offers.storageOffers.offerSelected.Amount = data.amount;
                } else {
                    var inplaceAmountParam = utilities.Eval(inplaceObj.$obj.attr('data-inplace-params'));

                    inplaceAmountParam.content = data.amount;

                    inplaceObj.$obj.attr('data-inplace-params', JSON.stringify(inplaceAmountParam).replace(/"/g, '\''));
                    inplaceObj.params = inplaceAmountParam;
                }
            }
        });

        imagePreview.on('saved.inplace', function (event, inplaceObj, responseInplace) {

            var urlObj = new Advantshop.Utilities.Uri(window.location.href),
                            scp = $('[data-part="sizeColorPickerDetails"]');

            if (scp.length === 0) {
                window.location.reload(true);
            }

            urlObj.deleteQueryParam('color');
            urlObj.deleteQueryParam('size');
            urlObj.deleteQueryParam('rnd');

            var colorSelected = $('.color-item.selected', scp).attr('data-color-id'),
                sizeSelected = $('.size-item.selected', scp).attr('data-size-id');

            if (colorSelected != null) {
                urlObj.addQueryParam('color', colorSelected);
            }

            if (sizeSelected != null) {
                urlObj.addQueryParam('size', sizeSelected);
            }

            urlObj.addQueryParam('rnd', Math.random());

            window.location.assign(urlObj.toString());
        });

        //#region inplace compability with zoom
        imagePreview.on('mouseenter.cloudZoom', function (e) {
            var inplaceObj = imagePreview.data('inplace');

            if (inplaceObj != null && inplaceObj.advancedCondrolsShow != null) {
                inplaceObj.advancedCondrolsShow();
            }
        });

        imagePreview.on('mouseleave.cloudZoom', function (e) {
            var inplaceObj = imagePreview.data('inplace');

            if (inplaceObj != null && inplaceObj.advancedCondrolsHide != null) {
                inplaceObj.advancedCondrolsHide();
            }
        });

        //#endregion

        fancyboxControls.on('click', function (e) {

            e.preventDefault();

            if (carouselDetails == null || carouselDetails.carouselData.slides.length === 0) {

                if ($.fancybox != null) {
                    $.fancybox([{ href: zoom.attr('href'), title: zoom.find('img').attr('title') || '' }]);
                }

                return;
            } else {
                if ($.fancybox != null) {
                    var arr = [],
                        item,
                        index = 0,
                        arrImgs;

                    for (var i = 0, arrItemsLength = carouselDetails.carouselData.slides.length; i < arrItemsLength; i += 1) {
                        item = carouselDetails.carouselData.slides.eq(i).find('[data-imagePicker-caller]');


                        arrImgs = Advantshop.Utilities.Eval(item.attr('data-imagePicker-img'));

                        arr.push({ href: arrImgs['big'].src, title: arrImgs['big'].title || '' });

                        if (item.hasClass('selected') === true) {
                            index = i;
                        }
                    }

                    $.fancybox(arr, { index: index });
                }
            }
        });

        var detailsDelivery = $('.js-details-delivery');

        if (detailsDelivery.length > 0) {
            var type = parseInt(detailsDelivery.attr("data-value"));

            switch (type) {
                case 1:
                    shippingCalcRender(detailsDelivery);
                    break;
                case 2:
                    shippingListRender(detailsDelivery);
                    break;
            }

            $doc.on('zone.changeCity', function () {
                shippingListRender(detailsDelivery);
            });

            $doc.on('set.spinbox', function () {
                shippingListRender(detailsDelivery);
            });
        }


        $(".b-customoptions").on("click", "input[type='radio'], input[type='checkbox'], a", function () {
            if (!$('#form').valid('cOptions')) {
                return false;
            }
            updateCustomOptions(hfProductId.value);
            shippingListRender(detailsDelivery);
        });

        $(".b-customoptions").on("change", "select", function () {
            updateCustomOptions(hfProductId.value);
            shippingListRender(detailsDelivery);
        });

        
        $(".b-customoptions").on("blur", "input[type='text'], textarea", function () {
            updateCustomOptions(hfProductId.value);
            shippingListRender(detailsDelivery);
        });

    });

    function loadShippings(offer, customOptions, amount) {

        return $.ajax({
            dataType: "json",
            cache: true,
            type: "POST",
            data: {
                offer: offer,
                customOptions: customOptions,
                amount: amount
            },
            url: "httphandlers/details/getshippings.ashx"
        });
    }

    function shippingCalcRender(container) {
        new EJS({ url: 'js/jspage/details/templates/deliveryCalc.tpl' }).update(container[0], {});
        container.find('.js-details-delivery-calc').on('click', function () {
            shippingListRender(container);
        });
    }

    function shippingListRender(container) {
        var offerId = $("#btnAdd").attr("data-offerid"),
            customOptions = htmlEncode($("#customOptionsHidden_" + $("#hfProductId").val()).length > 0 ? $("#customOptionsHidden_" + $("#hfProductId").val()).val() : null),
            amount = $("#txtAmount").val();

        var progressDelivery = new scriptManager.Progress.prototype.Init(container);
        progressDelivery.Show();

        loadShippings(offerId, customOptions, amount).done(function (data) {
            if (data != null) {
                new EJS({ url: 'js/jspage/details/templates/deliveryList.tpl' }).update(container[0], data);
                if (data.AdvancedObj != null) {
                    loadMultiship(data.AdvancedObj.WidgetCode, data.AdvancedObj.Dimensions, data.AdvancedObj.Weight, data.AdvancedObj.Cost);
                }
            }
        }).always(function () {
            progressDelivery.Hide();
        });
    }


    function loadMultiship(widgetCode, dimensions, weight, cost) {
        jQuery.getScript("https://api-maps.yandex.ru/2.0/?load=package.standard,package.geoQuery&lang=ru-RU", function () {
            jQuery.getScript(widgetCode, function () {
                mswidget.ready(function () {
                    ms$('body').prepend('<div id="mswidget" class="ms-widget-modal"></div>');
                    mswidget.initCartWidget({
                        'getCity': function () {
                            var city = $("a.js-location-call").text();
                            if (city) {
                                return { value: city };
                            } else {
                                return false;
                            }
                        },
                        'el': 'mswidget',
                        'itemsDimensions': function () { return [(new Function("return " + dimensions))()]; },
                        'weight': function () { return parseFloat(weight); },
                        'cost': function () { return parseFloat(cost); },
                        'onDeliveryChange': function (delivery) {
                            mswidget.cartWidget.close();
                        },
                    });
                });
            });
        });
    }


    function updateCustomOptions(productId) {
        var selectedOptions = "";
        $(".b-customoptions .prop-str").each(function (index) {

            var item = $(this).find("input[type='radio']:checked");

            if (item.length > 0 && item.val() > 0) {
                selectedOptions += (selectedOptions != "" ? ";" : "") + index + "_" + item.val();
                return;
            }

            item = $(this).find("input[type='checkbox']:checked");
            if (item.length > 0) {
                selectedOptions += (selectedOptions != "" ? ";" : "") + index + "_1";
                return;
            }

            item = $(this).find("select option:selected");
            if (item.length > 0 && item.val() > 0) {
                selectedOptions += (selectedOptions != "" ? ";" : "") + index + "_" + item.val();
                return;
            }

            item = $(this).find("input[type='text'], textarea");
            if (item.length > 0) {
                selectedOptions += (selectedOptions != "" ? ";" : "") + index + "_" + item.val();
                return;
            }
            index++;
        });


        var cOptions = $('#customOptionsHidden_' + productId);

        var params = {
            productId: productId,
            attributesXml: cOptions != null ? cOptions.value : null,
            selectedOptions: selectedOptions
        };

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: 'httphandlers/details/getCustomOptions.ashx',
            data: params,
            async: false,
            success: function (data) {
                if (data != null && data.error == null) {
                    $('#customOptionsHidden_' + productId).val(data.attributesXml);
                    Advantshop.Offers.prototype.UpdateProduct(productId);
                }
            },
            error: function (data) {
                throw Error(data.responseText);
            }
        });
    }

})(jQuery, jQuery(document));


