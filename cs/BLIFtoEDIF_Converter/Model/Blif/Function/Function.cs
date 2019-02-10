using System;
using System.Linq;

namespace BLIFtoEDIF_Converter.Model.Blif.Function
{
	public class Function
	{
		public Function(Port[] ports, LogicFunction logicFunction)
		{
			if(null == ports || 0 == ports.Length)
				throw new ArgumentNullException(nameof(ports), $"{nameof(ports)} is not defined");
			if (null == logicFunction)
				throw new ArgumentNullException(nameof(logicFunction), $"{nameof(logicFunction)} is not defined");
			Ports = ports;
			LogicFunction = logicFunction;

			if (ports.Length == 1)
			{
				if(logicFunction.LogicRows.Count != 1)
					throw new ArgumentNullException(nameof(ports), $"one constant {nameof(ports)} have many logic rows");
				var logicRow = logicFunction.LogicRows.First();
				if (logicRow.Output)
					IsVCC = true;
				else
					IsGND = true;
			}
		}
		
		public Port[] Ports { get; }
		public LogicFunction LogicFunction { get; }
		public Port OutputPort => Ports.Last();

		public bool IsGND { get; }
		public bool IsVCC { get; }
	}
}
