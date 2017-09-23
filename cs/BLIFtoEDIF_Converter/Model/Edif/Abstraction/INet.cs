using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface INet : IEdifTextConvertable
	{
		string Name { get; }
		IList<IPortRef> Joined { get; } 
	}
}
