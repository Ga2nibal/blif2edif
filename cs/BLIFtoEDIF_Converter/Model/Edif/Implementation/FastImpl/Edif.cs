using System;
using System.Text;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Edif : IEdif
	{
		public Edif(string name, IEdifVersion version, IEdifLevel level, IKeywordMap keywordMap, IStatus status, IExternal external, ILibrary library, IDesign design)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
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
			//(edif adder_as_main (edifVersion ...) (edifLevel ...) (keywordMap ...) (status ...) (external ...) (library ...) (design ...))
			StringBuilder builder = new StringBuilder();
			builder.Append("(edif");
			builder.Append(" ");
			builder.Append(Name);
			if (Version != null)
				builder.Append(" ").Append(Version.ToEdifText());
			if (Level != null)
				builder.Append(" ").Append(Level.ToEdifText());
			if (KeywordMap != null)
				builder.Append(" ").Append(KeywordMap.ToEdifText());
			if (Status != null)
				builder.Append(" ").Append(Status.ToEdifText());
			if (External != null)
				builder.Append(" ").Append(External.ToEdifText());
			if (Library != null)
				builder.Append(" ").Append(Library.ToEdifText());
			if (Design != null)
				builder.Append(" ").Append(Design.ToEdifText());
			builder.Append(")");
			return builder.ToString();
		}

		#endregion [IEdif implmentaion]
	}
}
