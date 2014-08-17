using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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
        
        private class MonthlyDataPoint {
            public Int32 year;
            public String month;
            public Int32 monthNumeric;
            public Double distance;
            public Int64 steps;
        }

        // GET: /DynamicResources/Ajax/OverviewHourlyData.json
        public JsonResult OverviewDailyData(Int64? date) {
            // > Determinar límite inferior de fecha, de acuerdo a Zona horaria del Cliente
            var lowerBound = date.HasValue
                ? date.Value.DateFromJavascriptEpoch().Add(+ClientUtcOffset) // ajustar a zona horaria de cliente
                : DateTime.UtcNow.Add(+ClientUtcOffset); // ajustar a zona horaria de cliente
            
            lowerBound = new DateTime(
                lowerBound.Year,
                lowerBound.Month,
                lowerBound.Day,
                0,
                0, 
                0, 
                DateTimeKind.Utc
            ).Add(-ClientUtcOffset); // ajustar a zona horaria UTC

            var higherRound = lowerBound.AddDays(1);

            // > Obtener lecturas
            var data = (
                from d in Database.DataStore.GetQueryable(s => s.User.Guid == CurrentUser.Guid)
                where
                    d.Activity != DataActivity.Sleep
                    && d.Timestamp >= lowerBound
                    && d.Timestamp <= higherRound
                group d by new {
                    year  = d.Timestamp.Year,
                    month = d.Timestamp.Month,
                    day   = d.Timestamp.Day,
                    hour  = d.Timestamp.Hour,
                    part  = (int)Math.Floor(d.Timestamp.Hour / 5d)
                } into g
                orderby g.Key.year, g.Key.month, g.Key.day, g.Key.hour, g.Key.part
                select new {
                    year     = g.Key.year,
                    month    = g.Key.month,
                    day      = g.Key.day,
                    hour     = g.Key.hour,
                    minute   = g.Key.part * 5,
                    distance = g.Sum(s => s.Steps * s.StrideLength)
                }
            ).ToList().Select(s => new object[] {
                new DateTime(s.year, s.month, s.day, s.hour, s.minute, 0, DateTimeKind.Utc)
                    .ToJavascriptEpoch(),
                RegionInfo.CurrentRegion.IsMetric
                    ? s.distance.CentimetersToKilometers()
                    : s.distance.CentimetersToMiles()
            });

            var dataFallback = new List<object[]>();
            for ( var i = lowerBound; i <= higherRound; i = i.AddMinutes(5) ) {
                dataFallback.Add(new object[] {
                    i.ToJavascriptEpoch(),
                    0d
                });
            }

            var dataFinal = data.Concat(
                dataFallback.Where(w => ! data.Any(a => a[0] == w[0]))
            ).OrderBy(b => b[0]);

            return Json(
                new {
                    allData = dataFinal
                },
                JsonRequestBehavior.AllowGet
            );
        }

        public JsonResult OverviewMonthlyData(Int64? date) {
            // > Determinar límite inferior de fecha, de acuerdo a Zona horaria del Cliente
            var lowerBound = date.HasValue
                ? date.Value.DateFromJavascriptEpoch().Add(+ClientUtcOffset) // ajustar a zona horaria de cliente
                : DateTime.UtcNow.Add(+ClientUtcOffset); // ajustar a zona horaria de cliente

            lowerBound = new DateTime(
                lowerBound.Year,
                lowerBound.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc
            ).Add(-ClientUtcOffset); // ajustar a zona horaria UTC

            var higherRound = lowerBound.AddMonths(1);

            // > Obtener lecturas
            var data = (
                from d in Database.DataStore.GetQueryable(f => f.User.Guid == CurrentUser.Guid)
                where
                    d.Activity != DataActivity.Sleep
                    && d.Timestamp >= lowerBound
                    && d.Timestamp <= higherRound
                group d by new {
                    // ReSharper disable PossibleInvalidOperationException (nunca ocurre porque en BD la columna nunca es NULL)
                    year  = DbFunctions.AddMinutes(d.Timestamp, (Int32)ClientUtcOffset.TotalMinutes).Value.Year,
                    month = DbFunctions.AddMinutes(d.Timestamp, (Int32)ClientUtcOffset.TotalMinutes).Value.Month,
                    day   = DbFunctions.AddMinutes(d.Timestamp, (Int32)ClientUtcOffset.TotalMinutes).Value.Day
                    // ReSharper restore PossibleInvalidOperationException
                } into g
                orderby g.Key.year, g.Key.month, g.Key.day
                select new {
                    year     = g.Key.year,
                    month    = g.Key.month,
                    day      = g.Key.day,
                    distance = g.Sum(s => s.Steps * s.StrideLength)
                }
            ).ToList().Select(s => new object[] {
                new DateTime(s.year, s.month, s.day, 0, 0, 0, DateTimeKind.Utc)
                    .Add(-ClientUtcOffset) // Se debe ajustar a UTC debido a que el agrupado arriba
                    .ToJavascriptEpoch(), 
                RegionInfo.CurrentRegion.IsMetric
                    ? s.distance.CentimetersToKilometers()
                    : s.distance.CentimetersToMiles()
            });

            var dataFallback = new List<object[]>();
            for ( var i = lowerBound; i <= higherRound; i = i.AddDays(1) ) {
                dataFallback.Add(new object[] {
                    i.ToJavascriptEpoch(),
                    0d
                });
            }

            var dataFinal = data.Concat(
                dataFallback.Where(w => ! data.Any(a => a[0] == w[0]))
            ).OrderBy(b => b[0]);

            return Json(
                new {
                    allData = dataFinal
                },
                JsonRequestBehavior.AllowGet
            );
        }

        public JsonResult OverviewMonthlyComparisonData() {
            // Obtener sumatoria por mes de los últimos 6 meses
            var lastMonthsStart = DateTime.UtcNow.AddMonths(-5).Add(+ClientUtcOffset);
            var monthlyActivityFallback = new List<MonthlyDataPoint>();
            
            for ( var i = lastMonthsStart; i <= DateTime.UtcNow; i = i.AddMonths(1) )
                monthlyActivityFallback.Add(new MonthlyDataPoint {
                    year = i.Year,
                    month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i.Month)
                        + (i.Year == DateTime.UtcNow.Year ? "" : " " + i.Year.ToString(CultureInfo.InvariantCulture))
                    ),
                    monthNumeric = i.Month,
                    distance     = 0,
                    steps        = 0
                });
            
            var monthlyActivity = (
                from d in Database.UserDataHourlyDistance.GetQueryable(f => f.User.Guid == CurrentUser.Guid)
                where d.Timestamp > lastMonthsStart
                group d by new {
                    // ReSharper disable PossibleInvalidOperationException (nunca ocurre porque en BD la columna nunca es NULL)
                    year  = DbFunctions.AddMinutes(d.Timestamp, (Int32)ClientUtcOffset.TotalMinutes).Value.Year,
                    month = DbFunctions.AddMinutes(d.Timestamp, (Int32)ClientUtcOffset.TotalMinutes).Value.Month
                    // ReSharper restore PossibleInvalidOperationException
                } into g
                orderby g.Key.year, g.Key.month
                select new {
                    year     = g.Key.year,
                    month    = g.Key.month,
                    distance = g.Sum(s => s.Distance),
                    steps    = g.Sum(s => s.Steps)
                }
            ).ToList().Select(s => new MonthlyDataPoint {
                year         = s.year,
                monthNumeric = s.month,

                month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(s.month)
                    + (s.year == DateTime.UtcNow.Year ? "" : " " + s.year.ToString(CultureInfo.InvariantCulture))
                ),
                distance = RegionInfo.CurrentRegion.IsMetric
                    ? s.distance.CentimetersToKilometers()
                    : s.distance.CentimetersToMiles(),
                steps = s.steps
            });

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
        public JsonResult OverviewActivityComparisonData() {
            // > Obtener sumatoria total por actividad
            var lastYearStart = DateTime.UtcNow.AddYears(-1).Add(+ClientUtcOffset);
            var yearActivity = (
                from d in Database.UserDataTotalDistance.GetQueryable(f => f.User.Guid == CurrentUser.Guid)
                where d.Timestamp >= lastYearStart
                group d by new {
                    activity = d.Activity
                } into g
                select new {
                    activity = g.Key.activity.ToString().ToLower(),
                    distance = g.Sum(s => s.TotalDistance),
                    steps    = g.Sum(s => s.TotalSteps)
                }
            ).ToList();
            
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