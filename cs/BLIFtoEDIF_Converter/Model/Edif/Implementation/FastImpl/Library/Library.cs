using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Library
{
	class Library : ILibrary, IEquatable<Library>
	{
		public Library(string name, IEdifLevel level, ITechnology technology, ICell cell)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			Level = level;
			Technology = technology;
			Cell = cell;
		}

		#region [ILibrary implementation]

		public string Name { get; }
		public IEdifLevel Level { get; }
		public ITechnology Technology { get; }
		public ICell Cell { get; }

		public string ToEdifText()
		{
			/*(library adder_as_main_lib
    (edifLevel 0)
    (technology (numberDefinition))
    (cell adder_as_main...))*/
			string levelString = Level != null ? " " + Level.ToEdifText() : string.Empty;
			string technologyString = Technology != null ? " " + Technology.ToEdifText() : string.Empty;
			string cellString = Cell != null ? " " + Cell.ToEdifText() : string.Empty;
			return $"(library {Name}{levelString}{technologyString}{cellString})";
		}

		#endregion [ILibrary implementation]

		#region [Equality]

		public bool Equals(Library other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && Equals(Level, other.Level) && Equals(Technology, other.Technology) && Equals(Cell, other.Cell);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Library) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Level != null ? Level.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Technology != null ? Technology.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Cell != null ? Cell.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Library left, Library right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Library left, Library right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
