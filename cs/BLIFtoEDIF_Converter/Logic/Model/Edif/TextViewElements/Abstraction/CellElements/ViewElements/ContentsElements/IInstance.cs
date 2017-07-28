using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.ContentsElements.InstanceElements;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.InterfaceElements;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.ContentsElements
{
	public interface IInstance
	{
		string Name { get; }
		IViewRef ViewRef { get; }
		IList<IProperty> Properties { get; } 
	}
}
