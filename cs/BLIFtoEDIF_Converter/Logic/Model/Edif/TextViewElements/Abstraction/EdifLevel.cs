using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public class EdifLevel : IEdifTextConvertable
	{
		public EdifLevel(int level)
		{
			Level = level;
		}

		public int Level { get; }
		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}
	}
}
