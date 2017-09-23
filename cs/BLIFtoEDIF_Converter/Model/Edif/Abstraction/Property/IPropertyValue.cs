using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property
{
	public interface IPropertyValue : IEdifTextConvertable
	{
		object Value { get; }
		PropertyValueType Type { get; }
	}
}
