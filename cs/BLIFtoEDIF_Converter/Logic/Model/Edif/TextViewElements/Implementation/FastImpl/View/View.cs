using System;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.View
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
			throw new System.NotImplementedException();
		}

		#endregion [IView implementaion]
	}
}
