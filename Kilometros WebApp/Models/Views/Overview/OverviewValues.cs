﻿using KilometrosDatabase.Helpers;
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
        public string TodayDistance {
            get {
                double distance
                    = RegionInfo.CurrentRegion.IsMetric
                    ? this.TodayDistanceCentimeters.CentimetersToKilometers()
                    : this.TodayDistanceCentimeters.CentimetersToMiles();

                return distance.ToLocalizedString();
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
        public string EquivalentCo2 {
            get {
                double co2
                    = RegionInfo.CurrentRegion.IsMetric
                    ? this.EquivalentCo2Grams.GramsToKilograms()
                    : this.EquivalentCo2Grams.GramsToPounds();
                
                return co2.ToLocalizedString();
            }
        }

        /// <summary>
        /// Equiavalencia de CO2 ahorrada por el Usuario en las últimas 24hrs.
        /// </summary>
        public int EquivalentKcalRaw {
            get;
            set;
        }

        /// <summary>
        /// Equivalencia de CO2 ahorrada por el Usuario.
        /// </summary>
        public string EquivalentKcal {
            get {
                return this.EquivalentKcalRaw.ToLocalizedString();
            }
        }

        /// <summary>
        /// Equivalencia de Dinero ahorrado por el Usuario.
        /// </summary>
        public int EquivalentCashRaw {
            get;
            set;
        }

        /// <summary>
        /// Equivalencia de Dinero ahorrado por el Usuario, en el formato cultural del Usuario y
        /// con el símbolo monetario del País.
        /// </summary>
        public string EquivalentCash {
            get {
                return this.EquivalentCashRaw.ToCurrencyString();
            }
        }

        public TipModel TipOfTheDay {
            get;
            set;
        }
    }
}