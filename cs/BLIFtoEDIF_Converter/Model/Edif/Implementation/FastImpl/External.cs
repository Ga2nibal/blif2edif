using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class External : IExternal
	{
		public External(string name, IEdifLevel edifLevel, ITechnology technology, IList<ICell> cells)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			EdifLevel = edifLevel;
			Technology = technology;
			Cells = cells;
		}

		#region [IExternal implementation]
		
		public string Name { get; }
		public IEdifLevel EdifLevel { get; }
		public ITechnology Technology { get; }
		public IList<ICell> Cells { get; }

		public string ToEdifText()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("(external").Append(" ").Append(Name);
			if (EdifLevel != null)
				builder.Append(" ").Append(EdifLevel.ToEdifText());
			if (Technology != null)
				builder.Append(" ").Append(Technology.ToEdifText());
			if (Cells != null && Cells.Count > 0)
				builder.Append(" ").Append(string.Join(" ", Cells.Select(i => i.ToEdifText())));
			builder.Append(")");
			//(external name (edifLevel 0) (technology(numberDefinition)) (cell LUT5))
			return builder.ToString();
		}

		#endregion [IExternal implementation]
	}
}
