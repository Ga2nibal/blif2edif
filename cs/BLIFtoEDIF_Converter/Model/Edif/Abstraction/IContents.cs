using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IContents : IEdifTextConvertable
	{
		IList<IInstance> Instances { get; } 
		IList<INet> Nets { get; } 
	}
}
