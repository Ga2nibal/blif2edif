using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Net : INet, IEquatable<Net>
	{
		public Net(string name, IList<IPortRef> joined)
		{
			Name = name;
			Joined = joined ?? new List<IPortRef>(0);
		}

		#region [INet implementation]
		
		public string Name { get; }
		public IList<IPortRef> Joined { get; }

		public string ToEdifText()
		{
			string joinedString = Joined != null && Joined.Count > 0
				? " " +"(joined " + string.Join(" ", Joined.Select(i => i.ToEdifText())) + ")"
				: string.Empty;
			//(net x81 (joined (portRef I0(instanceRef LUT_81)) (portRef I1(instanceRef LUT_C11))))
			return $"(net {Name}{joinedString})";
		}

		#endregion [INet implementation]

		#region [Equality]

		public bool Equals(Net other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && Joined.SequenceEqual(other.Joined);
			//.OrderBy(j => j.Name).ThenBy(p => p.InstanceRef?.ReferedInstanceName)
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Net) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Joined != null ? Joined.GetHashCode() : 0);
			}
		}

		public static bool operator ==(Net left, Net right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Net left, Net right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
