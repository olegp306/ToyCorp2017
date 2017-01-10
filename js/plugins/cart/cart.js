(function ($) {

    var advantshop = window.Advantshop,
        scriptManager = advantshop.ScriptsManager,
        utilities = advantshop.Utilities,
        storage = {},
        counter = -1,
        $win = $(window),
        $body = $(document.body),
        spinboxActiveIndex,
        cartpopup;

    var cart = function (selector, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);

        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Cart = cart;

    cart.prototype.InitTotal = function () {
        var objects = $('[data-plugin="cart"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            cart.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-cart-options')) || {});
        }
    };

    $(function(){
        if (utilities.Events.isExistEvent($body, 'click.cartAdd') != true) {
            $body.on('click.cartAdd', '[data-cart-add-productid],[data-cart-add-offerid]', function (event) {
                event.preventDefault();
                event.stopPropagation();

                if (!$('#form').valid('cOptions')) {
                    return;
                }
                
                var productId = this.getAttribute('data-cart-add-productid'),
                    urlObj = new Advantshop.Utilities.Uri(this.getAttribute('href')),
                    colorId = this.getAttribute('data-color-id');

                if (colorId != null && colorId != "") {
                    urlObj.addQueryParam('color', colorId);
                }

                var productData = {
                    productid: productId,
                    offerid: this.getAttribute('data-cart-add-offerid'),
                    url: urlObj.toString(),
                    amount: parseFloat(this.getAttribute('data-cart-amount') || $("#txtAmount").length > 0 ? $("#txtAmount").val() : 1),
                    AttributesXml: $("#customOptionsHidden_" + productId).length > 0 ? $("#customOptionsHidden_" + productId).val() : null,
                    payment: this.getAttribute('data-cart-payment')
                };

                cart.prototype.Add(productData);
            });
        }

        if (utilities.Events.isExistEvent($body, 'click.cartClear') != true) {
            $body.on('click.cartClear', '[data-cart-clear]', function () {
                cart.prototype.Clear();
            });
        }

        if (utilities.Events.isExistEvent($body, 'click.cartRemoveItem') != true) {
            $body.on('click.cartRemoveItem', '[data-cart-remove]', function () {
                cart.prototype.Remove(this.getAttribute('data-cart-remove'));
            });
        }

        if (utilities.Events.isExistEvent($body, 'click.cartApplyCertOrCupon') != true) {
            $body.on('click.cartApplyCertOrCupon', '[data-cart-apply-cert-cupon]', function () {
                var code = advantshop.GetJQueryObject(this.getAttribute('data-cart-apply-cert-cupon')).val();
                if (code == null || code == "") {
                    return;
                }
                cart.prototype.ApplyCertCupon(code).done(function (data) {
                    if(data == "true")
                    if ($("#ocCoupon").length) {
                        $("#ocCoupon").remove();
                        UpdateBasket();
                    }
                });
            });
        }

        if (utilities.Events.isExistEvent($body, 'click.cartRemoveCertificate') != true) {
            $body.on('click.cartRemoveCertificate', '[data-cart-remove-cert]', function () {
                cart.prototype.RemoveCertificate();
                UpdateBasket();
            });
        }

        if (utilities.Events.isExistEvent($body, 'click.cartRemoveCupon') != true) {
            $body.on('click.cartRemoveCupon', '[data-cart-remove-cupon]', function () {
                cart.prototype.RemoveCoupon();
                UpdateBasket();
            });
        }

        cart.prototype.InitTotal();

    }); // call document.ready

    cart.prototype.Init = function (selector, options) {
        var cartObj = new cart(selector, options);

        cartObj.GenerateHtml();
        cartObj.BindEvent();
        cartObj.SaveInStorage();

        return cartObj;
    };

    cart.prototype.Type = { Full: 'full', Mini: 'mini' };
    cart.prototype.TypeSite = { Default: 'default', Social: 'social', ProductAdded: 'productadded' };

    cart.prototype.State = { Create: 'create', Update: 'update', Add: 'add' };

    cart.prototype.GenerateHtml = function (state) {
        var cartObj = this;
        state = state || cart.prototype.State.Create;

        $.ajax({
            dataType: 'json',
            cache: false,
            type: 'GET',
            url: 'httphandlers/shoppingcart/getcart.ashx',
            success: function (data) {

                for (var type in cart.prototype.GetAllItemsInStorage()) {
                    for (var item in storage[type]) {
                        storage[type][item].GenerateCallback[storage[type][item].options.type].call(storage[type][item], data, state);
                    }
                }

                //событие для вызова обновления модуля
                $(document).trigger("update_related_products", [data]);
                $(document).trigger('cart.generated', [data]);
                UpdateBasket();
            },
            error: function () {
                $('<div />').html(localize("shoppingCartErrorGetingCart"));
            }
        });
    };

    cart.prototype.GenerateCallback = function () {
    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Mini] = function (data, state) {
        var cartObj = this;
        var cartDom = cartObj.$obj;

        var progressMini;
        var $list = cartObj.GetListJqueryObj();
        
        if ($list.hasClass('minicart-list-hidden') === true) {
            progressMini = new scriptManager.Progress.prototype.Init($list);
            progressMini.Show();
        }

        var html;

        switch (cartObj.options.typeSite) {
            case cart.prototype.TypeSite.Default:
                html = new EJS({ url: cartObj.options.tplMiniDefault }).render(data);
                break;
            case cart.prototype.TypeSite.Social:
                html = new EJS({ url:  cartObj.options.tplMiniSocial }).render(data);
                break;  
            case cart.prototype.TypeSite.ProductAdded:
                html = new EJS({ url: 'js/plugins/cart/templates/minicart.tpl' }).render(data);
                break;
            default:
                html = new EJS({ url: cartObj.options.tplMiniDefault }).render(data);
        }

        cartDom.html(html);

        cartObj.GetListJqueryObj().addClass('minicart-list-hidden');

        if (data.CountNumber === 0) {
            cartDom.addClass('cart-empty');
        } else {
            cartDom.removeClass('cart-empty');
        }

        if (cartObj.options.typeSite != cart.prototype.TypeSite.ProductAdded) {
            cart.prototype.GenerateCallback[cart.prototype.Type.Mini][state].call(cartObj, html);
        }

        if (progressMini != null) {
            progressMini.Hide();
        }
    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Mini][cart.prototype.State.Create] = function (html) {
        var cartObj = this;
        var cartDom = cartObj.$obj;

        cartDom.addClass(cartObj.options.type + 'cart');
        
    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Mini][cart.prototype.State.Add] = function (html) {
        var cartObj = this;
        
        cartObj.ScrollBarInit();
        cartObj.Show();
        cartObj.PosMiniCart();
    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Mini][cart.prototype.State.Update] = function (html) {
        var cartObj = this;
        cartObj.Hide();
    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Full] = function (data, state) {
        var cartObj = this,
            cartDom = cartObj.$obj;
        
        var progressFull = new scriptManager.Progress.prototype.Init(cartDom);

        progressFull.Show();

        // resize site in vk.com
        if (window.VK && window.VK.callMethod) {
            VK.init(function () {
                VK.callMethod("resizeWindow", 827, $("body").height());
            });
        }

        // $("#aCheckOut") - button go orderconfirmation.aspx
        if (data.Count === 0 || data.Valid.length > 0) {
            $("#aCheckOut").addClass("btn-disabled");
            $("#divBuyInOneClick").hide();
        } else {
            $("#aCheckOut").removeClass("btn-disabled");
            $("#divBuyInOneClick").show();
        }

        var html = new EJS({ url: 'js/plugins/cart/templates/fullcart.tpl' }).render(data);
        
        switch (cartObj.options.typeSite) {
            case cart.prototype.TypeSite.Default:
                html = new EJS({ url: cartObj.options.tplFullDefault }).render(data);
                break;
            case cart.prototype.TypeSite.Social:
                html = new EJS({ url: cartObj.options.tplFullSocial }).render(data);
                break;
            default:
                html = new EJS({ url: cartObj.options.tplFullDefault }).render(data);
        }

        cartDom.html(html);

        cart.prototype.GenerateCallback[cart.prototype.Type.Full][state].call(this, html);

        scriptManager.Spinbox.prototype.InitTotal();


        var spinboxes = cartDom.find('[data-plugin="spinbox"]'),
            spinboxTimer;

        if (spinboxActiveIndex != null) {
            spinboxes.eq(spinboxActiveIndex).focus().advMoveCaret('end');
            spinboxActiveIndex = null;
        }

        spinboxes.on('set.spinbox', function () {

           var spinbox = $(this);

           if (spinboxTimer != null) {
                clearTimeout(spinboxTimer);
            }

           spinboxTimer = setTimeout(function () {
               if (spinbox.val().length > 0) {

                   spinboxActiveIndex = spinboxes.index(spinbox);

                   cart.prototype.Update();
               } 
            }, 500);
        });

        progressFull.Hide();
    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Full][cart.prototype.State.Create] = function (html) {
        this.$obj.addClass(this.options.type + 'cart');
    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Full][cart.prototype.State.Update] = function (html) {

    };

    cart.prototype.GenerateCallback[cart.prototype.Type.Full][cart.prototype.State.Add] = function (html) {
    };

    cart.prototype.BindEvent = function () {
        var cartObj = this;
        var options = cartObj.options;

        cartObj.BindEventByType[options.type].call(cartObj);
    };

    cart.prototype.BindEventByType = {
        'mini': function () {
            var cartObj = this;
            var cartDom = cartObj.$obj;
            var options = cartObj.options;

            cartDom.on('click.cartClick', function () {
                document.location.href = "/OrderConfirmation.aspx";
            });

            cartDom.mouseenter('mouseenter', function () {
                if (cartDom.hasClass('cart-empty') != true && cartObj.GetListJqueryObj().hasClass('minicart-list-hidden') === true) {
                    cartObj.ScrollBarInit();
                    cartObj.Show();
                }
            });

            cartDom.on('mouseover.cartMouseOver', function () {
                if (cartObj.timer != null) {
                    clearTimeout(cartObj.timer);
                }
            });

            cartDom.on('mouseout.cartMouseOut', function () {
                if (cartObj.timer != null) {
                    clearTimeout(cartObj.timer);
                }

                cartObj.timer = setTimeout(function () { cartObj.Hide.call(cartObj); }, options.timeHide);
            });


            if (options.isHideClickOut === true) {
                $(document.body).on('click.cartClickOut', function (e) {
                    var $list = cartObj.GetListJqueryObj();
                    if ($list.hasClass('minicart-list-hidden') != true && $(e.target).closest(cartDom).length === 0) {
                        cartObj.Hide.call(cartObj);
                    }
                });
            }

            if (options.referencing === true) {
                $win.on('scroll.cartScroll', function () {
                    cartObj.PosMiniCart.call(cartObj);
                });
            }
        },
        'full': function () {

        }
    };

    cart.prototype.Hide = function () {
        var cartObj = this;
        var options = cartObj.options;
        var $list = cartObj.GetListJqueryObj();

        cartObj.$obj.removeClass('minicart-active');

        $list[options.animationHide](options.animationSpeed, function() {
            $list.addClass('minicart-list-hidden');
        });

        if (options.type === cart.prototype.Type.Mini && cartObj.timer != null) {
            clearTimeout(cartObj.timer);
        }
    };

    cart.prototype.Show = function () {
        var cartObj = this;
        var options = cartObj.options;
        var $list = cartObj.GetListJqueryObj();
        
        $list.removeClass('minicart-list-hidden').hide();
        $list[options.animationShow](options.animationSpeed);

        if (options.type === cart.prototype.Type.Mini) {

            if (cartObj.timer != null) {
                clearTimeout(cartObj.timer);
            }

            cartObj.timer = setTimeout(function () { cartObj.Hide.call(cartObj); }, options.timeHide);

            cartObj.$obj.addClass('minicart-active');
        }
    };

    cart.prototype.PosMiniCart = function () {
        var cartObj = this;
        var $list = cartObj.GetListJqueryObj();
        var currentScroll = $win.scrollTop();

        cartObj.positionDefault = {left: cartObj.$obj.offset().left - ($list.outerWidth() - cartObj.$obj.outerWidth()), top:cartObj.$obj.outerHeight()};

        /*if ($list.hasClass('minicart-list-hidden') != true) {*/
            if (currentScroll > cartObj.positionDefault.top) {
                $list.addClass('minicart-fixed');
                $list.css({ left: cartObj.positionDefault.left });
            } else {
                $list.removeClass('minicart-fixed');
                $list.css({ left: '' });
            }
        /*}*/

    };

    cart.prototype.Add = function (productData) {
        
        if (productData.offerid == null) {
            delete productData.offerid;
        }
        
        if (productData.attributesXml == null) {
            delete productData.attributesXml;
        }
        
        if (productData.payment == null) {
            delete productData.payment;
        }
        
        $.ajax({
            url: "httphandlers/shoppingcart/addtocart.ashx",
            cache: false,
            type: "POST",
            data: productData,
            dataType: "json",
            success: function (data) {
                if (data.status == 'success') {
                    
                    if (data.showCart) {
                        var html = new EJS({ url: data.tpl }).render(data);

                        if (cartpopup != null) {
                            cartpopup.modalClose();
                        }
                        
                        cartpopup = $.advModal({
                            htmlContent: html,
                            clickOut: false,
                            afterOpen: function() {
                                if ($("ul.carousel-related-products li").length > 0) {
                                    $("ul.carousel-related-products").jcarousel({ scroll: 1, visible: 4, itemFallbackDimension: 300 });
                                }
                                $(".modal .cart-close").on("click", function() {
                                    cartpopup.modalClose();
                                });

                                cartpopup.modalPosition();
                            }
                        });
                        cartpopup.modalShow();
                    }

                    cart.prototype.GenerateHtml(cart.prototype.State.Add);

                    $(document).trigger("add_to_cart", [productData.url]);
                    $(document).trigger("cart.add", [data]);

                } else if (data.status === 'redirect') {
                    window.location.href = $("base").attr("href") + productData.url;
                } else if (data.status === 'fail') {
                    throw (localize('shoppingCartErrorAddingToCart'));
                }
            },
            error: function (data) {
                throw (localize("shoppingCartErrorAddingToCart"));
            }
        });
    };

    cart.prototype.Update = function () {
        var cartObj = this;
        var counters = $('[data-cart-itemcount]');
        var listProducts = "";
        var itemId;

        for (var i = 0, arrLength = counters.length; i < arrLength; i++) {
            itemId = counters.eq(i).closest('tr').attr('data-itemid');
            listProducts += itemId + "_" + counters.eq(i).val() + ";";
        }

        $.ajax({
            dataType: "json",
            cache: false,
            data: { list: listProducts },
            type: "POST",
            url: "httphandlers/shoppingcart/updatecart.ashx",
            success: function (data) {

                cart.prototype.GenerateHtml(cart.prototype.State.Update);

                $(document).trigger('cart.update', data);
            },
            error: function (data) {
                alert("error recalc");
            }
        });
    };

    cart.prototype.Clear = function () {
        var cartObj = this;

        $.ajax({
            dataType: "json",
            cache: false,
            type: "POST",
            url: "httphandlers/shoppingcart/clearcart.ashx",
            success: function (data) {

                cart.prototype.GenerateHtml(cart.prototype.State.Update);

                $(document).trigger("update_related_products");
                $(document).trigger("cart.clear", [data]);
            },
            error: function (data) {
                notify("error clearcart" + " status text:" + data.statusText, notifyType.error, true);
            }
        });
    };

    cart.prototype.Remove = function (itemId) {
        var cartObj = this;

        $.ajax({
            dataType: "json",
            cache: false,
            data: { 'itemid': itemId },
            type: "POST",
            url: "httphandlers/shoppingcart/deletefromcart.ashx",
            success: function (data) {

                cart.prototype.GenerateHtml(cart.prototype.State.Update);

                $(document).trigger("cart.remove", [data]);
            },
            error: function (data) {
                alert("error delete");
            }
        });
    };

    cart.prototype.ApplyCertCupon = function (code) {
        var cartObj = this;
        return $.ajax({
            dataType: "text",
            cache: false,
            data: { 'code': code },
            type: "POST",
            url: "httphandlers/shoppingcart/applycertorcupon.ashx",
            success: function (data) {
                cart.prototype.GenerateHtml(cart.prototype.State.Update);
            },
            error: function (data) {
                alert("error delete");
            }
        });
    };

    cart.prototype.RemoveCertificate = function () {
        var cartObj = this;

        $.ajax({
            dataType: "text",
            cache: false,
            type: "POST",
            url: "httphandlers/shoppingcart/deletecertificate.ashx",
            success: function (data) {
                cart.prototype.GenerateHtml(cart.prototype.State.Update);
            },
            error: function (data) {
                alert("error delete");
            }
        });
    };

    cart.prototype.RemoveCoupon = function () {
        var cartObj = this;

        $.ajax({
            dataType: "text",
            cache: false,
            type: "POST",
            url: "httphandlers/shoppingcart/deleteCoupon.ashx",
            success: function (data) {
                cart.prototype.GenerateHtml(cart.prototype.State.Update);
            },
            error: function (data) {
                alert("error delete");
            }
        });
    };

    cart.prototype.SaveInStorage = function () {
        var cartObj = this;
        var cartDom = cartObj.$obj;
        var id;

        if (cartDom.attr('id')) {
            id = cartDom.attr('id');
        } else {
            id = 'cart-id-' + (counter += 1);
            cartDom.attr('id', id);
        }

        storage[cartObj.options.type] = storage[cartObj.options.type] || {};

        storage[cartObj.options.type][id] = this;

    };

    cart.prototype.GetAllItemsInStorage = function () {
        return storage;
    };

    cart.prototype.GetAllItemsByTypeInStorage = function (type) {
        return storage[type] || null;
    };

    cart.prototype.ScrollBarInit = function () {

        if ($.fn.jScrollPane == null) {
            return;
        }

        var cartObj = this;
        var $scrollDom = cartObj.$obj.find('[data-plugin="scrollbar"]');
  
        var height = $scrollDom.outerHeight();
        var scrollbarOpts = utilities.Eval($scrollDom.attr('data-cart-scrollbar'));
        
        if (height >= scrollbarOpts.height) {
            $scrollDom.css('height', scrollbarOpts.height);
            $scrollDom.jScrollPane();
        } else if ($scrollDom.data('jsp') != null) {
            $scrollDom.css('height', 'auto');
            $scrollDom.data('jsp').destroy();
        }
    };

    cart.prototype.GetListJqueryObj = function() {
        return this.$obj.find('.minicart-list');
    };
    
    cart.prototype.defaultOptions = {
        timeHide: 1000,
        animationShow: 'fadeIn',
        animationHide: 'fadeOut',
        animationSpeed: 700,
        referencing: true,
        isHideClickOut: true,
        type: cart.prototype.Type.Full,
        typeSite: cart.prototype.TypeSite.Default,
        tplMiniDefault : 'js/plugins/cart/templates/minicart.tpl',
        tplFullDefault: 'js/plugins/cart/templates/fullcart.tpl',
        tplMiniSocial : 'js/plugins/cart/templates/minicart-social.tpl',
        tplFullSocial: 'js/plugins/cart/templates/fullcart.tpl'
    };

})(jQuery);