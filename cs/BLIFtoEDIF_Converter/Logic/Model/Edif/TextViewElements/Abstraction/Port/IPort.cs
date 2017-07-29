namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port
{
	public interface IPort
	{
		string Name { get; }
		PortDirection Direction { get; }
	}
}
