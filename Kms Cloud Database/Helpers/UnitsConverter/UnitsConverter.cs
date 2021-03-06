﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public static partial class UnitsConverter {
        

        internal static Double ToDouble(this IConvertible @this) {
            return @this.ToDouble(System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        private const Int64 LinuxEpochTicks = 621355968000000000;

        public static Int64 ToJavascriptEpoch(this DateTime @this) {
            return @this.Ticks > LinuxEpochTicks
                ? @this.AddTicks(-LinuxEpochTicks).Ticks / 10000
                : 0;
        }

        public static DateTime DateFromJavascriptEpoch(this Int64 @this) {
            return new DateTime(LinuxEpochTicks + @this * 10000, DateTimeKind.Utc);
        }
    }
}
