using System;
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
			//(string "xc6slx4-3-tqg144")
			//(boolean (true))
			//(integer 0)
			switch (Type)
			{
				case PropertyValueType.Integer:
					return $"(integer {Value})"; //TODO: Validate 'Value' .net Type
				case PropertyValueType.Boolean:
					return $"(boolean ({Value.ToString().ToLowerInvariant()}))"; //TODO: Validate 'Value' .net Type
				case PropertyValueType.String:
					return $"(string \"{Value}\")"; //TODO: Validate 'Value' .net Type
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), $"{nameof(Type)} value '{Type}' is not expected.");
			}
		}

		#endregion [IPropertyValue implementation]
	}
}
