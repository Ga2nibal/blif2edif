using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	public class EdifVersion : IEdifVersion
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
	}
}
