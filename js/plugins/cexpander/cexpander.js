; (function ($) {
    var advantshop = Advantshop, scriptManager = advantshop.ScriptsManager, utilities = advantshop.Utilities, exContent = $('<div />', { 'class': 'cexpander-wrap-content' });

    var cExpander = function (selector, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);
        this.heightSmall = parseInt(this.$obj.attr('data-cexpander-height') || this.options.heightSmall);
        this.textShow = this.$obj.val() || this.$obj.html() || localize('Expande');
        this.textHide = this.$obj.attr('data-text-hide') || localize('Collapse');
        var content = $(this.$obj.attr('data-cexpander-content'));

        if (content.length === 0) {
            throw Error('Not found content for cExpander');
        }

        this.$content = content;

        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.CExpander = cExpander;

    cExpander.prototype.InitTotal = function () {

        var objects = $('[data-plugin ="cexpander"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            cExpander.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-cexpander-options')) || {});
        }
    };

    $(cExpander.prototype.InitTotal); // call document.ready

    cExpander.prototype.Init = function (selector, options) {

        var cExpanderObj = new cExpander(selector, options);

        cExpanderObj.GenerateHtml();
        cExpanderObj.BindEvent();

        return cExpanderObj;
    };

    cExpander.prototype.GenerateHtml = function () {
        this.$content.addClass(this.options.classExpande).css({ height: this.heightSmall, minHeight: this.heightSmall });


        this.$content.wrapInner(exContent.clone());
        this.$wrap = this.$content.children();
        this.heightMax = this.$wrap.outerHeight();
    }

    cExpander.prototype.BindEvent = function () {
        var cExpanderObj = this,
            $obj = this.$obj,
            $content = this.$content,
            opts = this.options;

        $obj.on('click.cexpander', function (e) {

            e.preventDefault();

            if ($content.hasClass(opts.classCollapse) === true) {

                if ($obj.is('input, button') === true) {
                    $obj.val(cExpanderObj.textShow);
                } else if ($obj.is('img') === true) {
                    $obj.attr('title', cExpanderObj.textShow);
                } else {
                    $obj.html(cExpanderObj.textShow);
                }

                $content.removeClass(opts.classCollapse).addClass(opts.classExpande);

                $content.stop(true, true).animate({ height: cExpanderObj.heightSmall }, opts.speed);

            } else {

                if ($obj.is('input, button') === true) {
                    $obj.val(cExpanderObj.textHide);
                } else if ($obj.is('img') === true) {
                    $obj.attr('title', cExpanderObj.textHide);
                } else {
                    $obj.html(cExpanderObj.textHide);
                }

                $content.removeClass(opts.classExpande).addClass(opts.classCollapse);

                $content.stop(true, true).animate({ height: cExpanderObj.heightMax }, opts.speed);
            }
        });

    };

    cExpander.prototype.defaultOptions = {
        heightSmall: 150,
        speed: 600,
        textExpande: 'Раскрыть',
        textCollapse: 'Cкрыть',
        classExpande: 'cexpander-expande',
        classCollapse: 'cexpander-collapse'
    };

})(jQuery);
