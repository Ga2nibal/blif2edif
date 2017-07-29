using System.Collections.Generic;
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
			throw new System.NotImplementedException();
		}

		#endregion [IDesign implementation]
	}
}
