using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Port
{
	class Port : IPort, IEquatable<Port>
	{
		public Port(string name, PortDirection direction)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			Direction = direction;
		}

		#region [IPort implementation]
		
		public string Name { get; }
		public PortDirection Direction { get; }

		public string ToEdifText()
		{
			//(port c1 (direction INPUT))
			return $"(port {Name} (direction {Direction}))";
		}

		#endregion [IPort implementation]

		public override string ToString()
		{
			return $"EdifPort. Name: {Name}, Direction: {Direction}";
		}

		#region [Equality]

		public bool Equals(Port other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name, StringComparison.InvariantCulture) && Direction == other.Direction;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Port) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (int) Direction;
			}
		}

		public static bool operator ==(Port left, Port right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Port left, Port right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
