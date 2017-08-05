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
			BlifToEdifModelConverter.EdifConstants edifConstants = new BlifToEdifModelConverter.EdifConstants(blif.Model.Name);
			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			IEdif edif = blif.ToEdif(factory, edifConstants, out renameLog);

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
			BlifToEdifModelConverter.EdifConstants edifConstants = new BlifToEdifModelConverter.EdifConstants(blif.Model.Name);
			IEdif edif = blif.ToEdif(factory, edifConstants, out renameLog);

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


			instanceCounter++; // == 2
			Assert.AreEqual("LUT_80", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
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
			Assert.AreEqual("AAAEBEA8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 3
			Assert.AreEqual("LUT_81", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.IsNull(edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("LUT3", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Type);
			Assert.AreEqual("E8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 4
			Assert.AreEqual("LUT_z0", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
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

			instanceCounter++; // == 5
			Assert.AreEqual("LUT_z1", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
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


			instanceCounter++; // == 6
			Assert.AreEqual("LUT_50", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.IsNull(edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("LUT3", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Type);
			Assert.AreEqual("E8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);



			instanceCounter++; // == 7
			Assert.AreEqual("LUT_51", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
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
			Assert.AreEqual("AABEBAA8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 8
			Assert.AreEqual("LUT_70", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
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
			Assert.AreEqual("AAAEBEA8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 9
			Assert.AreEqual("LUT_71", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.IsNull(edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("LUT3", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Type);
			Assert.AreEqual("E8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 10
			Assert.AreEqual("LUT_C00", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.IsNull(edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("LUT3", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Type);
			Assert.AreEqual("E8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 11
			Assert.AreEqual("LUT_C00", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.IsNull(edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("LUT3", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Type);
			Assert.AreEqual("E8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 12
			Assert.AreEqual("LUT_C11", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
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
			Assert.AreEqual("AAAEBEA8", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[1].Value.Value);


			instanceCounter++; // == 13
			Assert.AreEqual("c0_IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("c0_IBUF_renamed_0", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 14
			Assert.AreEqual("c1_IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("c1_IBUF_renamed_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 15
			Assert.AreEqual("x10_IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("x10_IBUF_renamed_2", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 16
			Assert.AreEqual("x11_IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("x11_IBUF_renamed_3", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 17
			Assert.AreEqual("x20_IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("x20_IBUF_renamed_4", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 18
			Assert.AreEqual("x21_IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("x21_IBUF_renamed_5", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("IBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 19
			Assert.AreEqual("C00_OBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("C00_OBUF_renamed_6", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("PBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 20
			Assert.AreEqual("C11_OBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("C11_OBUF_renamed_7", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("PBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 21
			Assert.AreEqual("z0_OBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("z0_OBUF_renamed_8", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("PBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			instanceCounter++; // == 22
			Assert.AreEqual("z1_OBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].Name);
			Assert.AreEqual("z1_OBUF_renamed_9", edif.Library.Cell.View.Contents.Instances[instanceCounter].RenamedSynonym);
			Assert.AreEqual("view_1", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.Name);
			Assert.AreEqual("PBUF", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", edif.Library.Cell.View.Contents.Instances[instanceCounter].ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties.Count);
			Assert.AreEqual("Xilinx", edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Type);
			Assert.AreEqual(true, edif.Library.Cell.View.Contents.Instances[instanceCounter].Properties[0].Value.Value);


			Assert.AreEqual(28, edif.Library.Cell.View.Contents.Nets.Count);


			int netCounter = 0; // == 0
			Assert.AreEqual("c0_IBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I3","LUT_40"),
					new Tuple<string, string>("I3","LUT_41"),
					new Tuple<string, string>("I3","LUT_80"),
					new Tuple<string, string>("O","c0_IBUF_renamed_0")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 1
			Assert.AreEqual("c1_IBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 5;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I4","LUT_40"),
					new Tuple<string, string>("I4","LUT_41"),
					new Tuple<string, string>("I4","LUT_80"),
					new Tuple<string, string>("I2","LUT_81"),
					new Tuple<string, string>("O","c1_IBUF_renamed_1")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 2
			Assert.AreEqual("x10_IBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I4","LUT_z0"),
					new Tuple<string, string>("I4","LUT_z1"),
					new Tuple<string, string>("I1","LUT_50"),
					new Tuple<string, string>("I1","LUT_51"),
					new Tuple<string, string>("I1","LUT_70"),
					new Tuple<string, string>("O","x10_IBUF_renamed_2")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 3
			Assert.AreEqual("x11_IBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I3","LUT_z0"),
					new Tuple<string, string>("I3","LUT_z1"),
					new Tuple<string, string>("I2","LUT_51"),
					new Tuple<string, string>("I2","LUT_70"),
					new Tuple<string, string>("I1","LUT_71"),
					new Tuple<string, string>("O","x11_IBUF_renamed_3")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 4
			Assert.AreEqual("x20_IBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I1","LUT_40"),
					new Tuple<string, string>("I1","LUT_41"),
					new Tuple<string, string>("I2","LUT_50"),
					new Tuple<string, string>("I3","LUT_51"),
					new Tuple<string, string>("I3","LUT_70"),
					new Tuple<string, string>("O","x20_IBUF_renamed_4")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 5
			Assert.AreEqual("x21_IBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I2","LUT_40"),
					new Tuple<string, string>("I2","LUT_41"),
					new Tuple<string, string>("I4","LUT_51"),
					new Tuple<string, string>("I4","LUT_70"),
					new Tuple<string, string>("I2","LUT_71"),
					new Tuple<string, string>("O","x21_IBUF_renamed_5")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 6
			Assert.AreEqual("x40", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_40"),
					new Tuple<string, string>("I2","LUT_z0"),
					new Tuple<string, string>("I2","LUT_z1"),
					new Tuple<string, string>("O","LUT_40")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 7
			Assert.AreEqual("x41", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_41"),
					new Tuple<string, string>("I1","LUT_z0"),
					new Tuple<string, string>("I1","LUT_z1"),
					new Tuple<string, string>("O","LUT_41")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 8
			Assert.AreEqual("x80", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_80"),
					new Tuple<string, string>("I1","LUT_C00"),
					new Tuple<string, string>("I2","LUT_C11"),
					new Tuple<string, string>("O","LUT_80")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 9
			Assert.AreEqual("x81", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_81"),
					new Tuple<string, string>("I1","LUT_C11"),
					new Tuple<string, string>("O","LUT_81")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 10
			Assert.AreEqual("z0_OBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_z0"),
					new Tuple<string, string>("O","LUT_z0"),
					new Tuple<string, string>("I","z0_OBUF_renamed_8")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 11
			Assert.AreEqual("z1_OBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_z1"),
					new Tuple<string, string>("O","LUT_z1"),
					new Tuple<string, string>("I","z1_OBUF_renamed_9")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 12
			Assert.AreEqual("x50", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I1","LUT_80"),
					new Tuple<string, string>("I0","LUT_50"),
					new Tuple<string, string>("O","LUT_50")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 13
			Assert.AreEqual("x51", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I2","LUT_80"),
					new Tuple<string, string>("I1","LUT_81"),
					new Tuple<string, string>("I0","LUT_51"),
					new Tuple<string, string>("O","LUT_51")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 14
			Assert.AreEqual("x70", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_70"),
					new Tuple<string, string>("I2","LUT_C00"),
					new Tuple<string, string>("I4","LUT_C11"),
					new Tuple<string, string>("O","LUT_70")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 15
			Assert.AreEqual("x71", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_71"),
					new Tuple<string, string>("I3","LUT_C11"),
					new Tuple<string, string>("O","LUT_71")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 16
			Assert.AreEqual("C00_OBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_C00"),
					new Tuple<string, string>("O","LUT_C00"),
					new Tuple<string, string>("I","C00_OBUF_renamed_6")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 17
			Assert.AreEqual("C11_OBUF", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_C11"),
					new Tuple<string, string>("O","LUT_C11"),
					new Tuple<string, string>("I","C11_OBUF_renamed_7")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			netCounter++; // == 18
			Assert.AreEqual("c0", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("c0", null),
					new Tuple<string, string>("I","c0_IBUF_renamed_0")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 19
			Assert.AreEqual("c1", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("c1", null),
					new Tuple<string, string>("I","c1_IBUF_renamed_1")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 20
			Assert.AreEqual("x10", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x10", null),
					new Tuple<string, string>("I","x10_IBUF_renamed_2")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 21
			Assert.AreEqual("x11", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x11", null),
					new Tuple<string, string>("I","x11_IBUF_renamed_3")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 22
			Assert.AreEqual("x20", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x20", null),
					new Tuple<string, string>("I","x20_IBUF_renamed_4")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 23
			Assert.AreEqual("x21", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x21", null),
					new Tuple<string, string>("I","x21_IBUF_renamed_5")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 24
			Assert.AreEqual("C00", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("C00", null),
					new Tuple<string, string>("I","C00_OBUF_renamed_6")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 25
			Assert.AreEqual("C11", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("C11", null),
					new Tuple<string, string>("I","C11_OBUF_renamed_7")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 26
			Assert.AreEqual("z0", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("z0", null),
					new Tuple<string, string>("I","z0_OBUF_renamed_8")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}

			netCounter++; // == 26
			Assert.AreEqual("z1", edif.Library.Cell.View.Contents.Nets[netCounter].Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("z1", null),
					new Tuple<string, string>("I","z1_OBUF_renamed_9")
				};
				for (int i = 0; i < netPortRefCount; i++)
				{
					Assert.AreEqual(portRefNameToInstanceRef[i].Item1, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].Name);
					Assert.AreEqual(portRefNameToInstanceRef[i].Item2, edif.Library.Cell.View.Contents.Nets[netCounter].Joined[i].InstanceRef.ReferedInstanceName);
				}
			}


			Assert.AreEqual("adder_as_main", edif.Design.Name);
			Assert.AreEqual(1, edif.Design.CellRefs.Count);
			Assert.AreEqual("adder_as_main", edif.Design.CellRefs[0].Name);
			Assert.AreEqual("adder_as_main_lib", edif.Design.CellRefs[0].LibraryRef.Name);
			Assert.AreEqual(1, edif.Design.Properties.Count);
			Assert.AreEqual("Xilinx", edif.Design.Properties[0].Owner);
			Assert.AreEqual(PropertyValueType.String, edif.Design.Properties[0].Value.Type);
			Assert.AreEqual("xc6slx4-3-tqg144", edif.Design.Properties[0].Value.Value);
			Assert.AreEqual(PropertyType.PART, edif.Design.Properties[0].PropertyType);


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
