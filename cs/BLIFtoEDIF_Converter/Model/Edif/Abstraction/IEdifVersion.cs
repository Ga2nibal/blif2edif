using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IEdifVersion : IEdifTextConvertable
	{
		int MajorVersion { get; }
		int MidVersion { get; }
		int MinorVersion { get; }
	}
}
