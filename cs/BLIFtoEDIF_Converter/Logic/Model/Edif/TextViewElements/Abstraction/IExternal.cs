using System.Collections.Generic;

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
