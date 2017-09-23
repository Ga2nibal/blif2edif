using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IComment : IEdifTextConvertable
	{
		string Text { get; }
	}
}
