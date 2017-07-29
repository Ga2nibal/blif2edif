using System;
using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Instance
{
	class Instance : IInstance
	{
		public Instance(string name, IViewRef viewRef, IList<IProperty> properties)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			ViewRef = viewRef;
			Properties = properties;
		}

		#region [IInstance implementation]
		
		public string Name { get; }
		public IViewRef ViewRef { get; }
		public IList<IProperty> Properties { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion  [IInstance implementation]
	}
}
