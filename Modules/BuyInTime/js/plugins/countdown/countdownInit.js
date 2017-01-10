; (function ($) {
    $(function () {
        var countdowns = $('[data-plugin="countdown"]'),
            arr,
            counter = 0,
            item,
            itemId,
            timeArr,
            isBigSize,
            params;



        for (var i = 0, il = countdowns.length; i < il; i += 1) {

            item = countdowns.eq(i);
            itemId = item.attr('id');

            arr = item.attr('data-countdown').split(' ');
            dateArr = arr[0] != null && arr[0].length > 0 ? arr[0].split('.') : null;
            timeArr = arr[1] != null && arr[1].length > 0 ? arr[1].split(':') : null;

            if (itemId === undefined) {
                itemId = 'countdownDynamicId_' + counter;
                item.attr('id', itemId);
                counter += 1;
            }

            isBigSize = item.closest('.countdown-big').length > 0

            params = {
                target: itemId,
                day: dateArr[0],
                month: dateArr[1],
                year: dateArr[2],
                hour: timeArr[0] || 0,
                minute: timeArr[1] || 0,
                second: timeArr[2] || 0,
                ampm: 'pm',
                style: 'flip',
                inline: true,
                rangeHi: 'day',
                width: 170,
                height:40
            };

            new Countdown(params);
        }
    });
})(jQuery);