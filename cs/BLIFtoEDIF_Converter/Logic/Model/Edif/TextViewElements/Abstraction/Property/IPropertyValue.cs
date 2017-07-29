using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property
{
	public interface IPropertyValue : IEdifTextConvertable
	{
		object Value { get; }
		PropertyValueType Type { get; }
	}
}
