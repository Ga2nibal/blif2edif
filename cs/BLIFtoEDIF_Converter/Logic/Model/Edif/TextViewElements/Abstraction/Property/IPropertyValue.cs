namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property
{
	public interface IPropertyValue
	{
		object Value { get; }
		PropertyValueType Type { get; }
	}
}
