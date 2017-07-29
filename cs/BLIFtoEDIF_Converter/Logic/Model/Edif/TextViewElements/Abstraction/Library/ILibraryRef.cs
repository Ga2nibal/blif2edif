using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library
{
	public interface ILibraryRef : IEdifTextConvertable
	{
		string Name { get; }
	}
}
