using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class External : IExternal
	{
		public External(string name, int edifLevel, ITechnology technology, IList<ICell> cells)
		{
			Name = name;
			EdifLevel = edifLevel;
			Technology = technology;
			Cells = cells;
		}

		#region [IExternal implementation]
		
		public string Name { get; }
		public int EdifLevel { get; }
		public ITechnology Technology { get; }
		public IList<ICell> Cells { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IExternal implementation]
	}
}
