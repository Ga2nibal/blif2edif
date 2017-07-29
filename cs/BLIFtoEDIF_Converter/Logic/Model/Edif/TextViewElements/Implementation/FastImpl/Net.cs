using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Net : INet
	{
		public Net(string name, IList<IPortRef> joined)
		{
			Name = name;
			Joined = joined;
		}

		#region [INet implementation]
		
		public string Name { get; }
		public IList<IPortRef> Joined { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [INet implementation]
	}
}
