var validateElemStoreReviews;

(function ($) {

    var cache = {};

    var validateElem = function (obj) {
        this.$obj = $(obj);
        var methodsCurrent = (new Function('return ' + this.$obj.attr('data-validelem-methods')))();

        if (Object.prototype.toString.call(methodsCurrent) != '[object Array]') {
            methodsCurrent = [];
        }

        if (methodsCurrent.length === 0) {
            methodsCurrent.push('required');
        } 

        this.methodsCurrent = methodsCurrent;

        return this;
    };


    validateElemStoreReviews = validateElemStoreReviews || validateElem;

    validateElem.prototype.Init = function (obj) {
        var validateElemObj = new validateElem(obj);

        validateElemObj.GenerateHtml();
        validateElemObj.BindEvent();

        cache[validateElemObj.$obj.attr('id')] = validateElemObj;

        return validateElemObj;
    };

    validateElem.prototype.InitTotal = function () {
        var $objs = $('[data-plugin="validelem"]');

        for (var i = 0, arrLength = $objs.length; i < arrLength; i += 1) {
            validateElem.prototype.Init($objs.eq(i));
        }

        var group;
        $(document.body).on('click', '[data-validelem-btn]', function (e) {
            group = (e.target).getAttribute('data-validelem-btn');

            if (validateElem.prototype.ValidGroup(group) != true) {
                e.stopImmediatePropagation();
                return false;
            }
        });

    };

    $(window).load(function () {

        validateElem.prototype.InitTotal();

    });

    validateElem.prototype.GenerateHtml = function () {
        var validateElemObj = this,
            validateElemDom = validateElemObj.$obj,
            isRequired = false, html = $([]),
            methodsCurrent = validateElemObj.methodsCurrent,
            container = validateElemDom,
            $icon = container.nextAll('validelem-icon'),
            $message = container.nextAll('validelem-message');

        for (var i = 0, arrLength = methodsCurrent.length; i < arrLength; i += 1) {
            if (methodsCurrent[i].indexOf('required') > -1) {
                isRequired = true;
                break;
            }
        }

        validateElemObj.Parts = validateElemObj.Parts || {};

        if (isRequired === true && $icon.length === 0 ) {
            var posDom = container.position();
            $icon = $(validateElem.prototype.iconHtml).addClass('validelem-icon-error').css({top: posDom.top, left: posDom.left +  container.outerWidth()});
            validateElemObj.Parts.$icon = $icon;
            html = $icon;
        }

        if ($message.length === 0) {
            $message = $(validateElem.prototype.messageHtml);
            validateElemObj.Parts.$message = $message;
            html = html.add($message);
        }

        if (html.length === 0) {
            return;
        }

        if (validateElemDom.parent('.input-wrap').length > 0) {
            container = validateElemDom.parent('.input-wrap');
        } else if (validateElemDom.parent('.textarea-wrap').length > 0) {
            container = validateElemDom.parent('.textarea-wrap');
        }

        container.after(html);
    };

    validateElem.prototype.BindEvent = function () {
        var validateElemObj = this,
            validateElemDom = validateElemObj.$obj;

        validateElemDom.on('focusout keyup', function () {
            validateElemObj.validElement();
        });
    };

    validateElem.prototype.ValidGroup = function (group) {
        var elems = $('[data-validelem-group="' + group + '"]');

        var id, validateItem, isValid = true;
        for (var i = 0, arrLength = elems.length; i < arrLength; i += 1) {
            id = elems[i].id;

            if (id in cache) {
                validateItem = cache[id];
            } else {
                validateItem = new validateElem(elems.eq(i));
            }

            if (validateItem.validElement() != true && isValid === true) {
                isValid = false;
            }
        }

        return isValid;
    };


    validateElem.prototype.validElement = function () {
        var validateElemObj = this,
            validateElemDom = validateElemObj.$obj,
            val = validateElemDom.val(),
            methods = validateElem.prototype.methods,
            methodName, methodErr = "", isValid;

        for (var i = 0, arrLength = validateElemObj.methodsCurrent.length; i < arrLength; i += 1) {
            methodName = validateElemObj.methodsCurrent[i];

            isValid = methods[methodName](val);

            if (isValid != true) {
                methodErr = methodName;
                break;
            }
        }

        if (isValid != true) {
            validateElemObj.showErrors(methodErr);
        } else {
            validateElemObj.hideErrors();
        }

        return isValid;
    };


    validateElem.prototype.showErrors = function (method) {
        var validateElemObj = this, parts = validateElemObj.Parts;

        if (parts.$icon != null && parts.$icon.length > 0) {
            parts.$icon.addClass('validelem-icon-error').removeClass('validelem-icon-success');
        }

        parts.$message.html(validateElem.prototype.message[method]);

        if (parts.$message.is(':hidden') === true) {
            parts.$message.stop(true, true).slideDown();
        }
    };

    validateElem.prototype.hideErrors = function () {
        var validateElemObj = this, parts = validateElemObj.Parts;

        if (parts.$icon != null && parts.$icon.length > 0) {
            parts.$icon.addClass('validelem-icon-success').removeClass('validelem-icon-error');
        }

        parts.$message.empty();
        parts.$message.stop(true, true).slideUp();
    };

    validateElem.prototype.methods = {
        email: function (val) {
            return /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i.test(val);
        },
        required: function (val) {
            return $.trim(val).length > 0;
        }
    };

    validateElem.prototype.message = {
        email: localizeStoreReviews('StoreReviewsValidEmail'),
        required: localizeStoreReviews('StoreReviewsValidRequired')
    };

    validateElem.prototype.iconHtml = '<span class="validelem-icon"></span>';
    validateElem.prototype.messageHtml = '<div class="validelem-message"></div>';

    validateElem.prototype.defaultOptions = {
        methods: ['required']
    };
})(jQuery);