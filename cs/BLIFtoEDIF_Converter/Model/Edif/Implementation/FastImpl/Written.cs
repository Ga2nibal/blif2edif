using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Written : IWritten, IEquatable<Written>
	{
		public Written(DateTime timestamp, IList<IComment> comments)
		{
			Timestamp = timestamp;
			Comments = comments ?? new List<IComment>();
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

		#region [Equality]

		public bool Equals(Written other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Timestamp.Equals(other.Timestamp) && Comments.SequenceEqual(other.Comments);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Written) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Timestamp.GetHashCode() * 397) ^ (Comments != null ? Comments.GetHashCode() : 0);
			}
		}

		public static bool operator ==(Written left, Written right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Written left, Written right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
