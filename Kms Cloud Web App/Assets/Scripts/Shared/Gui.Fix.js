$(function () {
    // Establece el alto del Sidebar al alto de los Contenidos
    $('#sidebar').height(
        $('#contenidos').height()
    );

    // Establece el idioma de jQuery DatePicker
    var lang = $('html').attr('lang');

    if ( Object.has($.datepicker.regional, lang) )
        $.datepicker.setDefaults($.datepicker.regional[lang]);
});