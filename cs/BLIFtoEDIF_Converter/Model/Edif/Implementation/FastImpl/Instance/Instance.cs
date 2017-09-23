using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Instance
{
	class Instance : IInstance
	{
		public Instance(string name, IViewRef viewRef, IList<IProperty> properties)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			ViewRef = viewRef;
			Properties = properties;
		}

		public Instance(string name, string renamedSynonym, IViewRef viewRef, IList<IProperty> properties)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			RenamedSynonym = renamedSynonym;
			ViewRef = viewRef;
			Properties = properties;
		}

		#region [IInstance implementation]

		public string Name { get; }
		public string RenamedSynonym { get; }
		public IViewRef ViewRef { get; }
		public IList<IProperty> Properties { get; }

		public string ToEdifText()
		{
			/*(instance LUT_40
              (viewRef view_1 (cellRef LUT5 (libraryRef UNISIMS)))
              (property XSTLIB (boolean (true)) (owner "Xilinx"))
              (property INIT (string "AABAAEA8") (owner "Xilinx"))
            )*/
			string nameStr = Name;
			if (!string.IsNullOrEmpty(RenamedSynonym))
				nameStr = $"(rename {RenamedSynonym} \"{Name}\")";
			string propertiesString = Properties != null && Properties.Count > 0
				? " " + string.Join(" ", Properties.Select(p => p.ToEdifText()))
				: String.Empty;
			string viewRefString = ViewRef != null ? " " + ViewRef.ToEdifText() : string.Empty;
			return $"(instance {nameStr}{viewRefString}{propertiesString})";
		}

		#endregion  [IInstance implementation]
	}
}
