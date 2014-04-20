/// <reference path="_shared.js" />

// Genera una URI 
function getKMS_ajaxUri(file) {
    return sprintf(
        "%s//%s/%s/DynamicResources/Ajax/%s",
        location.protocol,
        location.host,
        $('html').attr('lang'),
        file
    );
}
