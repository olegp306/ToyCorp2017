; (function ($, $doc) {

    'use strict';

    var zoneService = function () { };


    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.ZoneService = zoneService;

    zoneService.prototype.init = function () {

        if (zoneService.prototype.modal != null) {
            return zoneService.prototype.modal;
        }

        var result = zoneService.prototype.getData(),
            modal = $.advModal({
                afterOpen: function () {
                    zoneService.prototype.inputSearch.advMoveCaret('end');
                }
            });

        result.done(function (data) {

            zoneService.prototype.render(modal, data);

            zoneService.prototype.bindCaller();

            zoneService.prototype.bind(modal.find('.js-location-country'), modal.find('.js-location-cities-item'), modal.find('.js-location-search-confirm'));
        });

    };

    zoneService.prototype.getData = function (countryId) {
        return $.ajax({
            dataType: "json",
            cache: false,
            data: { countryId: countryId || 0 },
            type: "POST",
            url: "httphandlers/location/getcities.ashx"
        });
    };

    zoneService.prototype.render = function (modal, data) {

        modal.modalContent(new EJS({ url: 'js/plugins/location/template/cities-popup.tpl' }).render(data));

        zoneService.prototype.inputSearch = $(".js-location-search", modal);
        zoneService.prototype.modal = modal;
    };

    zoneService.prototype.save = function (city, countryId) {
        return $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            data: {
                city: city,
                countryID: countryId
            },
            url: "httphandlers/location/getzone.ashx"
        }).done(function (data) {
            zoneService.prototype.update(data);
        }).fail(function (data) {
            notify('Ошибка при сохранении выбранного города');
        });
    };

    zoneService.prototype.bind = function (btnsCountry, btnsCities, btnSearchConfirm) {

        btnsCountry.add(btnsCities).add(btnSearchConfirm).off('click.zone');

        btnsCountry.on('click.zone', function (e) {

            if (this.classList.contains('selected') === true) {
                return;
            }

            var countryId = this.getAttribute('data-countryid');

            zoneService.prototype.getData(countryId).done(function (data) {
                zoneService.prototype.render(zoneService.prototype.modal, data);
                zoneService.prototype.bind(zoneService.prototype.modal.find('.js-location-country'), zoneService.prototype.modal.find('.js-location-cities-item'), zoneService.prototype.modal.find('.js-location-search-confirm'));

                $doc.trigger('zone.changeCountry', [data]);
            });
        });

        btnsCities.on('click.zone', function (e) {
            var countryId = $(".js-location-country.selected").attr('data-countryid');

            zoneService.prototype.save(e.target.innerHTML, countryId).done(function (data) {

                zoneService.prototype.inputSearch.val('');

                zoneService.prototype.modal.modalClose();

                $doc.trigger('zone.changeCity', [data]);
            });
        });

        btnSearchConfirm.on('click.zone', function (e) {

            var countryId = $(".js-location-country.selected").attr('data-countryid');

            zoneService.prototype.save(zoneService.prototype.inputSearch.val(), countryId).done(function (data) {
                zoneService.prototype.modal.modalClose();

                zoneService.prototype.inputSearch.val('');

                $doc.trigger('zone.changeCity', [data]);
            });
        });

        zoneService.prototype.inputSearch.autocomplete("httphandlers/getcities.ashx", {
            delay: 300,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: 0,
            selectFirst: false,
            maxItemsToShow: 10
        });

        zoneService.prototype.inputSearch.on('ac.select', function (e, li, val) {

            var v = zoneService.prototype.inputSearch.val(),
                val = v.length > 0 ? v : val;

            var countryId = $(".js-location-country.selected").attr('data-countryid');
            zoneService.prototype.save(val, countryId).done(function (data) {
                zoneService.prototype.modal.modalClose();

                zoneService.prototype.inputSearch.val('');

                $doc.trigger('zone.changeCity', [data]);
            });
        });

    };

    zoneService.prototype.bindCaller = function () {
        $doc.on('click.zone', '.js-location-call', function (e) {
            zoneService.prototype.modal.modalShow();
            e.stopPropagation();
        });
    };

    zoneService.prototype.update = function (zone) {
        var country = zone.country || '',
            region = zone.region || '',
            city = zone.city || '',
            phone = zone.phone || '',
            literals = $('.js-location-replacement'),
            literalsItem,
            mask,
            result;

        for (var i = 0, il = literals.length; i < il; i++) {
            literalsItem = literals.eq(i);
            mask = literalsItem.attr('data-location-mask');

            if (mask == null) {
                continue;
            }

            result = mask.replace('#country#', country).replace('#region#', region).replace('#city#', city).replace('#phone#', phone);

            literalsItem.html(result);
        }
    };

    $(function () {
        var zone = new zoneService();

        zone.init();

    });

    $doc.on("msWidget.widget.city.change", function (e, city) {
        var countryId = $(".js-location-country.selected").attr('data-countryid');
        Advantshop.ScriptsManager.ZoneService.prototype.save(city, countryId);
    });

    $doc.on("zone.changeCity", function (e, data) {
        if (Advantshop.ScriptsManager.Inplace != null && Advantshop.ScriptsManager.Inplace.prototype.checkEnabled()) {
            var params = Advantshop.Utilities.Eval($(".js-phone").attr("data-inplace-params"));
            if (params != null) {
                params.cityId = data.cityId;
                $(".js-phone").attr("data-inplace-params", JSON.stringify(params).replace(/"/g, '\''));
                Advantshop.ScriptsManager.Inplace.prototype.InitTotal();
            }
        }
    });

})(jQuery, jQuery(document));
