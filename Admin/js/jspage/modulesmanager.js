
$(function () {

    if ($(".ckbActiveModule").length) {
        var ckb = $(".ckbActiveModule");

        $(".ckbActiveModule").on('change',function () {
            setModuleActive($(this).attr("data-modulestringid"), $(this).is(":checked"));
        });
    }
});

function setModuleActive(moduleStringId, active) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { moduleStringId: moduleStringId, active: active },
        url: "httphandlers/modules/setmoduleactive.ashx",
        success: function (data) {
            if ($(".lblIsActive").length) {
                $(".lblIsActive").text(data.state);
                $(".lblIsActive").css("color", data.active ? "green" : "red");
            }
        },
        error: function () {

        }
    });
}

function installModule(moduleStringId, moduleIdOnRemoteServer, moduleVersion) {
    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: {
            moduleStringId: moduleStringId,
            moduleIdOnRemoteServer: moduleIdOnRemoteServer,
            moduleVersion: moduleVersion
        },
        url: "httphandlers/modules/installmodule.ashx",
        success: function (data) {
            if (data.result != "error") {
                window.location = data.result;
            }
        },
        error: function () {
            var i = 1;
        }
    });
}