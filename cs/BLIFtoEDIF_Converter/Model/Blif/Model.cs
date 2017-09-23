using System;

namespace BLIFtoEDIF_Converter.Model.Blif
{
	public class Model
	{
		public Model(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
		}

		public string Name { get; }
	}
}
