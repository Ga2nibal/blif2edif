using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
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
			throw new System.NotImplementedException();
		}

		#endregion [ITechnology implementation]
	}
}
