using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance
{
	public interface IInstanceRef : IEdifTextConvertable
	{
		string ReferedInstanceName { get; }
	}
}
