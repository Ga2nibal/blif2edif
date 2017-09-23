using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.View
{
	class View : IView
	{
		public View(string name, ViewType viewType, IInterface @interface, IContents contents)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			ViewType = viewType;
			Interface = @interface;
			Contents = contents;
		}

		#region [IView implementaion]
		
		public string Name { get; }
		public ViewType ViewType { get; }
		public IInterface Interface { get; }
		public IContents Contents { get; }

		public string ToEdifText()
		{
			string interfaceString = Interface != null ? " " + Interface.ToEdifText() : string.Empty;
			string contentsString = Contents != null ? " " + Contents.ToEdifText() : string.Empty;
			//(view view_1 (viewType NETLIST) (interface..) (contents..))
			return $"(view {Name} (viewType {ViewType}){interfaceString}{contentsString})";
		}

		#endregion [IView implementaion]
	}
}
