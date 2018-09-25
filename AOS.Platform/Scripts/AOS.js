$(function ($) {

    // Modal login
    var ajaxFormSubmit = function () {
        debugger;
        var $form = $(this);
        var $target = $($form.attr('data-aos-target'));
        var $mok = $('#loginmodal');
        var $valSummary = $("#validationSummary");

        $.ajax({
            type: $form.attr('method'),
            url: $form.attr('action'),
            data: $form.serialize(),

            success: function (data) {
                //alert('xxxx');
                //$mok.modal('hide');
                //$resp = data;
                if (data.ok) {
                    $mok.modal('hide');
                    window.location = data.newurl;
                }
                else {
                    $valSummary.show();
                    $valSummary.val = data.msg;
                }
            }
        });

        //if ($resp.IsOK) {
        //    alert("OK");
        //}
        //else {
        //    alert("Not OK");
        //}


        //$mok.modal('hide');

        return false;
    }

    $("form[data-aos-ajax='true']").submit(ajaxFormSubmit)
});