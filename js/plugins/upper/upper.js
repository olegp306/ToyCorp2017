(function($) {

    var advantshop = window.Advantshop
    , scriptManager = advantshop.ScriptsManager
    , utilities = advantshop.Utilities;

    var upper = function(selector, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);

        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Upper = upper;

    upper.prototype.InitTotal = function() {
        var objects = $('[data-plugin ="upper"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            upper.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-upper-options')) || {});
        }
    };

    $(upper.prototype.InitTotal); // call document.ready

    upper.prototype.Init = function(selector, options) {

        var upperObj = new upper(selector, options);

        upperObj.GenerateHtml();
        upperObj.BindEvent();

        return upperObj;
    };


    upper.prototype.GenerateHtml = function() {
        var upperObj = this;
        var $panel = $('<div />', { 'class': 'adv-upper-panel' });
        var $panelAnimated = $('<div />', { 'class': 'adv-upper-panel-animated' });
        var $panelAnimatedInside = $('<div />', { 'class': 'adv-upper-panel-inside' });
        var $symbolUp = $('<div />', { 'class': 'adv-upper-symbol', html: this.options.symbolUp || '' });

        var containerScroll = $(upperObj.options.containerScroll);

        if (containerScroll.scrollTop() > upperObj.options.enabledTop) {
            $symbolUp[upperObj.options.effectShow](upperObj.options.effectShowSpeed);
        }

        $panelAnimated.append($panelAnimatedInside);
        $panel.append($symbolUp);
        $panel.append($panelAnimated);

        upperObj.Parts = {};
        upperObj.Parts.$panel = $panel;
        upperObj.Parts.$panelAnimated = $panelAnimated;
        upperObj.Parts.$symbolUp = $symbolUp;

        upperObj.$obj.append($panel);
    };

    upper.prototype.BindEvent = function() {
        var upperPrivate = this;
        var containerScroll = $(this.options.containerScroll);

        containerScroll.on('scroll.upper', function() {
            if (containerScroll.scrollTop() < upperPrivate.options.enabledTop) {
                upperPrivate.Parts.$symbolUp[upperPrivate.options.effectHide](upperPrivate.options.effectHideSpeed);
            } else {
                upperPrivate.Parts.$panel.show();
                upperPrivate.Parts.$symbolUp[upperPrivate.options.effectShow](upperPrivate.options.effectShowSpeed);
            }
        });

        upperPrivate.Parts.$panel.on('mouseover.upper', function() {
            if (containerScroll.scrollTop() > upperPrivate.options.enabledTop) {
                upperPrivate.Parts.$panelAnimated.stop(true, true);
                upperPrivate.Parts.$panelAnimated[upperPrivate.options.effectShow](upperPrivate.options.effectShowSpeed);
            }
        });

        upperPrivate.Parts.$panel.on('mouseout.upper', function() {
            upperPrivate.Parts.$panelAnimated.stop(true, true);
            upperPrivate.Parts.$panelAnimated[upperPrivate.options.effectHide](upperPrivate.options.effectHideSpeed);
        });

        upperPrivate.Parts.$panel.on('click.upper', function() {
            $(upperPrivate.options.containerScrollUp).animate({ scrollTop: 0 }, upperPrivate.options.scrollSpeed);
        });
    };

    upper.prototype.defaultOptions = {
        symbolUp: '&uarr;',
        effectShow: 'fadeIn',
        effectHide: 'fadeOut',
        effectShowSpeed: 'normal',
        effectHideSpeed: 'normal',
        enabledTop: 50,
        containerScroll: window,
        containerScrollUp: 'body, html',
        scrollSpeed: 1000
    };
})(jQuery);