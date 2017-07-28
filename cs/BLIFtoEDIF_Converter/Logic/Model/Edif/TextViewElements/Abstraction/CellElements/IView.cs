namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface ICellView
	{
		string Name { get; }
		CellViewType ViewType { get; }
	}
}
