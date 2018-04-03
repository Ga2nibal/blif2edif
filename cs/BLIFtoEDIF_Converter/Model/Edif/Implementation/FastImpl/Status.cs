using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Status : IStatus, IEquatable<Status>
	{
		public Status(IWritten written)
		{
			Written = written;
		}

		#region [IStatus implementation]
		
		public IWritten Written { get; }

		public string ToEdifText()
		{
			//(status (written ..))
			if (Written != null)
				return $"(status {Written.ToEdifText()})";
			else
				return "(status)";
		}

		#endregion [IStatus implementation]

		#region [Equality]

		public bool Equals(Status other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Written, other.Written);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Status) obj);
		}

		public override int GetHashCode()
		{
			return (Written != null ? Written.GetHashCode() : 0);
		}

		public static bool operator ==(Status left, Status right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Status left, Status right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
