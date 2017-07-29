using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Cell
{
	class CellRef : ICellRef
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
			throw new System.NotImplementedException();
		}

		#endregion [ICellRef implementation]
	}
}
