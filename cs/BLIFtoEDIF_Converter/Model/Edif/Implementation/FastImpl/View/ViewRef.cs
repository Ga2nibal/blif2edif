using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.View
{
	class ViewRef : IViewRef
	{
		public ViewRef(string name, ICellRef cellRef)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			CellRef = cellRef;
		}

		#region [IViewRef implementation]
		
		public string Name { get; }
		public ICellRef CellRef { get; }

		public string ToEdifText()
		{
			string cellRefString = CellRef != null ? " " + CellRef.ToEdifText() : string.Empty;
			//(viewRef view_1 (cellRef LUT5 (libraryRef UNISIMS)))
			return $"(viewRef {Name}{cellRefString})";
		}

		#endregion [IViewRef implementation]
	}
}
