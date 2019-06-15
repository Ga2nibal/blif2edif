using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Blif;
using BLIFtoEDIF_Converter.Model.Blif.Function;

namespace BLIFtoEDIF_Converter.Logic.Transformations.Feedback
{
	public class FeedbackTransformation
	{
		public static Blif AddFeedbackToFunction(Blif blifModel)
		{
			IReadOnlyList<Function> functions = blifModel.Functions.Select(f => ApplyFeedback(f)).ToArray();
			Blif result = new Blif(new Model.Blif.Model(blifModel.Model.Name), new Inputs(blifModel.Inputs.InputList.ToList()), new Outputs(blifModel.Outputs.OutputList.ToList()), functions);
			return result;
		}

		private static Function ApplyFeedback(Function function)
		{
			if(function.Ports.Length < 3)
				return new Function(function);
			List<LogicFunctionRow> logicRows = new List<LogicFunctionRow>();
			foreach (LogicFunctionRow logicFunctionLogicRow in function.LogicFunction.LogicRows)
			{
				List<bool?> rowData = new List<bool?>(logicFunctionLogicRow.RowData.Count + 1);
				rowData.AddRange(logicFunctionLogicRow.Input);
				rowData.Add(null);
				rowData.Add(logicFunctionLogicRow.Output);
				LogicFunctionRow row = new LogicFunctionRow(rowData);
				logicRows.Add(row);
			}


			for (int i = 0; i < function.Ports.Length - 1; i++)
			{
				List<bool?> rowData = new List<bool?>(function.Ports.Length + 1);
				for (int j = 0; j < function.Ports.Length - 1; j++)
				{
					if(i == j)
						rowData.Add(true);
					else
						rowData.Add(null);
				}
				rowData.Add(true); //feedback
				rowData.Add(true); //output
				LogicFunctionRow row = new LogicFunctionRow(rowData);
				logicRows.Add(row);
			}

			LogicFunction newLogicFunction = new LogicFunction(logicRows);


			List<Port> newPorts = function.Ports.ToList();
			newPorts.Add(new Port(function.Ports.Last()));


			return new Function(newPorts.ToArray(), newLogicFunction);
		}
	}
}
