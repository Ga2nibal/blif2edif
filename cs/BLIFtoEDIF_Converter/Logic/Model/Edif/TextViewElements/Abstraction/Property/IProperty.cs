using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property
{
	public interface IProperty : IEdifTextConvertable
	{
		PropertyType PropertyType { get; }
		IPropertyValue Value { get; }
		string Owner { get; }
	}
}
