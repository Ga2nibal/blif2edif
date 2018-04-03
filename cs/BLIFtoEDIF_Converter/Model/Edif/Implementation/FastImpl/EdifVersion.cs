using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	public class EdifVersion : IEdifVersion, IEquatable<EdifVersion>
	{
		public EdifVersion(int majorVersion, int midVersion, int minorVersion)
		{
			if(majorVersion < 0)
				throw new AggregateException($"'{majorVersion}' can not be less than zero");
			if (midVersion < 0)
				throw new AggregateException($"'{midVersion}' can not be less than zero");
			if (minorVersion < 0)
				throw new AggregateException($"'{minorVersion}' can not be less than zero");
			MajorVersion = majorVersion;
			MidVersion = midVersion;
			MinorVersion = minorVersion;
		}

		#region [IEdifVersion implementation]
		
		public int MajorVersion { get; }
		public int MidVersion { get; }
		public int MinorVersion { get; }

		public string ToEdifText()
		{
			//(edifVersion 2 0 0)
			return $"(edifVersion {MajorVersion} {MidVersion} {MinorVersion})";
		}

		#endregion [IEdifVersion implementation]

		#region [Equality]

		public bool Equals(EdifVersion other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return MajorVersion == other.MajorVersion && MidVersion == other.MidVersion && MinorVersion == other.MinorVersion;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EdifVersion) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = MajorVersion;
				hashCode = (hashCode * 397) ^ MidVersion;
				hashCode = (hashCode * 397) ^ MinorVersion;
				return hashCode;
			}
		}

		public static bool operator ==(EdifVersion left, EdifVersion right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(EdifVersion left, EdifVersion right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
