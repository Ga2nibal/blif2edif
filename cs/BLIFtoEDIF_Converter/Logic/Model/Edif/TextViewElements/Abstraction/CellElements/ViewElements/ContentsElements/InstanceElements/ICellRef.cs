namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.ContentsElements.InstanceElements
{
	public interface ICellRef
	{
		string Name { get; }
		ILibraryRef LibraryRef { get; }
	}
}
