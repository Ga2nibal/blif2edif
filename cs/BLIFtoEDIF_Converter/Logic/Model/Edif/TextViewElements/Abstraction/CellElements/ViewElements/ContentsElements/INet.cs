using System.Collections.Generic;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.ContentsElements
{
	public interface INet
	{
		string Name { get; }
		IList<IPortRef> Joined { get; } 
	}
}
