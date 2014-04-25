/// <reference path="../Shared/_shared.js" />

var kms_friendRequests_downloadInProgress = false;

function doKMS_friendRequests_refresh() {
    if (kms_friendRequests_downloadInProgress) return;
    kms_friendRequests_downloadInProgress = true;

    $.getJSON(
        getKMS_ajaxUri("FriendRequests.json")
    ).fail(function () {
        console.log("[!] - Error de descarga de datos");
    }).done(function (data) {
        $.each(data, function(i) {
            // Obtener la(s) solicitudes que ya se está(n) mostrando para ésta data
            var requestsMatching = $('#solicitudes .solicitudItem:not(.template)').filter(function() {
                return $(this).data('uid') == data[i].userId;
            });

            // Si no hay ninguna data, añadimos el item
            if ( requestsMatching.length < 1 ) {
                var friendItem = $('#solicitudes .solicitudItem.template')
                   .clone(true)
                   .data({
                       'uid': data.userId,
                       'total-distance': data.totalDistance,
                       'total-co2': data.totalCo2,
                       'total-kcal': data.totalKcal,
                       'total-cash': data.totalCash
                   });

                friendItem.children('img').attr('src', data.pictureUri);
                friendItem.children('h4').text(data.name + ' ' + data.lastName);
                friendItem.children('.kms').text(data.totalDistance + ' ' + $('body').data('distance-unit').toUpperCase());

                $('#solicitudes').append(friendItem);
                friendItem.slideDown();
            }
        });
    }).always(function () {
        kms_friendRequests_downloadInProgress = false;
    });
}

function doKMS_friendRequests_accept() {
    var friendItem = $(this).parent().parent();

    if (friendItem.data('aip')) return;
    friendItem.data('aip') = true;

    $.post(getKMS_ajaxUri("FriendRequestAccept.json"), {
        friendId: friendItem.data('uid')
    }).fail(function () {
        console.log("[!] - Error de descarga de datos");
    }).done(function () {
        friendItem.animate({
            width: 'hide',
            opacity: 'hide'
        }, {
            complete: function () {
                var newFriendItem = $('#amigos .amigosItem.template')
                    .clone(true)
                    .data($(this).data());

                friendItem.children('img').attr('src', $(this).children('img').attr('src'));
                friendItem.children('h4').text($(this).children('h4'));
                friendItem.children('.kms').text($(this).children('.kms'));

                $('#amigos').prepend(friendItem);
                friendItem.slideDown(); // TODO: Hacer las animaciones más suavecitas

                $(this).remove();
                doKMS_friendRequests_refresh();
            }
        })
    }).always(function () {
        friendItem.data('aip') = false;
    });
}

function doKMS_friendRequest_reject() {
    var friendItem = $(this).parent().parent();

    if (friendItem.data('aip')) return;
    friendItem.data('aip') = true;

    $.post(getKMS_ajaxUri("FriendRequestReject.json"), {
        friendId: friendItem.data('uid')
    }).fail(function () {
        console.log("[!] - Error de descarga de datos");
    }).done(function () {
        friendItem.animate({
            width: 'hide',
            opacity: 'hide'
        }, {
            complete: function () {
                $(this).remove();
            }
        });
        doKMS_friendRequests_refresh();
    });
}