using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Library
{
	class Library : ILibrary
	{
		public Library(string name, IEdifLevel level, ITechnology technology, ICell cell)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			Level = level;
			Technology = technology;
			Cell = cell;
		}

		#region [ILibrary implementation]

		public string Name { get; }
		public IEdifLevel Level { get; }
		public ITechnology Technology { get; }
		public ICell Cell { get; }

		public string ToEdifText()
		{
			/*(library adder_as_main_lib
    (edifLevel 0)
    (technology (numberDefinition))
    (cell adder_as_main...))*/
			string levelString = Level != null ? " " + Level.ToEdifText() : string.Empty;
			string technologyString = Technology != null ? " " + Technology.ToEdifText() : string.Empty;
			string cellString = Cell != null ? " " + Cell.ToEdifText() : string.Empty;
			return $"(library {Name}{levelString}{technologyString}{cellString})";
		}

		#endregion [ILibrary implementation]
	}
}
