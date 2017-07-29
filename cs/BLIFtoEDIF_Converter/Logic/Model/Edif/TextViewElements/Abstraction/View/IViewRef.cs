using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View
{
	public interface IViewRef : IEdifTextConvertable
	{
		string Name { get; }
		ICellRef CellRef { get; }
	}
}
