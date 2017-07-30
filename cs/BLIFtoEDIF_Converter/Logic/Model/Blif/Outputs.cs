using System.Collections.Generic;
using System.Linq;

namespace BLIFtoEDIF_Converter.Logic.Model.Blif
{
	public class Outputs
	{
		public Outputs(IReadOnlyList<Output> outputList)
		{
			if(null == outputList)
				outputList = new List<Output>();
			OutputList = outputList.ToList();
		}

		public IReadOnlyList<Output> OutputList { get; } 
	}
}
