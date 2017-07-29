using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Port
{
	class PortRef : IPortRef
	{
		public PortRef(string name, IInstanceRef instanceRef)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			InstanceRef = instanceRef;
		}

		#region [IPortRef implementation]
		
		public string Name { get; }
		public IInstanceRef InstanceRef { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IPortRef implementation]
	}
}
