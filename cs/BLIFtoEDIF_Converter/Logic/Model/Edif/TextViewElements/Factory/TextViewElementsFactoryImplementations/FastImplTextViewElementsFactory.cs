using System;
using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Instance;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Library;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Port;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl.View;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory.TextViewElementsFactoryImplementations
{
	class FastImplTextViewElementsFactory : ITextViewElementsFactory
	{
		public ICell CreateCell(string name, CellType cellType, IView view)
		{
			return new Cell(name, cellType, view);
		}

		public ICellRef CreateCellRef(string name, ILibraryRef libraryRef)
		{
			return new CellRef(name, libraryRef);
		}

		public IInstance CreateInstance(string name, IViewRef viewRef, IList<IProperty> properties)
		{
			return new Instance(name, viewRef, properties);
		}

		public IInstance CreateInstance(string name, string renamedSynonym, IViewRef viewRef, IList<IProperty> properties)
		{
			return new Instance(name, renamedSynonym, viewRef, properties);
		}

		public IInstanceRef CreateInstanceRef(string referedInstanceName)
		{
			return new InstanceRef(referedInstanceName);
		}

		public ILibrary CreateLibrary(string name, IEdifLevel level, ITechnology technology, ICell cell)
		{
			return new Library(name, level, technology, cell);
		}

		public ILibraryRef CreateLibraryRef(string name)
		{
			return new LibraryRef(name);
		}

		public IPort CreatePort(string name, PortDirection direction)
		{
			return new Port(name, direction);
		}

		public IPortRef CreatePortRef(string name, IInstanceRef instanceRef)
		{
			return new PortRef(name, instanceRef);
		}

		public IPropertyValue CreatePropertyValue(object value, PropertyValueType type)
		{
			return new PropertyValue(value, type);
		}

		public IProperty CreateProperty(PropertyType propertyType, IPropertyValue value, string owner)
		{
			return new Property(propertyType, value, owner);
		}

		public IView CreateView(string name, ViewType viewType, IInterface @interface, IContents contents)
		{
			return new View(name, viewType, @interface, contents);
		}

		public IViewRef CreateViewRef(string name, ICellRef cellRef)
		{
			return new ViewRef(name, cellRef);
		}

		public IComment CreateComment(string text)
		{
			return new Comment(text);
		}

		public IContents CreateContents(IList<IInstance> instances, IList<INet> nets)
		{
			return new Contents(instances, nets);
		}

		public IDesign CreateDesign(string name, IList<ICellRef> cellRefs, IList<IProperty> properties)
		{
			return new Design(name, cellRefs, properties);
		}

		public IEdif CreateEdif(string name, IEdifVersion version, IEdifLevel level, IKeywordMap keywordMap, 
			IStatus status, IExternal external, ILibrary library, IDesign design)
		{
			return new Implementation.FastImpl.Edif(name, version, level, keywordMap, status, external, library, design);
		}

		public IEdifLevel CreateEdifLevel(int level)
		{
			return new EdifLevel(level);
		}

		public IEdifVersion CreateEdifVersion(int majorVersion, int midVersion, int minorVersion)
		{
			return new EdifVersion(majorVersion, midVersion, minorVersion);
		}

		public IExternal CreateExternal(string name, IEdifLevel edifLevel, ITechnology technology, IList<ICell> cells)
		{
			return new External(name, edifLevel, technology, cells);
		}

		public IInterface CreateInterface(IList<IPort> ports, string designator, IList<IProperty> properties)
		{
			return new Interface(ports, designator, properties);
		}

		public IKeywordMap CreateKeywordMap(int keywordLevel)
		{
			return new KeywordMap(keywordLevel);
		}

		public INet CreateNet(string name, IList<IPortRef> joined)
		{
			return new Net(name, joined);
		}

		public IStatus CreateStatus(IWritten written)
		{
			return new Status(written);
		}

		public ITechnology CreateTechnology(string name)
		{
			return new Texhnology(name);
		}

		public IWritten CreateWritten(DateTime timestamp, IList<IComment> comments)
		{
			return new Written(timestamp, comments);
		}
	}
}
