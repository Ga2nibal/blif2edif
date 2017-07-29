using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library
{
	public interface ILibrary
	{
		EdifLevel Level { get; }
		ITechnology Technology { get; }
		ICell Cell { get; }
	}
}
