using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Texhnology : ITechnology
	{
		public Texhnology(string name)
		{
			Name = name;
		}

		#region [ITechnology implementation]
		
		public string Name { get; }

		public string ToEdifText()
		{
			//(technology (numberDefinition))
			return $"(technology ({Name}))";
		}

		#endregion [ITechnology implementation]
	}
}
