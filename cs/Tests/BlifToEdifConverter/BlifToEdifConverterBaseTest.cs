using System;
using BLIFtoEDIF_Converter.Logic;
using BLIFtoEDIF_Converter.Logic.Model.Blif;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory;
using BLIFtoEDIF_Converter.Parser.Blif;
using BLIFtoEDIF_Converter.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public abstract class BlifToEdifConverterBaseTest
	{
		protected abstract ITextViewElementsFactory GetTextViewElementsFactory();

		[TestMethod]
		public void TestMethod0()
		{
			string blifSrc = @"
.model CM82As
.inputs a b c d e
.outputs f g h
.names a s f f
01- 1
10- 1
1-1 1
-01 1
.names o r g g
11- 1
00- 1
1-1 1
-11 1
.names o d e h h 
01-- 1
0-1- 1
-11- 1
0--1 1
-1-1 1
.names a b c o o
00-- 1
0-0- 1
-00- 1
0--1 1
-0-1 1
--01 1
.names d e r r
01- 1
10- 1
0-1 1
-11 1
.names b c s s
01- 1
10- 1
1-1 1
-01 1
.end
";

			var srcLines = blifSrc.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
			Blif blif = BlifParser.GetBlif(srcLines);

			string renameLog;
			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			IEdif edif = blif.ToEdif(factory, out renameLog);

			Assert.IsNotNull(edif);

			string edifSrc = edif.ToEdifText();
			Assert.IsNotNull(edifSrc);
			string formattedEdifSrc = SrcCodeFormatter.FormatEdifCode(edifSrc);
			Assert.IsNotNull(renameLog);
			Console.WriteLine("# " + renameLog);
			Console.WriteLine();
			Assert.IsNotNull(formattedEdifSrc);
			Console.WriteLine(formattedEdifSrc);
		}

		[TestMethod]
		public void TestMethod1()
		{
			string blifSrc = @"
.model adder-as.blif
.inputs c0 c1 x20 x21 x10 x11
.outputs z0 z1 C0 C1
.names x10 x11 [4]0 [4]1 z1 z1
1001- 1
0110- 1
1---1 1
-1—-1 1
--1-1 1
---11 1

.names [7]0 [7]1 [8]0 [8]1 C1 C1
0110- 1
0101- 1
1001- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.names c1 c0 x21 x20 [4]1 [4]1
1001- 1
0110- 1
1---1 1
-1—-1 1
--1-1 1
---11 1 
.names x21 x20 x11 x10 [5]1 [5]1
1001- 1
1010- 1
0110- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.names x21 x11 [7]1 [7]1
11- 1
1-1 1
-11 1
.names c1 [5]1 [8]1 [8]1
11- 1
1-1 1
-11 1
.names x10 x11 [4]0 [4]1 z0 z0
1010- 1
0101- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.names [7]0 [8]0 C0 C0
11- 1
1-1 1
-11 1
.names c1 c0 x21 x20 [4]0 [4]0
0101- 1
1010- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.names x20 x10 [5]0 [5]0
11- 1
1—1 1
-11 1
.names x21 x20 x11 x10 [7]0 [7]0
0101- 1
1001- 1
0110- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.names c1 c0 [5]1 [5]0 [8]0 [8]0
0101- 1
0110- 1
1001- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.end
";

			var srcLines = blifSrc.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
			Blif blif = BlifParser.GetBlif(srcLines);

			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			string renameLog;
			IEdif edif = blif.ToEdif(factory, out renameLog);

			Assert.IsNotNull(edif);

			string edifSrc = edif.ToEdifText();
			Assert.IsNotNull(edifSrc);
			string formattedEdifSrc = SrcCodeFormatter.FormatEdifCode(edifSrc);
			Assert.IsNotNull(renameLog);
			Console.WriteLine("#" + renameLog);
			Console.WriteLine();
			Assert.IsNotNull(formattedEdifSrc);
			Console.WriteLine(formattedEdifSrc);
		}
	}
}
