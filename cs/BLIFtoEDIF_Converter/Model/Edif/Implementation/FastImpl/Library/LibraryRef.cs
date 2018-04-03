using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Library
{
	class LibraryRef : ILibraryRef, IEquatable<LibraryRef>
	{
		public LibraryRef(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
		}

		#region [ILibraryRef implementation]

		public string Name { get; }

		public string ToEdifText()
		{
			//(libraryRef UNISIMS)
			return $"(libraryRef {Name})";
		}

		#endregion [ILibraryRef implementation]

		#region [Equality]

		public bool Equals(LibraryRef other)
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
			return Equals((LibraryRef) obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public static bool operator ==(LibraryRef left, LibraryRef right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LibraryRef left, LibraryRef right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
