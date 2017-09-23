using BLIFtoEDIF_Converter.Model.Edif.Factory;
using BLIFtoEDIF_Converter.Model.Edif.Factory.TextViewElementsFactoryImplementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.BlifToEdifConverter
{
	[TestClass]
	public class BlifToEdifConverterFastImplTest : BlifToEdifConverterBaseTest
	{
		protected override ITextViewElementsFactory GetTextViewElementsFactory()
		{
			return new FastImplTextViewElementsFactory();
		}
	}
}
