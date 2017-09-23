using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.View
{
	public interface IViewRef : IEdifTextConvertable
	{
		string Name { get; }
		ICellRef CellRef { get; }
	}
}
