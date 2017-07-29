using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell
{
	public interface ICell : IEdifTextConvertable
	{
		CellType CellType { get; }
	}
}
