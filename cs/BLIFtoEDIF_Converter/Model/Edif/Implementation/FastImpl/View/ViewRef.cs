using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.View
{
	class ViewRef : IViewRef, IEquatable<ViewRef>
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

		#region [Equality]

		public bool Equals(ViewRef other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && Equals(CellRef, other.CellRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ViewRef) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (CellRef != null ? CellRef.GetHashCode() : 0);
			}
		}

		public static bool operator ==(ViewRef left, ViewRef right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ViewRef left, ViewRef right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
