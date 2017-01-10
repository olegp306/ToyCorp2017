
(function($){

    $(function () {

        var feedbackForm = $("#feedBack"),
            feedbackLink = $(".link-feedback");


        $(".link-feedback, .close-feedback").on('click', function () {
            if (feedbackForm.is(":visible") === true) {
                feedbackForm.removeClass("feedbackCenter");
                feedbackLink.removeClass("feedbackLinkOpen");
            } else {
                feedbackForm.addClass("feedbackCenter");
                feedbackLink.addClass("feedbackLinkOpen");
                initValidation($("form"));
            }
        });
    });

})(jQuery);






