using System;
using System.Collections.Generic;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IWritten : IEdifTextConvertable
	{
		DateTime Timestamp { get; }
		IList<IComment> Comments { get; } 
	}
}
