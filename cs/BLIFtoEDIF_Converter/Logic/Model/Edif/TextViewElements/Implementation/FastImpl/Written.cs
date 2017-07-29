using System;
using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Written : IWritten
	{
		public Written(DateTime timestamp, IList<IComment> comments)
		{
			Timestamp = timestamp;
			Comments = comments;
		}

		#region [IWritten implementation]
		
		public DateTime Timestamp { get; }
		public IList<IComment> Comments { get; }

		public string ToEdifText()
		{
			throw new NotImplementedException();
		}

		#endregion [IWritten implementation]
	}
}
