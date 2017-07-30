using System.Collections.Generic;
using System.Linq;

namespace BLIFtoEDIF_Converter.Logic.Model.Blif
{
	public class Inputs
	{
		public Inputs(IReadOnlyList<Input> inputsList)
		{
			if(null == inputsList)
				inputsList = new List<Input>();
			InputList = inputsList.ToList();
		}

		public IReadOnlyList<Input> InputList { get; } 
	}
}
