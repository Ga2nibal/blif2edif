using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIFtoEDIF_Converter.Model.Blif.Function
{
	public class LogicFunctionRow
	{
		private readonly bool?[] _rowData;

		public LogicFunctionRow(IEnumerable<bool?> rowData)
		{
			if (null == rowData) throw new ArgumentNullException(nameof(rowData), $"{nameof(rowData)} is not defined");
			if (!rowData.Last().HasValue)
				throw new ArgumentException($"{nameof(rowData)} can not contain 'Null' as last(output) value", nameof(rowData));
			_rowData = rowData.ToArray();
		}

		public LogicFunctionRow(LogicFunctionRow lfr) : this(lfr._rowData.ToArray())
		{
		}

		public bool? this[int index] => _rowData[index];

		public Boolean Output => _rowData.Last().Value;
		public List<bool?> Input => _rowData.Take(_rowData.Length - 1).ToList();
		public IReadOnlyList<bool?> RowData => _rowData;
	}
}
