using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell
{
	public interface ICell : IEdifTextConvertable
	{
		string Name { get; }
		CellType CellType { get; }
		IView View { get; }
	}
}
