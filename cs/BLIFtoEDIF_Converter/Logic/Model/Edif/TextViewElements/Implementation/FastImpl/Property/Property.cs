using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Property
{
	class Property : IProperty
	{
		public Property(PropertyType propertyType, IPropertyValue value, string owner)
		{
			if(null == value)
				throw new ArgumentNullException(nameof(value), $"{nameof(value)} is not defined");
			if (string.IsNullOrEmpty(owner))
				throw new ArgumentNullException(nameof(owner), $"{nameof(owner)} is not defined");
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
			//(property SHREG_EXTRACT_NGC (string "YES") (owner "Xilinx"))
			return $"(property {PropertyType} {Value.ToEdifText()} (owner \"{Owner}\"))"; //TODO: How to handle Owner == null
		}

		#endregion [IProperty implmentation]
	}
}
