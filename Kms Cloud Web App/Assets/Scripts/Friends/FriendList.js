/// <reference path="../Shared/_shared.js" />

var kms_friendList_currentPage        = 1;
var kms_friendList_downloadInProgress = false;

function doKMS_friendList_count() {
    return $('#amigos .amigosItem').length;
}

function doKMS_friendList_nextPage() {
    if (kms_friendList_downloadInProgress) return;
    kms_friendList_downloadInProgress = true;
    
    $('#amigosVerMas img').show();
    $('#amigosVerMas span').hide();

    $.ajax(getKMS_ajaxUri("FriendList.json"), {
        page: kms_friendList_currentPage++
    }).fail(function () {
        kms_friendList_currentPage--;
        console.log("[!] - Error de descarga de datos");
    }).done(function (data) {
        $.each(data, function (i, friend) {
            var friendItem = $('#amigos .amigosItem.template')
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

            $('#amigos').append(friendItem);
                
            friendItem.delay(
                Math.floor((i - 1) / 3) * 100
            ).slideDown(200);
        });
    }).always(function () {
        kms_friendList_downloadInProgress = false;

        if (doKMS_friendList_count() < parseInt($('#amigos').data('total-count'))) {
            $('#amigosVerMas img').hide();
            $('#amigosVerMas span').show();
        } else {
            $('#amigosVerMas').remove();
        }
    });
}

$(function () {
    $('#amigosVerMas').click(doKMS_friendList_nextPage);
});