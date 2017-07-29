using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public class KeywordMap : IEdifTextConvertable
	{
		public KeywordMap(int keywordLevel)
		{
			KeywordLevel = keywordLevel;
		}

		public int KeywordLevel { get; }
		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}
	}
}
