; (function ($) {

    'use strict';

    var advantshop = Advantshop,
        scriptManager = advantshop.ScriptsManager,
        utilities = advantshop.Utilities,
        CLASS_COLLAPSE = 'vis-collapse',
        CLASS_EXPAND = 'vis-expand';

    var vis = function (selector, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);
        this.$items = this.$obj.find(this.options.itemSelector);
        this.$control = this.$obj.find(this.options.controlSelector);
        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Vis = vis;

    vis.prototype.InitTotal = function () {

        var objects = $('[data-plugin="vis"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            vis.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-vis-options')) || {});
        }
    };

    $(vis.prototype.InitTotal); // call document.ready

    vis.prototype.Init = function (selector, options) {

        var visObj = new vis(selector, options);

        visObj.GenerateHtml();
        visObj.BindEvent();

        return visObj;
    };

    vis.prototype.GenerateHtml = function () {
        var visObj = this,
            control = visObj.$control,
            visibility = visObj.$obj.hasClass(CLASS_EXPAND) === true;

        visObj.CheckCanPlugin() === false ? control.addClass('vis-control-hidden') : control.removeClass('vis-control-hidden');

        if (visibility === true) {
            visObj.Show();
            control.text(visObj.options.textControlHide);
        } else {
            visObj.Hide();
            control.text(visObj.options.textControlShow);
        }
    };

    vis.prototype.BindEvent = function () {
        var visObj = this,
            control = visObj.$control;

        control.off('click.vis');

        control.on('click.vis', function () {
            visObj.ControlVisiblity();
        });
    };

    vis.prototype.ControlVisiblity = function () {

        var visObj = this,
            control = visObj.$control,
            visibility = visObj.$obj.hasClass(CLASS_EXPAND) === true;

        visObj.CheckCanPlugin() === false ? control.addClass('vis-control-hidden') : control.removeClass('vis-control-hidden');

        if (visibility === false) {
            visObj.Show();
            control.text(visObj.options.textControlHide);
        } else {
            visObj.Hide();
            control.text(visObj.options.textControlShow);
        }
    };

    vis.prototype.CheckCanPlugin = function () {
        return this.$items.length > this.options.visible;
    };

    vis.prototype.Show = function () {
        var visObj = this;

        visObj.$items.removeClass(visObj.options.classHidden);

        visObj.$obj.removeClass(CLASS_COLLAPSE).addClass(CLASS_EXPAND);
    };

    vis.prototype.Hide = function () {
        var visObj = this,
            opts = visObj.options,
            items = visObj.$items,
            startIndexHiddenElems,
            itemExtra,
            itemExtraIndex;

        if (opts.itemExtraSelector != null) {
            itemExtra = items.filter(opts.itemExtraSelector);
            itemExtraIndex = items.index(itemExtra) + 1;
            startIndexHiddenElems = opts.visible < itemExtraIndex ? itemExtraIndex : opts.visible;
        } else {
            startIndexHiddenElems = opts.visible;
        }

        items.slice(startIndexHiddenElems).addClass(opts.classHidden);

        visObj.$obj.removeClass(CLASS_EXPAND).addClass(CLASS_COLLAPSE);
    };

    vis.prototype.defaultOptions = {
        visible: 7,
        itemSelector: '.js-vis-item',
        itemExtraSelector: null,
        controlSelector: '.js-vis-control',
        classHidden: 'vis-hidden',
        textControlShow: 'Показать ещё',
        textControlHide: 'Скрыть'
    };

})(jQuery);
