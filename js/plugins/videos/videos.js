(function ($) {

    $(window).load(function () {
        var videos = $('[data-plugin="videos"]');
        var productId;
        for (var i = 0, arrLength = videos.length; i < arrLength; i += 1) {
            productId = videos.eq(i).attr('data-productId');

            if (productId == null) {
                return;
            }

            generate(productId, videos.eq(i));
        }

    });

    function generate(productId, place) {
        $.ajax({
            url: $("base").attr("href") + 'httphandlers/details/videos.ashx',
            dataType: 'JSON',
            data: { productId: productId },
            success: function (data) {
                if (data == null || data.length == 0) {
                    return;
                }

                var html = new EJS({ url: 'js/plugins/videos/templates/default.tpl' }).render(data);

                place.html(html);
            }
        });
    }
})(jQuery);