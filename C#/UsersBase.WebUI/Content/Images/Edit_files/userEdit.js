/// <reference path="jquery-2.2.3.js" />
/// <reference path="jquery-2.2.3.intellisense.js" />
/// <reference path="jquery.validate-vsdoc.js" />
/// Здесь был Вася!
(function () {
    "use strict";

    var $nameCheckbox = $('#NameCheckbox'),
        $dateCheckbox = $('#DateCheckbox'),
        $nameInput = $('#NameField'),
        $dateInput = $('#BirthDateField');

    if ($nameCheckbox.is(':checked')) {
        if ($nameInput.prop("disabled")) {
            $nameInput.prop("disabled", false);
        }
    } else {
        if (!$nameInput.prop("disabled")) {
            $nameInput.prop("disabled", true);
        }
    }

    if ($dateCheckbox.is(':checked')) {
        if ($dateInput.prop("disabled")) {
            $dateInput.prop("disabled", false);
        }
    } else {
        if (!$dateInput.prop("disabled")) {
            $dateInput.prop("disabled", true);
        }
    }

    $dateCheckbox.change(function () {
        var $checkbox = $(this);

        if ($checkbox.is(':checked')) {
            if ($dateInput.prop("disabled")) {
                $dateInput.prop("disabled", false);
            }
        } else {
            if (!$dateInput.prop("disabled")) {
                $dateInput.val('');
                $dateInput.prop("disabled", true);
            }
        }
    });

    $nameCheckbox.change(function () {
        var $checkbox = $(this);

        if ($checkbox.is(':checked')) {
            if ($nameInput.prop("disabled")) {
                $nameInput.prop("disabled", false);
            }
        } else {
            if (!$nameInput.prop("disabled")) {
                $nameInput.val('');
                $nameInput.prop("disabled", true);
            }
        }
    });

    $("#UserEdit").validate({
        rules: {
            changeList: {
                required: true,
                minlength: 1
            },
            name: {
                required: "#NameCheckbox:checked",
                minlength: 2
            },
            date: {
                reqired: "#DateCheckbox:checked"
            }
        }
    });
}());