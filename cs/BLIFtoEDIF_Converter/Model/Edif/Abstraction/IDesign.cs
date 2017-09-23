using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IDesign : IEdifTextConvertable
	{
		string Name { get; }
		IList<ICellRef> CellRefs { get; } 
		IList<IProperty> Properties { get; }
	}
}
