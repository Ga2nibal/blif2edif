using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Cell
{
	class CellRef : ICellRef, IEquatable<CellRef>
	{
		public CellRef(string name, ILibraryRef libraryRef)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			LibraryRef = libraryRef;
		}

		#region [ICellRef implementation]

		public string Name { get; }
		public ILibraryRef LibraryRef { get; }

		public string ToEdifText()
		{
			string libraryRefString = LibraryRef != null ? " " + LibraryRef.ToEdifText() : string.Empty;
			//cellRef LUT5 (libraryRef UNISIMS))
			return $"(cellRef {Name}{libraryRefString})";
		}

		#endregion [ICellRef implementation]

		#region [Equality]

		public bool Equals(CellRef other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && Equals(LibraryRef, other.LibraryRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CellRef) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (LibraryRef != null ? LibraryRef.GetHashCode() : 0);
			}
		}

		public static bool operator ==(CellRef left, CellRef right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CellRef left, CellRef right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
