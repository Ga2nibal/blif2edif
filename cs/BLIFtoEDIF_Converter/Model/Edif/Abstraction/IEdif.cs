using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IEdif : IEdifTextConvertable
	{
		string Name { get; }
		IEdifVersion Version { get; }
		IEdifLevel Level { get; }
		IKeywordMap KeywordMap { get; }
		IStatus Status { get; }
		IExternal External { get; }
		ILibrary Library { get; }
		IDesign Design { get; }
	}
}
