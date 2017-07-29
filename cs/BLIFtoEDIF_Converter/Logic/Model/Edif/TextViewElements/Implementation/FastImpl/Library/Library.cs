using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Library
{
	class Library : ILibrary
	{
		public Library(EdifLevel level, ITechnology technology, ICell cell)
		{
			Level = level;
			Technology = technology;
			Cell = cell;
		}

		#region [ILibrary implementation]

		public EdifLevel Level { get; }
		public ITechnology Technology { get; }
		public ICell Cell { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [ILibrary implementation]
	}
}
