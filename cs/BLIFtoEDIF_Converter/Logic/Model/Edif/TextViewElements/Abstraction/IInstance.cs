using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction
{
	public interface IInstance : IEdifTextConvertable
	{
		string Name { get; }
		IViewRef ViewRef { get; }
		IList<IProperty> Properties { get; } 
	}
}
