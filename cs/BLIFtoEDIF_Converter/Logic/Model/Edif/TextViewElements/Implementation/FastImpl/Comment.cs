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
			throw new System.NotImplementedException();
		}

		#endregion [IComment implementation]
	}
}
