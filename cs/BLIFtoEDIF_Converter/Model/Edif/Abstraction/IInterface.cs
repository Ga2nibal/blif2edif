using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IInterface : IEdifTextConvertable
	{
		IList<IPort> Ports { get; }
		string Designator { get; }
		IList<IProperty> Properties { get; }
	}
}
