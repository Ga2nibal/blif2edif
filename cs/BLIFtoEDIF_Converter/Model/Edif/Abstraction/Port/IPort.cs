using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port
{
	public interface IPort : IEdifTextConvertable
	{
		string Name { get; }
		PortDirection Direction { get; }
	}
}
