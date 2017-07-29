using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IContents : IEdifTextConvertable
	{
		IList<IInstance> Instances { get; } 
		IList<INet> Nets { get; } 
	}
}
