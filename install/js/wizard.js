$(function () {
    $("fieldset.group > legend > input").each(function () {
        ShowDivVisible.call(this, 0);
    });

    $("fieldset.group > legend > input").click(function () {
        ShowDivVisible.call(this, "slow");
    });

    if ($("#check_folders").length) {
        $("#check_folders").click(function () {
            getAccessInfo();
        });
    }
});

function getAccessInfo() {
    var content = $("#access_info");
    $.ajax({
        cache: false,
        type: "POST",
        url: "./httphandlers/getdirectoryaccessinfo.ashx",
        beforeSend: function () {
            $("#load_content").show();
        },
        success: function (data) {
            content.html(data);
        },
        complete: function () {
            $("#load_content").hide();
        }
    });
};

function ShowDivVisible(speed) {

    var sender = $(this);
    var parentBlock = sender.closest("fieldset.group");
    var block = parentBlock.find(".block-options");

    if (parentBlock.hasClass("simple")) return false;

    if (parentBlock.length && !sender.is(":checked")) {
        block.slideUp(speed);
        parentBlock.addClass("collapsed-block");
    }
    else {
        block.slideDown(speed);
        parentBlock.removeClass("collapsed-block");
    }
}

function ShowMethod(id) {
    $(document.getElementById(id)).show();
    $(document.getElementById(id)).siblings().hide();
}