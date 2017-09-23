using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIFtoEDIF_Converter.Model.Blif.Function
{
	public class LogicFunction
	{
		private readonly LogicFunctionRow[] _logicRows;

		public LogicFunction(IEnumerable<LogicFunctionRow> logicRows)
		{
			if (null == logicRows) throw new ArgumentNullException(nameof(logicRows), $"{nameof(logicRows)} is not defined");
			_logicRows = logicRows.ToArray();
		}

		public IReadOnlyList<LogicFunctionRow> LogicRows => _logicRows;
		public IReadOnlyList<bool> OutputColumn => _logicRows.Select(r => r.Output).ToArray();
	}
}
