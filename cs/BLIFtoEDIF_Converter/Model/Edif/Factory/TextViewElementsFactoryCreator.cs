using System;
using BLIFtoEDIF_Converter.Model.Edif.Factory.TextViewElementsFactoryImplementations;

namespace BLIFtoEDIF_Converter.Model.Edif.Factory
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
