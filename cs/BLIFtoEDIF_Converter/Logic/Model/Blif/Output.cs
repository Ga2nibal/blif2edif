using System;

namespace BLIFtoEDIF_Converter.Logic.Model.Blif
{
	public class Output
	{
		public Output(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
		}

		public string Name { get; }
	}
}
