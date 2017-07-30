using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Logic.Model.Blif.Function;

namespace BLIFtoEDIF_Converter.InitCalculator
{
	public class FunctionParser
	{
		public static List<Function> GetFunctions(IEnumerable<string> blifSrcLines)
		{
			if(null == blifSrcLines)
				throw new ArgumentNullException(nameof(blifSrcLines), $"{nameof(blifSrcLines)} is not defined");

			List<Function> result = new List<Function>();
			
			string functionDefStr = null;
			List<string> functionBody = new List<string>();
			const string functionKey = ".names";
			foreach (string blifSrcLineIt in blifSrcLines)
			{
				string blifSrcLine = blifSrcLineIt;
				int commentIndex = blifSrcLine.IndexOf("#", StringComparison.InvariantCulture);
				if (commentIndex >= 0)
					blifSrcLine = blifSrcLine.Substring(0, commentIndex);
				if (blifSrcLine.Contains("."))
				{
					if (functionDefStr != null)
					{
						LogicFunction logicFunction = FromStringDef(functionBody);
						functionBody.Clear();
						Port[] ports = Helpers.FromFuncDefStr(functionDefStr);
						Function func = new Function(ports, logicFunction);
						result.Add(func);
					}
					functionDefStr = null;
				}
				else if(functionDefStr != null && !string.IsNullOrEmpty(blifSrcLine)) //TODO: check invalid line
					functionBody.Add(blifSrcLine);
				if (blifSrcLine.Contains(functionKey))
					functionDefStr = blifSrcLine.Replace(functionKey, String.Empty).Trim();
			}

			return result;
		}
		public static LogicFunction FromStringDef(IEnumerable<string> logicFunctionStringDef)
		{
			List<LogicFunctionRow> logicFunctionRows =
				logicFunctionStringDef.Select(FromStringDef).ToList();

			LogicFunction result = new LogicFunction(logicFunctionRows);
			return result;
		}

		public static LogicFunctionRow FromStringDef(string stringDef)
		{
			string[] splittedInOutDef =
				stringDef.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			if (splittedInOutDef.Length != 2)
				throw new ArgumentException($"can not split '{nameof(stringDef)}' to input and output. " +
											$"stringDef: " + stringDef);
			List<bool?> rowData =
				splittedInOutDef[0].Select(d => d.ToLogicFunctionRowElement()).ToList();

			rowData.Add(splittedInOutDef[1].ToLogicFunctionRowElement());

			LogicFunctionRow logicFunctionRow = new LogicFunctionRow(rowData);
			return logicFunctionRow;
		}
	}
}
