using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Texhnology : ITechnology, IEquatable<Texhnology>
	{
		public Texhnology(string name)
		{
			Name = name;
		}

		#region [ITechnology implementation]
		
		public string Name { get; }

		public string ToEdifText()
		{
			//(technology (numberDefinition))
			return $"(technology ({Name}))";
		}

		#endregion [ITechnology implementation]

		#region [Equality]

		public bool Equals(Texhnology other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Texhnology) obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public static bool operator ==(Texhnology left, Texhnology right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Texhnology left, Texhnology right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
