(function ($, $win) {

    var advantshop = Advantshop,
        utilities = advantshop.Utilities,
        scriptManager = advantshop.ScriptsManager;

    var chart = function (selector, data, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);
        this.data = data;
        return this;
    };

    scriptManager.Chart = chart;

    chart.prototype.InitTotal = function () {

        var objs = $('[data-plugin="chart"]:visible'),
            obj, data, opts;

        for (var i = 0, arrLength = objs.length; i < arrLength; i += 1) {
            obj = objs.eq(i);
            data = utilities.Eval(obj.attr('data-chart'));
            opts = utilities.Eval(obj.attr('data-chart-options')) || {};

            if (data == null) {
                continue;
            }

            chart.prototype.Init(obj, data, opts);
        }
    };

    $(chart.prototype.InitTotal);

    chart.prototype.Init = function(selector, data, options) {

        if ($.plot == null) {
            return;
        }

        var chartObj = new chart(selector, data, options);

        chartObj.$obj.hide();

        $.plot(chartObj.$obj, chartObj.data, chartObj.options);

        chartObj.$obj.show();

        chartObj.BindEvent();
    };

    chart.prototype.BindEvent = function () {
        var chartObj = this, previousPoint = null;

        if (utilities.Events.isExistEvent($win, 'resize.chartResize') !== true) {
            $win.on('resize.chartResize', function () {
                if (this.resizeTO != null) {
                    clearTimeout(this.resizeTO);
                }

                this.resizeTO = setTimeout(function () {
                    chart.prototype.InitTotal();
                }, 100);
            });
        }
        if (utilities.Events.isExistEvent(chartObj.$obj, 'plothover') !== true) {
            chartObj.$obj.on('plothover', function (event, pos, item) {

                if (item != null) {
                    if (previousPoint != item.dataIndex) {

                        previousPoint = item.dataIndex;

                        var content = "";
                        
                        if (!$.isArray(item.datapoint[1])) {
                            content = "<div>" + $.plot.formatDate(new Date(item.datapoint[0]), '%d %b') + "</div> " +
                                      "<b>" + item.datapoint[1] + "</b>";
                        } else {
                            content = "<div>" + item.series.label + "</div> <b>" + item.datapoint[1][0][1] + "</b>";
                        }

                        chartObj.showTooltip(item, pos.pageX, pos.pageY, content);
                    }
                } else {
                    $('#chartTooltip').hide();
                    previousPoint = null;
                }
            });
        }
    };

    chart.prototype.showTooltip = function (item, x, y, content) {
        var tooltip = $('#chartTooltip');

        if (tooltip.length === 0) {
            tooltip = $('<div />', { id: 'chartTooltip', 'class': 'chart-tooltip', css: { zIndex: '9999' } });
            tooltip.appendTo('body');
        } else {
            tooltip.hide();
        }

        tooltip.html(content);

        tooltip.css({
            top: y + 0,
            left: x + 15,
            backgroundColor: item.series.color,
            color: "#fff",
            padding: '0 10px'
    });

        tooltip.stop(true, true).fadeIn(200);
    };

    chart.prototype.defaultOptions = {
        canvas: true,
        series: {
            lines: { show: true },
            points: { show: true }
        },
        grid: {
            hoverable: true
        }
    };

})(jQuery, $(window))