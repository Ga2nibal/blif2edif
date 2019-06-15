namespace BLIFtoEDIF_Converter.Model.TextConverter.Blif
{
	public interface IBlifWriter
	{
		string ToSourceCode(Model.Blif.Blif item);

		string ToSourceCode(Model.Blif.Model item);

		string ToSourceCode(Model.Blif.Inputs item);
		string ToSourceCode(Model.Blif.Input item);

		string ToSourceCode(Model.Blif.Outputs item);
		string ToSourceCode(Model.Blif.Output item);

		string ToSourceCode(Model.Blif.Function.Function item);

		string ToSourceCode(Model.Blif.Function.Port item);
		string ToSourceCode(Model.Blif.Function.PortDirection item);

		string ToSourceCode(Model.Blif.Function.LogicFunction item);
		string ToSourceCode(Model.Blif.Function.LogicFunctionRow item);
	}
}
