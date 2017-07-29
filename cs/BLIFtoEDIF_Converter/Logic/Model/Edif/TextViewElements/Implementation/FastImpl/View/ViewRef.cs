using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.View
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
			throw new System.NotImplementedException();
		}

		#endregion [IViewRef implementation]
	}
}
