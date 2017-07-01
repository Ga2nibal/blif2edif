using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIFtoEDIF_Converter.init_calculator
{
	public class LogicFunction
	{
		private LogicFunctionRow[] _logicRows;

		public LogicFunction(IEnumerable<LogicFunctionRow> logicRows)
		{
			if (null == logicRows) throw new ArgumentNullException(nameof(logicRows), $"{nameof(logicRows)} is not defined");
			_logicRows = logicRows.ToArray();
		}

		public IReadOnlyList<LogicFunctionRow> LogicRows => _logicRows;
		public IReadOnlyList<bool> OutputColumn => _logicRows.Select(r => r.Output).ToArray();

		public void ApplyToMatrix(int[][] truthMatrix)
		{
			ValidateDimention(truthMatrix);
			ResetResults(truthMatrix);
			foreach (LogicFunctionRow logicRow in LogicRows)
				logicRow.ApplyToMetrix(truthMatrix);
		}

		private void ValidateDimention(int[][] truthMatrix)
		{
			if(truthMatrix[0].Length != LogicRows[0].RowData.Count)
				throw new ArgumentException("Logic function and truth matrix dimention incomptible");
		}

		private static void ResetResults(int[][] truthMatrix)
		{
			foreach (int[] ints in truthMatrix)
			{
				int resultIndex = ints.Length - 1;
				ints[resultIndex] = 0;
			}
		}

		public class LogicFunctionRow
		{
			private readonly bool?[] _rowData;

			public LogicFunctionRow(IEnumerable<bool?> rowData)
			{
				if (null == rowData) throw new ArgumentNullException(nameof(rowData), $"{nameof(rowData)} is not defined");
				if(!rowData.Last().HasValue)
					throw new ArgumentException($"{nameof(rowData)} can not contain 'Null' as last(output) value", nameof(rowData));
				_rowData = rowData.ToArray();
			}

			public bool? this[int index] => _rowData[index];


			public Boolean Output => _rowData.Last().Value;
			public List<bool?> Input => _rowData.Take(_rowData.Length - 1).ToList();
			public IReadOnlyList<bool?> RowData => _rowData;

			public static LogicFunctionRow FromStringDef(string stringDef)
			{
				string[] splittedInOutDef = 
					stringDef.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
				if (splittedInOutDef.Length != 2)
					throw new ArgumentException($"can not split '{nameof(stringDef)}' to input and output. " +
					                            $"stringDef: " + stringDef);
				List<bool?> rowData =
					splittedInOutDef[0].Select(d => d.ToLogicFunctionRowElement()).ToList();

				rowData.Add(splittedInOutDef[1].ToLogicFunctionRowElement());

				LogicFunctionRow logicFunctionRow = new LogicFunctionRow(rowData);
				return logicFunctionRow;
			}

			public void ApplyToMetrix(int[][] truthMatrix)
			{
				foreach (int[] ints in truthMatrix)
					if (InputPatternMatched(ints))
						ints[ints.Length - 1] = Output ? 1 : 0;
			}

			private bool InputPatternMatched(int[] ints)
			{
				if(ints.Length != _rowData.Length)
					throw new ArgumentException(nameof(ints), "Logic function row can not be applied to truth matrix row. " +
															  $" Logic function row length == {_rowData.Length}. " +
					                                          $"Matrix row length == {ints.Length}");
				for (int i = 0; i < ints.Length - 1; i++)
				{
					if(!_rowData[i].HasValue)
						continue;
					if (_rowData[i].Value != (ints[i] != 0))
						return false;
				}

				return true;
			}
		}

		public static LogicFunction FromStringDef(IEnumerable<string> logicFunctionStringDef)
		{
			List<LogicFunctionRow> logicFunctionRows =
				logicFunctionStringDef.Select(LogicFunctionRow.FromStringDef).ToList();

			LogicFunction result = new LogicFunction(logicFunctionRows);
			return result;
		}
	}
}
