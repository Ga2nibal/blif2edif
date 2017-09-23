using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library
{
	public interface ILibraryRef : IEdifTextConvertable
	{
		string Name { get; }
	}
}
