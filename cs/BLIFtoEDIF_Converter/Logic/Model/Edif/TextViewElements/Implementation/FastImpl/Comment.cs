using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
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
