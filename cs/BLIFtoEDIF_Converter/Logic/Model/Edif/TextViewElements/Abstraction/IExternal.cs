using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IExternal : IEdifTextConvertable
	{
		string Name { get; }
		IEdifLevel EdifLevel { get; }
		ITechnology Technology { get; }
		IList<ICell> Cells { get; }
	}
}
