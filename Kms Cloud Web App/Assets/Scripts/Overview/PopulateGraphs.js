/// <reference path="../Shared/_shared.js" />

function doKMS_populateGraph() {
    $(function () {
        // > Descargar información de Gráfica Principal
        $.getJSON(
            getKMS_ajaxUri("OverviewHourlyData.json")
        ).fail(function() {
            console.log("[!] - Error de descarga de datos (OverviewHourlyData.json)");
        }).done(function(data) {
            $('#chartDiario').highcharts('StockChart', {
                chart: {
                    type: 'spline',
                    zoomType: 'xy'
                },
                credits: {
                    enabled: false
                },
                navigator: {
                    handles: {
                        backgroundColor: '#FFFFFF',
                        borderColor: '#00C6DD'
                    },
                    maskFill: 'rgba(178, 237, 243, 0.3)',
                    series: {
                        type: 'spline',
                        lineColor: '#00C6DD'
                    }
                },
                rangeSelector: {
                    buttons: [
                        {
                            type: 'day',
                            count: 1,
                            text: '1d'
                        }, {
                            type: 'week',
                            count: 1,
                            text: '7d'
                        }, {
                            type: 'month',
                            count: 1,
                            text: '1m'
                        }, {
                            type: 'month',
                            count: 3,
                            text: '3m'
                        }, {
                            type: 'month',
                            count: 6,
                            text: '6m'
                        }
                    ],
                    buttonTheme: { // styles for the buttons
                        fill: 'none',
                        stroke: 'none',
                        'stroke-width': 0,
                        r: 8,
                        style: {
                            color: '#00C6DD',
                            fontWeight: 'bold'
                        },
                        states: {
                            hover: {
                            },
                            select: {
                                fill: '#00C6DD',
                                style: {
                                    color: '#FFFFFF'
                                }
                            }
                        }
                    },
                    selected: 0
                },
                tooltip: {
                    valueDecimals: 2,
                    valueSuffix: ' ' + $('body').data('distance-unit')
                },
                xAxis: {
                    minRange: 3600000 * 24
                },
                yAxis: {
                    labels: {
                        format: '{value} ' + $('body').data('distance-unit')
                    },
                    min: 0
                },

                series: [{
                    data: data.allData,
                    dataGrouping: {
                        approximation: 'sum',
                        groupPixelWidth: 10
                    },
                    color: '#00C6DD',
                    lineWidth: 3,
                    marker: {
                        radius: 5,
                        lineWidth: 2,
                        lineColor: '#FFFFFF'
                    },
                    name: 'Distancia recorrida'
                }]
            });

            if (data.allData.count() < 3)
                $('#chartDiario').highcharts().showLoading("¡No hay datos!");
        });

        // > Descargar información de Gráfica Mensual
        $.getJSON(
            getKMS_ajaxUri("OverviewMonthlyData.json")
        ).fail(function() {
            console.log("[!] - Error de descarga de datos (OverviewMonthlyData.json)");
        }).done(function (data) {
            // Inicializar variables
            var graphLabels = [];
            var graphDataDistance = [];

            // Obtener datos
            $.each(data.allData, function (i, item) {
                graphLabels.push(item.month);
                graphDataDistance.push(Math.floor(item.distance));
            });

            // Generar gráfica
            new Chart(
                document.getElementById("chartMensual").getContext("2d")
            ).Bar({
                labels: graphLabels,
                datasets: [
                    {
                        fillColor: "rgba(0,197,219,1)",
                        strokeColor: "rgba(220,220,220,1)",
                        data: graphDataDistance
                    }
                ]
            }, {
                scaleLabel: "<%=value%> " + $('body').data('distance-unit')
            });
        });

        // > Descargar información de Gráfica de Distribución de Actividades
        $.getJSON(
            getKMS_ajaxUri("OverviewYearlyData.json")
        ).fail(function () {
            console.log("[!] - Error de descarga de datos (OverviewYearlyData.json)");
        }).done(function (data) {
            var activityTotal  = 0;
            var activityColors = {
                "running": $('#datos .running').css('border-bottom-color'),
                "walking": $('#datos .walking').css('border-bottom-color'),
                "sleep"  : $('#datos .sleep').css('border-bottom-color')
            };
            var activityData = {
                "running": 0,
                "walking": 0,
                "sleep"  : 0
            };
            var activityDataRaw = [];
            
            $.each(data.allData, function (i, item) {
                activityTotal += item.steps;
            });

            if (activityTotal < 1) {
                $('#datos .running').text('0%');
                $('#datos .walking').text('0%');
                $('#datos .sleep').text('100%');

                return new Chart(
                    document.getElementById("chartActividad").getContext("2d")
                ).Doughnut([{
                    value: 100,
                    color: activityColors["sleep"]
                }]);
            }

            var percentTotal = 0;
            $.each(data.allData, function (i, item) {
                activityData[item.activity] = Math.floor((item.steps / activityTotal) * 100);
                percentTotal += activityData[item.activity];
            });

            if (percentTotal < 100)
                activityData["sleep"] += 100 - percentTotal;

            $.each(activityData, function (i, item) {
                activityDataRaw.push({
                    value: item,
                    color: activityColors[i]
                });

                $('#datos .' + i).text(item.toFixed() + '%');
            });

            console.log(data);
            console.log(activityData);

            return new Chart(
                document.getElementById("chartActividad").getContext("2d")
            ).Doughnut(activityDataRaw);
        });
    });
}