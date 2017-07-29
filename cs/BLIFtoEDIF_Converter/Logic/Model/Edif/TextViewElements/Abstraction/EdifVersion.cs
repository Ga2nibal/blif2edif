using System;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public class EdifVersion
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

		public int MajorVersion { get; }
		public int MidVersion { get; }
		public int MinorVersion { get; }
	}
}
