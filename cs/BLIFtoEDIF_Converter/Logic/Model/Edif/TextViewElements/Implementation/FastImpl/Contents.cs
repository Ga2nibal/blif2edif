using System.Collections.Generic;
using System.Linq;
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
			string instancesString = Instances != null && Instances.Count > 0
				? " " + string.Join(" ", Instances.Select(i => i.ToEdifText()))
				: string.Empty;
			string netsString = Nets != null && Nets.Count > 0
				? " " + string.Join(" ", Nets.Select(n => n.ToEdifText())) : string.Empty;

			//(contents [(instance <Name> ..)] [(net <Name> ..)])
			return $"(contents{instancesString}{netsString})";
		}

		#endregion [IContents implementation]
	}
}
