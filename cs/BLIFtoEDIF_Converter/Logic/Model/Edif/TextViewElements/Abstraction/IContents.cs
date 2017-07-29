using System.Collections.Generic;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IContents
	{
		IList<IInstance> Instances { get; } 
		IList<INet> Nets { get; } 
	}
}
