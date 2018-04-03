using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Instance
{
	class InstanceRef : IInstanceRef, IEquatable<InstanceRef>
	{
		public InstanceRef(string referedInstanceName)
		{
			if(string.IsNullOrEmpty(referedInstanceName))
				throw new ArgumentNullException(nameof(referedInstanceName), $"{nameof(referedInstanceName)} is not defined");
			ReferedInstanceName = referedInstanceName;
		}

		#region [IInstanceRef implementation]

		public string ReferedInstanceName { get; }

		public string ToEdifText()
		{
			//(instanceRef LUT_40)
			return $"(instanceRef {ReferedInstanceName})";
		}

		#endregion [IInstanceRef implementation]

		#region [Equality]

		public bool Equals(InstanceRef other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(ReferedInstanceName, other.ReferedInstanceName);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((InstanceRef) obj);
		}

		public override int GetHashCode()
		{
			return (ReferedInstanceName != null ? ReferedInstanceName.GetHashCode() : 0);
		}

		public static bool operator ==(InstanceRef left, InstanceRef right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(InstanceRef left, InstanceRef right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
