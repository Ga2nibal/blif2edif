using System;
using System.Text;
using BLIFtoEDIF_Converter.Model.Blif;
using BLIFtoEDIF_Converter.Model.Blif.Function;

namespace BLIFtoEDIF_Converter.Model.TextConverter.Blif.Impl
{
	public class BlifWriter : IBlifWriter
	{
		#region [BlifWriter]

		public string ToSourceCode(Model.Blif.Blif blif)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(this.ToSourceCode(blif.Model));
			sb.AppendLine(this.ToSourceCode(blif.Inputs));
			sb.AppendLine(this.ToSourceCode(blif.Outputs));
			foreach (Function blifFunction in blif.Functions)
				sb.Append(this.ToSourceCode(blifFunction));
			sb.AppendLine(".end");
			return sb.ToString();
		}

		public string ToSourceCode(Model.Blif.Model item)
		{
			return $".model {item.Name}";
		}

		public string ToSourceCode(Inputs item)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(".inputs");
			foreach (Input input in item.InputList)
				sb.Append(" ").Append(ToSourceCode(input));
			return sb.ToString();
		}

		public string ToSourceCode(Input item)
		{
			return item.Name;
		}

		public string ToSourceCode(Outputs item)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(".outputs");
			foreach (Output output in item.OutputList)
				sb.Append(" ").Append(ToSourceCode(output));
			return sb.ToString();
		}

		public string ToSourceCode(Output item)
		{
			return item.Name;
		}

		public string ToSourceCode(Function item)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(".names");
			foreach (Port port in item.Ports)
				sb.Append(" ").Append(ToSourceCode(port));
			sb.AppendLine();
			sb.Append(ToSourceCode(item.LogicFunction));
			return sb.ToString();
		}

		public string ToSourceCode(Port item)
		{
			return item.Name;
		}

		public string ToSourceCode(PortDirection item)
		{
			throw new InvalidOperationException();
		}

		public string ToSourceCode(LogicFunction item)
		{
			StringBuilder sb = new StringBuilder();
			foreach (LogicFunctionRow logicFunctionRow in item.LogicRows)
			{
				string rowString = ToSourceCode(logicFunctionRow);
				if(!string.IsNullOrEmpty(rowString))
					sb.AppendLine(rowString);
			}

			return sb.ToString();
		}

		public string ToSourceCode(LogicFunctionRow item)
		{
			StringBuilder sb = new StringBuilder();
			if (item.Input.Count == 0 && !item.Output)
				return string.Empty;
			foreach (bool? x in item.Input)
				sb.Append(ToSourceCode(x));
			sb.Append(" ").Append(ToSourceCode(item.Output));
			return sb.ToString();
		}

		#endregion [BlifWriter]

		private string ToSourceCode(bool? logicFunctionRowData)
		{
			if (!logicFunctionRowData.HasValue)
				return "-";
			return logicFunctionRowData.Value ? "1" : "0";
		}
	}
}
