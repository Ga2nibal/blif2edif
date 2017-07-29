using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	interface IKeywordMap : IEdifTextConvertable
	{
		int KeywordLevel { get; }
	}
}
