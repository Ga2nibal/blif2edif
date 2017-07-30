using System;
using BLIFtoEDIF_Converter.Logic;
using BLIFtoEDIF_Converter.Logic.Model.Blif;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;
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

			Assert.AreEqual("adder_as_main", edif.Name);

			Assert.AreEqual(2, edif.Version.MajorVersion);
			Assert.AreEqual(0, edif.Version.MidVersion);
			Assert.AreEqual(0, edif.Version.MinorVersion);

			Assert.AreEqual(0, edif.Level.Level);

			Assert.AreEqual(0, edif.KeywordMap.KeywordLevel);


			Assert.IsTrue(edif.Status.Written.Timestamp > DateTime.Now.AddMinutes(-1)); //TODO: is ok??
			Assert.IsTrue(edif.Status.Written.Timestamp < DateTime.Now.AddMinutes(1)); //TODO: is ok??
			Assert.AreEqual(4, edif.Status.Written.Comments.Count);//TODO: is ok??
			Assert.AreEqual("This EDIF netlist is to be used within supported synthesis tools", edif.Status.Written.Comments[0].Text);
			Assert.AreEqual("for determining resource/timing estimates of the design component", edif.Status.Written.Comments[1].Text);
			Assert.AreEqual("represented by this netlist.", edif.Status.Written.Comments[2].Text);
			Assert.AreEqual("Command line: -w adder_as_main.ngc ", edif.Status.Written.Comments[3].Text);

			Assert.AreEqual("UNISIMS", edif.External.Name);
			Assert.AreEqual(0, edif.External.EdifLevel.Level);
			Assert.AreEqual("numberDefinition", edif.External.Technology.Name);
			Assert.AreEqual(4, edif.External.Cells.Count);

			int cellCounter = 0;
			Assert.AreEqual("LUT5", edif.External.Cells[cellCounter].Name);
			Assert.AreEqual(CellType.GENERIC, edif.External.Cells[cellCounter].CellType);
			Assert.AreEqual("view_1", edif.External.Cells[cellCounter].View.Name);
			Assert.AreEqual(ViewType.NETLIST, edif.External.Cells[cellCounter].View.ViewType);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Contents);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Designator);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Properties);
			Assert.AreEqual(6, edif.External.Cells[cellCounter].View.Interface.Ports.Count);
			for (int i = 0; i < 5; i++)
			{
				Assert.AreEqual("I"+i, edif.External.Cells[cellCounter].View.Interface.Ports[i].Name);
				Assert.AreEqual(PortDirection.INPUT, edif.External.Cells[cellCounter].View.Interface.Ports[i].Direction);
			}
			Assert.AreEqual("O", edif.External.Cells[cellCounter].View.Interface.Ports[5].Name);
			Assert.AreEqual(PortDirection.OUTPUT, edif.External.Cells[cellCounter].View.Interface.Ports[5].Direction);

			cellCounter++; // == 1
			Assert.AreEqual("LUT3", edif.External.Cells[cellCounter].Name);
			Assert.AreEqual(CellType.GENERIC, edif.External.Cells[cellCounter].CellType);
			Assert.AreEqual("view_1", edif.External.Cells[cellCounter].View.Name);
			Assert.AreEqual(ViewType.NETLIST, edif.External.Cells[cellCounter].View.ViewType);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Contents);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Designator);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Properties);
			Assert.AreEqual(4, edif.External.Cells[cellCounter].View.Interface.Ports.Count);
			for (int i = 0; i < 3; i++)
			{
				Assert.AreEqual("I" + i, edif.External.Cells[cellCounter].View.Interface.Ports[i].Name);
				Assert.AreEqual(PortDirection.INPUT, edif.External.Cells[cellCounter].View.Interface.Ports[i].Direction);
			}
			Assert.AreEqual("O", edif.External.Cells[cellCounter].View.Interface.Ports[3].Name);
			Assert.AreEqual(PortDirection.OUTPUT, edif.External.Cells[cellCounter].View.Interface.Ports[3].Direction);

			cellCounter++; // == 2
			Assert.AreEqual("IBUF", edif.External.Cells[cellCounter].Name);
			Assert.AreEqual(CellType.GENERIC, edif.External.Cells[cellCounter].CellType);
			Assert.AreEqual("view_1", edif.External.Cells[cellCounter].View.Name);
			Assert.AreEqual(ViewType.NETLIST, edif.External.Cells[cellCounter].View.ViewType);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Contents);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Designator);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Properties);
			Assert.AreEqual(2, edif.External.Cells[cellCounter].View.Interface.Ports.Count);
			Assert.AreEqual("I", edif.External.Cells[cellCounter].View.Interface.Ports[0].Name);
			Assert.AreEqual(PortDirection.INPUT, edif.External.Cells[cellCounter].View.Interface.Ports[0].Direction);
			Assert.AreEqual("O", edif.External.Cells[cellCounter].View.Interface.Ports[1].Name);
			Assert.AreEqual(PortDirection.OUTPUT, edif.External.Cells[cellCounter].View.Interface.Ports[1].Direction);

			cellCounter++; // == 3
			Assert.AreEqual("OBUF", edif.External.Cells[cellCounter].Name);
			Assert.AreEqual(CellType.GENERIC, edif.External.Cells[cellCounter].CellType);
			Assert.AreEqual("view_1", edif.External.Cells[cellCounter].View.Name);
			Assert.AreEqual(ViewType.NETLIST, edif.External.Cells[cellCounter].View.ViewType);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Contents);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Designator);
			Assert.IsNull(edif.External.Cells[cellCounter].View.Interface.Properties);
			Assert.AreEqual(2, edif.External.Cells[cellCounter].View.Interface.Ports.Count);
			Assert.AreEqual("I", edif.External.Cells[cellCounter].View.Interface.Ports[0].Name);
			Assert.AreEqual(PortDirection.INPUT, edif.External.Cells[cellCounter].View.Interface.Ports[0].Direction);
			Assert.AreEqual("O", edif.External.Cells[cellCounter].View.Interface.Ports[1].Name);
			Assert.AreEqual(PortDirection.OUTPUT, edif.External.Cells[cellCounter].View.Interface.Ports[1].Direction);




			Assert.AreEqual("adder_as_main_lib", edif.Library.Name);
			Assert.AreEqual(0, edif.Library.Level.Level);
			Assert.AreEqual("numberDefinition", edif.Library.Technology.Name);
			Assert.AreEqual("adder_as_main", edif.Library.Cell.Name);
			Assert.AreEqual(CellType.GENERIC, edif.Library.Cell.CellType);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Name);
			Assert.AreEqual(ViewType.NETLIST, edif.Library.Cell.View.ViewType);
			{
				const int portCount = 10;
				Assert.AreEqual(portCount, edif.Library.Cell.View.Interface.Ports.Count);
				Tuple<string, PortDirection>[] portEtalons = new Tuple<string, PortDirection>[portCount]
				{
					new Tuple<string, PortDirection>("c0", PortDirection.INPUT),
					new Tuple<string, PortDirection>("c1", PortDirection.INPUT),
					new Tuple<string, PortDirection>("x10", PortDirection.INPUT),
					new Tuple<string, PortDirection>("x11", PortDirection.INPUT),
					new Tuple<string, PortDirection>("x20", PortDirection.INPUT),
					new Tuple<string, PortDirection>("x21", PortDirection.INPUT),
					new Tuple<string, PortDirection>("C00", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("C11", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("z0", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("z1", PortDirection.OUTPUT)
				};

				for (int i = 0; i < portCount; i++)
				{
					Assert.AreEqual(portEtalons[i].Item1, edif.Library.Cell.View.Interface.Ports[i].Name);
					Assert.AreEqual(portEtalons[i].Item2, edif.Library.Cell.View.Interface.Ports[i].Direction);
				}
			}
			Assert.AreEqual("xc6slx4-3-tqg144", edif.Library.Cell.View.Interface.Designator);
			{
				const int propertiesCount = 7;
				Assert.AreEqual(propertiesCount, edif.Library.Cell.View.Interface.Properties.Count);
				Tuple<PropertyType, PropertyValueType, object, string>[] propertiesEtalons = 
					new Tuple < PropertyType, PropertyValueType, object, string>[propertiesCount]
				{
					new Tuple<PropertyType, PropertyValueType, object, string>
						(PropertyType.TYPE, PropertyValueType.String, "adder_as_main", "Xilinx"),
					new Tuple<PropertyType, PropertyValueType, object, string>
						(PropertyType.KEEP_HIERARCHY, PropertyValueType.String, "TRUE", "Xilinx"),
					new Tuple<PropertyType, PropertyValueType, object, string>
						(PropertyType.SHREG_MIN_SIZE, PropertyValueType.String, "2", "Xilinx"),
					new Tuple<PropertyType, PropertyValueType, object, string>
						(PropertyType.SHREG_EXTRACT_NGC, PropertyValueType.String, "YES", "Xilinx"),
					new Tuple<PropertyType, PropertyValueType, object, string>
						(PropertyType.NLW_UNIQUE_ID, PropertyValueType.Integer, 0, "Xilinx"),
					new Tuple<PropertyType, PropertyValueType, object, string>
						(PropertyType.NLW_MACRO_TAG, PropertyValueType.Integer, 0, "Xilinx"),
					new Tuple<PropertyType, PropertyValueType, object, string>
						(PropertyType.NLW_MACRO_ALIAS, PropertyValueType.String, "adder_as_main_adder_as_main", "Xilinx")
				};

				for (int i = 0; i < propertiesCount; i++)
				{
					Assert.AreEqual(propertiesEtalons[i].Item1, edif.Library.Cell.View.Interface.Properties[i].PropertyType);
					Assert.AreEqual(propertiesEtalons[i].Item2, edif.Library.Cell.View.Interface.Properties[i].Value.Type);
					Assert.AreEqual(propertiesEtalons[i].Item3, edif.Library.Cell.View.Interface.Properties[i].Value.Value);
					Assert.AreEqual(propertiesEtalons[i].Item4, edif.Library.Cell.View.Interface.Properties[i].Owner);
				}
			}

			Assert.AreEqual(22, edif.Library.Cell.View.Contents.Instances.Count);

			int instanceCounter = 0;
			Assert.AreEqual("LUT_40", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.IsNull(edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("LUT5", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Type);
			Assert.AreEqual("AABAAEA8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 1
			Assert.AreEqual("LUT_41", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.IsNull(edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("LUT5", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Type);
			Assert.AreEqual("AAAEBAA8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			//string edifSrc = edif.ToEdifText();
			//Assert.IsNotNull(edifSrc);
			//string formattedEdifSrc = SrcCodeFormatter.FormatEdifCode(edifSrc);
			//Assert.IsNotNull(renameLog);
			//Console.WriteLine("#" + renameLog);
			//Console.WriteLine();
			//Assert.IsNotNull(formattedEdifSrc);
			//Console.WriteLine(formattedEdifSrc);
		}
	}
}
