using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
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
			string portsString = Ports != null && Ports.Count > 0
				? " " + string.Join(" ", Ports.Select(i => i.ToEdifText()))
				: string.Empty;
			string propertiesString = Properties != null && Properties.Count > 0
				? " " + string.Join(" ", Properties.Select(i => i.ToEdifText()))
				: string.Empty;
			string designatorString = string.IsNullOrEmpty(Designator) ? string.Empty : $" (designator \"{Designator}\")";
			//(interface (port z1 (direction OUTPUT)) (designator "xxx") (property TYPE (string "propvalue") (owner "xzc")))
			return $"(interface{portsString}{designatorString}{propertiesString})";
		}

		#endregion [IInterface implementation]
	}
}
