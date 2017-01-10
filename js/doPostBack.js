function __doPostBackJQ(eventTarget, eventArgument) {
    var target = $("#__EVENTTARGET");
    var arg = $("#__EVENTARGUMENT");

    if (!target.length) {
        target = $("<input>", { type: "hidden", id: "__EVENTTARGET", name: "__EVENTTARGET" });
        $("#form").append(target);
    }
    if (!arg.length) {
        arg = $("<input>", { type: "hidden", id: "__EVENTARGUMENT", name: "__EVENTARGUMENT" });
        $("#form").append(arg);
    }

    target.val(eventTarget);
    arg.val(eventArgument);

    $("#form").submit();
}

