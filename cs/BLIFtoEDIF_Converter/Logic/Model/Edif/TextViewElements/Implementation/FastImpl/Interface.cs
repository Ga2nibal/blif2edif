using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Interface : IInterface
	{
		public Interface(IList<IPort> ports, string designator, IList<IProperty> properties)
		{
			Ports = ports;
			Designator = designator;
			Properties = properties;
		}

		#region [IInterface implementation]
		
		public IList<IPort> Ports { get; }
		public string Designator { get; }
		public IList<IProperty> Properties { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IInterface implementation]
	}
}
