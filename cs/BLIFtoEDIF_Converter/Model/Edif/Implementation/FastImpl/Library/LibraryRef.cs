using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Library
{
	class LibraryRef : ILibraryRef
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
	}
}
