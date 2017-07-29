using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Library
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
