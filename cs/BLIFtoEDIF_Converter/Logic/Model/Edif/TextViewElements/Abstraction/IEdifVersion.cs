using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	interface IEdifVersion : IEdifTextConvertable
	{
		int MajorVersion { get; }
		int MidVersion { get; }
		int MinorVersion { get; }
	}
}
