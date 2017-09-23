using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
{
	class Comment : IComment
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
	}
}
