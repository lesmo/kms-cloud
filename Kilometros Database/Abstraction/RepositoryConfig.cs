using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Abstraction {
    internal static class RepositoryConfig {
        public static class DateTimeEntityPropertiesConfig {
            public static string[] AutosetOnInsert = {
                "CreationDate",
                "LastUseDate",
                "LastEditDate"
            };

            public static string[] AutosetOnUpdate = {
                "LastUseDate",
                "LastEditDate"
            };
        }

        public static class GuidPropertiesConfig {
            public static string[] AutosetOnInsert = {
                "Guid"
            };
        }
    }
}
