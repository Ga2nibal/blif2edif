using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Property
{
	class PropertyValue : IPropertyValue
	{
		public PropertyValue(object value, PropertyValueType type)
		{
			Value = value;
			Type = type;
		}

		#region [IPropertyValue implementation]
		
		public object Value { get; }
		public PropertyValueType Type { get; }
		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IPropertyValue implementation]
	}
}
