using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BLIFtoEDIF_Converter.Logic.InitCalculator;
using BLIFtoEDIF_Converter.Model.Blif;
using BLIFtoEDIF_Converter.Model.Blif.Function;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Cell;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Library;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Property;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.View;
using BLIFtoEDIF_Converter.Model.Edif.Factory;
using BLIFtoEDIF_Converter.Util;
using PortDirection = BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port.PortDirection;

namespace BLIFtoEDIF_Converter.Logic
{
	public static class BlifToEdifModelConverter
	{
		public class EdifAdditionalData
		{
			public EdifAdditionalData(string modelName)
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

			private string _designPropertyValue;

			public string DesignPropertyStringValue
			{
				get { return _designPropertyValue ?? $"{Device}{Speed}-{Package}"; }
				set { _designPropertyValue = value; }
			}

			public string PropertyOwner { get; set; } = "Xilinx";

			public string GenericIbufName { get; set; } = "IBUF";
			public string GenericObufName { get; set; } = "OBUF";
			public string GenericViewName { get; set; } = "view_1";

			private string _libraryInterfaceDesignator;
			public string LibraryInterfaceDesignator
			{
				get { return _libraryInterfaceDesignator ?? $"{Device}{Speed}-{Package}"; }
				set { _libraryInterfaceDesignator = value; }
			}

			public string Device { get; set; } = "xc6slx4";
			public string Package { get; set; } = "tqg144";
			public string Speed { get; set; } = "-3";

			public DateTime StatusWrittenTimestamp { get; set; } = DateTime.Now;
		}

		public static IEdif ToEdif(this Blif blif, ITextViewElementsFactory edifFactory, 
			EdifAdditionalData edifAdditionalData, out string renameLog)
		{
			if (null == blif)
				throw new ArgumentNullException(nameof(blif), $"{nameof(blif)} is not defined");
			if (null == edifFactory)
				throw new ArgumentNullException(nameof(edifFactory), $"{nameof(edifFactory)} is not defined");
			HashSet<string> sanitizedNames;
			RenamePortsInEdifConvention(blif, out renameLog, out sanitizedNames);
			IEdifVersion edifVersion = edifFactory.CreateEdifVersion(edifAdditionalData.EdifMajorVersion,
				edifAdditionalData.EdifMidVersion,
				edifAdditionalData.EdifMinorVersion);
			IEdifLevel edifLevel = edifFactory.CreateEdifLevel(edifAdditionalData.ModelEdifLevel);
			IKeywordMap keywordMap = edifFactory.CreateKeywordMap(edifAdditionalData.KeywordMapLevel);

			IWritten written = edifFactory.CreateWritten(edifAdditionalData.StatusWrittenTimestamp,
				new List<IComment>() {edifFactory.CreateComment("# RenameLog: " + renameLog) });
			IStatus status = edifFactory.CreateStatus(written);

			IEdifLevel edifExternalLevel = edifFactory.CreateEdifLevel(edifAdditionalData.EdifExternalLevel);
			ITechnology externalTechnology = edifFactory.CreateTechnology(edifAdditionalData.TechnologyExternalName);
			IList<ICell> externalGenericCells = GetExternalGenericCells(blif, edifFactory, edifAdditionalData);
			IExternal external = edifFactory.CreateExternal(edifAdditionalData.ExternalName, edifExternalLevel,
				externalTechnology,
				externalGenericCells);

			string libraryName = GetLibraryName(blif, edifAdditionalData);
			IEdifLevel edifLibraryLevel = edifFactory.CreateEdifLevel(edifAdditionalData.EdifLibraryLevel);
			ITechnology libraryTechnology = edifFactory.CreateTechnology(edifAdditionalData.TechnologyLibraryName);
			ICell libraryCell = GetLibraryCell(blif, edifFactory, edifAdditionalData, sanitizedNames);
			ILibrary library = edifFactory.CreateLibrary(libraryName, edifLibraryLevel, libraryTechnology, libraryCell);

			ILibraryRef libraryRef = edifFactory.CreateLibraryRef(libraryName);
			ICellRef cellRef = edifFactory.CreateCellRef(edifAdditionalData.ModelName, libraryRef);
			IPropertyValue propertyValue = edifFactory.CreatePropertyValue(edifAdditionalData.DesignPropertyStringValue,
				PropertyValueType.String);
			IProperty property = edifFactory.CreateProperty(PropertyType.PART, propertyValue, edifAdditionalData.PropertyOwner);
			IDesign design = edifFactory.CreateDesign(edifAdditionalData.ModelName, new List<ICellRef>() {cellRef},
				new List<IProperty>() {property});

			IEdif result = edifFactory.CreateEdif(edifAdditionalData.ModelName, edifVersion, edifLevel, keywordMap, status,
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

		private const string restrictedDotSymbol = ".";
		private static readonly string[] RestrictedEdifPortNameSubStrings = new string[] {"[", "]", "(", ")", restrictedDotSymbol };
		private static string SanitizePortsInEdifConvention(string portBlifName)
		{
			string sanitizedName = portBlifName;
			foreach (string restrictedEdifPortNameSubString in RestrictedEdifPortNameSubStrings)
			{
				if(restrictedDotSymbol.Equals(restrictedEdifPortNameSubString, StringComparison.InvariantCulture))
					sanitizedName = sanitizedName.Replace(restrictedEdifPortNameSubString, "_");
				else
					sanitizedName = sanitizedName.Replace(restrictedEdifPortNameSubString, string.Empty);
			}
			return sanitizedName;
		}

		private static bool IsValidEdifPortName(string portBlifName)
		{
			return RestrictedEdifPortNameSubStrings.All(restrictedEdifPortNameSubString => !portBlifName.Contains(restrictedEdifPortNameSubString));
		}

		private static ICell GetLibraryCell(Blif blif, ITextViewElementsFactory edifFactory, 
			EdifAdditionalData edifAdditionalData, HashSet<string> sanitizedNames)
		{
			IInterface @interface = CreateLibraryinterface(blif, edifFactory, edifAdditionalData);
			IContents contents = CreateLibraryContents(blif, edifFactory, edifAdditionalData, sanitizedNames);
			IView view = edifFactory.CreateView(edifAdditionalData.GenericViewName, ViewType.NETLIST, @interface, contents);
			ICell cellResult = edifFactory.CreateCell(edifAdditionalData.ModelName, CellType.GENERIC, view);
			return cellResult;
		}

		private static IContents CreateLibraryContents(Blif blif, ITextViewElementsFactory edifFactory,
			EdifAdditionalData edifAdditionalData, HashSet<string> sanitizedNames)
		{
			List<IInstance> instances = CreateLibraryContentsInstances(blif, edifFactory, edifAdditionalData);
			List<INet> nets = CreateLibraryContentsNets(blif, edifFactory, instances, sanitizedNames);

			IContents contentsResult = edifFactory.CreateContents(instances, nets);
			return contentsResult;
		}

		private static List<IInstance> CreateLibraryContentsInstances(Blif blif, ITextViewElementsFactory edifFactory,
			EdifAdditionalData edifAdditionalData)
		{
			List<IInstance> result = new List<IInstance>();

			List<IInstance> luts = CreateLibraryContentsInstancesLuts(blif, edifFactory, edifAdditionalData);
			result.AddRange(luts);

			List<IInstance> inoutbuffers = CreateLibraryContentsInstancesInOutBuffers(blif, edifFactory, edifAdditionalData);
			result.AddRange(inoutbuffers);

			return result;
		}

		private static List<IInstance> CreateLibraryContentsInstancesLuts(Blif blif, ITextViewElementsFactory edifFactory, 
			EdifAdditionalData edifAdditionalData)
		{
			List<IInstance> result = new List<IInstance>();

			foreach (Function function in blif.Functions)
			{
				string lutName = CreateContentsInstancesLutName(function);

				string cellRefName = CreateGenericLutName(function);
				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(edifAdditionalData.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(cellRefName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(edifAdditionalData.GenericViewName, cellRef);

				List<IProperty> properties = new List<IProperty>();
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					edifAdditionalData.PropertyOwner, true);
				properties.Add(xstlibProperty);
				if (!function.IsGND && !function.IsVCC)
				{
					InitFuncValue initFunctionValue = function.CalculateInit();
					IProperty initProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.INIT,
						edifAdditionalData.PropertyOwner, initFunctionValue.ToString());
					properties.Add(initProperty);
				}

				IInstance lutInstance = edifFactory.CreateInstance(lutName, viewRef, properties);

				result.Add(lutInstance);
			}

			return result;
		}

		private static string CreateContentsInstancesLutName(Function function)
		{
			if (function.IsGND)
				return "XST_GND";
			if (function.IsVCC)
				return "XST_VCC";

			return "LUT_" + function.OutputPort.Name;
		}

		private static List<IInstance> CreateLibraryContentsInstancesInOutBuffers(Blif blif, ITextViewElementsFactory edifFactory,
			EdifAdditionalData edifAdditionalData)
		{
			List<IInstance> buffersResult = new List<IInstance>();

			int renamedIndex = 0;
			foreach (Input input in blif.Inputs.InputList)
			{
				string oldNameIbuf = GetInstanceIbufName(input);
				string renamedIbufName = CreateRenamedName(oldNameIbuf, renamedIndex++);

				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(edifAdditionalData.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(edifAdditionalData.GenericIbufName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(edifAdditionalData.GenericViewName, cellRef);
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					edifAdditionalData.PropertyOwner, true);
				IInstance ibufInstance = edifFactory.CreateInstance(oldNameIbuf, renamedIbufName, viewRef, new List<IProperty>() { xstlibProperty });

				buffersResult.Add(ibufInstance);
			}

			foreach (Output output in blif.Outputs.OutputList)
			{
				string oldNameObuf = GetInstanceObufName(output);
				string renamedIbufName = CreateRenamedName(oldNameObuf, renamedIndex++);

				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(edifAdditionalData.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(edifAdditionalData.GenericObufName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(edifAdditionalData.GenericViewName, cellRef);
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					edifAdditionalData.PropertyOwner, true);
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
						string portRefName = GetOutputPortName(function);
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
			EdifAdditionalData edifAdditionalData)
		{
			List<IPort> ports = blif.Inputs.InputList.Select(input => edifFactory.CreatePort(input.Name, PortDirection.INPUT)).ToList();
			ports.AddRange(blif.Outputs.OutputList.Select(output => edifFactory.CreatePort(output.Name, PortDirection.OUTPUT)));

			List<IProperty> properties = new List<IProperty>
			{
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.TYPE, edifAdditionalData.PropertyOwner, edifAdditionalData.ModelName),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.KEEP_HIERARCHY, edifAdditionalData.PropertyOwner, "TRUE"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.SHREG_MIN_SIZE, edifAdditionalData.PropertyOwner, "2"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.SHREG_EXTRACT_NGC, edifAdditionalData.PropertyOwner, "YES"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_UNIQUE_ID, edifAdditionalData.PropertyOwner, 0),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_MACRO_TAG, edifAdditionalData.PropertyOwner, 0),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_MACRO_ALIAS, edifAdditionalData.PropertyOwner,
					edifAdditionalData.ModelName + "_" + edifAdditionalData.ModelName)
			};

			IInterface @interfaceResult = edifFactory.CreateInterface(ports, edifAdditionalData.LibraryInterfaceDesignator, properties);
			return @interfaceResult;
		}

		private static IList<ICell> GetExternalGenericCells(Blif blif, ITextViewElementsFactory edifFactory,
			EdifAdditionalData edifAdditionalData)
		{
			HashSet<ICell> genericLutCells = new HashSet<ICell>(blif.Functions.Select(func => CreateGenericLut(edifFactory, func, edifAdditionalData)));

			IList<ICell> result = genericLutCells.ToList();

			result.Add(CreateGenericInputCell(edifFactory, edifAdditionalData));
			result.Add(CreateGenericOutputCell(edifFactory, edifAdditionalData));
			return result;
		}

		private static ICell CreateGenericLut(ITextViewElementsFactory edifFactory, Function func,
			EdifAdditionalData edifAdditionalData)
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
			int size = func.Ports.Length - 1;
			string lutName = CreateGenericLutName(func);
			List<IPort> ports = new List<IPort>();
			for (int i = 0; i < size; i++)
			{
				string inputPortName = CreateImputPortNameInGenericLut(i);
				IPort inputPort = edifFactory.CreatePort(inputPortName, PortDirection.INPUT);
				ports.Add(inputPort);
			}

			var outputPortName = GetOutputPortName(func);

			IPort outputPort = edifFactory.CreatePort(outputPortName, PortDirection.OUTPUT);
			ports.Add(outputPort);
			IInterface inInterface = edifFactory.CreateInterface(ports, null, null);
			IView view = edifFactory.CreateView(edifAdditionalData.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(lutName, CellType.GENERIC, view);
			return resultCell;
		}

		private static string GetOutputPortName(Function function)
		{
			string outputPortName = "O";

			if (function.IsGND)
				outputPortName = "G";
			else if (function.IsVCC)
				outputPortName = "P";

			return outputPortName;
		}

		private static string CreateGenericLutName(Function function)
		{
			if (function.IsGND)
				return "GND";
			else if(function.IsVCC)
				return "VCC";

			var blifFunctionPorts = function.Ports;
			int inputSize = blifFunctionPorts.Length - 1;
			return "LUT" + inputSize.ToString();
		}

		private static string CreateImputPortNameInGenericLut(int index)
		{
			return "I" + index.ToString();
		}

		private static ICell CreateGenericOutputCell(ITextViewElementsFactory edifFactory,
			EdifAdditionalData edifAdditionalData)
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
			IView view = edifFactory.CreateView(edifAdditionalData.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(edifAdditionalData.GenericObufName, CellType.GENERIC, view);
			return resultCell;
		}

		private static ICell CreateGenericInputCell(ITextViewElementsFactory edifFactory,
			EdifAdditionalData edifAdditionalData)
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
			IView view = edifFactory.CreateView(edifAdditionalData.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(edifAdditionalData.GenericIbufName, CellType.GENERIC, view);
			return resultCell;
		}


		private static string GetLibraryName(Blif blif, EdifAdditionalData edifAdditionalData)
		{
			return edifAdditionalData.ModelName + "_lib";
		}
	}
}
