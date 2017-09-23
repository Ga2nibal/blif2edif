using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	//TODO: Is it map?
	public class KeywordMap : IKeywordMap
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
	}
}
