using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IExternal : IEdifTextConvertable
	{
		string Name { get; }
		IEdifLevel EdifLevel { get; }
		ITechnology Technology { get; }
		IList<ICell> Cells { get; }
	}
}
