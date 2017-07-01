using System;
using System.Linq;

namespace BLIFtoEDIF_Converter.init_calculator
{
	public class InitFuncValue
	{
		public long Value { get; }

		public InitFuncValue(long value)
		{
			//TODO: check six bytes
			Value = value;
		}

		public InitFuncValue(int[][] truthMatrix)
		{
			byte[] results = truthMatrix.Select(r => (byte)r.Last()).Reverse().ToArray();
			string strBinary = string.Join(String.Empty, results);
			Value = Convert.ToInt64(strBinary, 2);
		}

		public override string ToString()
		{
			return Value.ToString("X");
		}
	}
}
