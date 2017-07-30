using System;
using System.Linq;
using System.Text;

namespace BLIFtoEDIF_Converter.Util
{
	public static class SrcCodeFormatter
	{
		public static string FormatEdifCode(string edifCode)
		{
			if(string.IsNullOrEmpty(edifCode))
				throw new ArgumentNullException(nameof(edifCode), $"{nameof(edifCode)} is not defined");
			edifCode = edifCode.Trim();
			if (!edifCode.StartsWith("("))
				throw new ArgumentException($"{nameof(edifCode)} must have '(' first character", nameof(edifCode));
			int openBracketsCount = edifCode.Count(c => c == '(');
			int clodeBracketsCount = edifCode.Count(c => c == ')');
			if(openBracketsCount != clodeBracketsCount)
				throw new ArgumentException($"{nameof(edifCode)} open brackets count must be equal to close bracket count",
					nameof(edifCode));
			StringBuilder builder = new StringBuilder(edifCode.Length);

			int tabIndex = 0;
			bool closeBracketOnNewLine = false;
			foreach (char c in edifCode)
			{
				if (c == '(')
				{
					builder.AppendLine();
					for (int i = 0; i < tabIndex; i++)
						builder.Append('\t');

					tabIndex++;
					closeBracketOnNewLine = false;
				}

				if (c == ')')
				{
					tabIndex--;
					if (closeBracketOnNewLine)
					{
						builder.AppendLine();
						for (int i = 0; i < tabIndex; i++)
							builder.Append('\t');
					}
					closeBracketOnNewLine = true;
				}

				builder.Append(c);
			}

			string result = builder.ToString();
			if (result.StartsWith(Environment.NewLine))
				result = result.TrimStart(Environment.NewLine.ToCharArray());

			return result;
		}
	}
}
