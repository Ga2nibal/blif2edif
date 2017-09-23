using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Blif.Function;

namespace BLIFtoEDIF_Converter.Logic.InitCalculator
{
	public static class InitCalculator
	{
		public static InitFuncValue CalculateInit(this Function blifFunction)
		{
			int length = blifFunction.Ports.Length;
			length--; //last index is output
			int[][] truthMatrix = CreateInitTruthMatrix(length);
			blifFunction.LogicFunction.ApplyToMatrix(truthMatrix);
			InitFuncValue initFuncValue = new InitFuncValue(truthMatrix);
			return initFuncValue;
		}

		public static void ApplyToMatrix(this LogicFunction blifLogicFunction, int[][] truthMatrix)
		{
			blifLogicFunction.ValidateDimention(truthMatrix);
			ResetResults(truthMatrix);
			foreach (LogicFunctionRow logicRow in blifLogicFunction.LogicRows)
				logicRow.ApplyToMetrix(truthMatrix);
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
		public static void ApplyToMetrix(this LogicFunctionRow logicFunctionRow, int[][] truthMatrix)
		{
			foreach (int[] ints in truthMatrix)
				if (logicFunctionRow.InputPatternMatched(ints))
					ints[ints.Length - 1] = logicFunctionRow.Output ? 1 : 0;
		}

		private static bool InputPatternMatched(this LogicFunctionRow logicFunctionRow, int[] ints)
		{
			if (ints.Length != logicFunctionRow.RowData.Count)
				throw new ArgumentException(nameof(ints), "Logic function row can not be applied to truth matrix row. " +
														  $" Logic function row length == {logicFunctionRow.RowData.Count}. " +
														  $"Matrix row length == {ints.Length}");
			for (int i = 0; i < ints.Length - 1; i++)
			{
				if (!logicFunctionRow.RowData[i].HasValue)
					continue;
				if (logicFunctionRow.RowData[i].Value != (ints[i] != 0))
					return false;
			}

			return true;
		}

		private static void ValidateDimention(this LogicFunction blifLogicFunction, int[][] truthMatrix)
		{
			if (truthMatrix[0].Length != blifLogicFunction.LogicRows[0].RowData.Count)
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
	}
}
