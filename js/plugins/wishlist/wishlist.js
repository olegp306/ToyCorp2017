(function ($, $document) {

    'use strict';

    var advantshop = window.Advantshop,
        scriptManager = advantshop.ScriptsManager,
        utilities = advantshop.Utilities,
        isRequestProcessing;

    var wishlist = function (selector) {
        this.options = $.extend({}, this.defaultOptions);
        this.$obj = advantshop.GetJQueryObject(selector);
        this.offerid = this.$obj.attr('data-offerid');
        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Wishlist = wishlist;

    wishlist.prototype.InitTotal = function () {

        //todo исправить жесткую привязку к дитейлсу
        var sizeColorPickerControl = $('[data-part="sizeColorPickerDetails"]'),
            objects = $('[data-plugin="wishlist"]');

        if (sizeColorPickerControl.length > 0) {
            sizeColorPickerControl.on('changeSize changeColor', function (e, scp) {
                if (objects.length > 0) {
                    wishlist.prototype.checkState(objects.eq(0), scp.storageOffers.offerSelected.OfferId);
                }
            });
        }

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            wishlist.prototype.Init(objects.eq(i));
        }
    };

    $(wishlist.prototype.InitTotal); //call document.ready

    wishlist.prototype.Init = function (selector) {
        var wishlistObj = new wishlist(selector);

        wishlistObj.BindEvent();

        return wishlistObj;
    };

    wishlist.prototype.BindEvent = function () {
        var wishlistObj = this;

        wishlistObj.$obj.on('click.wishlist', function (e) {

            if (wishlistObj.$obj.hasClass('js-wishlist-added') === true) {
                return;
            }

            wishlistObj.Add();
        });

    };

    wishlist.prototype.Add = function () {
        var wishlistObj = this;

        if (isRequestProcessing != null && isRequestProcessing == wishlistObj.$obj) {
            return;
        }

        isRequestProcessing = wishlistObj.$obj;

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            async: true,
            data: {
                offerId: wishlistObj.$obj.attr('data-offerid'),
                customOptions: htmlEncode($("#customOptionsHidden_" + $("#hfProductId").val()).length > 0 ? $("#customOptionsHidden_" + $("#hfProductId").val()).val() : null)
            },
            url: "httphandlers/details/addtowishlist.ashx",
            success: function (data) {

                isRequestProcessing = null;

                wishlistObj.$obj.addClass('js-wishlist-added');

                $(document).trigger("add_to_wishlist");

                wishlistObj.$obj.text(localize("AlreadyInWishlist"));
                wishlistObj.$obj.attr("href", "wishlist.aspx");

                $(document).trigger('wishlist.add', [data]);
            },
            error: function () {
                notify(localize("WishlistError"), notifyType.error, true);
            },
            complete: function () {
                isRequestProcessing = null;
            }
        });
    };

    wishlist.prototype.checkState = function (obj, offerId) {
        $.ajax({
            dataType: 'json',
            data: { offerId: offerId },
            url: 'httphandlers/details/wishlistcheck.ashx',
            success: function (data) {
                if (data.isExist === true) {
                    obj
                    .text(localize("AlreadyInWishlist"))
                    .attr({
                        'href': 'wishlist.aspx',
                        'data-offerid': offerId
                    })
                    .addClass('js-wishlist-added');
                } else {
                    obj
                     .text(localize("AddToWishList"))
                    .attr({
                        'href': 'javascript:void();',
                        'data-offerid': offerId
                    })
                    .removeClass('js-wishlist-added');
                }
            }
        });
    };

})(jQuery, jQuery(document));