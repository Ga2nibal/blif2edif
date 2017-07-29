﻿using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	public class EdifLevel : IEdifLevel
	{
		public EdifLevel(int level)
		{
			Level = level;
		}

		#region [IEdifLevel implementation]
		
		public int Level { get; }
		public string ToEdifText()
		{
			//(edifLevel 0)
			return $"(edifLevel {Level})";
		}

		#endregion [IEdifLevel implementation]
	}
}
