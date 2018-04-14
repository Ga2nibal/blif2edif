using System;
using System.Text;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Edif : IEdif, IEquatable<Edif>
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
		public IStatus Status { get; set; }
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

		#region [Equality]

		public bool Equals(Edif other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && Equals(Version, other.Version) && Equals(Level, other.Level) &&
					Equals(KeywordMap, other.KeywordMap) && Equals(Status, other.Status) && Equals(External, other.External) &&
					Equals(Library, other.Library) && Equals(Design, other.Design);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Edif) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Level != null ? Level.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (KeywordMap != null ? KeywordMap.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Status != null ? Status.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (External != null ? External.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Library != null ? Library.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Design != null ? Design.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Edif left, Edif right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Edif left, Edif right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
