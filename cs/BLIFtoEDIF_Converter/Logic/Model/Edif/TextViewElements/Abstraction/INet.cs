using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface INet : IEdifTextConvertable
	{
		string Name { get; }
		IList<IPortRef> Joined { get; } 
	}
}
