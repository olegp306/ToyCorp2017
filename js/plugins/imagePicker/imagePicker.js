(function ($, body) {
    $(function () {

        body.on('click', '[data-imagePicker-caller]', function (e) {
            var target = $(this),
                arrImgs = Advantshop.Utilities.Eval(target.attr('data-imagePicker-img')),
                group = $('[data-imagePicker-group="' + target.attr('data-imagePicker-group') + '"]'),
                isInplaceEnabled = Advantshop.ScriptsManager.Inplace != null ? Advantshop.ScriptsManager.Inplace.prototype.checkEnabled() : false,
                place,
                path;


            for (var key in arrImgs) {
                place = $(arrImgs[key].place);

                if (place.length === 0 || arrImgs[key].src == null || arrImgs[key].src.length === 0) {
                    continue;
                }

                switch (place[0].tagName.toLowerCase()) {
                    case 'img':
                        place[0].src = arrImgs[key].src + (isInplaceEnabled === true ? '?rnd=' + Math.random() : '');
                        place[0].alt = arrImgs[key].title;
                        place[0].title = arrImgs[key].title;
                        break;
                    case 'a':
                        place[0].href = arrImgs[key].src;
                        place[0].title = arrImgs[key].title;

                        if (place.hasClass('cloud-zoom') === true) {

                            if (place.data('zoom') != null) {
                                place.data('zoom').destroy();
                            }

                            place.CloudZoom();
                        }

                        break;
                    default:
                        break;
                }

                group.removeClass('selected');
                target.addClass('selected');
            }
        })
    });
})(jQuery, $(document.body));