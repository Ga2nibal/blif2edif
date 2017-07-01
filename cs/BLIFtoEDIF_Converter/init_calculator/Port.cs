﻿using System;

namespace BLIFtoEDIF_Converter.init_calculator
{
	public class Port
	{
		public Port(string portName, PortDirection portDirection)
		{
			if(null == portName) throw new ArgumentNullException(nameof(portName), $"{nameof(portName)} is not defined");
			Name = portName;
			Direction = portDirection;
		}

		public string Name { get; }
		public PortDirection Direction { get; }
	}

	
}
