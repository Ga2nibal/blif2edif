using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property
{
	public interface IProperty : IEdifTextConvertable
	{
		PropertyType PropertyType { get; }
		IPropertyValue Value { get; }
		string Owner { get; }
	}
}
