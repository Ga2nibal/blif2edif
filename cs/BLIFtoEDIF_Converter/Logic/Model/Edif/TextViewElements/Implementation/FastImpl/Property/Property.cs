using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Property
{
	class Property : IProperty
	{
		public Property(PropertyType propertyType, IPropertyValue value, string owner)
		{
			PropertyType = propertyType;
			Value = value;
			Owner = owner;
		}

		#region [IProperty implmentation]
		
		public PropertyType PropertyType { get; }
		public IPropertyValue Value { get; }
		public string Owner { get; }
		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IProperty implmentation]
	}
}
