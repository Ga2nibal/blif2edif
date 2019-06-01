using System;

namespace BLIFtoEDIF_Converter.Model.Blif.Function
{
	public class Port
	{
		public Port(Port p) : this(p.Name, p.Direction)
		{
		}

		public Port(string portName, PortDirection portDirection)
		{
			if(null == portName) throw new ArgumentNullException(nameof(portName), $"{nameof(portName)} is not defined");
			Name = portName;
			Direction = portDirection;
		}

		public string Name { get; set; }
		public PortDirection Direction { get; }
	}
}
