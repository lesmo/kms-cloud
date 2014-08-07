function doKMS_setupGraphs() {
    var lang = $('html').attr('lang');
    var i18n = $.datepicker.regional[""];

    if (Object.has($.datepicker.regional, lang))
        i18n = $.datepicker.regional[lang];

    Highcharts.setOptions({
        global: {
            useUTC: false
        },
        lang: {
            months: i18n.monthNames,
            shortMonths: i18n.monthNamesShort,
            weekdays: i18n.dayNames,
            resetZoom: 'Restablecer Zoom',
            resetZoomTitle: 'Restablecer Zoom a 1:1',
            rangeSelectorZoom: 'Zoom',
            rangeSelectorFrom: 'Desde',
            rangeSelectorTo: 'Hasta',
            loading: 'Cargando ...'
        }
    });

    $.datepicker.setDefaults({
        dateFormat: 'yy-mm-dd',
        onSelect: function () {
            this.onchange();
            this.onblur();
        }
    });
}