using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;
using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell
{
	public interface ICell : IEdifTextConvertable
	{
		string Name { get; }
		CellType CellType { get; }
		IView View { get; }
	}
}
