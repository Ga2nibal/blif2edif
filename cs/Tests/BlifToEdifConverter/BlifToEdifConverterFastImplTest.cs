using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory.TextViewElementsFactoryImplementations;
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
