using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;
using BLIFtoEDIF_Converter.Logic.TextConverter.Edif;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance
{
	public interface IInstance : IEdifTextConvertable
	{
		string Name { get; }
		IViewRef ViewRef { get; }
		IList<IProperty> Properties { get; } 
	}
}
