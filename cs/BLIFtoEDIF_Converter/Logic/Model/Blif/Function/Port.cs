﻿using System;

namespace BLIFtoEDIF_Converter.Logic.Model.Blif.Function
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
