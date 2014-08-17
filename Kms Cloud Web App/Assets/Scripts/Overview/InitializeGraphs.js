/// <reference path="../Shared/_shared.js" />
/// <reference path="HighchartsSetup.js"/>

$(function () {
	// > Descargar información de Gráficas Principales
	// Gráfica diaria
	$.getJSON(
		getKMS_ajaxUri("OverviewDailyData.json"),
		{ c: $("body").data("ajax-cache") }
	).fail(function() {
		console.log("[!] - Error de descarga de datos (OverviewHourlyData.json)");
	}).done(function (data) {
		$("#graficaPorHora .graph").highcharts().series[0].setData(data.allData);
		$("#graficaPorHora .graph").highcharts().hideLoading();

		doKMS_redimSidebar();
	});

	// Gráfica mensual
	$.getJSON(
		getKMS_ajaxUri("OverviewMonthlyData.json"),
		{ c: $("body").data("ajax-cache") }
	).fail(function () {
		console.log("[!] - Error de descarga de datos (OverviewHourlyData.json)");
	}).done(function (data) {
	    $("#graficaPorDia .graph").highcharts().series[0].setData(data.allData);
	    $("#graficaPorDia .graph").highcharts().hideLoading();

		doKMS_redimSidebar();
	});

	// > Descargar información de Comparativa de Gráfica Mensual
	$.getJSON(
		getKMS_ajaxUri("OverviewMonthlyComparisonData.json"),
		{ c: $("body").data("ajax-cache") }
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
			scaleLabel: "<%=value%> " + $("body").data("distance-unit")
		});

		doKMS_redimSidebar();
	});

	// > Descargar información de Gráfica de Distribución de Actividades
	$.getJSON(
		getKMS_ajaxUri("OverviewActivityComparisonData.json"),
		{ c: $("body").data("ajax-cache") }
	).fail(function () {
		console.log("[!] - Error de descarga de datos (OverviewYearlyData.json)");
	}).done(function (data) {
		var activityTotal  = 0;
		var activityColors = {
			"running": $("#datos .running").css("border-bottom-color"),
			"walking": $("#datos .walking").css("border-bottom-color"),
			"sleep"  : $("#datos .sleep").css("border-bottom-color")
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
			$("#datos .running").text("0%");
			$("#datos .walking").text("0%");
			$("#datos .sleep").text("100%");

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

			$("#datos ." + i).text(item.toFixed() + "%");
		});

		new Chart(
			document.getElementById("chartActividad").getContext("2d")
		).Doughnut(activityDataRaw);

		doKMS_redimSidebar();
	});
});