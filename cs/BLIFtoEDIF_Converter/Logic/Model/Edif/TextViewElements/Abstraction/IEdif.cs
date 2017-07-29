using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IEdif
	{
		EdifVersion Version { get; }
		EdifLevel Level { get; }
		KeywordMap KeywordMap { get; }
		IStatus Status { get; }
		IExternal External { get; }
		ILibrary Library { get; }
		IDesign Design { get; }
	}
}
