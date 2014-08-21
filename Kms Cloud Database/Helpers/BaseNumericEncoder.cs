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
            var inputArray = inputString.ToCharArray().Reverse();
            var pos = -1;

            return inputArray.Aggregate(
                0,
                (current, c) =>
                    (int)(current + mCharMap.IndexOf(c) * (long)Math.Pow(inputString.Length, ++pos))
            );
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
