using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port
{
	public interface IPortRef : IEdifTextConvertable
	{
		string Name { get; }
		IInstanceRef InstanceRef { get; }
	}
}
