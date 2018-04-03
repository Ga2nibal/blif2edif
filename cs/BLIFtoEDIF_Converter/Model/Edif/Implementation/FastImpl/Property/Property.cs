using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Property
{
	class Property : IProperty, IEquatable<Property>
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

		public override string ToString()
		{
			return
				$"PropertyType: '{PropertyType}'. Owner: '{Owner}'. Value.Type: '{Value?.Type}'. Value.Value: '{Value?.Value}'.";
		}

		#region [Equality]

		public bool Equals(Property other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return PropertyType == other.PropertyType && Equals(Value, other.Value) && string.Equals(Owner, other.Owner);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Property) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) PropertyType;
				hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Owner != null ? Owner.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Property left, Property right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Property left, Property right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
