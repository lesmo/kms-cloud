﻿function doKMS_setupGraphs() {
    Highcharts.setOptions({
        global: {
            useUTC: false
        },
        lang: {
            months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            shortMonths: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            resetZoom: 'Restablecer Zoom',
            resetZoomTitle: 'Restablecer Zoom a 1:1',
            rangeSelectorZoom: 'Zoom',
            rangeSelectorFrom: 'Desde',
            rangeSelectorTo: 'Hasta',
            loading: 'Cargando ...'
        }
    });
}