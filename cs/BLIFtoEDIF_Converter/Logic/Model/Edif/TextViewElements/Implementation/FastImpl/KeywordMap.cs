using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
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
			throw new System.NotImplementedException();
		}

		#endregion [IKeywordMap implementation]
	}
}
