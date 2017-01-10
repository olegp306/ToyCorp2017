; (function () {
    'use strict';

    $(function () {
        $("#btnRegister").on("click", function () {
            if (!$("#form").valid("reg")) {
                return;
            }
        });
    });
})();