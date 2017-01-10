$(function () {

    // Tracking events
    // Trigger example: $(document).trigger("add_to_cart");
    $(document).on("add_to_cart", function (e, url) {
        trackEvent("addToCart");
        })
        .on("buy_one_click_pre", function () {
            trackEvent("buyOneClickForm");
        })
        .on("buy_one_click_confirm", function () {
            trackEvent("buyOneClickConfirm");
        })
        .on("compare.add", function () {
            trackEvent("addToCompare");
        })
        .on("add_to_wishlist", function () {
            trackEvent("addToWishlist");
        })
        .on("send_feedback", function () {
            trackEvent("sendFeedback");
        })
        .on("add_response", function () {
            trackEvent("addResponse");
        })
        .on("module_callback", function () {
            trackEvent("getCallBack");
        });


    // Send event
    function trackEvent(target) {
        try {
            var counterId = $(".yacounterid").attr("data-counterId");
            var yaCounter = window["yaCounter" + counterId];
            yaCounter.reachGoal(target);
        } catch (err) {
        }
    }
});