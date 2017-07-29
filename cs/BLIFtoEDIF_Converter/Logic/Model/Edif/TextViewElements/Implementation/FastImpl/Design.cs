using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Design : IDesign
	{
		public Design(string name, IList<ICellRef> cellRefs, IList<IProperty> properties)
		{
			Name = name;
			CellRefs = cellRefs;
			Properties = properties;
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
	}
}
