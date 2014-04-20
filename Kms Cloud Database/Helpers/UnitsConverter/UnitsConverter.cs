using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public static partial class UnitsConverter {
        internal static Double ToDouble(this IConvertible @this) {
            return @this.ToDouble(System.Globalization.NumberFormatInfo.InvariantInfo);
        }
    }
}
