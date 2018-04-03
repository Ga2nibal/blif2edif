using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.View
{
	class View : IView, IEquatable<View>
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

		#region [Equality]

		public bool Equals(View other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && ViewType == other.ViewType && Equals(Interface, other.Interface) && Equals(Contents, other.Contents);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((View) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (int) ViewType;
				hashCode = (hashCode * 397) ^ (Interface != null ? Interface.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Contents != null ? Contents.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(View left, View right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(View left, View right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
