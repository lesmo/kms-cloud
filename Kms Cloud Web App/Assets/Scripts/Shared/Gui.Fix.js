/**
 * Redimensiona la barra lateral al tamaño del contenido. Debería llamarse
 * cada vez que se modifica la altura del contenido.
 */
function doKMS_redimSidebar() {
    $('#sidebar').height(
        $('#contenidos').height()
    ).css({
        visibility: 'visible'
    }).animate({
        opacity: 1
    });
}

// > Redimensionar barra lateral en cuanto haya dimensiones
$(window).ready(doKMS_redimSidebar);

$(function () {
    // Establece el idioma de jQuery DatePicker (y las Highcharts, incidentalmente)
    var lang = $('html').attr('lang');

    if ( Object.has($.datepicker.regional, lang) )
        $.datepicker.setDefaults($.datepicker.regional[lang]);
});