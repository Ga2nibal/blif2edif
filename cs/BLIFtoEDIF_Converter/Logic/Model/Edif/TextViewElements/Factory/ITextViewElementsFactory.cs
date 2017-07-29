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

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory
{
	public interface ITextViewElementsFactory
	{
		ICell CreateCell(string name, CellType cellType, IView view);

		ICellRef CreateCellRef(string name, ILibraryRef libraryRef);

		IInstance CreateInstance(string name, IViewRef viewRef, IList<IProperty> properties);

		IInstanceRef CreateInstanceRef(string referedInstanceName);

		ILibrary CreateLibrary(string name, IEdifLevel level, ITechnology technology, ICell cell);

		ILibraryRef CreateLibraryRef(string name);

		IPort CreatePort(string name, PortDirection direction);

		IPortRef CreatePortRef(string name, IInstanceRef instanceRef);

		IPropertyValue CreatePropertyValue(object value, PropertyValueType type);

		IProperty CreateProperty(PropertyType propertyType, IPropertyValue value, string owner);

		IView CreateView(string name, ViewType viewType, IInterface @interface, IContents contents);

		IViewRef CreateViewRef(string name, ICellRef cellRef);

		IComment CreateComment(string text);

		IContents CreateContents(IList<IInstance> instances, IList<INet> nets);

		IDesign CreateDesign(string name, IList<ICellRef> cellRefs, IList<IProperty> properties);

		IEdif CreateEdif(EdifVersion version, EdifLevel level, KeywordMap keywordMap, IStatus status, IExternal external,
			ILibrary library, IDesign design);

		IEdifLevel CreateEdifLevel(int level);

		IEdifVersion CreateEdifVersion(int majorVersion, int midVersion, int minorVersion);

		IExternal CreateExternal(string name, int edifLevel, ITechnology technology, IList<ICell> cells);

		IInterface CreateInterface(IList<IPort> ports, string designator, IList<IProperty> properties);

		IKeywordMap CreateKeywordMap(int keywordLevel);

		INet CreateNet(string name, IList<IPortRef> joined);

		IStatus CreateStatus(IWritten written);

		ITechnology CreateTechnology(string name);

		IWritten CreateWritten(DateTime timestamp, IList<IComment> comments);
	}
}
