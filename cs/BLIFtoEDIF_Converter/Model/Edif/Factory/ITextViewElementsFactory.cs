using System;
using System.Collections.Generic;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;

namespace BLIFtoEDIF_Converter.Model.Edif.Factory
{
	public interface ITextViewElementsFactory
	{
		ICell CreateCell(string name, CellType cellType, IView view);

		ICellRef CreateCellRef(string name, ILibraryRef libraryRef);

		IInstance CreateInstance(string name, IViewRef viewRef, IList<IProperty> properties);

		IInstance CreateInstance(string name, string renamedSynonym, IViewRef viewRef, IList<IProperty> properties);

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

		IEdif CreateEdif(string name, IEdifVersion version, IEdifLevel level, IKeywordMap keywordMap, IStatus status, IExternal external,
			ILibrary library, IDesign design);

		IEdifLevel CreateEdifLevel(int level);

		IEdifVersion CreateEdifVersion(int majorVersion, int midVersion, int minorVersion);

		IExternal CreateExternal(string name, IEdifLevel edifLevel, ITechnology technology, IList<ICell> cells);

		IInterface CreateInterface(IList<IPort> ports, string designator, IList<IProperty> properties);

		IKeywordMap CreateKeywordMap(int keywordLevel);

		INet CreateNet(string name, IList<IPortRef> joined);

		IStatus CreateStatus(IWritten written);

		ITechnology CreateTechnology(string name);

		IWritten CreateWritten(DateTime timestamp, IList<IComment> comments);
	}
}
