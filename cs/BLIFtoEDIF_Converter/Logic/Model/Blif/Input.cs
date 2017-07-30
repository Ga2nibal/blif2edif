using System;

namespace BLIFtoEDIF_Converter.Logic.Model.Blif
{
	public class Input
	{
		public Input(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
		}

		public string Name { get; set; }
	}
}
