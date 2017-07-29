using System;
using System.Collections.Generic;
using System.Linq;
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
			string commentsString = Comments != null && Comments.Count > 0
				? " " + string.Join(" ", Comments.Select(i => i.ToEdifText()))
				: string.Empty;
			//(written (timestamp 2017 6 5 10 47 40) (comment "messageComment"))
			return $"(written (timestamp {Timestamp.Year} {Timestamp.Month} {Timestamp.Day} {Timestamp.Hour.ToString("D2")} {Timestamp.Minute.ToString("D2")} {Timestamp.Second.ToString("D2")}){commentsString})";
		}

		#endregion [IWritten implementation]
	}
}
