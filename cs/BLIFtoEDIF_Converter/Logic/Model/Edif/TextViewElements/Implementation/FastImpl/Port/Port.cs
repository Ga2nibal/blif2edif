using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Port
{
	class Port : IPort
	{
		public Port(string name, PortDirection direction)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			Direction = direction;
		}

		#region [IPort implementation]
		
		public string Name { get; }
		public PortDirection Direction { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IPort implementation]
	}
}
