using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface ITechnology : IEdifTextConvertable
	{
		string Name { get; }
	}
}
