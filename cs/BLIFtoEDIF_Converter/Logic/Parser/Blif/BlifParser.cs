using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLIFtoEDIF_Converter.Logic.InitCalculator;
using BLIFtoEDIF_Converter.Model.Blif;
using BLIFtoEDIF_Converter.Model.Blif.Function;

namespace BLIFtoEDIF_Converter.Logic.Parser.Blif
{
	public class BlifParser
	{
		public static Model.Blif.Blif GetBlif(IEnumerable<string> blifSrcLines)
		{
			if (null == blifSrcLines)
				throw new ArgumentNullException(nameof(blifSrcLines), $"{nameof(blifSrcLines)} is not defined");

			List<string> blifNormalizedSrcLines = NormalizeLines(blifSrcLines).ToList();
			List<Function> result = GetFunctions(blifNormalizedSrcLines);

			Model.Blif.Model model = null;
			Inputs inputs = null;
			Outputs outputs = null;
			foreach (string blifSrcLine in blifNormalizedSrcLines)
			{
				if (model == null)
					TryGetBlifModel(blifSrcLine, out model);
				if (inputs == null)
					TryGetInputs(blifSrcLine, out inputs);
				if (outputs == null)
					TryGetOutputs(blifSrcLine, out outputs);
				if(model != null && inputs != null && outputs != null)
					break;
			}

			return new Model.Blif.Blif(model, inputs, outputs, result);
		}

		private static IEnumerable<string> NormalizeLines(IEnumerable<string> blifSrcLines)
		{
			const string commentSymbol = "#";
			const string concatenationLineSymbol = "\\";

			StringBuilder concatinatedLineBuilder = new StringBuilder();
			foreach (string line in blifSrcLines)
			{
				string blifSrcLine = line;
				int commentIndex = blifSrcLine.IndexOf(commentSymbol, StringComparison.InvariantCulture);
				if (commentIndex >= 0)
					blifSrcLine = blifSrcLine.Substring(0, commentIndex);

				if (blifSrcLine.EndsWith(concatenationLineSymbol))
				{
					concatinatedLineBuilder.Append(blifSrcLine);
					concatinatedLineBuilder.Remove(concatinatedLineBuilder.Length - 1, 1); //Remove concatenationLineSymbol
					continue;
				}

				if (concatinatedLineBuilder.Length != 0)
				{
					concatinatedLineBuilder.Append(blifSrcLine);
					yield return concatinatedLineBuilder.ToString();
					concatinatedLineBuilder.Clear();
					continue;
				}

				if(!string.Empty.Equals(blifSrcLine.Trim()))
					yield return blifSrcLine;
			}
		}

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
				if (blifSrcLine.Contains(functionKey) || blifSrcLine.StartsWith(".exdc") || blifSrcLine.StartsWith(".end"))
				{
					if (functionDefStr != null)
					{
						Port[] ports = Helpers.FromFuncDefStr(functionDefStr);
						LogicFunction logicFunction = FromStringDef(functionBody);
						functionBody.Clear();
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

			if(!logicFunctionRows.Any())
				logicFunctionRows.Add(new LogicFunctionRow(new bool?[] { false })); //GND

			LogicFunction result = new LogicFunction(logicFunctionRows);
			return result;
		}

		public static LogicFunctionRow FromStringDef(string stringDef)
		{
			string[] splittedInOutDef =
				stringDef.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
			if (splittedInOutDef.Length == 1)
			{
				const string vccConst = "1";
				if (!vccConst.Equals(splittedInOutDef[0].Trim()))
					throw new ArgumentException($"can not split '{nameof(stringDef)}' to input and output. " +
												$"stringDef: {stringDef}. VCC value should be '{vccConst}'");
				return new LogicFunctionRow(new bool?[]{true});
			}


			if (splittedInOutDef.Length != 2)
				throw new ArgumentException($"can not split '{nameof(stringDef)}' to input and output. " +
											$"stringDef: " + stringDef);
			List<bool?> rowData =
				splittedInOutDef[0].Select(d => d.ToLogicFunctionRowElement()).ToList();

			rowData.Add(splittedInOutDef[1].ToLogicFunctionRowElement());

			LogicFunctionRow logicFunctionRow = new LogicFunctionRow(rowData);
			return logicFunctionRow;
		}

		public static bool TryGetBlifModel(string srcLine, out Model.Blif.Model model)
		{
			model = null;
			const string modelKeyword = ".model ";
			srcLine = srcLine.Trim();
			if (!srcLine.Contains(modelKeyword))
				return false;
			srcLine = srcLine.Replace(modelKeyword, string.Empty);
			model = new Model.Blif.Model(srcLine);
			return true;
		}

		public static bool TryGetInputs(string srcLine, out Inputs inputs)
		{
			inputs = null;
			const string modelKeyword = ".inputs ";
			srcLine = srcLine.Trim();
			if (!srcLine.Contains(modelKeyword))
				return false;
			srcLine = srcLine.Replace(modelKeyword, string.Empty);
			string[] inputStrings = srcLine.Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries);
			var inputCollection = inputStrings.Select(s => new Input(s));
			inputs = new Inputs(inputCollection);
			return true;
		}

		public static bool TryGetOutputs(string srcLine, out Outputs outputs)
		{
			outputs = null;
			const string modelKeyword = ".outputs ";
			srcLine = srcLine.Trim();
			if (!srcLine.Contains(modelKeyword))
				return false;
			srcLine = srcLine.Replace(modelKeyword, string.Empty);
			string[] inputStrings = srcLine.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			var outputCollection = inputStrings.Select(s => new Output(s));
			outputs = new Outputs(outputCollection);
			return true;
		}
	}
}
