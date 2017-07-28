namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.CellElements.ViewElements.InterfaceElements
{
	public interface IPort
	{
		string Name { get; }
		PortDirection Direction { get; }
	}
}
