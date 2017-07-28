namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IEdifVersion
	{
		int MajorVersion { get; }
		int MidVersion { get; }
		int MinorVersion { get; }
	}
}
