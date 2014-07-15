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
        // GET: /DynamicResources/Ajax/Overview.json
        public JsonResult Overview() {
            // > Obtener últimas 24hrs. de lecturas
            //   [MUST REVIEW] Las lecturas deberían distinguir entre la actividad {Correr},
            //                 {Caminar} y {Sueño}. Actualmente se "agregan" todas las actividades
            //                 que no correspondan a {Sueño}, puesto que la vista NO dispone de
            //                 ésta diferenciación en la Gráfica del día de Hoy.
            var lastData = (
                from d in CurrentUser.UserDataHourlyDistance
                where
                    d.Activity != DataActivity.Sleep
                orderby d.Timestamp
                select d
            ).Take(1).FirstOrDefault();

            var yesterdayStart = lastData == null
                ? DateTime.UtcNow.AddHours(-24)
                : lastData.Timestamp.AddHours(-24);

            var lastDayActivity = (
                from d in CurrentUser.UserDataHourlyDistance
                where
                    d.Activity != DataActivity.Sleep
                    && d.Timestamp > yesterdayStart
                group d by new {
                    userGuid = d.User.Guid,
                    year = d.Timestamp.Year,
                    month = d.Timestamp.Month,
                    day = d.Timestamp.Day,
                    hour = d.Timestamp.Hour
                } into g
                select new {
                    hour = new DateTime(new TimeSpan(g.Key.hour, 0, 0).Ticks).ToString("h tt"),
                    distance = RegionInfo.CurrentRegion.IsMetric
                        ? g.Sum(s => s.Distance).CentimetersToKilometers()
                        : g.Sum(s => s.Distance).CentimetersToMiles(),
                    steps = g.Sum(s => s.Steps)
                }
            );

            // Obtener sumatoria por mes de los últimos 12 meses
            var lastMonthStart = lastData.Timestamp.AddMonths(-12);
            var monthlyActivity = (
                from d in CurrentUser.UserDataHourlyDistance
                where d.Timestamp > lastMonthStart
                group d by new {
                    year = d.Timestamp.Year,
                    month = d.Timestamp.Month
                } into g
                orderby g.Key.year, g.Key.month
                select new {
                    year = g.Key.year,
                    month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.month)
                    ),
                    totalDistance = RegionInfo.CurrentRegion.IsMetric
                        ? g.Sum(s => s.Distance).CentimetersToKilometers()
                        : g.Sum(s => s.Distance).CentimetersToMiles(),
                    totalSteps = g.Sum(s => s.Steps)
                }
            );

            object[] monthlyActivityFinal;
            var firstMonthlyActivity = monthlyActivity.FirstOrDefault();

            if ( monthlyActivity.Count(c => c.year == firstMonthlyActivity.year) == monthlyActivity.Count() ) {
                monthlyActivityFinal = monthlyActivity.ToArray();
            } else {
                var lastYear = firstMonthlyActivity.year;
                var monthlyActivityFinalList = new List<object>();
                var month = "";

                foreach ( var activity in monthlyActivity.Reverse() ) {
                    if ( activity.year == firstMonthlyActivity.year )
                        month = activity.month;
                    else
                        month = activity.month + " (" + activity.year.ToString() + ")";

                    monthlyActivityFinalList.Add(new {
                        year = activity.year,
                        month = month,
                        totalDistance = activity.totalDistance,
                        totalSteps = activity.totalSteps
                    });
                }

                monthlyActivityFinalList.Reverse();
                monthlyActivityFinal = monthlyActivityFinalList.ToArray();
            }

            // > Obtener sumatoria total por actividad
            var lastWeekStart = lastData.Timestamp.AddDays(-7);
            var activityDistribution = (
                from d in CurrentUser.UserDataTotalDistance
                where d.Timestamp >= lastWeekStart
                group d by new {
                    activity = d.Activity
                } into g
                select new {
                    activity = g.Key.activity.ToString().ToLower(),
                    totalDistance = g.Sum(s => s.TotalDistance),
                    totalSteps = g.Sum(s => s.TotalSteps)
                }
            );

            // > Preparar respuesta en JSON
            return Json(
                new {
                    daily = lastDayActivity,
                    monthly = monthlyActivityFinal,
                    activity = activityDistribution,
                },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}