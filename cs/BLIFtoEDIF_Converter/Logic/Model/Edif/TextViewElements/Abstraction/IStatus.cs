using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IStatus : IEdifTextConvertable
	{
		IWritten Written { get; }
	}
}
