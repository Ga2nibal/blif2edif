﻿using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Instance
{
	class InstanceRef : IInstanceRef
	{
		public InstanceRef(string referedInstanceName)
		{
			if(string.IsNullOrEmpty(referedInstanceName))
				throw new ArgumentNullException(nameof(referedInstanceName), $"{nameof(referedInstanceName)} is not defined");
			ReferedInstanceName = referedInstanceName;
		}

		#region [IInstanceRef implementation]

		public string ReferedInstanceName { get; }

		public string ToEdifText()
		{
			//(instanceRef LUT_40)
			return $"(instanceRef {ReferedInstanceName})";
		}

		#endregion [IInstanceRef implementation]
	}
}
