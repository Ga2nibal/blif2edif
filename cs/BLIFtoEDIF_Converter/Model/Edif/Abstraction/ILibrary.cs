namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface ILibrary
	{
		EdifLevel Level { get; }
		ITechnology Technology { get; }
		ICell Cell { get; }
	}
}
