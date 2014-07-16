/// <reference path="../Shared/_shared.js" />

var added = false;

function doKMS_friendRequest_send(friendId) {
    if (added)
        return;

    var $button = $(this);
    $button.addClass('disabled').attr('href', '').text('Enviando solicitud ...');

    $.post(getKMS_ajaxUri("FriendRequestAccept.json"), {
        friendId: friendItem.data('uid')
    }).fail(function () {
        console.log("[!] - Error de descarga de datos");
    }).done(function () {
        $button.text('Solicitud enviada');
    });
}