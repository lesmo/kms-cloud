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

$(function () {
    var $fileInput = $('#Usuario input[type=file]');
    $('#Usuario input[type=file]').change(function () {
        if ($fileInput.val().length < 1)
            return;
        else
            $fileInput.parent().trigger('submit');
    });
});