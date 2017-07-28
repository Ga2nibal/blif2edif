using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.ContentsElements;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements
{
	public interface IContents
	{
		IList<IInstance> Instances { get; } 
		IList<INet> Nets { get; } 
	}
}
