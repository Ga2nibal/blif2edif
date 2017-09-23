using System;
using System.Collections.Generic;
using System.Linq;
using BLIFtoEDIF_Converter.Logic.InitCalculator;
using BLIFtoEDIF_Converter.Logic.Parser.Blif;
using BLIFtoEDIF_Converter.Model.Blif.Function;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class InitFunctionTest
	{
		[TestMethod]
		public void TestMethod1()
		{
			LogicFunction logicFunction = BlifParser.FromStringDef(
				new string[]
				{
					"01- 1",
					"10- 1",
					"1-1 1",
					"-01 1"
				});
			Port[] ports = Helpers.FromFuncDefStr("a s f f");
			Function function = new Function(ports, logicFunction);
			var initRes = function.CalculateInit();
			string res = initRes.ToString();
			Assert.AreEqual(res, "BE");
		}

		[TestMethod]
		public void TestMethod2()
		{
			LogicFunction logicFunction = BlifParser.FromStringDef(
				new string[]
				{
					"11- 1",
					"00- 1",
					"1-1 1",
					"-11 1"
				});
			Port[] ports = Helpers.FromFuncDefStr("o r g g");
			Function function = new Function(ports, logicFunction);
			var initRes = function.CalculateInit();
			string res = initRes.ToString();
			Assert.AreEqual(res, "EB");
		}

		[TestMethod]
		public void TestMethod3()
		{
			LogicFunction logicFunction = BlifParser.FromStringDef(
				new string[]
				{
					"01-- 1",
					"0-1- 1",
					"-11- 1",
					"0--1 1",
					"-1-1 1"
				});
			Port[] ports = Helpers.FromFuncDefStr("o d e h h");
			Function function = new Function(ports, logicFunction);
			var initRes = function.CalculateInit();
			string res = initRes.ToString();
			Assert.AreEqual(res, "E0FE");
		}

		[TestMethod]
		public void TestMethod4()
		{
			LogicFunction logicFunction = BlifParser.FromStringDef(
				new string[]
				{
					"00-- 1",
					"0-0- 1",
					"-00- 1",
					"0--1 1",
					"-0-1 1",
					"--01 1"
				});
			Port[] ports = Helpers.FromFuncDefStr("a b c o o");
			Function function = new Function(ports, logicFunction);
			var initRes = function.CalculateInit();
			string res = initRes.ToString();
			Assert.AreEqual(res, "2BBF");
		}

		[TestMethod]
		public void TestMethod5()
		{
			LogicFunction logicFunction = BlifParser.FromStringDef(
				new string[]
				{
					"01- 1",
					"10- 1",
					"0-1 1",
					"-11 1"
				});
			Port[] ports = Helpers.FromFuncDefStr("d e r r");
			Function function = new Function(ports, logicFunction);
			var initRes = function.CalculateInit();
			string res = initRes.ToString();
			Assert.AreEqual(res, "BE");
		}

		[TestMethod]
		public void TestMethod6()
		{
			LogicFunction logicFunction = BlifParser.FromStringDef(
				new string[]
				{
					"01- 1",
					"10- 1",
					"1-1 1",
					"-01 1"
				});
			Port[] ports = Helpers.FromFuncDefStr("b c s s");
			Function function = new Function(ports, logicFunction);
			var initRes = function.CalculateInit();
			string res = initRes.ToString();
			Assert.AreEqual(res, "BE");
		}

		[TestMethod]
		public void TestMethod888()
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
			var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			List<Function> funcs = BlifParser.GetFunctions(srcLines);

			List<InitFuncValue> initValues = funcs.Select(f => f.CalculateInit()).ToList();

			List<string> stringResults = initValues.Select(iv => iv.ToString()).ToList();

			string result = string.Join(Environment.NewLine, stringResults);
			
			Assert.AreEqual(@"BE
EB
E0FE
2BBF
BE
BE", result);
		}

		[TestMethod]
		public void TestMethodAdder()
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
.names x21 x20 x11 x10 [7]1 [7]1
1010- 1
1---1 1
--1-1 1
.names c1 c0 [5]1 [5]0 [8]1 [8]1
1010- 1
1---1 1
--1-1 1
.names x10 x11 [4]0 [4]1 z0 z0
1010- 1
0101- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.names [7]0 [7]1 [8]0 [8]1 C0 C0
1010- 1
1---1 1
--1-1 1
.names c1 c0 x21 x20 [4]0 [4]0
0101- 1
1010- 1
1---1 1
-1—-1 1
--1-1 1
---11 1
.names x21 x20 x11 x10 [5]0 [5]0
0101- 1
-1—-1 1
---11 1
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
			var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			List<Function> funcs = BlifParser.GetFunctions(srcLines);

			List<InitFuncValue> initValues = funcs.Select(f => f.CalculateInit()).ToList();

			List<string> stringResults = initValues.Select(iv => iv.ToString()).ToList();

			string result = string.Join(Environment.NewLine, stringResults);

			Assert.AreEqual(@"AAAEBAA8
AAAEBEA8
AAAEBAA8
AABEBAA8
AABAA0A0
AABAA0A0
AABAAEA8
AABAA0A0
AABAAEA8
AA88AE88
AAAEBEA8
AAAEBEA8", result);
		}
	}
}
