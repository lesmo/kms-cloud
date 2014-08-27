using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
	public class Base36Encoder : BaseNumericEncoder {
		private const string Base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public Base36Encoder() : base(Base36) {}
	}

	public class KmsSerialEncoder : BaseNumericEncoder {
		private const string Base27 = "0123456789ACEFHJKLMNPRTVWXZ";

		public KmsSerialEncoder() : base(Base27) {}

		public override Int64 Decode(string inputString) {
			long result = 0;
			var pos = 0;
			var inputArray = inputString.ToUpper().ToCharArray().Reverse().ToArray();

			foreach ( char c in inputArray ) {
				result += mCharMap.IndexOf((char)c) * (long)Math.Pow(inputArray.Length, pos);
				pos++;
			}

			return result;
		}

		public override String Encode(Int64 inputNumber) {
			var sb = new StringBuilder();
			
			do {
				var remainder = inputNumber % mCharMapArray.Length;
				sb.Append(mCharMapArray[remainder]);
				inputNumber = (inputNumber - remainder) / mCharMapArray.Length;
			} while ( inputNumber > 0 );

			return new String(sb.ToString().ToCharArray().Reverse().ToArray());
		}
	}

	public class BaseNumericEncoder {
		protected readonly char[] mCharMapArray;// = Clist.ToCharArray();
		protected readonly string mCharMap;

		public BaseNumericEncoder(String charMap) {
			mCharMap      = charMap;
			mCharMapArray = charMap.ToCharArray();
		}

		public virtual Int64 Decode(string inputString) {
			long result = 0;
			var pow = 0;

			for ( var i = inputString.Length - 1; i >= 0; i-- ) {
				var c   = inputString[i];
				var pos = mCharMap.IndexOf(c);

				if ( pos > -1 )
					result += pos * (long)Math.Pow(mCharMapArray.Length, pow);
				else
					return -1;

				pow++;
			}
			
			return result;
		}

		public virtual String Encode(long inputNumber) {
			var sb = new StringBuilder();
			inputNumber = Math.Abs(inputNumber);

			do {
				sb.Append(mCharMapArray[inputNumber % (long)mCharMapArray.Length]);
				inputNumber /= (long)mCharMapArray.Length;
			} while ( inputNumber != 0 );

			return new String(sb.ToString().ToCharArray().Reverse().ToArray());
		}
	}
}
