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
			
			for (int k = 0; k < edifCode.Length; k++)
			{
				char c = edifCode[k];
				if (c == '(')
				{
					var nextSubstring = edifCode.Substring(k);
					if (!nextSubstring.StartsWith("(libraryRef", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(boolean", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(string", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(cellRef", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(owner", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(true", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(false", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(rename", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(instanceRef", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(instanceRef", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(numberDefiniti", StringComparison.InvariantCultureIgnoreCase) &&
						!nextSubstring.StartsWith("(keywordLeve", StringComparison.InvariantCultureIgnoreCase))
					{
						builder.AppendLine();
						for (int i = 0; i < tabIndex; i++)
							builder.Append("  ");
					}

					tabIndex++;
					closeBracketOnNewLine = false;
				}

				if (c == ')')
				{
					tabIndex--;
					if (closeBracketOnNewLine && edifCode[k-1]!=')')
					{
						builder.AppendLine();
						for (int i = 0; i < tabIndex; i++)
							builder.Append("  ");
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
