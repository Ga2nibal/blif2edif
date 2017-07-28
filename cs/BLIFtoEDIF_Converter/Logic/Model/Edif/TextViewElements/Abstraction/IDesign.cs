using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.ContentsElements.InstanceElements;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.InterfaceElements;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IDesign
	{
		string Name { get; }
		IList<ICellRef> CellRefs { get; } 
		IList<IProperty> Properties { get; }
	}
}
