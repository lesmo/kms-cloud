$(function() {
    // > Configurar algunas cosas globales de las gráficas (idiomas principalmente)
    var lang = $("html").attr("lang");
    var i18n = $.datepicker.regional[""];

    if (Object.has($.datepicker.regional, lang))
        i18n = $.datepicker.regional[lang];

    Date.setLocale(lang);

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
        dateFormat: "MM d, yy"
    });

    // > Crear las tabs
    $('#graficaPrincipalTabs').tabs({
        heightStyle: "auto",
        activate: function(e, ui) {
            ui.newPanel.children(".graph").highcharts().reflow();
        }
    });

    // > Crear datepickers
    var dayCurrentDate = null, dayCurrentReq  = null;
    $("#graficaPorHora .graph-button input").datepicker({
        minDate: new Date(parseInt($("body").data("user-signup"))),
        maxDate: new Date(),
        onSelect: function() {
            if (dayCurrentDate != null && dayCurrentDate == $(this).datepicker("getDate"))
                return;

            dayCurrentDate = $(this).datepicker("getDate");

            if (dayCurrentReq != null)
                dayCurrentReq.abort();

            $("#graficaPorHora .graph-button button").button("option", "label", $(this).val());
            $("#graficaPorHora .graph").highcharts().showLoading();

            dayCurrentReq = $.getJSON(
                getKMS_ajaxUri("OverviewDailyData.json"),
                {
                    date: $(this).datepicker("getDate").getTime(),
                    _: $("body").data("ajax-cache")
                }
            ).fail(function () {
                $("#graficaPorHora .graph").highcharts().showLoading("Ocurrió un problema durante la descarga, intenta de nuevo");
            }).done(function (data) {
                $("#graficaPorHora .graph").highcharts().series[0].setData(data.allData);
                $("#graficaPorHora .graph").highcharts().hideLoading();
            });
        }
    }).datepicker("setDate", new Date());
    $("#graficaPorHora .graph-button button").button({
        icons: {
            primary: "ui-icon-calendar",
            secondary: "ui-icon-triangle-1-s"
        },
        label: $("#graficaPorHora .graph-button input").val()
    }).click(function() {
        $("#graficaPorHora .graph-button input").datepicker("show");
    });

    var selectedYear = null, monthCurrentDate = null, monthCurrentReq = null;
    $("#graficaPorDia .graph-button input").monthpicker({
        selectedYear: new Date().getFullYear(),
        pattern: "mmm yyyy",
        startYear: new Date(parseInt($("body").data("user-signup"))).getFullYear(),
        finalYear: new Date().getFullYear(),
        monthNames: i18n.monthNames
    }).bind("monthpicker-change-year", function(e, year) {
        var disabledMonths = [];
        selectedYear = year;

        if (year === new Date().getFullYear().toString()) {
            for (var i = new Date().getMonth() + 2; i < 12; i++)
                disabledMonths.push(i);
            if (new Date().getMonth() + 1 < 12)
                disabledMonths.push(12);
        } else if (parseInt(year) > new Date().getFullYear()) {
            for (var i = 1; i <= 12; i++)
                disabledMonths.push(i);
        }

        if (year === new Date(parseInt($("body").data("user-signup"))).getFullYear().toString()) {
            for (var i = 1; i < new Date(parseInt($("body").data("user-signup"))).getMonth(); i++)
                disabledMonths.push(i);
        }

        $(this).monthpicker("disableMonths", disabledMonths);
    }).bind("monthpicker-click-month", function() {
        if (monthCurrentDate != null && monthCurrentDate == $(this).monthpicker("getDate"))
            return;

        monthCurrentDate = $(this).monthpicker("getDate");
        console.log(monthCurrentDate);

        if (monthCurrentReq != null)
            monthCurrentReq.abort();

        $("#graficaPorDia .graph-button button").button("option", "label", $(this).val());
        $("#graficaPorDia .graph").highcharts().showLoading();

        monthCurrentReq = $.getJSON(
            getKMS_ajaxUri("OverviewMonthlyData.json"),
            {
                date: monthCurrentDate.getTime(),
                _: $("body").data("ajax-cache")
            }
        ).fail(function() {
            $("#graficaPorDia .graph").highcharts().showLoading("Ocurrió un problema durante la descarga, intenta de nuevo");
        }).done(function(data) {
            $("#graficaPorDia .graph").highcharts().series[0].setData(data.allData);
            $("#graficaPorDia .graph").highcharts().hideLoading();
        });
    }).val(
        Date.create().format("{Month} {yyyy}")
    ).trigger("monthpicker-change-year", Date.create().format("{yyyy}"));
    $("#graficaPorDia .graph-button button").button({
        icons: {
            primary: "ui-icon-calendar",
            secondary: "ui-icon-triangle-1-s"
        },
        label: $("#graficaPorDia .graph-button input").val()
    }).click(function() {
        $("#graficaPorDia .graph-button input").monthpicker("show");
    });

    // > Crear las gráficas
    $("#graficaPorHora .graph, #graficaPorDia .graph").each(function () {
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
            rangeSelector: {
                enabled: false
            },
            tooltip: {
                valueDecimals: 2,
                valueSuffix: " " + $("body").data("distance-unit")
            },
            xAxis: {
                minRange: 3 * 3600 * 1000
            },
            yAxis: {
                aligh: "left",
                labels: {
                    format: "{value} " + $("body").data("distance-unit")
                },
                min: 0
            },
            series: [
                {
                    data: [],
                    dataGrouping: {
                        groupPixelWidth: 7,
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

    $("#graficaPorHora .graph").highcharts().xAxis[0].update({
        labels: {
            formatter: function () {
                return this.isLast ? "24:00" : Date.create(this.value).format("{HH}:{mm}");
            }
        }
    }, true);
});