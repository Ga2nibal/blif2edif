using System;
using System.Collections.Generic;

namespace BLIFtoEDIF_Converter.init_calculator
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
						LogicFunction logicFunction = LogicFunction.FromStringDef(functionBody);
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
	}
}
