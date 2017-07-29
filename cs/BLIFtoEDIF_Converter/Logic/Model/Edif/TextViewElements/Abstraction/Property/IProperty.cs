namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property
{
	public interface IProperty
	{
		PropertyType PropertyType { get; }
		IPropertyValue Value { get; }
		string Owner { get; }
	}
}
