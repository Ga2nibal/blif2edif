using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Property
{
	class PropertyValue : IPropertyValue, IEquatable<PropertyValue>
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

		#region [Equality]

		public bool Equals(PropertyValue other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value) && Type == other.Type;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PropertyValue) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ (int) Type;
			}
		}

		public static bool operator ==(PropertyValue left, PropertyValue right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PropertyValue left, PropertyValue right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
