using System.Collections.Generic;
using BLIFtoEDIF_Converter.init_calculator;
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
	}
}
