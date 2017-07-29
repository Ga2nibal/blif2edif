using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;
using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
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
