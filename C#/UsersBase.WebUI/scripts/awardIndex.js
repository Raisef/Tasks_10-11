/// <reference path="jquery-2.2.3.js" />
/// <reference path="jquery-2.2.3.intellisense.js" />

(function () {
    var $deleteButton = $('.delete-button');
        

    $deleteButton.click(function () {
        
        if (confirm("Do you really want to delete award from all users?")) {
            var $btn = $(document.activeElement, this),
                $form = $btn.parent();

            $form.submit();
                    }
    });

}());