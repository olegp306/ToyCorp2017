$(function () {
    if ($("#modalGiftCertificate").length) {
        $.advModal({
            htmlContent: $("#modalGiftCertificate"),
            control: "#printCert",
            beforeOpen: ShowModalPreviewGiftCertificate,
            title: localize("giftcertificateTitle")
        });
    }

    if ($("#hfPaymentMethod").length) {
        $("input[type=radio][name='rbListPaymentMetods']").first().attr("checked", "checked");
        $("#hfPaymentMethod").val($("input[type=radio][name='rbListPaymentMetods']").first().val());
        $("input[type=radio][name='rbListPaymentMetods']").click(function () {
            $("#hfPaymentMethod").val($(this).val());
        });
    }
});

function ShowModalPreviewGiftCertificate() {
    getStingPrice($("#txtSum").val(), function (price) {
        $("#lblToName").text($("#txtTo").val());
        $("#lblFromName").text($("#txtFrom").val());
        $("#lblMessage").html($("#txtMessage").val().replace(/\n/g, "<br />"));
        $("#lblSum").html(price);
    });
}
