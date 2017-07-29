using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IInterface : IEdifTextConvertable
	{
		IList<init_calculator.Port> Ports { get; }
		string Designator { get; }
		IList<IProperty> Properties { get; }
	}
}
