namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.ContentsElements.InstanceElements
{
	public interface IViewRef
	{
		string Name { get; }
		ICellRef CellRef { get; }
	}
}
