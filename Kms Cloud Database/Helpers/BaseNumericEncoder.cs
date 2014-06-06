using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public class Base36Encoder : BaseNumericEncoder {
        private const string Base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public Base36Encoder() : base(Base36) {
        }
    }

    public class BaseNumericEncoder {
        private readonly char[] mCharMapArray;// = Clist.ToCharArray();
        private readonly string mCharMap;

        public BaseNumericEncoder(String charMap) {
            mCharMap      = charMap;
            mCharMapArray = charMap.ToCharArray();
        }

        public long Decode(string inputString) {
            long result = 0;
            var pow = 0;
            for ( var i = inputString.Length - 1; i >= 0; i-- ) {
                var c = inputString[i];
                var pos = mCharMap.IndexOf(c);
                if ( pos > -1 )
                    result += pos * (long)Math.Pow(mCharMapArray.Length, pow);
                else
                    return -1;
                pow++;
            }
            return result;
        }

        public string Encode(long inputNumber) {
            var sb = new StringBuilder();

            inputNumber = Math.Abs(inputNumber);
            do {
                sb.Append(mCharMapArray[inputNumber % (long)mCharMapArray.Length]);
                inputNumber /= (long)mCharMapArray.Length;
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
