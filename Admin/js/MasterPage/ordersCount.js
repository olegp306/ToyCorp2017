;(function ($) {
    $(function () {
        var ordersCount = $('[data-value="orders"]', '#MenuAdmin');

        for (var i = 0, l = ordersCount.length; i < l; i += 1) {
            Advantshop.ScriptsManager.NoticeStatistic.prototype.Init(ordersCount.eq(i), { type: 'orders', cssClass: 'orders-count' });
        }

    });

})(jQuery);







