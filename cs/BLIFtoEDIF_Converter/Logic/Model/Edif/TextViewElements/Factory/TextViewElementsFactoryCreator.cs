using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory.TextViewElementsFactoryImplementations;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory
{
	public static class TextViewElementsFactoryCreator
	{
		public static ITextViewElementsFactory CreaTextViewElementsFactory(Implementations implementation)
		{
			switch (implementation)
			{
				case Implementations.FastImpl:
					return new FastImplTextViewElementsFactory();
				default:
					throw new ArgumentOutOfRangeException(nameof(implementation), $"{nameof(implementation)} value '{implementation}' is not expected");
			}
		}
	}
}
