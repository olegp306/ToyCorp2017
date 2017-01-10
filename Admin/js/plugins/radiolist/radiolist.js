;(function ($) {

    $(function () {

        var objects = $('[data-plugin="radiolist"]'),
            labels = objects.find('label'),
            radios = objects.find('input[type="radio"]');

        objects.addClass('radiolist');
        radios.filter(':checked').closest('label').addClass('radiolist-checked');

        objects.on('click', function (e) {
            var lbl = $(e.target).closest('label');

            if (lbl.is('label') === true && lbl.hasClass('radiolist-checked') === false) {
                labels.removeClass('radiolist-checked');
                lbl.addClass('radiolist-checked');
            }
        });
    });
})(jQuery);