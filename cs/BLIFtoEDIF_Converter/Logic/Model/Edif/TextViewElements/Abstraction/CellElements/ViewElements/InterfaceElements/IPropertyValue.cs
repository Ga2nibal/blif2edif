namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.InterfaceElements
{
	public interface IPropertyValue
	{
		object Value { get; }
		PropertyValueType Type { get; }
	}
}
