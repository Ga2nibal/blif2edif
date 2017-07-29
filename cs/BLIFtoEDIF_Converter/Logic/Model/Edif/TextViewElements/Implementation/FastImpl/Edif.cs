using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Edif : IEdif
	{
		public Edif(string name, IEdifVersion version, IEdifLevel level, IKeywordMap keywordMap, IStatus status, IExternal external, ILibrary library, IDesign design)
		{
			Name = name;
			Version = version;
			Level = level;
			KeywordMap = keywordMap;
			Status = status;
			External = external;
			Library = library;
			Design = design;
		}

		#region [IEdif implmentaion]

		public string Name { get; }
		public IEdifVersion Version { get; }
		public IEdifLevel Level { get; }
		public IKeywordMap KeywordMap { get; }
		public IStatus Status { get; }
		public IExternal External { get; }
		public ILibrary Library { get; }
		public IDesign Design { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IEdif implmentaion]
	}
}
