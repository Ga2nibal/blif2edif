using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View
{
	public interface IView : IEdifTextConvertable
	{
		string Name { get; }
		ViewType ViewType { get; }
		IInterface Interface { get; }
		IContents Contents { get; }
	}
}
