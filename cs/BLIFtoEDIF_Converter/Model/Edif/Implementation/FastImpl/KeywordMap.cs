using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	//TODO: Is it map?
	public class KeywordMap : IKeywordMap, IEquatable<KeywordMap>
	{
		public KeywordMap(int keywordLevel)
		{
			KeywordLevel = keywordLevel;
		}

		#region [IKeywordMap implementation]
		
		public int KeywordLevel { get; }
		public string ToEdifText()
		{
			//(keywordMap (keywordLevel 0))
			return $"(keywordMap (keywordLevel {KeywordLevel}))";
		}

		#endregion [IKeywordMap implementation]

		#region [Equality]

		public bool Equals(KeywordMap other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return KeywordLevel == other.KeywordLevel;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((KeywordMap) obj);
		}

		public override int GetHashCode()
		{
			return KeywordLevel;
		}

		public static bool operator ==(KeywordMap left, KeywordMap right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(KeywordMap left, KeywordMap right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
