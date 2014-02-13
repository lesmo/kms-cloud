using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static class Base36Encoder {
        private const string Clist = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly char[] Clistarr = Clist.ToCharArray();

        public static long Decode(string inputString) {
            long result = 0;
            var pow = 0;
            for ( var i = inputString.Length - 1; i >= 0; i-- ) {
                var c = inputString[i];
                var pos = Clist.IndexOf(c);
                if ( pos > -1 )
                    result += pos * (long)Math.Pow(Clist.Length, pow);
                else
                    return -1;
                pow++;
            }
            return result;
        }

        public static string Encode(long inputNumber) {
            var sb = new StringBuilder();
            inputNumber = Math.Abs(inputNumber);
            do {
                sb.Append(Clistarr[inputNumber % (long)Clist.Length]);
                inputNumber /= (long)Clist.Length;
            } while ( inputNumber != 0 );
            return Reverse(sb.ToString());
        }

        private static string Reverse(string s) {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

    }
}
