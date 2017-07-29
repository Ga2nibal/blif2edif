using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Edif : IEdif
	{
		public Edif(EdifVersion version, EdifLevel level, KeywordMap keywordMap, IStatus status, IExternal external, ILibrary library, IDesign design)
		{
			Version = version;
			Level = level;
			KeywordMap = keywordMap;
			Status = status;
			External = external;
			Library = library;
			Design = design;
		}

		#region [IEdif implmentaion]

		public EdifVersion Version { get; }
		public EdifLevel Level { get; }
		public KeywordMap KeywordMap { get; }
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
