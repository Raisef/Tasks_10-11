$(function(){
    $(".showpassword").each(function(index,input) {
        var $input = $(input);
        $("<p class='opt'/>").append(
            $("<input type='checkbox' class='showpasswordcheckbox' id='showPassword' />").click(function() {
                var change = $(this).is(":checked") ? "text" : "password";
                var rep = $("<input placeholder='Password' type='" + change + "' />")
                    .attr("id", $input.attr("id"))
                    .attr("name", $input.attr("name"))
                    .attr('class', $input.attr('class'))
                    .val($input.val())
                    .insertBefore($input);
                $input.remove();
                $input = rep;
            })
        ).append($("<label for='showPassword'/>").text("Показать пароль")).insertAfter($input.parent());
    });
    $('#showPassword').click(function () {
        if ($("#showPassword").is(":checked")) {
            $('.icon-lock').addClass('icon-unlock');
            $('.icon-unlock').removeClass('icon-lock');
        } else {
            $('.icon-unlock').addClass('icon-lock');
            $('.icon-lock').removeClass('icon-unlock');
        }
    });
});



