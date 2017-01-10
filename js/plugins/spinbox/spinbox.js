(function ($) {

    var advantshop = Advantshop, scriptManager = advantshop.ScriptsManager, utilities = advantshop.Utilities;

    var spinbox = function (selector, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);

        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Spinbox = spinbox;

    spinbox.prototype.InitTotal = function () {
        var objects = $('[data-plugin="spinbox"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            spinbox.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-spinbox-options')) || {});
        }
    };

    $(spinbox.prototype.InitTotal); // call document.ready

    spinbox.prototype.Init = function (selector, options) {
        var spinboxObj = new spinbox(selector, options);

        spinboxObj.GenerateHtml();
        spinboxObj.BindEvent();

        return spinboxObj;
    };

    spinbox.prototype.GenerateHtml = function () {
        var spinboxObj = this;
        var numVal;
        numVal = spinboxObj.$obj.val().replace(',', '.');
        var value$Obj = Number(numVal);
        spinboxObj.$obj.addClass('spinbox');
        var $plus = $('<span />', { 'class': 'spinbox-control spinbox-plus' + (value$Obj === Number(spinboxObj.options.max) ? ' spinbox-disabled-plus' : '') });
        var $minus = $('<span />', { 'class': 'spinbox-control spinbox-minus' + (value$Obj === Number(spinboxObj.options.min) ? ' spinbox-disabled-minus' : '') });

        spinboxObj.Parts = {};
        spinboxObj.Parts.$plus = $plus;
        spinboxObj.Parts.$minus = $minus;

        var tempCollection = $plus.add($minus);

        spinboxObj.$obj.after(tempCollection);
        spinboxObj.$obj.val();
    };

    spinbox.prototype.Validate = function (code) {
        var spinboxObj = this,
            result = true,
            val = spinboxObj.$obj.val(),
            char = String.fromCharCode(code),
            isSeparatorInput = char === '.' || char === ',';

        if (/[\d,.]/g.test(char) === false) {
            result = false;
        }

        if ((val.indexOf('.') != -1 || val.indexOf(',') != -1) && isSeparatorInput) {
            result = false;
        }


        return result;
    };

    spinbox.prototype.BindEvent = function () {
        var spinboxObj = this;

        spinboxObj.$obj.on('keypress.spinbox', function (e) {
            var isValid = spinboxObj.Validate(e.keyCode);

            if (isValid === true) {
                spinboxObj.valOld = spinboxObj.$obj.val();
            }

            return isValid;
        });

        spinboxObj.$obj.on('keyup.spinbox', function (e) {
            var caretPos;
            switch (e.keyCode) {
                case 40:
                    // down arrow
                    spinboxObj.minus(e);
                    break;
                case 38:
                    // up arrow
                    spinboxObj.plus(e);
                    break;
                default:
                    spinboxObj.manual(e);
                    break;
            }
        });

        spinboxObj.$obj.on('focus.spinbox', function () {
            spinboxObj.valOld = Number(spinboxObj.$obj.val().replace(',', '.'));
        });

        spinboxObj.$obj.on('blur.spinbox', function (e) {
            var num = Number(spinboxObj.$obj.val().replace(',', '.')).toFixed(3);
            var newVal = num;

            if (isNaN(num) === true) {
                newVal = spinboxObj.options.min;
            }

            var overstep = (newVal % spinboxObj.options.step).toFixed(3);
            if (overstep != 0 && overstep != spinboxObj.options.step) {
                newVal = (newVal + spinboxObj.options.step - newVal % spinboxObj.options.step).toFixed(3);
            }

            if (newVal > spinboxObj.options.max) {
                newVal = spinboxObj.options.max;
            }

            if (newVal < spinboxObj.options.min) {
                newVal = spinboxObj.options.min;
            }

            spinboxObj.valNew = parseFloat(newVal);
            spinboxObj.$obj.val(parseFloat(newVal));


            spinboxObj.set(e);

        });


        spinboxObj.Parts.$plus.on('click.spinbox', function (e) {
            spinboxObj.plus(e);
        });

        spinboxObj.Parts.$minus.on('click.spinbox', function (e) {
            spinboxObj.minus(e);
        });
    };

    spinbox.prototype.plus = function (e) {
        var spinboxObj = this;
        var caretPos = spinboxObj.caretPosition();
        var valOld = Number(spinboxObj.$obj.val());
        var valNew = Number((valOld + Number(spinboxObj.options.step)).toFixed(3));

        if (valNew > spinboxObj.options.max) {
            return;
        }

        spinboxObj.valOld = valOld;
        spinboxObj.valNew = valNew;

        spinboxObj.set(e);

        spinboxObj.$obj.advMoveCaret(caretPos);
    };

    spinbox.prototype.minus = function (e) {
        var spinboxObj = this;
        var caretPos = spinboxObj.caretPosition();
        var valOld = Number(spinboxObj.$obj.val());
        var valNew = Number((valOld - Number(spinboxObj.options.step)).toFixed(3));

        if (valNew < spinboxObj.options.min) {
            return;
        }

        spinboxObj.valOld = valOld;
        spinboxObj.valNew = valNew;

        spinboxObj.set(e);

        spinboxObj.$obj.advMoveCaret(caretPos);
    };

    spinbox.prototype.manual = function (e) {
        var spinboxObj = this;

        var newVal = spinboxObj.$obj.val().replace(',', '.');
        var newValNumber = parseFloat(newVal);

        if (isNaN(newValNumber) === true) {
            spinboxObj.valOld = 0;
            return false;
        }


        if (newValNumber < spinboxObj.options.min) {
            spinboxObj.$obj.val(spinboxObj.options.min);
        }

        if (newValNumber > spinboxObj.options.max) {
            spinboxObj.$obj.val(spinboxObj.options.max);
        }

        if (newVal[newVal.length - 1] == '.'){
            spinboxObj.set(e);
        } else if (newVal == '0') {
            spinboxObj.valNew = 0;
            spinboxObj.set(e);
        } else {
            spinboxObj.valNew = Number(Number(spinboxObj.$obj.val().replace(',', '.')).toFixed(3));
            spinboxObj.set(e);
        }

        return true;
    };

    spinbox.prototype.set = function (e) {
        var spinboxObj = this,
            input = spinboxObj.$obj,
            minus = spinboxObj.Parts.$minus,
            plus = spinboxObj.Parts.$plus,
            valNew = input.val().replace(',', '.');


        if (spinboxObj.valOld == spinboxObj.valNew) {
            return;
        }

        if (valNew[valNew.length - 1] != '.')
            valNew = spinboxObj.valNew;

        if (input.val().length === 0) {

            if (e.type === 'click') {
                valNew = spinboxObj.options.min;
            } else {
                return;
            }
        }


        plus.removeClass('spinbox-disabled-plus');
        minus.removeClass('spinbox-disabled-minus');

        if (valNew >= Number(spinboxObj.options.max)) {
            plus.addClass('spinbox-disabled-plus');
            minus.removeClass('spinbox-disabled-minus');
        }

        if (valNew <= Number(spinboxObj.options.min)) {
            plus.removeClass('spinbox-disabled-plus');
            minus.addClass('spinbox-disabled-minus');
        }

        spinboxObj.valOld = spinboxObj.valNew;

        input.val(valNew);

        if (valNew[0] != '.' && valNew[valNew.length - 1] != '.') {
            input.trigger('set.spinbox');
        }
    };

    spinbox.prototype.caretPosition = function () {
        var pos = -1;
        if (this.$obj[0]) {
            pos = this.$obj[0].selectionStart;
            if (pos == this.$obj.val().length)
                pos = "end";
            if (pos == 0)
                pos = "start";
        }
        return pos;
    };

    spinbox.prototype.defaultOptions = {
        min: 0,
        max: Number.POSITIVE_INFINITY,
        step: 1
    };
})(jQuery);
