using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Port
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
			//(port c1 (direction INPUT))
			return $"(port {Name} (direction {Direction}))";
		}

		#endregion [IPort implementation]
	}
}
