using System;
using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortDirection = BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port.PortDirection;

namespace Tests.ConvertTextViewElementsTests
{
	[TestClass]
	public abstract class ConvertTextViewElementsBaseTest
	{
		protected abstract ITextViewElementsFactory GetTextViewElementsFactory();

		[TestMethod]
		public void TestCellWithNullView()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			ICell cell = factory.CreateCell("test_name", CellType.GENERIC, null);

			string textView = cell.ToEdifText();
			Assert.AreEqual("(cell cm82a (cellType GENERIC))", textView);
		}

		//TODO:
		//[TestMethod]
		//public void TestCellWithNullViewAndName()
		//{
		//	ITextViewElementsFactory factory = GetTextViewElementsFactory();
		//	ICell cell = factory.CreateCell(null, CellType.GENERIC, null);

		//	string textView = cell.ToEdifText();
		//}

		[TestMethod]
		public void TestCell()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			IPort port0 = factory.CreatePort("I0", PortDirection.INPUT);
			IPort port1 = factory.CreatePort("I1", PortDirection.INPUT);
			var ports = new List<IPort>();
			ports.Add(port0);
			ports.Add(port1);
			IInterface @interface = factory.CreateInterface(ports, null, null);
			IView view = factory.CreateView("PRIM", ViewType.NETLIST, null, null);
			ICell cell = factory.CreateCell("LUT2", CellType.GENERIC, view);

			string textView = cell.ToEdifText();
			Assert.AreEqual(@"(cell LUT2 (cellType GENERIC) (view PRIM(viewType NETLIST) (interface (port I0(direction INPUT)) (port I1(direction INPUT)) (port O(direction OUTPUT)))))", textView);
		}

		[TestMethod]
		public void TestCellRef()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			ILibraryRef libraryRef = factory.CreateLibraryRef("UNISIMS");
			ICellRef cell = factory.CreateCellRef("OBUF", libraryRef);

			string textView = cell.ToEdifText();
			Assert.AreEqual(@"(cellRef OBUF (libraryRef UNISIMS))", textView);
		}

		[TestMethod]
		public void TestInstance()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IViewRef viewRef = factory.CreateViewRef("TESTVIEWREF", null);
			IPropertyValue propertyValue = factory.CreatePropertyValue(1, PropertyValueType.Integer);
			IProperty property = factory.CreateProperty(PropertyType.INIT, propertyValue, "OWNER");
			IInstance instance = factory.CreateInstance("TESTNAME", viewRef, new List<IProperty>() {property});

			string textView = instance.ToEdifText();
			Assert.AreEqual(@"(instance TESTNAME (viewRef TESTVIEWREF) (property INIT (integer 1) (owner ""OWNER"")))", textView);
		}

		[TestMethod]
		public void TestInstanceRef()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			
			IInstanceRef instance = factory.CreateInstanceRef("TESTNAME");

			string textView = instance.ToEdifText();
			Assert.AreEqual(@"(instanceRef TESTNAME)", textView);
		}

		[TestMethod]
		public void TestLibrary()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IEdifLevel level = factory.CreateEdifLevel(0);
			ITechnology technology = factory.CreateTechnology("tech");
			ICell cell = factory.CreateCell("cellname", CellType.GENERIC, null);
			ILibrary library = factory.CreateLibrary("libraryname", level, technology, cell);

			string textView = library.ToEdifText();
			Assert.AreEqual(@"(library libraryname (edifLevel 0) (technology (tech)) (cell cellname))", textView);
		}

		[TestMethod]
		public void TestLibraryRef()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();
			
			ILibraryRef libraryRef = factory.CreateLibraryRef("libraryRefName");

			string textView = libraryRef.ToEdifText();
			Assert.AreEqual(@"(libraryRef libraryRefName)", textView);
		}

		[TestMethod]
		public void TestPort()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IPort port = factory.CreatePort("portame", PortDirection.INPUT);

			string textView = port.ToEdifText();
			Assert.AreEqual(@"(port portame (direction INPUT))", textView);
		}

		[TestMethod]
		public void TestPortRef()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IInstanceRef instanceRef = factory.CreateInstanceRef("instancerefname");
			IPortRef portRef = factory.CreatePortRef("portame", instanceRef);

			string textView = portRef.ToEdifText();
			Assert.AreEqual(@"(portRef portame (instanceRef instancerefname))", textView);
		}

		[TestMethod]
		public void TestPropertyValue()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IPropertyValue propertyValue = factory.CreatePropertyValue(1, PropertyValueType.Integer);
			string textView = propertyValue.ToEdifText();
			Assert.AreEqual(@"(integer 1)", textView);

			propertyValue = factory.CreatePropertyValue(true, PropertyValueType.Boolean);
			textView = propertyValue.ToEdifText();
			Assert.AreEqual(@"(boolean (true))", textView);

			propertyValue = factory.CreatePropertyValue("testv", PropertyValueType.String);
			textView = propertyValue.ToEdifText();
			Assert.AreEqual(@"(string ""testv"")", textView);
		}

		[TestMethod]
		public void TestProperty()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IPropertyValue propertyValue = factory.CreatePropertyValue(true, PropertyValueType.Boolean);
			IProperty property = factory.CreateProperty(PropertyType.KEEP_HIERARCHY, propertyValue, "HOST");
			string textView = property.ToEdifText();
			Assert.AreEqual(@"(property KEEP_HIERARCHY (boolean (true)) (owner ""HOST""))", textView);
		}

		[TestMethod]
		public void TestView()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IContents contents = factory.CreateContents(null, null);
			IInterface inInterface = factory.CreateInterface(null, null, null);
			IView view = factory.CreateView("viewname", ViewType.NETLIST, inInterface, contents);
			string textView = view.ToEdifText();
			Assert.AreEqual(@"(view viewname (viewType NETLIST) (interface) (contents)", textView);
		}

		[TestMethod]
		public void TestViewRef()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			ICellRef cellRef = factory.CreateCellRef("cellrefname", null);
			IViewRef viewref = factory.CreateViewRef("viewrefname", cellRef);
			string textView = viewref.ToEdifText();
			Assert.AreEqual(@"(viewRef viewrefname (cellRef cellrefname))", textView);
		}

		[TestMethod]
		public void TestComment()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IComment comment = factory.CreateComment("commentmessage");
			string textView = comment.ToEdifText();
			Assert.AreEqual(@"(comment ""commentmessage"")", textView);

			comment = factory.CreateComment("comment message");
			textView = comment.ToEdifText();
			Assert.AreEqual(@"(comment ""comment message"")", textView);
		}

		[TestMethod]
		public void TestContents()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IInstance instance = factory.CreateInstance("INSTANCE_0", null, null);
			INet net = factory.CreateNet("netname", null);
			IContents contents = factory.CreateContents(new List<IInstance>() {instance}, new List<INet>() {net});
			string textView = contents.ToEdifText();
			Assert.AreEqual(@"(contents (instance INSTANCE_0) (net netname))", textView);
		}

		[TestMethod]
		public void TestDesign()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			ICellRef cellRef = factory.CreateCellRef("cellrefname", null);
			IPropertyValue propertyValue = factory.CreatePropertyValue(10, PropertyValueType.Integer);
			IProperty property = factory.CreateProperty(PropertyType.PART, propertyValue, "ZZZ");
			IDesign design = factory.CreateDesign("designname", new List<ICellRef>() {cellRef}, new List<IProperty>() {property});
			string textView = design.ToEdifText();
			Assert.AreEqual(@"(design designname (cellRef cellrefname) (property PART (integer 10) (owner ""ZZZ"")))", textView);
		}

		[TestMethod]
		public void TestEdif()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IEdifVersion edifVersion = factory.CreateEdifVersion(2, 0, 0);
			IEdifLevel edifLevel = factory.CreateEdifLevel(1);
			IKeywordMap keywordMap = factory.CreateKeywordMap(2);
			IStatus status = factory.CreateStatus(null);
			IExternal external = factory.CreateExternal("externalName", edifLevel, null, null);
			ILibrary library = factory.CreateLibrary("library_name", edifLevel, null, null);
			IDesign design = factory.CreateDesign("designname", null, null);
			IEdif edif = factory.CreateEdif("adder_as_main", edifVersion, edifLevel, keywordMap, status, external, library, design);
			string textView = edif.ToEdifText();
			Assert.AreEqual(@"(edif adder_as_main (edifVersion 2 0 0) (edifLevel 1) (keywordMap (keywordLevel 2)) (status) (external externalName) (library library_name (edifLevel 1)) (design designname)",
				textView);
		}

		[TestMethod]
		public void TestEdifLevel()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IEdifLevel edifLevel = factory.CreateEdifLevel(2);
			string textView = edifLevel.ToEdifText();
			Assert.AreEqual(@"(edifLevel 1)", textView);
		}

		[TestMethod]
		public void TestEdifVersionl()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IEdifVersion edifVersion = factory.CreateEdifVersion(1, 2, 3);
			string textView = edifVersion.ToEdifText();
			Assert.AreEqual(@"(edifVersion 1 2 3)", textView);
		}

		[TestMethod]
		public void TestExternal()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			ITechnology technology = factory.CreateTechnology("ttt");
			ICell cell = factory.CreateCell("testCell", CellType.GENERIC, null);
			IEdifLevel edifLevel = factory.CreateEdifLevel(0);
			IExternal external = factory.CreateExternal("externalName", edifLevel, technology, new List<ICell>() { cell });
			string textView = external.ToEdifText();
			Assert.AreEqual(@"external externalName (edifLevel 0) (technology (ttt)) (cell testCell))", textView);
		}

		[TestMethod]
		public void TestInterface()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IPort port = factory.CreatePort("port1", PortDirection.OUTPUT);
			IPropertyValue propertyValue = factory.CreatePropertyValue(3, PropertyValueType.Integer);
			IProperty property = factory.CreateProperty(PropertyType.XSTLIB, propertyValue, "ZXC");
			IInterface inInterface = factory.CreateInterface(new List<IPort>() {port}, "desssignator2", new List<IProperty>() {property});
			string textView = inInterface.ToEdifText();
			Assert.AreEqual(@"(interface (port port1 (direction OUTPUT)) (designator ""desssignator2"") (property XSTLIB (integer 3) (owner ""ZXC""))", textView);
		}

		[TestMethod]
		public void TestKeywordMap()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IKeywordMap keywordMap = factory.CreateKeywordMap(33);
			string textView = keywordMap.ToEdifText();
			Assert.AreEqual(@"(keywordMap (keywordLevel 33))", textView);
		}

		[TestMethod]
		public void TestNet()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IPortRef portRef = factory.CreatePortRef("portRefName", null);
			INet net = factory.CreateNet("netName33", new List<IPortRef>() {portRef});
			string textView = net.ToEdifText();
			Assert.AreEqual(@"(net netName33 (joined (portRef portRefName))", textView);
		}

		[TestMethod]
		public void TestStatus()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IWritten written = factory.CreateWritten(new DateTime(2017, 1, 1, 2, 3, 40), null);
			IStatus status = factory.CreateStatus(written);
			string textView = status.ToEdifText();
			Assert.AreEqual(@"(status (written (timestamp 2017 1 1 02 03 40)))", textView);
		}

		[TestMethod]
		public void TestTechnology()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			ITechnology technology = factory.CreateTechnology("technologyName");
			string textView = technology.ToEdifText();
			Assert.AreEqual(@"(technology (technologyName))", textView);
		}

		[TestMethod]
		public void TestWritten()
		{
			ITextViewElementsFactory factory = GetTextViewElementsFactory();

			IComment comment = factory.CreateComment("commentMessage");
			IWritten written = factory.CreateWritten(new DateTime(2017, 1, 1, 2, 3, 40), new List<IComment>() { comment });
			string textView = written.ToEdifText();
			Assert.AreEqual(@"written (timestamp 2017 6 5 10 47 40) (comment ""commentMessage""))", textView);
		}
	}
}
