;/// <reference path="../Shared/_shared.js" />

function doKMS_populateGraph() {
    $(function () {
        // > Descargar información de Gráficas de AJAX de Overview
        $.getJSON(
            getKMS_ajaxUri("Overview.json")
        ).fail(function () {
            console.log("[!] - Error de descarga de datos");
        }).done(function (data) {
            // --- Grafica del día de hoy ---
            // Inicializar variables
            var graphLabels = [], graphDataDistance = [], graphDataSteps = [];
            
            // Obtener datos
            $.each(data.daily, function (i, item) {
                graphLabels.push(item.hour);
                graphDataDistance.push(item.distance);
                graphDataSteps.push(item.steps);
            });

            // Generar gráfica
            new Chart(
                document
                    .getElementById("chartDiario")
                    .getContext("2d")
            ).Line({
                labels: graphLabels,
                datasets: [
                    {
                        fillColor: "rgba(220,220,220,0)",
                        strokeColor: "rgba(0, 197, 219, 1)",
                        pointColor: "rgba(0, 197, 219, 1)",
                        pointStrokeColor: "#fff",
                        data: graphDataDistance
                    }
                ]
            }, {
                scaleLabel: "<%=value%> " + $('body').data('distance-unit')
            });

            // --- Gráfica por mes ---
            // Inicializar variables
            graphLabels = [];
            graphDataDistance = [];
            graphDataSteps = [];

            // Obtener datos
            $.each(data.monthly, function (i, item) {
                graphLabels.push(item.month);
                graphDataDistance.push(item.totalDistance);
                graphDataSteps.push(item.totalSteps);
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

            // --- Distribución de actividad --
            var activityTotal = 3; // inicialización en 3 para evitar error de división por cero
            var activityColors = {
                "running": $('#datos .running').css('border-bottom-color'),
                "walking": $('#datos .walking').css('border-bottom-color'),
                "sleep": $('#datos .sleep').css('border-bottom-color')
            };
            var activityData = {
                "running": 0,
                "walking": 0,
                "sleep": 0
            };
            var activityDataRaw    = [];
            
            $.each(data.activity, function (i, item) {
                if ( item.activity != "sleep" )
                    activityTotal += item.totalDistance;
            });

            $.each(data.activity, function (i, item) {  
                activityData[item.activity] = ((item.totalDistance + 1) / activityTotal) * 100; 
            });

            $.each(activityData, function (i, item) {
                activityDataRaw.push({
                    value: item,
                    color: activityColors[i]
                });

                $('#datos .' + i).text(item.toFixed() + '%');
            });

            new Chart(
                document.getElementById("chartActividad").getContext("2d")
            ).Doughnut(activityDataRaw);
        });
    });
}
