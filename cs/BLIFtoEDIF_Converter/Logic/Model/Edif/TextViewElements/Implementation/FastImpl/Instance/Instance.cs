using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Instance
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

		#region [IInstance implementation]
		
		public string Name { get; }
		public IViewRef ViewRef { get; }
		public IList<IProperty> Properties { get; }

		public string ToEdifText()
		{
			/*(instance LUT_40
              (viewRef view_1 (cellRef LUT5 (libraryRef UNISIMS)))
              (property XSTLIB (boolean (true)) (owner "Xilinx"))
              (property INIT (string "AABAAEA8") (owner "Xilinx"))
            )*/
			string propertiesString = Properties != null && Properties.Count > 0
				? " " + string.Join(" ", Properties.Select(p => p.ToEdifText()))
				: String.Empty;
			string viewRefString = ViewRef != null ? " " + ViewRef.ToEdifText() : string.Empty;
			return $"(instance {Name}{viewRefString}{propertiesString})";
		}

		#endregion  [IInstance implementation]
	}
}
