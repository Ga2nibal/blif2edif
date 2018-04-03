using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	public class EdifLevel : IEdifLevel, IEquatable<EdifLevel>
	{
		public EdifLevel(int level)
		{
			Level = level;
		}

		#region [IEdifLevel implementation]
		
		public int Level { get; }
		public string ToEdifText()
		{
			//(edifLevel 0)
			return $"(edifLevel {Level})";
		}

		#endregion [IEdifLevel implementation]

		#region [Equality]

		public bool Equals(EdifLevel other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Level == other.Level;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EdifLevel) obj);
		}

		public override int GetHashCode()
		{
			return Level;
		}

		public static bool operator ==(EdifLevel left, EdifLevel right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(EdifLevel left, EdifLevel right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
