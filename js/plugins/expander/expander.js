(function($) {

    var advantshop = Advantshop, scriptManager = advantshop.ScriptsManager, utilities = advantshop.Utilities;

    var expander = function(selector, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);

        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Expander = expander;

    expander.prototype.InitTotal = function() {
        if (!$('.expander-enable').length) {
            return;
        }

        var objects = $('[data-plugin ="expander"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            expander.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-expander-options')) || {});
        }
    };

    $(expander.prototype.InitTotal); // call document.ready

    expander.prototype.Init = function(selector, options) {

        var expanderObj = new expander(selector, options);

        expanderObj.GenerateHtml();
        expanderObj.BindEvent();

        return expanderObj;
    };

    expander.prototype.GenerateHtml = function() {
        var expanderObj = this;
        var options = expanderObj.options;
        var $controls = expanderObj.$obj.children('[data-expander-control]');
        $controls.addClass('expander-control');

        var content;
        for (var i = 0, arrLength = $controls.length; i < arrLength; i++) {

            content = $controls.eq(i).attr('data-expander-control');

            if ($(content).is(':visible')) {
                $controls.eq(i).removeClass(options.classCollapsed).addClass(options.classExpanded);
            } else {
                $controls.eq(i).removeClass(options.classExpanded).addClass(options.classCollapsed);
            }
        }
    };

    expander.prototype.BindEvent = function() {
        var expanderObj = this;
        var $obj = expanderObj.$obj;

        expanderObj.controls = $obj.children('[data-expander-control]');

        if (this.controls.length) {
            expanderObj.controls.on(expanderObj.options.eventShow, function(e) {
                var $control = $(e.target);
                var $content = $($control.attr('data-expander-control'));

                if (!$content.length) {
                    return;
                }

                if ($content.is(':visible')) {
                    expanderObj.Hide($control, $content);
                } else {
                    expanderObj.Show($control, $content);
                }

                e.stopPropagation();
            });
        }
    };

    expander.prototype.Show = function($control, $content) {

        if (!$content.length) {
            $content = $(control.attr('data-expander-control'));
        }

        if (!$content.length) {
            $control.attr('data-expander-visible', 'false');
            return;
        }

        var options = this.options;

        $content.stop(true, true);

        $content[options.animationShow](options.animationSpeed, function() {
            $control.removeClass(options.classCollapsed).addClass(options.classExpanded);
            $control.attr('data-expander-active', 'true');
        });
    };

    expander.prototype.Hide = function($control, $content) {

        if (!$content.length) {
            $content = $($control.attr('data-expander-control'));
        }

        if (!$content.length) {
            control.attr('data-expander-visible', 'false');
            return;
        }

        var options = this.options;

        $content.stop(true, true);

        $content[options.animationHide](options.animationSpeed, function() {
            $control.removeClass(options.classExpanded).addClass(options.classCollapsed);
            $control.attr('data-expander-active', 'false');
        });
    };

    expander.prototype.defaultOptions = {
        eventShow: 'click.expander',
        eventHide: 'click.expander',
        animationShow: 'slideDown',
        animationHide: 'slideUp',
        animationSpeed: 700,
        classExpanded: 'expander-expanded',
        classCollapsed: 'expander-collapse'
    };

})(jQuery);