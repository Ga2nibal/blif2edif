using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BLIFtoEDIF_Converter.InitCalculator;
using BLIFtoEDIF_Converter.Logic.Model.Blif;
using BLIFtoEDIF_Converter.Logic.Model.Blif.Function;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Cell;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Instance;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Library;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Property;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.View;
using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Factory;
using BLIFtoEDIF_Converter.Util;
using PortDirection = BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction.Port.PortDirection;

namespace BLIFtoEDIF_Converter.Logic
{
	public static class BlifToEdifModelConverter
	{
		public class EdifConstants
		{
			public EdifConstants(string modelName)
			{
				if(string.IsNullOrEmpty(modelName))
					throw new ArgumentNullException(nameof(modelName), $"{nameof(modelName)} is not defined");
				if (modelName.Contains(".blif") || modelName.Contains(".edif") || modelName.Contains("-"))
					modelName = modelName.Replace(".blif", string.Empty).Replace(".edif", string.Empty)
						.Replace("-", "_");
				ModelName = modelName;
			}
			public string ModelName { get; set; }

			public int EdifLevel { get; set; } = 0;

			private int? _modelEdifLevel;
			private int? _externalEdifLevel;
			private int? _libraryEdifLevel;
			public int ModelEdifLevel {
				get { return _modelEdifLevel ?? EdifLevel; }
				set { _modelEdifLevel = value; }
			}
			public int EdifExternalLevel {
				get { return _externalEdifLevel ?? EdifLevel; }
				set { _externalEdifLevel = value; }
			}
			public int EdifLibraryLevel {
				get { return _libraryEdifLevel ?? EdifLevel; }
				set { _libraryEdifLevel = value; }
			}

			public int EdifMajorVersion { get; set; } = 2;
			public int EdifMidVersion { get; set; } = 0;
			public int EdifMinorVersion { get; set; } = 0;

			public int KeywordMapLevel { get; set; } = 0;

			public string StatusWrittenComment { get; set; } = "Do we need it in converter?";

			public string ExternalName { get; set; } = "UNISIMS";

			public string TechnologyName { get; set; } = "numberDefinition";

			private string _technologyExternalName;

			public string TechnologyExternalName
			{
				get { return _technologyExternalName ?? TechnologyName; }
				set { _technologyExternalName = value; }
			}

			private string _technologyLibraryName;
			public string TechnologyLibraryName {
				get { return _technologyLibraryName ?? TechnologyName; }
				set { _technologyLibraryName = value; }
			}

			public string DesignPropertyStringValue { get; set; } = "xc6slx4-3-tqg144";

			public string PropertyOwner { get; set; } = "Xilinx";

			public string GenericIbufName { get; set; } = "IBUF";
			public string GenericObufName { get; set; } = "OBUF";
			public string GenericViewName { get; set; } = "view_1";
			public string LibraryInterfaceDesignator { get; set; } = "xc6slx4-3-tqg144";

			public DateTime StatusWrittenTimestamp { get; set; } = DateTime.Now;
		}

		public static IEdif ToEdif(this Blif blif, ITextViewElementsFactory edifFactory, 
			EdifConstants edifConstants, out string renameLog)
		{
			if (null == blif)
				throw new ArgumentNullException(nameof(blif), $"{nameof(blif)} is not defined");
			if (null == edifFactory)
				throw new ArgumentNullException(nameof(edifFactory), $"{nameof(edifFactory)} is not defined");
			HashSet<string> sanitizedNames;
			RenamePortsInEdifConvention(blif, out renameLog, out sanitizedNames);
			IEdifVersion edifVersion = edifFactory.CreateEdifVersion(edifConstants.EdifMajorVersion,
				edifConstants.EdifMidVersion,
				edifConstants.EdifMinorVersion);
			IEdifLevel edifLevel = edifFactory.CreateEdifLevel(edifConstants.ModelEdifLevel);
			IKeywordMap keywordMap = edifFactory.CreateKeywordMap(edifConstants.KeywordMapLevel);

			IWritten written = edifFactory.CreateWritten(edifConstants.StatusWrittenTimestamp,
				new List<IComment>() {edifFactory.CreateComment(edifConstants.StatusWrittenComment)});
			IStatus status = edifFactory.CreateStatus(written);

			IEdifLevel edifExternalLevel = edifFactory.CreateEdifLevel(edifConstants.EdifExternalLevel);
			ITechnology externalTechnology = edifFactory.CreateTechnology(edifConstants.TechnologyExternalName);
			IList<ICell> externalGenericCells = GetExternalGenericCells(blif, edifFactory, edifConstants);
			IExternal external = edifFactory.CreateExternal(edifConstants.ExternalName, edifExternalLevel,
				externalTechnology,
				externalGenericCells);

			string libraryName = GetLibraryName(blif, edifConstants);
			IEdifLevel edifLibraryLevel = edifFactory.CreateEdifLevel(edifConstants.EdifLibraryLevel);
			ITechnology libraryTechnology = edifFactory.CreateTechnology(edifConstants.TechnologyLibraryName);
			ICell libraryCell = GetLibraryCell(blif, edifFactory, edifConstants, sanitizedNames);
			ILibrary library = edifFactory.CreateLibrary(libraryName, edifLibraryLevel, libraryTechnology, libraryCell);

			ILibraryRef libraryRef = edifFactory.CreateLibraryRef(libraryName);
			ICellRef cellRef = edifFactory.CreateCellRef(edifConstants.ModelName, libraryRef);
			IPropertyValue propertyValue = edifFactory.CreatePropertyValue(edifConstants.DesignPropertyStringValue,
				PropertyValueType.String);
			IProperty property = edifFactory.CreateProperty(PropertyType.PART, propertyValue, edifConstants.PropertyOwner);
			IDesign design = edifFactory.CreateDesign(edifConstants.ModelName, new List<ICellRef>() {cellRef},
				new List<IProperty>() {property});

			IEdif result = edifFactory.CreateEdif(edifConstants.ModelName, edifVersion, edifLevel, keywordMap, status,
				external, library, design);

			return result;
		}

		private static void RenamePortsInEdifConvention(Blif blif, out string renameLog,
			out HashSet<string> allSanitizedPortNames)
		{
			StringBuilder renameLogBuilder = new StringBuilder();

			List<string> allPortNames = new List<string>();
			allPortNames.AddRange(blif.Inputs.InputList.Select(i => i.Name));
			allPortNames.AddRange(blif.Outputs.OutputList.Select(o => o.Name));
			allPortNames.AddRange(blif.Functions.SelectMany(f => f.Ports).Select(i => i.Name));
			
			HashSet<string> validLowerCaseDistinctName = new HashSet<string>(allPortNames.Where(n => !n.Any(c => char.IsUpper(c)))
				.Where(n => IsValidEdifPortName(n)).Distinct());
			allSanitizedPortNames = new HashSet<string>(validLowerCaseDistinctName);
			Dictionary<string, string> renamedNames = new Dictionary<string, string>();

			int renameUniqueIndex = 0;
			foreach (Input input in blif.Inputs.InputList)
			{
				if(validLowerCaseDistinctName.Contains(input.Name))
					continue;

				string sanitizedName;
				if (!renamedNames.TryGetValue(input.Name, out sanitizedName))
				{
					sanitizedName = SanitizePortsInEdifConvention(input.Name);
					if (!renamedNames.ContainsKey(input.Name) &&
					    allSanitizedPortNames.Contains(sanitizedName, StringComparer.InvariantCultureIgnoreCase))
					{
						renameLogBuilder.Append(" [").Append(sanitizedName).Append(" => ");
						sanitizedName = sanitizedName + "_R" + renameUniqueIndex;
						renameLogBuilder.Append(sanitizedName).Append("]");

						renameUniqueIndex++;
					}
					renamedNames.Add(input.Name, sanitizedName);
				}
				input.Name = sanitizedName;
				allSanitizedPortNames.Add(sanitizedName);
			}

			foreach (Output output in blif.Outputs.OutputList)
			{
				if (validLowerCaseDistinctName.Contains(output.Name))
					continue;
				string sanitizedName;

				if (!renamedNames.TryGetValue(output.Name, out sanitizedName))
				{
					sanitizedName = SanitizePortsInEdifConvention(output.Name);
					if (!renamedNames.ContainsKey(output.Name) &&
					    allSanitizedPortNames.Contains(sanitizedName, StringComparer.InvariantCultureIgnoreCase))
					{
						renameLogBuilder.Append(" [").Append(sanitizedName).Append(" => ");
						sanitizedName = sanitizedName + "_R" + renameUniqueIndex;
						renameLogBuilder.Append(sanitizedName).Append("]");

						renameUniqueIndex++;
					}
					renamedNames.Add(output.Name, sanitizedName);
				}
				output.Name = sanitizedName;
				allSanitizedPortNames.Add(sanitizedName);
			}

			foreach (Function function in blif.Functions)
				foreach (Port port in function.Ports)
				{
					if (validLowerCaseDistinctName.Contains(port.Name))
						continue;

					string sanitizedName;
					if (!renamedNames.TryGetValue(port.Name, out sanitizedName))
					{
						sanitizedName = SanitizePortsInEdifConvention(port.Name);
						if (allSanitizedPortNames.Contains(sanitizedName, StringComparer.InvariantCultureIgnoreCase))
						{
							renameLogBuilder.Append(" [").Append(sanitizedName).Append(" => ");
							sanitizedName = sanitizedName + "_R" + renameUniqueIndex;
							renameLogBuilder.Append(sanitizedName).Append("]");

							renameUniqueIndex++;
						}
						renamedNames.Add(port.Name, sanitizedName);
					}
					port.Name = sanitizedName;
					allSanitizedPortNames.Add(sanitizedName);
				}

			renameLog = renameLogBuilder.ToString();
		}

		private static readonly string[] RestrictedEdifPortNameSubStrings = new string[] {"[", "]", "(", ")"};
		private static string SanitizePortsInEdifConvention(string portBlifName)
		{
			string sanitizedName = portBlifName;
			foreach (string restrictedEdifPortNameSubString in RestrictedEdifPortNameSubStrings)
				sanitizedName = sanitizedName.Replace(restrictedEdifPortNameSubString, string.Empty);
			return sanitizedName;
		}

		private static bool IsValidEdifPortName(string portBlifName)
		{
			return RestrictedEdifPortNameSubStrings.All(restrictedEdifPortNameSubString => !portBlifName.Contains(restrictedEdifPortNameSubString));
		}

		private static ICell GetLibraryCell(Blif blif, ITextViewElementsFactory edifFactory, 
			EdifConstants edifConstants, HashSet<string> sanitizedNames)
		{
			IInterface @interface = CreateLibraryinterface(blif, edifFactory, edifConstants);
			IContents contents = CreateLibraryContents(blif, edifFactory, edifConstants, sanitizedNames);
			IView view = edifFactory.CreateView(edifConstants.GenericViewName, ViewType.NETLIST, @interface, contents);
			ICell cellResult = edifFactory.CreateCell(edifConstants.ModelName, CellType.GENERIC, view);
			return cellResult;
		}

		private static IContents CreateLibraryContents(Blif blif, ITextViewElementsFactory edifFactory,
			EdifConstants edifConstants, HashSet<string> sanitizedNames)
		{
			List<IInstance> instances = CreateLibraryContentsInstances(blif, edifFactory, edifConstants);
			List<INet> nets = CreateLibraryContentsNets(blif, edifFactory, instances, sanitizedNames);

			IContents contentsResult = edifFactory.CreateContents(instances, nets);
			return contentsResult;
		}

		private static List<IInstance> CreateLibraryContentsInstances(Blif blif, ITextViewElementsFactory edifFactory,
			EdifConstants edifConstants)
		{
			List<IInstance> result = new List<IInstance>();

			List<IInstance> luts = CreateLibraryContentsInstancesLuts(blif, edifFactory, edifConstants);
			result.AddRange(luts);

			List<IInstance> inoutbuffers = CreateLibraryContentsInstancesInOutBuffers(blif, edifFactory, edifConstants);
			result.AddRange(inoutbuffers);

			return result;
		}

		private static List<IInstance> CreateLibraryContentsInstancesLuts(Blif blif, ITextViewElementsFactory edifFactory, 
			EdifConstants edifConstants)
		{
			List<IInstance> result = new List<IInstance>();

			foreach (Function function in blif.Functions)
			{
				string lutName = CreateContentsInstancesLutName(function);

				string cellRefName = CreateGenericLutName(function.Ports.Length - 1);
				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(edifConstants.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(cellRefName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(edifConstants.GenericViewName, cellRef);

				List<IProperty> properties = new List<IProperty>();
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					edifConstants.PropertyOwner, true);
				properties.Add(xstlibProperty);
				InitFuncValue initFunctionValue = function.CalculateInit();
				IProperty initProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.INIT,
					edifConstants.PropertyOwner, initFunctionValue.ToString());
				properties.Add(initProperty);

				IInstance lutInstance = edifFactory.CreateInstance(lutName, viewRef, properties);

				result.Add(lutInstance);
			}

			return result;
		}

		private static string CreateContentsInstancesLutName(Function function)
		{
			return "LUT_" + function.OutputPort.Name;
		}

		private static List<IInstance> CreateLibraryContentsInstancesInOutBuffers(Blif blif, ITextViewElementsFactory edifFactory,
			EdifConstants edifConstants)
		{
			List<IInstance> buffersResult = new List<IInstance>();

			int renamedIndex = 0;
			foreach (Input input in blif.Inputs.InputList)
			{
				string oldNameIbuf = GetInstanceIbufName(input);
				string renamedIbufName = CreateRenamedName(oldNameIbuf, renamedIndex++);

				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(edifConstants.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(edifConstants.GenericIbufName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(edifConstants.GenericViewName, cellRef);
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					edifConstants.PropertyOwner, true);
				IInstance ibufInstance = edifFactory.CreateInstance(oldNameIbuf, renamedIbufName, viewRef, new List<IProperty>() { xstlibProperty });

				buffersResult.Add(ibufInstance);
			}

			foreach (Output output in blif.Outputs.OutputList)
			{
				string oldNameObuf = GetInstanceObufName(output);
				string renamedIbufName = CreateRenamedName(oldNameObuf, renamedIndex++);

				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(edifConstants.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(edifConstants.GenericObufName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(edifConstants.GenericViewName, cellRef);
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					edifConstants.PropertyOwner, true);
				IInstance obufInstance = edifFactory.CreateInstance(oldNameObuf, renamedIbufName, viewRef, new List<IProperty>() { xstlibProperty });

				buffersResult.Add(obufInstance);
			}

			return buffersResult;
		}

		private static string CreateRenamedName(string oldNameIbuf, int i)
		{
			return oldNameIbuf + "_renamed_" + i;
		}

		private static string GetInstanceObufName(Output output)
		{
			return output.Name + "_OBUF";
		}

		private static string GetInstanceIbufName(Input input)
		{
			return input.Name + "_IBUF";
		}

		private static List<INet> CreateLibraryContentsNets(Blif blif, ITextViewElementsFactory edifFactory,
			List<IInstance> instances, HashSet<string> sanitizedNames)
		{
			List<INet> netResult = new List<INet>();

			List<INet> ibufNets = CreateIbufNets(blif, edifFactory, instances);
			List<INet> obufNets = CreateObufNets(blif, edifFactory, instances);
			List<INet> lutNets = CreateLutNets(blif, edifFactory, sanitizedNames);
			List<INet> selfInOutNets = CreateSelfInOutNet(blif, edifFactory, instances, sanitizedNames);

			netResult.AddRange(ibufNets);
			netResult.AddRange(obufNets);
			netResult.AddRange(lutNets);
			netResult.AddRange(selfInOutNets);

			return netResult;
		}

		private static List<INet> CreateSelfInOutNet(Blif blif, ITextViewElementsFactory edifFactory, List<IInstance> instances, HashSet<string> sanitizedNames)
		{
			List<INet> netsResult = new List<INet>();

			foreach (Input input in blif.Inputs.InputList)
			{
				string ibufNetName = input.Name;
				string instanceIbufName = GetInstanceIbufName(input);

				List<IPortRef> portRefs = new List<IPortRef>();

				IPortRef selfPort = edifFactory.CreatePortRef(ibufNetName, null);
				portRefs.Add(selfPort);

				IInstance instance = instances.First(ins => ins.Name.Equals(instanceIbufName, StringComparison.InvariantCulture));
				IInstanceRef instanceRef = edifFactory.CreateInstanceRef(instance.RenamedSynonym);
				IPortRef outPortRef = edifFactory.CreatePortRef("I", instanceRef);
				portRefs.Add(outPortRef);

				INet net = edifFactory.CreateNet(ibufNetName, portRefs);
				netsResult.Add(net);
			}

			foreach(Output output in blif.Outputs.OutputList)
			{
				string obufNetName = output.Name;
				string instanceObufName = GetInstanceObufName(output);

				List<IPortRef> portRefs = new List<IPortRef>();

				IPortRef selfPort = edifFactory.CreatePortRef(obufNetName, null);
				portRefs.Add(selfPort);

				IInstance instance = instances.First(ins => ins.Name.Equals(instanceObufName, StringComparison.InvariantCulture));
				IInstanceRef instanceRef = edifFactory.CreateInstanceRef(instance.RenamedSynonym);
				IPortRef outPortRef = edifFactory.CreatePortRef("O", instanceRef);
				portRefs.Add(outPortRef);

				INet net = edifFactory.CreateNet(obufNetName, portRefs);
				netsResult.Add(net);
			}

			return netsResult;
		}

		private static List<INet> CreateIbufNets(Blif blif, ITextViewElementsFactory edifFactory,
			List<IInstance> instances)
		{
			List<INet> ibufNetsResult = new List<INet>();

			foreach (Input input in blif.Inputs.InputList)
			{
				string ibufNetName = GetInstanceIbufName(input);

				List<IPortRef> portRefs = new List<IPortRef>();

				foreach (Function function in blif.Functions)
				{
					for (int i = 0; i < function.Ports.Length - 1; i++) //function.Ports.Length - 1 -> "-1" without OutputPort??
					{
						Port blifFunctionInputPort = function.Ports[i];
						if (blifFunctionInputPort.Name.Equals(input.Name, StringComparison.CurrentCulture))
						{
							string lutName = CreateContentsInstancesLutName(function);
							IInstanceRef instancePortRef = edifFactory.CreateInstanceRef(lutName);
							string portRefName = "I" + (function.Ports.Length - i - 1 - 1); //input index from end ("-i" - from end; "-2" - minus 1 for index and minus 1 for output port)
							IPortRef inPortRef = edifFactory.CreatePortRef(portRefName, instancePortRef);
							portRefs.Add(inPortRef);
						}
					}
				}

				IInstance ibufInstance = instances.First(ins => ins.Name.Equals(ibufNetName, StringComparison.InvariantCulture));
				IInstanceRef outInstancePortRef = edifFactory.CreateInstanceRef(ibufInstance.RenamedSynonym);
				IPortRef outPortRef = edifFactory.CreatePortRef("O", outInstancePortRef);
				portRefs.Add(outPortRef);

				INet net = edifFactory.CreateNet(ibufNetName, portRefs);
				ibufNetsResult.Add(net);
			}

			return ibufNetsResult;
		}

		private static List<INet> CreateObufNets(Blif blif, ITextViewElementsFactory edifFactory,
			List<IInstance> instances)
		{
			List<INet> obufNetsResult = new List<INet>();

			foreach (Output output in blif.Outputs.OutputList)
			{
				string obufNetName = GetInstanceObufName(output);

				List<IPortRef> portRefs = new List<IPortRef>();

				foreach (Function function in blif.Functions)
				{
					for (int i = 0; i < function.Ports.Length - 1; i++) //function.Ports.Length - 1 -> "-1" without OutputPort??
					{
						Port blifFunctionInputPort = function.Ports[i];
						if (blifFunctionInputPort.Name.Equals(output.Name, StringComparison.CurrentCulture))
						{
							string lutName = CreateContentsInstancesLutName(function);
							IInstanceRef instancePortRef = edifFactory.CreateInstanceRef(lutName);
							string portRefName = "I" + (function.Ports.Length - i - 1 - 1); //input index from end ("-i" - from end; "-2" - minus 1 for index and minus 1 for output port)
							IPortRef inPortRef = edifFactory.CreatePortRef(portRefName, instancePortRef);
							portRefs.Add(inPortRef);
						}
					}

					if (function.OutputPort.Name.Equals(output.Name))
					{
						string lutName = CreateContentsInstancesLutName(function);
						IInstanceRef instancePortRef = edifFactory.CreateInstanceRef(lutName);
						string portRefName = "O"; 
						IPortRef inPortRef = edifFactory.CreatePortRef(portRefName, instancePortRef);
						portRefs.Add(inPortRef);
					}
				}

				IInstance ibufInstance = instances.First(ins => ins.Name.Equals(obufNetName, StringComparison.InvariantCulture));
				IInstanceRef outInstancePortRef = edifFactory.CreateInstanceRef(ibufInstance.RenamedSynonym);
				IPortRef outPortRef = edifFactory.CreatePortRef("I", outInstancePortRef);
				portRefs.Add(outPortRef);

				INet net = edifFactory.CreateNet(obufNetName, portRefs);
				obufNetsResult.Add(net);
			}

			return obufNetsResult;
		}

		private static List<INet> CreateLutNets(Blif blif, ITextViewElementsFactory edifFactory,
			HashSet<string> sanitizedName)
		{
			List <INet> result = new List<INet>();

			foreach (Function netFunction in blif.Functions)
			{
				if(blif.Outputs.OutputList.Any(o => o.Name.Equals(netFunction.OutputPort.Name)))
					continue; //This <net> must be created in 'CreateObufNets(Blif blif, ITextViewElementsFactory edifFactory, List<IInstance> instances)' method
				string netName = netFunction.OutputPort.Name;
				if (char.IsNumber(netName[0]))
				{
					netName = "x" + netName;
					while (sanitizedName.Contains(netName, StringComparer.InvariantCultureIgnoreCase))
						netName = "x" + netName;
				}
				sanitizedName.Add(netName);

				List<IPortRef> portRefs = new List<IPortRef>();

				foreach (Function functionSearch in blif.Functions)
				{
					for (int i = 0; i < functionSearch.Ports.Length - 1; i++) //function.Ports.Length - 1 -> "-1" without OutputPort??
					{
						Port blifFunctionInputPort = functionSearch.Ports[i];
						if (blifFunctionInputPort.Name.Equals(netFunction.OutputPort.Name, StringComparison.CurrentCulture))
						{
							string lutName = CreateContentsInstancesLutName(functionSearch);
							IInstanceRef instancePortRef = edifFactory.CreateInstanceRef(lutName);
							string portRefName = "I" + (functionSearch.Ports.Length - i - 1 - 1); //input index from end ("-i" - from end; "-2" - minus 1 for index and minus 1 for output port)
							IPortRef inPortRef = edifFactory.CreatePortRef(portRefName, instancePortRef);
							portRefs.Add(inPortRef);
						}
					}

					if (functionSearch.OutputPort.Name.Equals(netFunction.OutputPort.Name))
					{
						string lutName = CreateContentsInstancesLutName(functionSearch);
						IInstanceRef instancePortRef = edifFactory.CreateInstanceRef(lutName);
						string portRefName = "O";
						IPortRef inPortRef = edifFactory.CreatePortRef(portRefName, instancePortRef);
						portRefs.Add(inPortRef);
					}
				}

				INet net = edifFactory.CreateNet(netName, portRefs);
				result.Add(net);
			}

			return result;
		}

		private static IInterface CreateLibraryinterface(Blif blif, ITextViewElementsFactory edifFactory,
			EdifConstants edifConstants)
		{
			List<IPort> ports = blif.Inputs.InputList.Select(input => edifFactory.CreatePort(input.Name, PortDirection.INPUT)).ToList();
			ports.AddRange(blif.Outputs.OutputList.Select(output => edifFactory.CreatePort(output.Name, PortDirection.OUTPUT)));

			List<IProperty> properties = new List<IProperty>
			{
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.TYPE, edifConstants.PropertyOwner, edifConstants.ModelName),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.KEEP_HIERARCHY, edifConstants.PropertyOwner, "TRUE"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.SHREG_MIN_SIZE, edifConstants.PropertyOwner, "2"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.SHREG_EXTRACT_NGC, edifConstants.PropertyOwner, "YES"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_UNIQUE_ID, edifConstants.PropertyOwner, 0),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_MACRO_TAG, edifConstants.PropertyOwner, 0),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_MACRO_ALIAS, edifConstants.PropertyOwner,
					edifConstants.ModelName + "_" + edifConstants.ModelName)
			};

			IInterface @interfaceResult = edifFactory.CreateInterface(ports, edifConstants.LibraryInterfaceDesignator, properties);
			return @interfaceResult;
		}

		private static IList<ICell> GetExternalGenericCells(Blif blif, ITextViewElementsFactory edifFactory,
			EdifConstants edifConstants)
		{
			IEnumerable<int> genericLutSizes = blif.Functions.Select(f => f.Ports.Length - 1).Distinct();
			IEnumerable<ICell> genericLutCells = genericLutSizes.Select(size => CreateGenericLut(edifFactory, size, edifConstants));

			IList<ICell> result = genericLutCells.ToList();

			result.Add(CreateGenericInputCell(edifFactory, edifConstants));
			result.Add(CreateGenericOutputCell(edifFactory, edifConstants));
			return result;
		}

		private static ICell CreateGenericLut(ITextViewElementsFactory edifFactory, int size,
			EdifConstants edifConstants)
		{
			/*(cell LUT3
      (cellType GENERIC)
        (view view_1
          (viewType NETLIST)
          (interface
            (port I0
              (direction INPUT)
            )
            (port I1
              (direction INPUT)
            )
            (port I2
              (direction INPUT)
            )
            (port O
              (direction OUTPUT)
            )
          )
      )
    )*/
			string lutName = CreateGenericLutName(size);
			List<IPort> ports = new List<IPort>();
			for (int i = 0; i < size; i++)
			{
				string inputPortName = CreateImputPortNameInGenericLut(i);
				IPort inputPort = edifFactory.CreatePort(inputPortName, PortDirection.INPUT);
				ports.Add(inputPort);
			}
			IPort outputPort = edifFactory.CreatePort("O", PortDirection.OUTPUT);
			ports.Add(outputPort);
			IInterface inInterface = edifFactory.CreateInterface(ports, null, null);
			IView view = edifFactory.CreateView(edifConstants.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(lutName, CellType.GENERIC, view);
			return resultCell;
		}

		private static string CreateGenericLutName(int size)
		{
			return "LUT" + size.ToString();
		}

		private static string CreateImputPortNameInGenericLut(int index)
		{
			return "I" + index.ToString();
		}

		private static ICell CreateGenericOutputCell(ITextViewElementsFactory edifFactory,
			EdifConstants edifConstants)
		{
			/*(cell OBUF
      (cellType GENERIC)
        (view view_1
          (viewType NETLIST)
          (interface
            (port I
              (direction INPUT)
            )
            (port O
              (direction OUTPUT)
            )
          )
      )
    )*/
			IPort inputPort = edifFactory.CreatePort("I", PortDirection.INPUT);
			IPort outputPort = edifFactory.CreatePort("O", PortDirection.OUTPUT);
			IInterface inInterface = edifFactory.CreateInterface(new List<IPort>() { inputPort, outputPort }, null, null);
			IView view = edifFactory.CreateView(edifConstants.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(edifConstants.GenericObufName, CellType.GENERIC, view);
			return resultCell;
		}

		private static ICell CreateGenericInputCell(ITextViewElementsFactory edifFactory,
			EdifConstants edifConstants)
		{
			/*(cell IBUF
      (cellType GENERIC)
        (view view_1
          (viewType NETLIST)
          (interface
            (port I
              (direction INPUT)
            )
            (port O
              (direction OUTPUT)
            )
          )
      )
    )*/
			IPort inputPort = edifFactory.CreatePort("I", PortDirection.INPUT);
			IPort outputPort = edifFactory.CreatePort("O", PortDirection.OUTPUT);
			IInterface inInterface = edifFactory.CreateInterface(new List<IPort>() {inputPort, outputPort}, null, null);
			IView view = edifFactory.CreateView(edifConstants.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(edifConstants.GenericIbufName, CellType.GENERIC, view);
			return resultCell;
		}


		private static string GetLibraryName(Blif blif, EdifConstants edifConstants)
		{
			return edifConstants.ModelName + "_lib";
		}
	}
}
