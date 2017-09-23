using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.View
{
	public interface IView : IEdifTextConvertable
	{
		string Name { get; }
		ViewType ViewType { get; }
		IInterface Interface { get; }
		IContents Contents { get; }
	}
}
