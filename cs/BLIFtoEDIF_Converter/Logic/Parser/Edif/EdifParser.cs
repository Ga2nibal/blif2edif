using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;
using BLIFtoEDIF_Converter.Model.Edif.Factory;

namespace BLIFtoEDIF_Converter.Logic.Parser.Edif
{
	public class EdifParser
	{
		public static IEdif GetEdif(string edifSource)
		{
			if (null == edifSource)
				throw new ArgumentNullException(nameof(edifSource), $"{nameof(edifSource)} is not defined");
			edifSource = Regex.Replace(edifSource, @"\s+", " "); //Replace multiple spaces
			edifSource = edifSource.Replace("( ", "(").Replace(" )", ")");
			edifSource = edifSource.Trim();

			ITextViewElementsFactory edifFactory = TextViewElementsFactoryCreator.CreaTextViewElementsFactory(Implementations.FastImpl);

			string name = GetEdifName(edifSource);
			IEdifVersion edifVersion = GetEdifVesrion(edifSource, edifFactory);
			IEdifLevel edifLevel = GetEdifLevel(GetBlockSource(edifSource, 0, 2), edifFactory);
			IKeywordMap keywordApp = GetKeywordMap(edifSource, edifFactory);
			IStatus status = GetStatus(edifSource, edifFactory);
			IExternal external = GetExternal(edifSource, edifFactory);

			ILibrary library = GetOneLinraryOrThrow(edifSource, edifFactory);
			IDesign design = GetDesign(edifSource, edifFactory);

			return edifFactory.CreateEdif(name, edifVersion, edifLevel, keywordApp, status, external,
				library, design);
		}

		private static ILibrary GetOneLinraryOrThrow(string edifSource, ITextViewElementsFactory edifFactory)
		{
			var libraryBodies = GetBlockBodies(edifSource, @"\(\s*library\s+");
			if (libraryBodies == null || libraryBodies.Count == 0)
				return null;
			else if (libraryBodies.Count == 1)
				return GetLibrary(libraryBodies[0], edifFactory);
			else
				throw new ArgumentException($"Edif Parser found {libraryBodies.Count} matches of 'Library' block in edifSource");
		}

		private static IDesign GetDesign(string designSource, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(designSource, @"\(\s*design\s+([\w\d]+)[\W\D]+(.*)");
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of 'design names' in designSource");
			string name = matches[0].Groups[1].Value;
			IList<ICellRef> cellRefs = GetCellRefs(designSource, edifFactory);
			IList<IProperty> properties = GetProperties(designSource, edifFactory);
			return edifFactory.CreateDesign(name, cellRefs, properties);
		}

		private static ILibrary GetLibrary(string librarySource, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(librarySource, @"\(\s*library\s+([\w\d]+)[\W\D]+");
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of 'Library name' in librarySource");
			string name = matches[0].Groups[1].Value;
			ICell cell = GetMaxOneCellOrThrow(librarySource, edifFactory);
			IEdifLevel edifLevel = GetEdifLevel(librarySource, edifFactory);
			ITechnology technology = GetTechnology(librarySource, edifFactory);
			return edifFactory.CreateLibrary(name, edifLevel, technology, cell);
		}

		private static IExternal GetExternal(string edifSource, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(edifSource, @"\(\s*external\s+([\w\d]+)[\W\D]+(.*)");
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif External block");
			string name = matches[0].Groups[1].Value;
			string externalBody = GetSourceToEndOfBodyBlock(matches[0].Groups[2].Value);
			IEdifLevel edifLevel = GetEdifLevel(externalBody, edifFactory);
			ITechnology technology = GetTechnology(externalBody, edifFactory);
			IList<ICell> cells = GetCells(externalBody, edifFactory);
			return edifFactory.CreateExternal(name, edifLevel, technology, cells);
		}

		private static ICell GetMaxOneCellOrThrow(string body, ITextViewElementsFactory edifFactory)
		{
			List<string> cellSources = GetBlockBodies(body, @"\(\s*cell\s+([\w\d]+)[\W\D]+");
			if (cellSources == null || cellSources.Count == 0)
				return null;
			if (cellSources.Count == 1)
				return GetCell(cellSources[0], edifFactory);
			throw new ArgumentException($"Edif Parser found {cellSources.Count} matches of cells in body");
		}

		private static IList<IProperty> GetProperties(string body, ITextViewElementsFactory edifFactory)
		{
			List<string> cellSources = GetBlockBodies(body, @"\(\s*property\s+([\w\d]+)[\W\D]+");
			return cellSources.Select(s => GetProperty(s, edifFactory)).ToList();
		}

		private static IList<ICellRef> GetCellRefs(string body, ITextViewElementsFactory edifFactory)
		{
			List<string> cellSources = GetBlockBodies(body, @"\(\s*cellRef\s+([\w\d]+)[\W\D]+");
			return cellSources.Select(s => GetCellRef(s, edifFactory)).ToList();
		}

		private static IList<ICell> GetCells(string body, ITextViewElementsFactory edifFactory)
		{
			List<string> cellSources = GetBlockBodies(body, @"\(\s*cell\s+([\w\d]+)[\W\D]+");
			return cellSources.Select(s => GetCell(s, edifFactory)).ToList();
		}

		private static ICell GetCell(string cellBody, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(cellBody, @"\(\s*cell\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			CellType cellType = (CellType)Enum.Parse(typeof(CellType),
				Regex.Matches(cellBody, @"\(\s*cellType\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value,
				true);
			var viewBodies = GetBlockBodies(cellBody, @"\(\s*view\s+([\w\d]+)[\W\D]+");
			if(viewBodies.Count != 1)
				throw new ArgumentException($"Edif Parser found {viewBodies.Count} matches of view in cell block");
			IView view = GetView(viewBodies[0], edifFactory);
			return edifFactory.CreateCell(name, cellType, view);
		}

		private static IView GetView(string viewBody, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(viewBody, @"\(\s*view\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			ViewType cellType = (ViewType)Enum.Parse(typeof(ViewType),
				Regex.Matches(viewBody, @"\(\s*viewType\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value,
				true);
			var interfaceBodies = GetBlockBodies(viewBody, @"\(\s*interface[\W\D]+");
			if (interfaceBodies.Count != 1)
				throw new ArgumentException($"Edif Parser found {interfaceBodies.Count} matches of interface in viewBody block");
			IInterface edifInterface = GetInterface(interfaceBodies[0], edifFactory);
			var contentsBodies = GetBlockBodies(viewBody, @"\(\s*contents[\W\D]+");
			if (contentsBodies.Count != 1)
				throw new ArgumentException($"Edif Parser found {contentsBodies.Count} matches of content in viewBody block");
			IContents contents = GetContent(contentsBodies[0], edifFactory);
			return edifFactory.CreateView(name, cellType, edifInterface, contents);
		}

		private static IContents GetContent(string contentsBody, ITextViewElementsFactory edifFactory)
		{
			List<string> instancesSources = GetBlockBodies(contentsBody, @"\(\s*instance\s+([\w\d]+)[\W\D]+");
			IList<IInstance> instances = instancesSources.Select(s => GetInstance(s, edifFactory)).ToList();

			List<string> netsSources = GetBlockBodies(contentsBody, @"\(\s*net\s+([\w\d]+)[\W\D]+");
			IList<INet> nets = netsSources.Select(s => GetNet(s, edifFactory)).ToList();

			return edifFactory.CreateContents(instances, nets);
		}

		private static INet GetNet(string netsSource, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(netsSource, @"\(\s*net\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			IList<IPortRef> joinedPortRef;
			List<string> joinedBlock = GetBlockBodies(netsSource, @"\(\s*joined\W");
			if (joinedBlock.Count == 0)
			{
				Debug.Assert(false, "Can not find 'joined' block in netsSource");
				joinedPortRef = new List<IPortRef>();
			}
			else if (joinedBlock.Count == 1)
			{
				List<string> portRefSources = GetBlockBodies(joinedBlock[0], @"\(\s*portRef\s+([\w\d]+)[\W\D]+");
				joinedPortRef = portRefSources.Select(portRefSource => GetPortRef(portRefSource, edifFactory)).ToList();
			}
			else
				throw new ArgumentException($"Edif Parser found {joinedBlock.Count} matches of 'joined' block in netsSource");

			return edifFactory.CreateNet(name, joinedPortRef);
		}

		private static IPortRef GetPortRef(string portRefSource, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(portRefSource, @"\(\s*portRef\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			List<IInstanceRef> instanceRefs = GetInstanceRefs(portRefSource, edifFactory);
			IInstanceRef instanceRef;
			if (instanceRefs == null || instanceRefs.Count == 0)
				instanceRef = null;
			else if (instanceRefs.Count == 1)
				instanceRef = instanceRefs[0];
			else
				throw new ArgumentException($"Edif Parser found {instanceRefs.Count} matches of 'instanceRef' block in portRefSource");
			return edifFactory.CreatePortRef(name, instanceRef);
		}

		private static List<IInstanceRef> GetInstanceRefs(string source, ITextViewElementsFactory edifFactory)
		{
			List<string> instanceRefSources = GetBlockBodies(source, @"\(\s*instanceRef\W");
			List<IInstanceRef> result = instanceRefSources.Select(instanceRefSource => GetInstanceRef(instanceRefSource, edifFactory)).ToList();
			return result;
		}

		private static IInstanceRef GetInstanceRef(string instanceRefSource, ITextViewElementsFactory edifFactory)
		{
			string referedInstanceName = Regex.Matches(instanceRefSource, @"\(\s*instanceRef\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			return edifFactory.CreateInstanceRef(referedInstanceName);
		}

		private static IInstance GetInstance(string instanceBody, ITextViewElementsFactory edifFactory)
		{
			string name;
			string renamedSynonym;
			var renameMatch = Regex.Matches(instanceBody, @"\(\s*rename\s+([\w\d]+)[\W\D]+""([\w\d]+)""\s*\)");
			if (renameMatch.Count == 0)
			{
				name = Regex.Matches(instanceBody, @"\(\s*instance\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
				renamedSynonym = null;
			}
			else if(renameMatch.Count == 1)
			{
				name = renameMatch[0].Groups[2].Value;
				renamedSynonym = renameMatch[0].Groups[1].Value;
			}
			else
				throw new ArgumentException($"Edif Parser found {renameMatch.Count} matches of rename block in instanceBody");

			List<string> viewRefSources = GetBlockBodies(instanceBody, @"\(\s*viewRef\s+([\w\d]+)[\W\D]+");
			IViewRef viewRef;
			if (viewRefSources.Count == 0)
				viewRef = null;
			else if(viewRefSources.Count == 1)
				viewRef = GetViewRef(viewRefSources[0], edifFactory);
			else
				throw new ArgumentException($"Edif Parser found {renameMatch.Count} matches of viewRef block in instanceBody");

			IList<IProperty> properties = GetProperties(instanceBody, edifFactory);
			return edifFactory.CreateInstance(name, renamedSynonym, viewRef, properties);
		}

		private static IViewRef GetViewRef(string viewRefSource, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(viewRefSource, @"\(\s*viewRef\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			ICellRef cellRef;
			List<string> cellRefSources = GetBlockBodies(viewRefSource, @"\(\s*cellRef\s+([\w\d]+)[\W\D]+");
			if (cellRefSources.Count == 0)
				cellRef = null;
			else if (cellRefSources.Count == 1)
				cellRef = GetCellRef(cellRefSources[0], edifFactory);
			else
				throw new ArgumentException($"Edif Parser found {cellRefSources.Count} matches of cellRef block in viewRefSource");
			return edifFactory.CreateViewRef(name, cellRef);
		}

		private static ICellRef GetCellRef(string cellRefSource, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(cellRefSource, @"\(\s*cellRef\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			ILibraryRef libraryRef;
			List<string> libraryRefSources = GetBlockBodies(cellRefSource, @"\(\s*libraryRef\s+([\w\d]+)[\W\D]+");
			if (libraryRefSources.Count == 0)
				libraryRef = null;
			else if (libraryRefSources.Count == 1)
				libraryRef = GetLibraryRef(libraryRefSources[0], edifFactory);
			else
				throw new ArgumentException($"Edif Parser found {libraryRefSources.Count} matches of libraryRef block in cellRefSource");
			return edifFactory.CreateCellRef(name, libraryRef);
		}

		private static ILibraryRef GetLibraryRef(string libraryRefSource, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(libraryRefSource, @"\(\s*libraryRef\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			return edifFactory.CreateLibraryRef(name);
		}

		private static IInterface GetInterface(string interfaceBody, ITextViewElementsFactory edifFactory)
		{
			List<string> portSources = GetBlockBodies(interfaceBody, @"\(\s*port\s+([\w\d]+)[\W\D]+");
			var ports = portSources.Select(s => GetPort(s, edifFactory)).ToList();
			string designator = GetDesignator(interfaceBody);
			IList<IProperty> properties = GetProperties(interfaceBody, edifFactory);
			return edifFactory.CreateInterface(ports, designator, properties);
		}

		private static IProperty GetProperty(string propertyBody, ITextViewElementsFactory edifFactory)
		{
			PropertyType propertyType = GetPropertyType(propertyBody);
			IPropertyValue propertyValue = GetPropertyValue(propertyBody, edifFactory);
			string owner = GetPropertyOwner(propertyBody);
			return edifFactory.CreateProperty(propertyType, propertyValue, owner);
		}

		private static IPropertyValue GetPropertyValue(string propertyBody, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(propertyBody, @"\(\s*property\s+[\w\d]+\s*\(([^\)]+)\)");
			if (matches.Count == 0)
				return null;
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif PropertyValue in propertyBody");
			if (matches[0].Groups[1].Value.StartsWith("owner "))
				return null;
			var typeValue = matches[0].Groups[1].Value.Split(' ');
			if(typeValue.Length != 2)
				throw new ArgumentException($"GetPropertyValue. typeValue.Length : {typeValue.Length}, but '2' expected. propertyBody: {propertyBody}. typeValueMatch: {matches[0].Groups[1].Value}");
			return edifFactory.CreatePropertyValue(typeValue[1],
				(PropertyValueType)Enum.Parse(typeof(PropertyValueType), typeValue[0], true));
		}

		private static PropertyType GetPropertyType(string propertyBody)
		{
			return (PropertyType)Enum.Parse(typeof(PropertyType),
				Regex.Matches(propertyBody, @"\(\s*property\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value,
				true);
		}

		private static string GetPropertyOwner(string propertyBody)
		{
			var matches = Regex.Matches(propertyBody, @"\(\s*owner\s+""([\w\d]+)""\s*\)");
			if (matches.Count == 0)
				return null;
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif technology block");
			return matches[0].Groups[1].Value;
		}

		private static IPort GetPort(string portBody, ITextViewElementsFactory edifFactory)
		{
			string name = Regex.Matches(portBody, @"\(\s*port\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value;
			PortDirection direction = GetPortDirection(portBody);
			return edifFactory.CreatePort(name, direction);
		}

		private static PortDirection GetPortDirection(string portBody)
		{
			return (PortDirection)Enum.Parse(typeof(PortDirection),
				Regex.Matches(portBody, @"\(\s*direction\s+([\w\d]+)[\W\D]+")[0].Groups[1].Value,
				true);
		}

		private static string GetDesignator(string interfaceBody)
		{
			var matches = Regex.Matches(interfaceBody, @"\(\s*designator\s+([\d\w""]+)[\W\D]*\)");
			if (matches.Count == 0)
				return null;
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of designator block");
			Match match = matches[0];
			return match.Groups[1].Value;
		}

		private static List<string> GetBlockBodies(string source, string pattern)
		{
			var cellMatches = Regex.Matches(source, pattern);
			List<string> blockSources = new List<string>();
			foreach (Match cellMatch in cellMatches)
				blockSources.Add(GetBlockSource(source, cellMatch.Index));
			return blockSources;
		}

		private static ITechnology GetTechnology(string edifSource, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(edifSource, @"\(\s*technology\s+\(*\s*([\w\d]+)\s*\)*\)");
			if (matches.Count == 0)
				return null;
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif technology block");
			Match match = matches[0];
			return edifFactory.CreateTechnology(match.Groups[1].Value);
		}

		private static string GetSourceToEndOfBodyBlock(string edifSourceFromStartExternalBody)
		{
			StringBuilder result = new StringBuilder();
			int externalNesting = 1;
			foreach (char sourceChar in edifSourceFromStartExternalBody)
			{
				if (sourceChar == '(')
					externalNesting++;
				else if (sourceChar == ')')
					externalNesting--;
				if (externalNesting == 0)
					break;
				result.Append(sourceChar);
			}
			if (externalNesting != 0)
				throw new ArgumentException("Can not find end of Block.");
			return result.ToString();
		}

		private static string GetBlockSource(string source, int startBlockIndex, int? maxNestingLevelInResult = null)
		{
			if(source[startBlockIndex] != '(')
				throw new ArgumentException($"source[startBlockIndex] should be '(', but {source[startBlockIndex]}. startBlockIndex: {startBlockIndex}. source: {source}");
			StringBuilder result = new StringBuilder();
			result.Append(source[startBlockIndex]);
			int externalNesting = 1;
			for (int i = startBlockIndex+1; i < source.Length; i++)
			{
				char sourceChar = source[i];
				if (sourceChar == '(')
					externalNesting++;
				if (!maxNestingLevelInResult.HasValue ||
					externalNesting <= maxNestingLevelInResult.Value)
					result.Append(sourceChar);
				if (sourceChar == ')')
					externalNesting--;
				if (externalNesting == 0)
					break;
			}
			if (externalNesting != 0)
				throw new ArgumentException($"GetBlockSource. Can not find end of Block. startBlockIndex: {startBlockIndex}. source: {source}");
			return result.ToString();
		}

		private static IStatus GetStatus(string edifSource, ITextViewElementsFactory edifFactory)
		{
			return null;
			//TODO: Implement me
			//var matches = Regex.Matches(edifSource, @"\(\s*keywordMap[\W\D]*\(\s*keywordLevel\s+(\d+)[\W\D]*\)[\W\D]*\)");
			//if (matches.Count != 1)
			//	throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif keywordMap+keywordLevel block");
			//Match match = matches[0];
			//return edifFactory.CreateStatus(int.Parse(match.Groups[1].Value));
		}

		private static IWritten GetWritten(string edifSource, ITextViewElementsFactory edifFactory)
		{
			return null;
			//TODO: Implement me
			//var matches = Regex.Matches(edifSource, @"\(\s*keywordMap[\W\D]*\(\s*keywordLevel\s+(\d+)[\W\D]*\)[\W\D]*\)");
			//if (matches.Count != 1)
			//	throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif keywordMap+keywordLevel block");
			//Match match = matches[0];
			//return edifFactory.CreateWritten(int.Parse(match.Groups[1].Value));
		}

		private static IKeywordMap GetKeywordMap(string edifSource, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(edifSource, @"\(\s*keywordMap[\W\D]*\(\s*keywordLevel\s+(\d+)[\W\D]*\)[\W\D]*\)");
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif keywordMap+keywordLevel block");
			Match match = matches[0];
			return edifFactory.CreateKeywordMap(int.Parse(match.Groups[1].Value));
		}

		private static IEdifLevel GetEdifLevel(string body, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(body, @"\(\s*edifLevel\s+(\d+)[\W\D]*\)");
			if (matches.Count == 0)
				return null;
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif level block");
			Match match = matches[0];
			return edifFactory.CreateEdifLevel(int.Parse(match.Groups[1].Value));
		}

		private static IEdifVersion GetEdifVesrion(string edifSource, ITextViewElementsFactory edifFactory)
		{
			var matches = Regex.Matches(edifSource, @"\(\s*edifVersion\s+(\d+)\s+(\d+)\s+(\d+)[\W\D]*\)");
			if (matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} matches of edif version block");
			Match match = matches[0];
			return edifFactory.CreateEdifVersion(int.Parse(match.Groups[1].Value),
				int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
		}

		public static string GetEdifName(string edifSource)
		{
			var matches = Regex.Matches(edifSource, @"edif\s+([\w\d]+)[\W\D]");
			if(matches.Count != 1)
				throw new ArgumentException($"Edif Parser found {matches.Count} edif names");
			return matches[0].Groups[1].Value;
		}
	}
}
