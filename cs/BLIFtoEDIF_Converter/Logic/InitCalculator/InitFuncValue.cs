using System;
using System.Linq;
using System.Text;

namespace BLIFtoEDIF_Converter.Logic.InitCalculator
{
	public class InitFuncValue
	{
		public string Value { get; }

		public InitFuncValue(int[][] truthMatrix)
		{
			byte[] results = truthMatrix.Select(r => (byte)r.Last()).Reverse().ToArray();
			string strBinary = string.Join(String.Empty, results);
			Value = BinaryStringToHexString(strBinary);
		}

		public override string ToString()
		{
			return Value;
		}

		public static string BinaryStringToHexString(string binary)
		{
			StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

			// TODO: check all 1's or 0's... Will throw otherwise

			int mod4Len = binary.Length % 8;
			if (mod4Len != 0)
			{
				// pad to length multiple of 8
				binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
			}

			for (int i = 0; i < binary.Length; i += 8)
			{
				string eightBits = binary.Substring(i, 8);
				result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
			}

			var fullHex = result.ToString();
			fullHex = fullHex.TrimStart('0');
			if (string.Empty.Equals(fullHex))
				fullHex = "0";

			return fullHex;
		}
	}
}
