using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BLIFtoEDIF_Converter.Logic;
using BLIFtoEDIF_Converter.Logic.Model.Blif;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
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
		public void TestOnjectModelCompareAdderAs()
		{
			string blifSrc = GetEmbeddedResouceSrc("Tests.DataFiles.AdderAs.adder-as.blif");

			var srcLines = blifSrc.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
			Blif blif = BlifParser.GetBlif(srcLines);

			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			string renameLog;
			BlifToEdifModelConverter.EdifConstants edifConstants = new BlifToEdifModelConverter.EdifConstants(blif.Model.Name);
			IEdif edif = blif.ToEdif(factory, edifConstants, out renameLog);

			Assert.IsNotNull(edif);

			Assert.AreEqual("adder_as", edif.Name);

			Assert.AreEqual(2, edif.Version.MajorVersion);
			Assert.AreEqual(0, edif.Version.MidVersion);
			Assert.AreEqual(0, edif.Version.MinorVersion);

			Assert.AreEqual(0, edif.Level.Level);

			Assert.AreEqual(0, edif.KeywordMap.KeywordLevel);


			Assert.IsTrue(edif.Status.Written.Timestamp > DateTime.Now.AddMinutes(-1)); //TODO: is ok??
			Assert.IsTrue(edif.Status.Written.Timestamp < DateTime.Now.AddMinutes(1)); //TODO: is ok??
			Assert.AreEqual(1, edif.Status.Written.Comments.Count);//TODO: is ok??
			Assert.AreEqual("Do we need it in converter?", edif.Status.Written.Comments[0].Text);

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




			Assert.AreEqual("adder_as_lib", edif.Library.Name);
			Assert.AreEqual(0, edif.Library.Level.Level);
			Assert.AreEqual("numberDefinition", edif.Library.Technology.Name);
			Assert.AreEqual("adder_as", edif.Library.Cell.Name);
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
					new Tuple<string, PortDirection>("C0_R0", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("C1_R1", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("z0", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("z1", PortDirection.OUTPUT)
				};

				for (int i = 0; i < portCount; i++)
				{
					IPort port = edif.Library.Cell.View.Interface.Ports[i];
					Tuple<string, PortDirection> etalonPort = portEtalons.FirstOrDefault(
						et => et.Item1.Equals(port.Name, StringComparison.InvariantCulture));
					Assert.IsNotNull(etalonPort, $"Can not find etalon port with name '{port.Name}'");
					Assert.AreEqual(etalonPort.Item1, edif.Library.Cell.View.Interface.Ports[i].Name);
					Assert.AreEqual(etalonPort.Item2, edif.Library.Cell.View.Interface.Ports[i].Direction);
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
						(PropertyType.TYPE, PropertyValueType.String, "adder_as", "Xilinx"),
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
						(PropertyType.NLW_MACRO_ALIAS, PropertyValueType.String, "adder_as_adder_as", "Xilinx")
				};

				for (int i = 0; i < propertiesCount; i++)
				{
					IProperty property = edif.Library.Cell.View.Interface.Properties[i];
					Tuple<PropertyType, PropertyValueType, object, string> etalonProperty = propertiesEtalons.FirstOrDefault(
						et => et.Item1 == property.PropertyType && et.Item2 == property.Value.Type
						&& et.Item3.Equals(property.Value.Value) && et.Item4.Equals(property.Owner));
					Assert.IsNotNull(etalonProperty, $"Can not find etalon propery for '{property}'");
					Assert.AreEqual(etalonProperty.Item1, edif.Library.Cell.View.Interface.Properties[i].PropertyType);
					Assert.AreEqual(etalonProperty.Item2, edif.Library.Cell.View.Interface.Properties[i].Value.Type);
					Assert.AreEqual(etalonProperty.Item3, edif.Library.Cell.View.Interface.Properties[i].Value.Value);
					Assert.AreEqual(etalonProperty.Item4, edif.Library.Cell.View.Interface.Properties[i].Owner);
				}
			}

			Assert.AreEqual(22, edif.Library.Cell.View.Contents.Instances.Count);
			
			string instanceName = "LUT_40";
			IInstance instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABAAEA8", instance.Properties[1].Value.Value);


			// == 1
			instanceName = "LUT_41";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_41", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBAA8", instance.Properties[1].Value.Value);


			// == 2
			instanceName = "LUT_80";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_80", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBEA8", instance.Properties[1].Value.Value);


			// == 3
			instanceName = "LUT_81";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_81", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT3", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("E8", instance.Properties[1].Value.Value);


			// == 4
			instanceName = "LUT_z0";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_z0", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABAAEA8", instance.Properties[1].Value.Value);

			// == 5
			instanceName = "LUT_z1";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_z1", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBAA8", instance.Properties[1].Value.Value);


			// == 6
			instanceName = "LUT_50";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_50", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT3", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("E8", instance.Properties[1].Value.Value);



			// == 7
			instanceName = "LUT_51";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_51", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABEBAA8", instance.Properties[1].Value.Value);


			// == 8
			instanceName = "LUT_70";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_70", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBEA8", instance.Properties[1].Value.Value);


			// == 9
			instanceName = "LUT_71";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_71", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT3", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("E8", instance.Properties[1].Value.Value);


			// == 10
			instanceName = "LUT_C0_R0";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT3", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("E8", instance.Properties[1].Value.Value);
			
			
			instanceName = "LUT_C1_R1";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBEA8", instance.Properties[1].Value.Value);

			
			instanceName = "c0_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("c0_IBUF_renamed_0", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			
			instanceName = "c1_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("c1_IBUF_renamed_1", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			
			instanceName = "x10_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x10_IBUF_renamed_4", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			
			instanceName = "x11_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x11_IBUF_renamed_5", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);
			

			instanceName = "x20_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x20_IBUF_renamed_2", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			
			instanceName = "x21_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x21_IBUF_renamed_3", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			
			instanceName = "C0_R0_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("C0_R0_OBUF_renamed_8", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			
			instanceName = "C1_R1_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("C1_R1_OBUF_renamed_9", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "z0_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("z0_OBUF", instance.Name);
			Assert.AreEqual("z0_OBUF_renamed_6", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			
			instanceName = "z1_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("z1_OBUF_renamed_7", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			Assert.AreEqual(28, edif.Library.Cell.View.Contents.Nets.Count);


			string netName = "c0_IBUF";
			INet netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I3","LUT_40"),
					new Tuple<string, string>("I3","LUT_41"),
					new Tuple<string, string>("I3","LUT_80"),
					new Tuple<string, string>("O","c0_IBUF_renamed_0")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			
			netName = "c1_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
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
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x10_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I4","LUT_z0"),
					new Tuple<string, string>("I4","LUT_z1"),
					new Tuple<string, string>("I1","LUT_50"),
					new Tuple<string, string>("I1","LUT_51"),
					new Tuple<string, string>("I1","LUT_70"),
					new Tuple<string, string>("O","x10_IBUF_renamed_4")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}
			
			netName = "x11_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I3","LUT_z0"),
					new Tuple<string, string>("I3","LUT_z1"),
					new Tuple<string, string>("I2","LUT_51"),
					new Tuple<string, string>("I2","LUT_70"),
					new Tuple<string, string>("I1","LUT_71"),
					new Tuple<string, string>("O","x11_IBUF_renamed_5")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x20_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x20_IBUF", netFinded.Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I1","LUT_40"),
					new Tuple<string, string>("I1","LUT_41"),
					new Tuple<string, string>("I2","LUT_50"),
					new Tuple<string, string>("I3","LUT_51"),
					new Tuple<string, string>("I3","LUT_70"),
					new Tuple<string, string>("O","x20_IBUF_renamed_2")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x21_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x21_IBUF", netFinded.Name);
			{
				const int netPortRefCount = 6;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I2","LUT_40"),
					new Tuple<string, string>("I2","LUT_41"),
					new Tuple<string, string>("I4","LUT_51"),
					new Tuple<string, string>("I4","LUT_70"),
					new Tuple<string, string>("I2","LUT_71"),
					new Tuple<string, string>("O","x21_IBUF_renamed_3")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x40";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x40", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_40"),
					new Tuple<string, string>("I2","LUT_z0"),
					new Tuple<string, string>("I2","LUT_z1"),
					new Tuple<string, string>("O","LUT_40")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x41";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x41", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_41"),
					new Tuple<string, string>("I1","LUT_z0"),
					new Tuple<string, string>("I1","LUT_z1"),
					new Tuple<string, string>("O","LUT_41")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x80";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x80", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_80"),
					new Tuple<string, string>("I1","LUT_C0_R0"),
					new Tuple<string, string>("I2","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_80")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}'");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x81";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x81", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_81"),
					new Tuple<string, string>("I1","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_81")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "z0_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z0_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_z0"),
					new Tuple<string, string>("O","LUT_z0"),
					new Tuple<string, string>("I","z0_OBUF_renamed_6")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "z1_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z1_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_z1"),
					new Tuple<string, string>("O","LUT_z1"),
					new Tuple<string, string>("I","z1_OBUF_renamed_7")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x50";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x50", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I1","LUT_80"),
					new Tuple<string, string>("I0","LUT_50"),
					new Tuple<string, string>("O","LUT_50")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x51";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x51", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I2","LUT_80"),
					new Tuple<string, string>("I1","LUT_81"),
					new Tuple<string, string>("I0","LUT_51"),
					new Tuple<string, string>("O","LUT_51")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x70";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x70", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_70"),
					new Tuple<string, string>("I2","LUT_C0_R0"),
					new Tuple<string, string>("I4","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_70")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x71";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x71", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_71"),
					new Tuple<string, string>("I3","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_71")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "C0_R0_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C0_R0_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_C0_R0"),
					new Tuple<string, string>("O","LUT_C0_R0"),
					new Tuple<string, string>("I","C0_R0_OBUF_renamed_8")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "C1_R1_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C1_R1_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_C1_R1"),
					new Tuple<string, string>("I","C1_R1_OBUF_renamed_9")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "c0";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("c0", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("c0", null),
					new Tuple<string, string>("I","c0_IBUF_renamed_0")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "c1";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("c1", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("c1", null),
					new Tuple<string, string>("I","c1_IBUF_renamed_1")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x10";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x10", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x10", null),
					new Tuple<string, string>("I","x10_IBUF_renamed_4")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x11";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x11", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x11", null),
					new Tuple<string, string>("I","x11_IBUF_renamed_5")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x20";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x20", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x20", null),
					new Tuple<string, string>("I","x20_IBUF_renamed_2")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x21";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x21", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x21", null),
					new Tuple<string, string>("I","x21_IBUF_renamed_3")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "C0_R0";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C0_R0", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("C0_R0", null),
					new Tuple<string, string>("O","C0_R0_OBUF_renamed_8")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "C1_R1";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C1_R1", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("C1_R1", null),
					new Tuple<string, string>("O","C1_R1_OBUF_renamed_9")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "z0";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z0", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("z0", null),
					new Tuple<string, string>("O","z0_OBUF_renamed_6")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "z1";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z1", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("z1", null),
					new Tuple<string, string>("O","z1_OBUF_renamed_7")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			Assert.AreEqual("adder_as", edif.Design.Name);
			Assert.AreEqual(1, edif.Design.CellRefs.Count);
			Assert.AreEqual("adder_as", edif.Design.CellRefs[0].Name);
			Assert.AreEqual("adder_as_lib", edif.Design.CellRefs[0].LibraryRef.Name);
			Assert.AreEqual(1, edif.Design.Properties.Count);
			Assert.AreEqual("Xilinx", edif.Design.Properties[0].Owner);
			Assert.AreEqual(PropertyValueType.String, edif.Design.Properties[0].Value.Type);
			Assert.AreEqual("xc6slx4-3-tqg144", edif.Design.Properties[0].Value.Value);
			Assert.AreEqual(PropertyType.PART, edif.Design.Properties[0].PropertyType);
		}

		[TestMethod]
		public void TestOnjectModelCompareAdderAs2()
		{
			string blifSrc = GetEmbeddedResouceSrc("Tests.DataFiles.AdderAs2.adder-as.blif");

			var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			Blif blif = BlifParser.GetBlif(srcLines);

			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			string renameLog;
			string edifModelName = "adder_as_main";
			BlifToEdifModelConverter.EdifConstants edifConstants = new BlifToEdifModelConverter.EdifConstants(edifModelName);
			IEdif edif = blif.ToEdif(factory, edifConstants, out renameLog);

			Assert.IsNotNull(edif);

			Assert.AreEqual(edifModelName, edif.Name);

			Assert.AreEqual(2, edif.Version.MajorVersion);
			Assert.AreEqual(0, edif.Version.MidVersion);
			Assert.AreEqual(0, edif.Version.MinorVersion);

			Assert.AreEqual(0, edif.Level.Level);

			Assert.AreEqual(0, edif.KeywordMap.KeywordLevel);


			Assert.IsTrue(edif.Status.Written.Timestamp > DateTime.Now.AddMinutes(-1)); //TODO: is ok??
			Assert.IsTrue(edif.Status.Written.Timestamp < DateTime.Now.AddMinutes(1)); //TODO: is ok??
			Assert.AreEqual(1, edif.Status.Written.Comments.Count);//TODO: is ok??
			Assert.AreEqual("Do we need it in converter?", edif.Status.Written.Comments[0].Text);

			Assert.AreEqual("UNISIMS", edif.External.Name);
			Assert.AreEqual(0, edif.External.EdifLevel.Level);
			Assert.AreEqual("numberDefinition", edif.External.Technology.Name);
			Assert.AreEqual(3, edif.External.Cells.Count);

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
				Assert.AreEqual("I" + i, edif.External.Cells[cellCounter].View.Interface.Ports[i].Name);
				Assert.AreEqual(PortDirection.INPUT, edif.External.Cells[cellCounter].View.Interface.Ports[i].Direction);
			}
			Assert.AreEqual("O", edif.External.Cells[cellCounter].View.Interface.Ports[5].Name);
			Assert.AreEqual(PortDirection.OUTPUT, edif.External.Cells[cellCounter].View.Interface.Ports[5].Direction);
			
			cellCounter++; // == 1
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

			cellCounter++; // == 2
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
					new Tuple<string, PortDirection>("C0_R0", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("C1_R1", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("z0", PortDirection.OUTPUT),
					new Tuple<string, PortDirection>("z1", PortDirection.OUTPUT)
				};

				for (int i = 0; i < portCount; i++)
				{
					IPort port = edif.Library.Cell.View.Interface.Ports[i];
					Tuple<string, PortDirection> etalonPort = portEtalons.FirstOrDefault(
						et => et.Item1.Equals(port.Name, StringComparison.InvariantCulture));
					Assert.IsNotNull(etalonPort, $"Can not find etalon port with name '{port.Name}'");
					Assert.AreEqual(etalonPort.Item1, edif.Library.Cell.View.Interface.Ports[i].Name);
					Assert.AreEqual(etalonPort.Item2, edif.Library.Cell.View.Interface.Ports[i].Direction);
				}
			}
			Assert.AreEqual("xc6slx4-3-tqg144", edif.Library.Cell.View.Interface.Designator);
			{
				const int propertiesCount = 7;
				Assert.AreEqual(propertiesCount, edif.Library.Cell.View.Interface.Properties.Count);
				Tuple<PropertyType, PropertyValueType, object, string>[] propertiesEtalons =
					new Tuple<PropertyType, PropertyValueType, object, string>[propertiesCount]
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
					IProperty property = edif.Library.Cell.View.Interface.Properties[i];
					Tuple<PropertyType, PropertyValueType, object, string> etalonProperty = propertiesEtalons.FirstOrDefault(
						et => et.Item1 == property.PropertyType && et.Item2 == property.Value.Type
							&& et.Item3.Equals(property.Value.Value) && et.Item4.Equals(property.Owner));
					Assert.IsNotNull(etalonProperty, $"Can not find etalon propery for '{property}'");
					Assert.AreEqual(etalonProperty.Item1, edif.Library.Cell.View.Interface.Properties[i].PropertyType);
					Assert.AreEqual(etalonProperty.Item2, edif.Library.Cell.View.Interface.Properties[i].Value.Type);
					Assert.AreEqual(etalonProperty.Item3, edif.Library.Cell.View.Interface.Properties[i].Value.Value);
					Assert.AreEqual(etalonProperty.Item4, edif.Library.Cell.View.Interface.Properties[i].Owner);
				}
			}

			Assert.AreEqual(22, edif.Library.Cell.View.Contents.Instances.Count);

			string instanceName = "LUT_40";
			IInstance instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABAAEA8", instance.Properties[1].Value.Value);


			// == 1
			instanceName = "LUT_41";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_41", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBAA8", instance.Properties[1].Value.Value);


			// == 2
			instanceName = "LUT_80";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_80", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBEA8", instance.Properties[1].Value.Value);


			// == 3
			instanceName = "LUT_81";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_81", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABAA0A0", instance.Properties[1].Value.Value);


			// == 4
			instanceName = "LUT_z0";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_z0", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABAAEA8", instance.Properties[1].Value.Value);

			// == 5
			instanceName = "LUT_z1";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_z1", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBAA8", instance.Properties[1].Value.Value);


			// == 6
			instanceName = "LUT_50";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_50", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AA88AE88", instance.Properties[1].Value.Value);



			// == 7
			instanceName = "LUT_51";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_51", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABEBAA8", instance.Properties[1].Value.Value);


			// == 8
			instanceName = "LUT_70";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_70", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBEA8", instance.Properties[1].Value.Value);


			// == 9
			instanceName = "LUT_71";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual("LUT_71", instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABAA0A0", instance.Properties[1].Value.Value);


			// == 10
			instanceName = "LUT_C0_R0";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AABAA0A0", instance.Properties[1].Value.Value);


			instanceName = "LUT_C1_R1";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.IsNull(instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("LUT5", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(2, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);

			Assert.AreEqual("Xilinx", instance.Properties[1].Owner);
			Assert.AreEqual(PropertyType.INIT, instance.Properties[1].PropertyType);
			Assert.AreEqual(PropertyValueType.String, instance.Properties[1].Value.Type);
			Assert.AreEqual("AAAEBEA8", instance.Properties[1].Value.Value);


			instanceName = "c0_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("c0_IBUF_renamed_0", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "c1_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("c1_IBUF_renamed_1", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "x10_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x10_IBUF_renamed_4", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "x11_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x11_IBUF_renamed_5", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "x20_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x20_IBUF_renamed_2", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "x21_IBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("x21_IBUF_renamed_3", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("IBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "C0_R0_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("C0_R0_OBUF_renamed_8", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "C1_R1_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("C1_R1_OBUF_renamed_9", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "z0_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("z0_OBUF", instance.Name);
			Assert.AreEqual("z0_OBUF_renamed_6", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			instanceName = "z1_OBUF";
			instance = edif.Library.Cell.View.Contents.Instances.FirstOrDefault(
				i => instanceName.Equals(i.Name));
			Assert.IsNotNull(instance, $"Can not find instance with name '{instanceName}'");
			Assert.AreEqual(instanceName, instance.Name);
			Assert.AreEqual("z1_OBUF_renamed_7", instance.RenamedSynonym);
			Assert.AreEqual("view_1", instance.ViewRef.Name);
			Assert.AreEqual("OBUF", instance.ViewRef.CellRef.Name);
			Assert.AreEqual("UNISIMS", instance.ViewRef.CellRef.LibraryRef.Name);
			Assert.AreEqual(1, instance.Properties.Count);
			Assert.AreEqual("Xilinx", instance.Properties[0].Owner);
			Assert.AreEqual(PropertyType.XSTLIB, instance.Properties[0].PropertyType);
			Assert.AreEqual(PropertyValueType.Boolean, instance.Properties[0].Value.Type);
			Assert.AreEqual(true, instance.Properties[0].Value.Value);


			Assert.AreEqual(28, edif.Library.Cell.View.Contents.Nets.Count);


			string netName = "c0_IBUF";
			INet netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
			{
				const int netPortRefCount = 5;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I3","LUT_40"),
					new Tuple<string, string>("I3","LUT_41"),
					new Tuple<string, string>("I3","LUT_80"),
					new Tuple<string, string>("I3","LUT_81"),
					new Tuple<string, string>("O","c0_IBUF_renamed_0")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "c1_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
			{
				const int netPortRefCount = 5;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I4","LUT_40"),
					new Tuple<string, string>("I4","LUT_41"),
					new Tuple<string, string>("I4","LUT_80"),
					new Tuple<string, string>("I4","LUT_81"),
					new Tuple<string, string>("O","c1_IBUF_renamed_1")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x10_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
			{
				const int netPortRefCount = 7;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I4","LUT_z0"),
					new Tuple<string, string>("I4","LUT_z1"),
					new Tuple<string, string>("I1","LUT_50"),
					new Tuple<string, string>("I1","LUT_51"),
					new Tuple<string, string>("I1","LUT_70"),
					new Tuple<string, string>("I1","LUT_71"),
					new Tuple<string, string>("O","x10_IBUF_renamed_4")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x11_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual(netName, netFinded.Name);
			{
				const int netPortRefCount = 7;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I3","LUT_z0"),
					new Tuple<string, string>("I3","LUT_z1"),
					new Tuple<string, string>("I2","LUT_50"),
					new Tuple<string, string>("I2","LUT_51"),
					new Tuple<string, string>("I2","LUT_70"),
					new Tuple<string, string>("I2","LUT_71"),
					new Tuple<string, string>("O","x11_IBUF_renamed_5")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x20_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x20_IBUF", netFinded.Name);
			{
				const int netPortRefCount = 7;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I1","LUT_40"),
					new Tuple<string, string>("I1","LUT_41"),
					new Tuple<string, string>("I3","LUT_50"),
					new Tuple<string, string>("I3","LUT_51"),
					new Tuple<string, string>("I3","LUT_70"),
					new Tuple<string, string>("I3","LUT_71"),
					new Tuple<string, string>("O","x20_IBUF_renamed_2")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x21_IBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x21_IBUF", netFinded.Name);
			{
				const int netPortRefCount = 7;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I2","LUT_40"),
					new Tuple<string, string>("I2","LUT_41"),
					new Tuple<string, string>("I4","LUT_50"),
					new Tuple<string, string>("I4","LUT_51"),
					new Tuple<string, string>("I4","LUT_70"),
					new Tuple<string, string>("I4","LUT_71"),
					new Tuple<string, string>("O","x21_IBUF_renamed_3")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x40";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x40", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_40"),
					new Tuple<string, string>("I2","LUT_z0"),
					new Tuple<string, string>("I2","LUT_z1"),
					new Tuple<string, string>("O","LUT_40")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x41";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x41", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_41"),
					new Tuple<string, string>("I1","LUT_z0"),
					new Tuple<string, string>("I1","LUT_z1"),
					new Tuple<string, string>("O","LUT_41")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x80";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x80", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_80"),
					new Tuple<string, string>("I2","LUT_C0_R0"),
					new Tuple<string, string>("I2","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_80")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}'");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x81";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x81", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_81"),
					new Tuple<string, string>("I1","LUT_C0_R0"),
					new Tuple<string, string>("I1","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_81")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "z0_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z0_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_z0"),
					new Tuple<string, string>("O","LUT_z0"),
					new Tuple<string, string>("I","z0_OBUF_renamed_6")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "z1_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z1_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{

					new Tuple<string, string>("I0","LUT_z1"),
					new Tuple<string, string>("O","LUT_z1"),
					new Tuple<string, string>("I","z1_OBUF_renamed_7")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "x50";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x50", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I1","LUT_80"),
					new Tuple<string, string>("I1","LUT_81"),
					new Tuple<string, string>("I0","LUT_50"),
					new Tuple<string, string>("O","LUT_50")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x51";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x51", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I2","LUT_80"),
					new Tuple<string, string>("I2","LUT_81"),
					new Tuple<string, string>("I0","LUT_51"),
					new Tuple<string, string>("O","LUT_51")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x70";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x70", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_70"),
					new Tuple<string, string>("I4","LUT_C0_R0"),
					new Tuple<string, string>("I4","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_70")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x71";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x71", netFinded.Name);
			{
				const int netPortRefCount = 4;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_71"),
					new Tuple<string, string>("I3","LUT_C0_R0"),
					new Tuple<string, string>("I3","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_71")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "C0_R0_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C0_R0_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_C0_R0"),
					new Tuple<string, string>("O","LUT_C0_R0"),
					new Tuple<string, string>("I","C0_R0_OBUF_renamed_8")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "C1_R1_OBUF";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C1_R1_OBUF", netFinded.Name);
			{
				const int netPortRefCount = 3;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("I0","LUT_C1_R1"),
					new Tuple<string, string>("O","LUT_C1_R1"),
					new Tuple<string, string>("I","C1_R1_OBUF_renamed_9")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}


			netName = "c0";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("c0", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("c0", null),
					new Tuple<string, string>("I","c0_IBUF_renamed_0")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "c1";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("c1", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("c1", null),
					new Tuple<string, string>("I","c1_IBUF_renamed_1")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x10";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x10", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x10", null),
					new Tuple<string, string>("I","x10_IBUF_renamed_4")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x11";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x11", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x11", null),
					new Tuple<string, string>("I","x11_IBUF_renamed_5")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x20";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x20", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x20", null),
					new Tuple<string, string>("I","x20_IBUF_renamed_2")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "x21";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("x21", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("x21", null),
					new Tuple<string, string>("I","x21_IBUF_renamed_3")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "C0_R0";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C0_R0", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("C0_R0", null),
					new Tuple<string, string>("O","C0_R0_OBUF_renamed_8")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "C1_R1";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("C1_R1", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("C1_R1", null),
					new Tuple<string, string>("O","C1_R1_OBUF_renamed_9")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "z0";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z0", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("z0", null),
					new Tuple<string, string>("O","z0_OBUF_renamed_6")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
				}
			}

			netName = "z1";
			netFinded = edif.Library.Cell.View.Contents.Nets.FirstOrDefault(n => netName.Equals(n.Name));
			Assert.IsNotNull(netFinded, $"Can not find net with name '{netName}'");
			Assert.AreEqual("z1", netFinded.Name);
			{
				const int netPortRefCount = 2;
				Tuple<string, string>[] portRefNameToInstanceRef = new Tuple<string, string>[netPortRefCount]
				{
					new Tuple<string, string>("z1", null),
					new Tuple<string, string>("O","z1_OBUF_renamed_7")
				};
				Assert.AreEqual(netPortRefCount, netFinded.Joined.Count);
				for (int i = 0; i < netPortRefCount; i++)
				{
					IPortRef portRef = netFinded.Joined[i];
					Tuple<string, string> portRefEtalon = portRefNameToInstanceRef.FirstOrDefault(
						pr => pr.Item1.Equals(portRef.Name) && string.Equals(pr.Item2, portRef.InstanceRef?.ReferedInstanceName));
					Assert.IsNotNull(portRefEtalon, $"Can not find etalon with name '{portRef.Name}' and ReferedInstanceName: '{portRef.InstanceRef?.ReferedInstanceName}");
					Assert.AreEqual(portRefEtalon.Item1, netFinded.Joined[i].Name);
					Assert.AreEqual(portRefEtalon.Item2, netFinded.Joined[i].InstanceRef?.ReferedInstanceName);
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
		}

		[TestMethod]
		public void TestSrcStringCompareAdderAs()
		{
			string blifSrc = GetEmbeddedResouceSrc("Tests.DataFiles.AdderAs.adder-as.blif");

			var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			Blif blif = BlifParser.GetBlif(srcLines);

			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			string renameLog;
			BlifToEdifModelConverter.EdifConstants edifConstants = new BlifToEdifModelConverter.EdifConstants("adder_as_main");
			IEdif edif = blif.ToEdif(factory, edifConstants, out renameLog);
			string edifSrc = edif.ToEdifText();
			Assert.IsNotNull(edifSrc);
			
			string edifSrcEtalon = GetEmbeddedResouceSrc("Tests.DataFiles.AdderAs.adder_as.edif");
			string notFormattedEdifEtalon = RemoveFormatting(edifSrcEtalon);
			string notFormattingResult = RemoveFormatting(edifSrc);
			Assert.AreEqual(notFormattedEdifEtalon, notFormattingResult);
		}

		private static string RemoveFormatting(string src)
		{
			return src.Replace(" ", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty)
				.Replace("\n", string.Empty);
		}

		private static string GetEmbeddedResouceSrc(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream, Encoding.Default))
			{
				string result = reader.ReadToEnd();
				return result;
			}
		}
	}
}
