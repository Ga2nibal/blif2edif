using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance
{
	public interface IInstanceRef : IEdifTextConvertable
	{
		string ReferedInstanceName { get; }
	}
}
