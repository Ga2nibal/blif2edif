using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library
{
	public interface ILibrary : IEdifTextConvertable
	{
		string Name { get; }
		IEdifLevel Level { get; }
		ITechnology Technology { get; }
		ICell Cell { get; }
	}
}
