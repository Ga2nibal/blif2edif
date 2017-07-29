using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ConvertTextViewElementsTests
{
	[TestClass]
	public class ConvertTextViewElementsFastimplTest : ConvertTextViewElementsBaseTest
	{
		protected override ITextViewElementsFactory GetTextViewElementsFactory()
		{
			return TextViewElementsFactoryCreator.CreaTextViewElementsFactory(Implementations.FastImpl);
		}
	}
}
