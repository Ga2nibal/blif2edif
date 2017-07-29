using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IExternal
	{
		string Name { get; }
		int EdifLevel { get; }
		ITechnology Technology { get; }
		IList<ICell> Cells { get; }
	}
}
