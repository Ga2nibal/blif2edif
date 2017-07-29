using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Cell
{
	class Cell : ICell
	{
		public Cell(string name, CellType cellType, IView view)
		{
			Name = name;
			CellType = cellType;
			View = view;
		}

		#region [ICell implementation]

		public string Name { get; }
		public CellType CellType { get; }
		public IView View { get; }

		public string ToEdifText()
		{
			string viewString = View != null ? " " + View.ToEdifText() : string.Empty;
			return $"(cell {Name} (cellType {CellType}){viewString})";
		}

		#endregion [ICell implementation]
	}
}
