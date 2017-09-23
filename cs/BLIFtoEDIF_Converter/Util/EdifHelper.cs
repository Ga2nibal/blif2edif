using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.Edif.Factory;

namespace BLIFtoEDIF_Converter.Util
{
	public class EdifHelper
	{
		public static IProperty CreateEdifProperty(ITextViewElementsFactory edifFactory, PropertyType propertyType, string owner, string value)
		{
			IPropertyValue propertyValue = edifFactory.CreatePropertyValue(value, PropertyValueType.String);
			IProperty property = edifFactory.CreateProperty(propertyType, propertyValue, owner);
			return property;
		}

		public static IProperty CreateEdifProperty(ITextViewElementsFactory edifFactory, PropertyType propertyType, string owner, bool value)
		{
			IPropertyValue propertyValue = edifFactory.CreatePropertyValue(value, PropertyValueType.Boolean);
			IProperty property = edifFactory.CreateProperty(propertyType, propertyValue, owner);
			return property;
		}

		public static IProperty CreateEdifProperty(ITextViewElementsFactory edifFactory, PropertyType propertyType, string owner, int value)
		{
			IPropertyValue propertyValue = edifFactory.CreatePropertyValue(value, PropertyValueType.Integer);
			IProperty property = edifFactory.CreateProperty(propertyType, propertyValue, owner);
			return property;
		}
	}
}
