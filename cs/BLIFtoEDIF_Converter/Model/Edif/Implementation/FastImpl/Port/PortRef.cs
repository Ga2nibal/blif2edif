using System;
using System.Text;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Port
{
	class PortRef : IPortRef, IEquatable<PortRef>
	{
		public PortRef(string name, IInstanceRef instanceRef)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			InstanceRef = instanceRef;
		}

		#region [IPortRef implementation]
		
		public string Name { get; }
		public IInstanceRef InstanceRef { get; }

		public string ToEdifText()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("(portRef");
			if (Name != null)
			{
				builder.Append(" ");
				builder.Append(Name);
			}
			if (InstanceRef != null)
			{
				builder.Append(" ");
				builder.Append(InstanceRef.ToEdifText());
			}
			builder.Append(")");
			//(portRef I (instanceRef x20_IBUF_renamed_4))
			return builder.ToString();
		}

		#endregion [IPortRef implementation]

		public override string ToString()
		{
			return $"Edif PortRef. Name: {Name}, InstanceRef: {InstanceRef}";
		}

		#region [Equality]

		public bool Equals(PortRef other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name, StringComparison.InvariantCulture) && Equals(InstanceRef, other.InstanceRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PortRef) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (InstanceRef != null ? InstanceRef.GetHashCode() : 0);
			}
		}

		public static bool operator ==(PortRef left, PortRef right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PortRef left, PortRef right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
