using System;
using BLIFtoEDIF_Converter.Logic;
using BLIFtoEDIF_Converter.Logic.Model.Blif;
using BLIFtoEDIF_Converter.Logic.Model.Blif.Function;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl;
using BLIFtoEDIF_Converter.Parser.Blif;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class BlifParserTest
	{
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
			var srcLines = blifSrc.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
			Blif blif = BlifParser.GetBlif(srcLines);

			Assert.IsNotNull(blif);

			Assert.IsNotNull(blif.Model);
			Assert.IsNotNull(blif.Model.Name);
			Assert.AreEqual("CM82As", blif.Model.Name);

			Assert.IsNotNull(blif.Inputs);
			Assert.AreEqual(5, blif.Inputs.InputList.Count);
			Assert.AreEqual("a", blif.Inputs.InputList[0].Name);
			Assert.AreEqual("b", blif.Inputs.InputList[1].Name);
			Assert.AreEqual("c", blif.Inputs.InputList[2].Name);
			Assert.AreEqual("d", blif.Inputs.InputList[3].Name);
			Assert.AreEqual("e", blif.Inputs.InputList[4].Name);

			Assert.IsNotNull(blif.Outputs);
			Assert.AreEqual(3, blif.Outputs.OutputList.Count);
			Assert.AreEqual("f", blif.Outputs.OutputList[0].Name);
			Assert.AreEqual("g", blif.Outputs.OutputList[1].Name);
			Assert.AreEqual("h", blif.Outputs.OutputList[2].Name);

			Assert.IsNotNull(blif.Functions);
			Assert.AreEqual(6, blif.Functions.Count);
			Assert.AreEqual(4, blif.Functions[0].Ports.Length);
			Assert.AreEqual("a", blif.Functions[0].Ports[0].Name);
			Assert.AreEqual(PortDirection.Input, blif.Functions[0].Ports[0].Direction);
			Assert.AreEqual("s", blif.Functions[0].Ports[1].Name);
			Assert.AreEqual(PortDirection.Input, blif.Functions[0].Ports[1].Direction);
			Assert.AreEqual("f", blif.Functions[0].Ports[2].Name);
			Assert.AreEqual(PortDirection.Input, blif.Functions[0].Ports[2].Direction);
			Assert.AreEqual("f", blif.Functions[0].OutputPort.Name);
			Assert.AreEqual(PortDirection.Output, blif.Functions[0].OutputPort.Direction);
		}
	}
}
