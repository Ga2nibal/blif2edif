using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl;
using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library
{
	public interface ILibrary : IEdifTextConvertable
	{
		EdifLevel Level { get; }
		ITechnology Technology { get; }
		ICell Cell { get; }
	}
}
