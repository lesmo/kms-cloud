using System.ComponentModel;
using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    public partial class AjaxController {

        private struct TotalDataPoint {
            public String activity;
            public Int64 distance;
            public Int64 steps;
        }

        private class MonthlyDataPoint {
            public Int32 year;
            public String month;
            public Int32 monthNumeric;
            public Double distance;
            public Int64 steps;
        }

        // GET: /DynamicResources/Ajax/OverviewHourlyData.json
        public JsonResult OverviewHourlyData() {
            // > Obtener lecturas
            //   [MUST REVIEW] Las lecturas deberían distinguir entre la actividad {Correr},
            //                 {Caminar} y {Sueño}. Actualmente se "agregan" todas las actividades
            //                 que no correspondan a {Sueño}, puesto que la vista NO dispone de
            //                 ésta diferenciación en la Gráfica del día de Hoy.
            var lastYearStart = DateTime.UtcNow.AddYears(-1);
            var lastData =
                CurrentUser.UserDataHourlyDistance.OrderByDescending(b => b.Timestamp).FirstOrDefault()
                ?? new UserDataHourlyDistance {
                    Activity = DataActivity.Walking,
                    Steps = 0,
                    Timestamp = DateTime.UtcNow.AddMinutes(-2),
                    User = CurrentUser
                };

            var data = (
                from d in CurrentUser.UserDataHourlyDistance
                where
                    d.Activity != DataActivity.Sleep
                    && d.Timestamp > lastYearStart
                group d by new {
                    year = d.Timestamp.Year,
                    month = d.Timestamp.Month,
                    day = d.Timestamp.Day,
                    hour = d.Timestamp.Hour,
                    minute = d.Timestamp.Minute
                } into g
                orderby g.Key.year, g.Key.month, g.Key.day, g.Key.hour, g.Key.minute
                select new object[] {
                    new DateTime(g.Key.year, g.Key.month, g.Key.day, g.Key.hour, g.Key.minute, 0).ToJavascriptEpoch(),
                    RegionInfo.CurrentRegion.IsMetric
                        ? g.Sum(s => s.Distance).CentimetersToKilometers()
                        : g.Sum(s => s.Distance).CentimetersToMiles()
                }
            );

            var paddingData = new List<object[]>();
            for ( var i = lastData.Timestamp.AddMinutes(2); i < DateTime.UtcNow; i = i.AddMinutes(2) )
                paddingData.Add(new object[] {
                    i.ToJavascriptEpoch(),
                    null
                });

            return Json(
                new {
                    allData = paddingData.Concat(data).OrderBy(b => b[0])
                },
                JsonRequestBehavior.AllowGet
            );
        }

        public JsonResult OverviewMonthlyData() {
            // Obtener sumatoria por mes de los últimos 6 meses
            var lastMonthsStart = DateTime.UtcNow.AddMonths(-6);
            var monthlyActivityFallback = new List<MonthlyDataPoint>();

            for ( var i = lastMonthsStart; i <= DateTime.UtcNow; i = i.AddMonths(1) )
                monthlyActivityFallback.Add(new MonthlyDataPoint {
                    year = i.Year,
                    month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i.Month)
                        + (i.Year == DateTime.UtcNow.Year ? "" : " " + i.Year.ToString(CultureInfo.InvariantCulture))
                    ),
                    monthNumeric = i.Month,
                    distance = 0,
                    steps = 0
                });

            var monthlyActivity = (
                from d in CurrentUser.UserDataHourlyDistance
                where d.Timestamp > lastMonthsStart
                group d by new {
                    year  = d.Timestamp.Year,
                    month = d.Timestamp.Month
                } into g
                orderby g.Key.year, g.Key.month
                select new MonthlyDataPoint {
                    year = g.Key.year,
                    month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.month)
                        + (g.Key.year == DateTime.UtcNow.Year ? "" : " " + g.Key.year.ToString(CultureInfo.InvariantCulture))
                    ),
                    monthNumeric =  g.Key.month,
                    distance = RegionInfo.CurrentRegion.IsMetric
                        ? g.Sum(s => s.Distance).CentimetersToKilometers()
                        : g.Sum(s => s.Distance).CentimetersToMiles(),
                    steps = g.Sum(s => s.Steps)
                }
            );

            var monthlyActivityFinal = monthlyActivity.Concat(
                monthlyActivityFallback.Where(
                    w => ! monthlyActivity.Any(a => a.month == w.month && a.year == w.year)
                )
            ).OrderBy(b => b.year).ThenBy(b => b.monthNumeric);

            return Json(
                new {
                    allData = monthlyActivityFinal
                },
                JsonRequestBehavior.AllowGet
            );
        }

        // GET: /DynamicResources/Ajax/Overview.json
        public JsonResult OverviewYearlyData() {
            // > Obtener sumatoria total por actividad
            var lastYearStart = DateTime.UtcNow.AddYears(-1);
            var yearActivity = (
                from d in CurrentUser.UserDataTotalDistance
                where d.Timestamp >= lastYearStart && d.Activity != 0
                group d by new {
                    activity = d.Activity
                } into g
                select new TotalDataPoint {
                    activity = g.Key.activity.ToString().ToLower(),
                    distance = g.Sum(s => s.TotalDistance),
                    steps    = g.Sum(s => s.TotalSteps)
                }
            );

            // > Preparar respuesta en JSON
            return Json(
                new {
                    allData = yearActivity
                },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}