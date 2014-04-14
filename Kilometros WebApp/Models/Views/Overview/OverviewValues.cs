using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views {
    public class OverviewValues {
        public OverviewValues() {
            this.TodaySleepTime
                = new TimeSpan(0, 0, 0);
        }

        /// <summary>
        /// Información sobre la Región del Usuario
        /// </summary>
        public RegionInfo RegionInfo {
            get;
            set;
        }
        /// <summary>
        /// Información sobre la Cultura del Usuario
        /// </summary>
        public CultureInfo CultureInfo {
            get;
            set;
        }

        /// <summary>
        ///     Horas de Sueño del las últimas 24hrs.
        ///     [MUST REVIEW]
        /// </summary>
        public TimeSpan TodaySleepTime {
            get;
            set;
        }

        /// <summary>
        ///     Distancia Recorrida en las últimas 24hrs.
        /// </summary>
        public double TodayDistanceCentimeters {
            get;
            set;
        }

        /// <summary>
        /// Distancia Recorrida por el Usuario en las unidades de la región del Usuario
        /// </summary>
        public double TodayDistance {
            get {
                if ( this.RegionInfo.IsMetric )
                    return this.TodayDistanceCentimeters / 1000;
                else
                    return this.TodayDistanceCentimeters / 160934.4;
            }
        }

        /// <summary>
        /// Distancia Recorrida por el Usuario en las unidades de la región del Usuario, en el formato de la Cultura del Usuario
        /// </summary>
        public string TodayDistanceString {
            get {
                return this.TodayDistance.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }

        /// <summary>
        /// Equiavalencia de CO2 ahorrada por el Usuario en las últimas 24hrs.
        /// </summary>
        public int EquivalentCo2Grams {
            get;
            set;
        }

        /// <summary>
        ///  Equiavalencia de CO2 ahorrada por el Usuario, en las unidades de la región del Usuario.
        /// </summary>
        public int EquivalentCo2 {
            get {
                if ( this.RegionInfo.IsMetric )
                    return this.EquivalentCo2Grams / 1000;
                else
                    return (int)(this.EquivalentCo2Grams / 453.59237);
            }
        }

        /// <summary>
        /// Equivalencia de CO2 ahorrada por el Usuario, 
        /// </summary>
        public string EquivalentCo2String {
            get {
                return this.EquivalentCo2.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }

        /// <summary>
        /// Equiavalencia de CO2 ahorrada por el Usuario en las últimas 24hrs.
        /// </summary>
        public int EquivalentKcal {
            get;
            set;
        }

        /// <summary>
        /// Equivalencia de CO2 ahorrada por el Usuario.
        /// </summary>
        public string EquivalentKcalString {
            get {
                return this.EquivalentKcal.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }

        /// <summary>
        /// Equivalencia de Dinero ahorrado por el Usuario.
        /// </summary>
        public int EquivalentCash {
            get;
            set;
        }

        /// <summary>
        /// Equivalencia de Dinero ahorrado por el Usuario, en el formato cultural del Usuario y
        /// con el símbolo monetario del País.
        /// </summary>
        public string EquivalentCashString {
            get {
                return this.RegionInfo.CurrencySymbol + this.EquivalentCash.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }
    }
}