using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Cell
{
	class Cell : ICell, IEquatable<Cell>
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

		#region [Equality]

		public bool Equals(Cell other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && CellType == other.CellType && Equals(View, other.View);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Cell) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (int) CellType;
				hashCode = (hashCode * 397) ^ (View != null ? View.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Cell left, Cell right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Cell left, Cell right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
