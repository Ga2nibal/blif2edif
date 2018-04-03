using System;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Comment : IComment, IEquatable<Comment>
	{
		public Comment(string text)
		{
			Text = text;
		}

		#region [IComment implementation]

		public string Text { get; }

		public string ToEdifText()
		{
			//(comment "represented by this netlist.")
			return $"(comment \"{Text}\")";
		}

		#endregion [IComment implementation]

		#region [Equality]

		public bool Equals(Comment other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Text, other.Text);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Comment) obj);
		}

		public override int GetHashCode()
		{
			return (Text != null ? Text.GetHashCode() : 0);
		}

		public static bool operator ==(Comment left, Comment right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Comment left, Comment right)
		{
			return !Equals(left, right);
		}

		#endregion [Equality]
	}
}
