using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Contents : IContents, IEquatable<Contents>
	{
		public Contents(IList<IInstance> instances, IList<INet> nets)
		{
			Instances = instances ?? new List<IInstance>(0);
			Nets = nets ?? new List<INet>(0);
		}

		#region [IContents implementation]
		
		public IList<IInstance> Instances { get; }
		public IList<INet> Nets { get; }

		public string ToEdifText()
		{
			string instancesString = Instances != null && Instances.Count > 0
				? " " + string.Join(" ", Instances.Select(i => i.ToEdifText()))
				: string.Empty;
			string netsString = Nets != null && Nets.Count > 0
				? " " + string.Join(" ", Nets.Select(n => n.ToEdifText())) : string.Empty;

			//(contents [(instance <Name> ..)] [(net <Name> ..)])
			return $"(contents{instancesString}{netsString})";
		}

		#endregion [IContents implementation]

		#region [Equality]

		public bool Equals(Contents other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Instances.OrderBy(i => i.Name).SequenceEqual(other.Instances.OrderBy(i => i.Name)) && Nets.OrderBy(n => n.Name).SequenceEqual(other.Nets.OrderBy(n => n.Name));
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Contents) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Instances != null ? Instances.GetHashCode() : 0) * 397) ^ (Nets != null ? Nets.GetHashCode() : 0);
			}
		}

		public static bool operator ==(Contents left, Contents right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Contents left, Contents right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
