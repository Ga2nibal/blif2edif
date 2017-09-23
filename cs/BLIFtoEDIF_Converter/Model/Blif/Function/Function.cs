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
		}
		
		public Port[] Ports { get; }
		public LogicFunction LogicFunction { get; }
		public Port OutputPort => Ports.Last();
	}
}
