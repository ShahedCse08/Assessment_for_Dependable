$(document).ready(function () {
    $('#togglePassword').click(function () {
        if ($('#txtPassword').attr('type') === 'password') {
            $('#txtPassword').attr('type', 'text');
            $(this).find('i').removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            $('#txtPassword').attr('type', 'password');
            $(this).find('i').removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    $('#txtUsername').focus();

    if ($.trim($('#lblMessage').text()) !== '') {
        $('#lblMessage').show();
    }

    $('.form-input').on('input', function () {
        $(this).siblings('.error-message').hide();
    });
});
