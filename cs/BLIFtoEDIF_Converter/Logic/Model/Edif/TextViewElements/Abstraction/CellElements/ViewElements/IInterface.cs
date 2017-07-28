using System.Collections.Generic;
using BLIFtoEDIF_Converter.init_calculator;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.InterfaceElements;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements
{
	public interface IInterface
	{
		IList<Port> Ports { get; }
		string Designator { get; }
		IList<IProperty> Properties { get; }
	}
}
