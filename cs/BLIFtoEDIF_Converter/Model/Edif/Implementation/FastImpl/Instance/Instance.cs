using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Instance
{
	class Instance : IInstance, IEquatable<Instance>
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
			Properties = properties ?? new List<IProperty>(0);
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

		#region [Equality]

		public bool Equals(Instance other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && string.Equals(RenamedSynonym, other.RenamedSynonym) && Equals(ViewRef, other.ViewRef) && Properties.SequenceEqual(other.Properties);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Instance) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RenamedSynonym != null ? RenamedSynonym.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ViewRef != null ? ViewRef.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Properties != null ? Properties.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Instance left, Instance right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Instance left, Instance right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
