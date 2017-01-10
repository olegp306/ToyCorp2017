
(function ($) {
    var mpCurrentSaasData;

    $(function () {
        if ($("#modalCurrentSaasData").length) {
            mpCurrentSaasData = $.advModal({
                title: 'Сообщение',
                htmlContent: $("#modalCurrentSaasData"),
                control: $(".battery"),
                afterClose: function () { initValidation($("form")); },
                clickOut: false
            });
        }

        var radioChart = $('[name="gr-chart"]');

        if (radioChart.length > 0) {
            radioChart.on('click', function () {
                for (var i = 0, arrLength = radioChart.length; i < arrLength; i += 1){
                    $(radioChart[i].value).hide();
                }

                $(this.value).show();

                Advantshop.ScriptsManager.Chart.prototype.InitTotal();
            });
        }
    });

    function showChartOrderWeek() {
        $('#chartMounth').hide();
        $('#chartWeek').show();
    }

    function showChartOrderMounth() {
        $('#chartMounth').show();
        $('#chartWeek').hide();
    }
})(jQuery)




