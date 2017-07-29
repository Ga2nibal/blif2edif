using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell
{
	public interface ICellRef : IEdifTextConvertable
	{
		string Name { get; }
		ILibraryRef LibraryRef { get; }
	}
}
