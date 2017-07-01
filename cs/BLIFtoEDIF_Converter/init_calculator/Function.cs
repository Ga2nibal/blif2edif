using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIFtoEDIF_Converter.init_calculator
{
	public class Function
	{
		public Function(Port[] ports, LogicFunction logixFunction)
		{
			if(null == ports || 0 == ports.Length)
				throw new ArgumentNullException(nameof(ports), $"{nameof(ports)} is not defined");
			if (null == logixFunction)
				throw new ArgumentNullException(nameof(logixFunction), $"{nameof(logixFunction)} is not defined");
			Ports = ports;
			LogixFunction = logixFunction;
		}

		public string BLIF_Name { get; } = ".names";
		public Port[] Ports { get; }
		public LogicFunction LogixFunction { get; }
		public Port OutputPort => Ports.Last();

		public InitFuncValue CalculateInit()
		{
			int length = Ports.Length;
			length--; //last index is output
			int[][] truthMatrix = CreateInitTruthMatrix(length);
			LogixFunction.ApplyToMatrix(truthMatrix);
			InitFuncValue initFuncValue = new InitFuncValue(truthMatrix);
			return initFuncValue;
		}

		internal static int[][] CreateInitTruthMatrix(int length)
		{
			int count = (int)Math.Pow(2, length);
			int[][] result = new int[count][];
			for (int i = 0; i < count; i++)
			{
				const int toBase = 2;
				string strBinaryNumber = Convert.ToString(i, toBase);
				if (strBinaryNumber.Length > length)
					strBinaryNumber = strBinaryNumber.Substring(strBinaryNumber.Length - length);
				else
					for (int j = strBinaryNumber.Length; j < length; j++)
						strBinaryNumber = '0' + strBinaryNumber;
				List<int> binary = strBinaryNumber.Select(c => c - '0').ToList();

				binary.Add(0); //INIT RESULT
				result[i] = binary.ToArray();
			}
			return result;
		}
	}
}
