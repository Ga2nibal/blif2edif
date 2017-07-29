using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port
{
	public interface IPortRef : IEdifTextConvertable
	{
		string Name { get; }
		IInstanceRef InstanceRef { get; }
	}
}
