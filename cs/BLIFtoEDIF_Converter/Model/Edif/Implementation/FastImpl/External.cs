using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class External : IExternal, IEquatable<External>
	{
		public External(string name, IEdifLevel edifLevel, ITechnology technology, IList<ICell> cells)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			EdifLevel = edifLevel;
			Technology = technology;
			Cells = cells ?? new List<ICell>(0);
		}

		#region [IExternal implementation]
		
		public string Name { get; }
		public IEdifLevel EdifLevel { get; }
		public ITechnology Technology { get; }
		public IList<ICell> Cells { get; }

		public string ToEdifText()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("(external").Append(" ").Append(Name);
			if (EdifLevel != null)
				builder.Append(" ").Append(EdifLevel.ToEdifText());
			if (Technology != null)
				builder.Append(" ").Append(Technology.ToEdifText());
			if (Cells != null && Cells.Count > 0)
				builder.Append(" ").Append(string.Join(" ", Cells.Select(i => i.ToEdifText())));
			builder.Append(")");
			//(external name (edifLevel 0) (technology(numberDefinition)) (cell LUT5))
			return builder.ToString();
		}

		#endregion [IExternal implementation]

		#region [Equality]

		public bool Equals(External other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && Equals(EdifLevel, other.EdifLevel) && Equals(Technology, other.Technology) && Cells.SequenceEqual(other.Cells);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((External) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EdifLevel != null ? EdifLevel.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Technology != null ? Technology.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Cells != null ? Cells.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(External left, External right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(External left, External right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
