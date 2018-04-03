using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Design : IDesign, IEquatable<Design>
	{
		public Design(string name, IList<ICellRef> cellRefs, IList<IProperty> properties)
		{
			Name = name;
			CellRefs = cellRefs ?? new List<ICellRef>(0);
			Properties = properties ?? new List<IProperty>(0);
		}

		#region [IDesign implementation]
		
		public string Name { get; }
		public IList<ICellRef> CellRefs { get; }
		public IList<IProperty> Properties { get; }
		public string ToEdifText()
		{
			string cellRefsString = CellRefs != null && CellRefs.Count > 0
				? " " + string.Join(" ", CellRefs.Select(i => i.ToEdifText()))
				: string.Empty;
			string propertiesString = Properties != null && Properties.Count > 0
				? " " + string.Join(" ", Properties.Select(n => n.ToEdifText())) : string.Empty;

			//(design adder_as_main (cellRef adder_as_main (libraryRef adder_as_main_lib)) (property PART(string "xc6slx4-3-tqg144")(owner "Xilinx")))
			return $"(design {Name}{cellRefsString}{propertiesString})";
		}

		#endregion [IDesign implementation]

		#region [Equality]

		public bool Equals(Design other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && CellRefs.SequenceEqual(other.CellRefs) && Properties.SequenceEqual(other.Properties);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Design) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (CellRefs != null ? CellRefs.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Properties != null ? Properties.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Design left, Design right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Design left, Design right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
