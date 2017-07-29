using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IEdifVersion : IEdifTextConvertable
	{
		int MajorVersion { get; }
		int MidVersion { get; }
		int MinorVersion { get; }
	}
}
