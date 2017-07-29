using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Contents : IContents
	{
		public Contents(IList<IInstance> instances, IList<INet> nets)
		{
			Instances = instances;
			Nets = nets;
		}

		#region [IContents implementation]
		
		public IList<IInstance> Instances { get; }
		public IList<INet> Nets { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IContents implementation]
	}
}
