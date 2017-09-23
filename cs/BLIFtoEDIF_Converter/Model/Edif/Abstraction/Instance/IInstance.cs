using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;
using BLIFtoEDIF_Converter.Model.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance
{
	public interface IInstance : IEdifTextConvertable
	{
		string Name { get; }
		string RenamedSynonym { get; }
		IViewRef ViewRef { get; }
		IList<IProperty> Properties { get; } 
	}
}
