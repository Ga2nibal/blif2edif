using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Interface : IInterface, IEquatable<Interface>
	{
		public Interface(IList<IPort> ports, string designator, IList<IProperty> properties)
		{
			Ports = ports ?? new List<IPort>(0);
			Designator = designator;
			Properties = properties ?? new List<IProperty>(0);
		}

		#region [IInterface implementation]
		
		public IList<IPort> Ports { get; }
		public string Designator { get; }
		public IList<IProperty> Properties { get; }

		public string ToEdifText()
		{
			string portsString = Ports != null && Ports.Count > 0
				? " " + string.Join(" ", Ports.Select(i => i.ToEdifText()))
				: string.Empty;
			string propertiesString = Properties != null && Properties.Count > 0
				? " " + string.Join(" ", Properties.Select(i => i.ToEdifText()))
				: string.Empty;
			string designatorString = string.IsNullOrEmpty(Designator) ? string.Empty : $" (designator \"{Designator}\")";
			//(interface (port z1 (direction OUTPUT)) (designator "xxx") (property TYPE (string "propvalue") (owner "xzc")))
			return $"(interface{portsString}{designatorString}{propertiesString})";
		}

		#endregion [IInterface implementation]

		#region [Equality]

		public bool Equals(Interface other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Ports.OrderBy(p => p.Name).SequenceEqual(other.Ports.OrderBy(p => p.Name)) && string.Equals(Designator, other.Designator) && Properties.SequenceEqual(other.Properties);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Interface) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Ports != null ? Ports.Count.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Designator != null ? Designator.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Properties != null ? Properties.Count.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Interface left, Interface right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Interface left, Interface right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
