/// <reference path="jquery-2.2.3.js" />
/// <reference path="jquery-2.2.3.intellisense.js" />
/// <reference path="jquery.validate-vsdoc.js" />
(function () {
    "use strict";

    $("#UserEdit").validate({
        rules: {
            name: {
                required: "true",
                minlength: 2
            }
        }
    });
}());