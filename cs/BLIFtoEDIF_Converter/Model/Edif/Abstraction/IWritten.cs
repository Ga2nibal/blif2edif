using System;
using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction
{
	public interface IWritten : IEdifTextConvertable
	{
		DateTime Timestamp { get; }
		IList<IComment> Comments { get; } 
	}
}
