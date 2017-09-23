using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Blif.Function;

namespace BLIFtoEDIF_Converter.Logic.InitCalculator
{
	public static class Helpers
	{
		private static char[] MinusSynonymsChars;
		private static string[] MinusSynonymsStrings;

		static Helpers()
		{
			MinusSynonymsChars = new char[]
			{
				(char) 45, (char) 8722, (char) 8208,
				(char) 8209, (char) 0xA7F7, (char) 8210,
				(char) 8211, (char) 8212
			};

			MinusSynonymsStrings = MinusSynonymsChars.
				Select(c => $"{c}").ToArray();
		}

		public static Port[] FromFuncDefStr(string portDefString)
		{
			string[] portNames = portDefString.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
			int lastIndex = portNames.Length - 1;
			List<Port> portList = new List<Port>();
			for (int i = 0; i < portNames.Length; i++)
			{
				Port port;
				if(i == lastIndex)
					port = new Port(portNames[i], PortDirection.Output);
				else
					port= new Port(portNames[i], PortDirection.Input);
				portList.Add(port);
			}
			return portList.ToArray();
		}

		public static bool? ToLogicFunctionRowElement(this string logicElementDef)
		{
			bool? output;
			if (IsMinus(logicElementDef))
				output = null;
			else if (logicElementDef.Equals("1", StringComparison.InvariantCulture))
				output = true;
			else if (logicElementDef.Equals("0", StringComparison.InvariantCulture))
				output = false;
			else
				throw new ArgumentException($"{nameof(logicElementDef)} can be defined as '-' or '1' or '0'," +
												$"but actually value is '{logicElementDef}'");
			return output;
		}

		public static bool? ToLogicFunctionRowElement(this char logicElementDef)
		{
			bool? output;
			if (IsMinus(logicElementDef))
				output = null;
			else if (logicElementDef == '1')
				output = true;
			else if (logicElementDef == '0')
				output = false;
			else
				throw new ArgumentException($"{nameof(logicElementDef)} can be defined as '-' or '1' or '0'," +
												$"but actually value is '{logicElementDef}'");
			return output;
		}
		
		private static bool IsMinus(char symbol)
		{
			return MinusSynonymsChars.Contains(symbol);
		}

		private static bool IsMinus(string symbol)
		{
			return MinusSynonymsStrings.Contains(symbol);
		}
	}
}
