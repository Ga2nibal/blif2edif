using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port
{
	public interface IPort : IEdifTextConvertable
	{
		string Name { get; }
		PortDirection Direction { get; }
	}
}
