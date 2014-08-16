$(function() {
    // > Configurar algunas cosas globales de las gráficas (idiomas principalmente)
    var lang = $("html").attr("lang");
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
            resetZoom: "Restablecer Zoom",
            resetZoomTitle: "Restablecer Zoom a 1:1",
            rangeSelectorZoom: "Zoom",
            rangeSelectorFrom: "Desde",
            rangeSelectorTo: "Hasta",
            loading: "Cargando datos ..."
        }
    });

    $.datepicker.setDefaults({
        dateFormat: "DD, M d, yy"
    });

    // > Crear las tabs
    $('#graficaPrincipalTabs').tabs({
        heightStyle: "auto"
    });

    // > Crear datepickers
    var dayCurrentDate = null, dayCurrentReq  = null;
    $("#graficaDiaria2 .datepicker input").datepicker({
        minDate: new Date(parseInt($("body").data("user-signup"))),
        maxDate: new Date()
    }).datepicker("setDate", new Date()).select(function () {
        if (dayCurrentDate != null && dayCurrentDate == $(this).datepicker("getDate"))
            return;

        dayCurrentDate = $(this).datepicker("getDate");

        if (dayCurrentReq != null)
            dayCurrentReq.abort();

        $("#graficaDiaria2 .graph").highcharts().showLoading();

        dayCurrentReq = $.getJSON(
            getKMS_ajaxUri("OverviewDailyData.json"),
            {
                date: $(this).datepicker("getDate").getTime(),
                _: $("body").data("ajax-cache")
            }
        ).fail(function() {
            $("#graficaDiaria2 .graph").highcharts().showLoading("Ocurrió un problema durante la descarga, intenta de nuevo");
        }).done(function (data) {
            $("#graficaDiaria2 .graph").highcharts().series[0].setData(data.allData);
            $("#graficaDiaria2 .graph").highcharts().hideLoading();
        });
    });

    var monthCurrentString = null, monthCurrentReq = null;
    $("#graficaMensual2 .datepicker input").MonthPicker({
        StartYear: new Date().getFullYear()
    }).val(new Date().format("{MM}/{yyyy}")).change(function() {
        if (monthCurrentString != null && monthCurrentString == $(this).val())
            return $(this).val(new Date().format("{MM}/{yyyy}"));

        var userSignupDate = new Date(parseInt($("body").data("user-signup")));
        if ($(this).MonthPicker('GetSelectedYear') < userSignupDate.getYear() || $(this).MonthPicker('GetSelectedYear') > new Date().getYear())
            return $(this).val(new Date().format("{MM}/{yyyy}"));

        if (monthCurrentReq != null)
            monthCurrentReq.abort();

        $("#graficaMensual2 .graph").highcharts().showLoading();

        monthCurrentReq = $.getJSON(
            getKMS_ajaxUri("OverviewMonthlyData.json"),
            {
                date: new Date().getTime(),
                _: $("body").data("ajax-cache")
            }
        ).fail(function () {
            $("#graficaMensual2 .graph").highcharts().showLoading("Ocurrió un problema durante la descarga, intenta de nuevo");
        }).done(function (data) {
            $("#graficaMensual2 .graph").highcharts().series[0].setData(data.allData);
            $("#graficaMensual2 .graph").highcharts().hideLoading();
        });
    });
    
    // > Crear las gráficas
    $("#graficaDiaria .graph, #graficaMensual .graph").each(function() {
        $(this).highcharts("StockChart", {
            title: {
                text: null
            },
            chart: {
                type: "areaspline",
                panning: true,
                zoomType: "x"
            },
            credits: {
                enabled: false
            },
            legend: {
                enabled: false
            },
            navigator: {
                enabled: false
            },
            tooltip: {
                valueDecimals: 2,
                valueSuffix: " " + $("body").data("distance-unit")
            },
            xAxis: {
                minRange: 3 * 3600 * 1000,
                type: "datetime"
            },
            yAxis: {
                title: null,
                labels: {
                    format: "{value} " + $("body").data("distance-unit")
                },
                min: 0
            },
            series: [
                {
                    data: [],
                    dataGrouping: {
                        approximation: "sum",
                        units: [
                            [
                                "hour",
                                [1]
                            ],
                            [
                                "day",
                                [1]
                            ], [
                                "month",
                                [1]
                            ]
                        ]
                    },
                    color: "#00C6DD",
                    lineWidth: 3,
                    marker: {
                        enabled: false,
                        radius: 5,
                        lineWidth: 2,
                        lineColor: "#FFFFFF"
                    },
                    name: "Distancia",
                    pointPadding: 0,
                    groupPadding: 0
                }
            ]
        });

        $(this).highcharts().showLoading();
    });
});