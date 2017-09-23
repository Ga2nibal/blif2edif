using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell
{
	public interface ICellRef : IEdifTextConvertable
	{
		string Name { get; }
		ILibraryRef LibraryRef { get; }
	}
}
