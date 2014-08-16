/// <reference path="_shared.js" />

// Genera una URL hacia el Recurso AJAX del Servidor
function getKMS_ajaxUri(file) {
    return sprintf(
        "%s//%s/%s/DynamicResources/Ajax/%s",
        location.protocol,
        location.host,
        $('html').data('culture-code'),
        file
    );
}

function setKMS_userPicture() {
    $('#Usuario input[type=file]').trigger('click');
}

$(function() {
    var $fileInput = $('#Usuario input[type=file]');
    $('#Usuario input[type=file]').change(function() {
        if ($fileInput.val().length < 1)
            return;
        else
            $fileInput.parent().trigger('submit');
    });
});

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}


var now = new Date();
var later = new Date();

// Set time for how long the cookie should be saved
later.setTime(now.getTime() + 365 * 24 * 60 * 60 * 1000);

// Set cookie for the time zone offset in minutes
setCookie("tzom", now.getTimezoneOffset(), later, "/");