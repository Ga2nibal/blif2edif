using System.Collections.Generic;
using System.Linq;
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
			string joinedString = Joined != null && Joined.Count > 0
				? " " +"(joined " + string.Join(" ", Joined.Select(i => i.ToEdifText())) + ")"
				: string.Empty;
			//(net x81 (joined (portRef I0(instanceRef LUT_81)) (portRef I1(instanceRef LUT_C11))))
			return $"(net {Name}{joinedString})";
		}

		#endregion [INet implementation]
	}
}
