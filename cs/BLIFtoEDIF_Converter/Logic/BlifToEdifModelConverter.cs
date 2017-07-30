using System;
using System.Collections.Generic;
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
		private static class DummyEdifConstants
		{
			public const int EdifLevel = 0;
			public const int EdifExternalLevel = 0;
			public const int EdifLibraryLevel = 0;

			public const int EdifMajorVersion = 2;
			public const int EdifMidVersion = 0;
			public const int EdifMinorVersion = 0;

			public const int KeywordMapLevel = 0;

			public const string StatusWrittenComment = "Do we need this in converter?";

			public const string ExternalName = "UNISIMS";

			public const string TechnologyExternalName = "numberDefinition";
			public const string TechnologyLibraryName = "numberDefinition";

			public const string DesignPropertyStringValue = "xc6slx4-3-tqg144";

			public const string PropertyOwner = "Xilinx";

			public const string GenericIbufName = "IBUF";
			public const string GenericObufName = "OBUF";
			public const string GenericViewName = "view_1";
			public const string LibraryInterfaceDesignator = "xc6slx4-3-tqg144";
		}

		public static IEdif ToEdif(this Blif blif, ITextViewElementsFactory edifFactory, out string renameLog)
		{
			if (null == blif)
				throw new ArgumentNullException(nameof(blif), $"{nameof(blif)} is not defined");
			if (null == edifFactory)
				throw new ArgumentNullException(nameof(edifFactory), $"{nameof(edifFactory)} is not defined");
			RenamePortsInEdifConvention(blif, out renameLog);
			IEdifVersion edifVersion = edifFactory.CreateEdifVersion(DummyEdifConstants.EdifMajorVersion,
				DummyEdifConstants.EdifMidVersion,
				DummyEdifConstants.EdifMinorVersion);
			IEdifLevel edifLevel = edifFactory.CreateEdifLevel(DummyEdifConstants.EdifLevel);
			IKeywordMap keywordMap = edifFactory.CreateKeywordMap(DummyEdifConstants.KeywordMapLevel);

			IWritten written = edifFactory.CreateWritten(DateTime.Now,
				new List<IComment>() {edifFactory.CreateComment(DummyEdifConstants.StatusWrittenComment)});
			IStatus status = edifFactory.CreateStatus(written);

			IEdifLevel edifExternalLevel = edifFactory.CreateEdifLevel(DummyEdifConstants.EdifExternalLevel);
			ITechnology externalTechnology = edifFactory.CreateTechnology(DummyEdifConstants.TechnologyExternalName);
			IList<ICell> externalGenericCells = GetExternalGenericCells(blif, edifFactory);
			IExternal external = edifFactory.CreateExternal(DummyEdifConstants.ExternalName, edifExternalLevel,
				externalTechnology,
				externalGenericCells);

			string libraryName = GetLibraryName(blif);
			IEdifLevel edifLibraryLevel = edifFactory.CreateEdifLevel(DummyEdifConstants.EdifLibraryLevel);
			ITechnology libraryTechnology = edifFactory.CreateTechnology(DummyEdifConstants.TechnologyLibraryName);
			ICell libraryCell = GetLibraryCell(blif, edifFactory);
			ILibrary library = edifFactory.CreateLibrary(libraryName, edifLibraryLevel, libraryTechnology, libraryCell);

			ILibraryRef libraryRef = edifFactory.CreateLibraryRef(libraryName);
			ICellRef cellRef = edifFactory.CreateCellRef(blif.Model.Name, libraryRef);
			IPropertyValue propertyValue = edifFactory.CreatePropertyValue(DummyEdifConstants.DesignPropertyStringValue,
				PropertyValueType.String);
			IProperty property = edifFactory.CreateProperty(PropertyType.PART, propertyValue, DummyEdifConstants.PropertyOwner);
			IDesign design = edifFactory.CreateDesign(blif.Model.Name, new List<ICellRef>() {cellRef},
				new List<IProperty>() {property});

			IEdif result = edifFactory.CreateEdif(blif.Model.Name, edifVersion, edifLevel, keywordMap, status,
				external, library, design);

			return result;
		}

		private static void RenamePortsInEdifConvention(Blif blif, out string renameLog)
		{
			StringBuilder renameLogBuilder = new StringBuilder();

			List<string> allPortNames = new List<string>();
			allPortNames.AddRange(blif.Inputs.InputList.Select(i => i.Name));
			allPortNames.AddRange(blif.Outputs.OutputList.Select(o => o.Name));
			allPortNames.AddRange(blif.Functions.SelectMany(f => f.Ports).Select(i => i.Name));
			
			HashSet<string> validLowerCaseDistinctName = new HashSet<string>(allPortNames.Where(n => !n.Any(c => char.IsUpper(c)))
				.Where(n => IsValidEdifPortName(n)).Distinct());
			HashSet<string> allSanitizedPortNames = new HashSet<string>(validLowerCaseDistinctName);
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

		private static ICell GetLibraryCell(Blif blif, ITextViewElementsFactory edifFactory)
		{
			IInterface @interface = CreateLibraryinterface(blif, edifFactory);
			IContents contents = CreateLibraryContents(blif, edifFactory);
			IView view = edifFactory.CreateView(DummyEdifConstants.GenericViewName, ViewType.NETLIST, @interface, contents);
			ICell cellResult = edifFactory.CreateCell(blif.Model.Name, CellType.GENERIC, view);
			return cellResult;
		}

		private static IContents CreateLibraryContents(Blif blif, ITextViewElementsFactory edifFactory)
		{
			List<IInstance> instances = CreateLibraryContentsInstances(blif, edifFactory);
			List<INet> nets = CreateLibraryContentsNets(blif, edifFactory, instances);

			IContents contentsResult = edifFactory.CreateContents(instances, nets);
			return contentsResult;
		}

		private static List<IInstance> CreateLibraryContentsInstances(Blif blif, ITextViewElementsFactory edifFactory)
		{
			List<IInstance> result = new List<IInstance>();

			List<IInstance> luts = CreateLibraryContentsInstancesLuts(blif, edifFactory);
			result.AddRange(luts);

			List<IInstance> inoutbuffers = CreateLibraryContentsInstancesInOutBuffers(blif, edifFactory);
			result.AddRange(inoutbuffers);

			return result;
		}

		private static List<IInstance> CreateLibraryContentsInstancesLuts(Blif blif, ITextViewElementsFactory edifFactory)
		{
			List<IInstance> result = new List<IInstance>();

			foreach (Function function in blif.Functions)
			{
				string lutName = CreateContentsInstancesLutName(function);

				string cellRefName = CreateGenericLutName(function.Ports.Length - 1);
				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(DummyEdifConstants.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(cellRefName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(DummyEdifConstants.GenericViewName, cellRef);

				List<IProperty> properties = new List<IProperty>();
				InitFuncValue initFunctionValue = function.CalculateInit();
				IProperty initProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.INIT,
					DummyEdifConstants.PropertyOwner, initFunctionValue.ToString());
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

		private static List<IInstance> CreateLibraryContentsInstancesInOutBuffers(Blif blif, ITextViewElementsFactory edifFactory)
		{
			List<IInstance> buffersResult = new List<IInstance>();

			int renamedIndex = 0;
			foreach (Input input in blif.Inputs.InputList)
			{
				string oldNameIbuf = GetInstanceIbufName(input);
				string renamedIbufName = CreateRenamedName(oldNameIbuf, renamedIndex++);

				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(DummyEdifConstants.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(DummyEdifConstants.GenericIbufName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(DummyEdifConstants.GenericViewName, cellRef);
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					DummyEdifConstants.PropertyOwner, true);
				IInstance ibufInstance = edifFactory.CreateInstance(oldNameIbuf, renamedIbufName, viewRef, new List<IProperty>() { xstlibProperty });

				buffersResult.Add(ibufInstance);
			}

			foreach (Output output in blif.Outputs.OutputList)
			{
				string oldNameObuf = GetInstanceObufName(output);
				string renamedIbufName = CreateRenamedName(oldNameObuf, renamedIndex++);

				ILibraryRef libraryRef = edifFactory.CreateLibraryRef(DummyEdifConstants.ExternalName);
				ICellRef cellRef = edifFactory.CreateCellRef(DummyEdifConstants.GenericObufName, libraryRef);
				IViewRef viewRef = edifFactory.CreateViewRef(DummyEdifConstants.GenericViewName, cellRef);
				IProperty xstlibProperty = EdifHelper.CreateEdifProperty(edifFactory, PropertyType.XSTLIB,
					DummyEdifConstants.PropertyOwner, true);
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
			List<IInstance> instances)
		{
			List<INet> netResult = new List<INet>();

			List<INet> ibufNets = CreateIbufNets(blif, edifFactory, instances);
			List<INet> obufNets = CreateObufNets(blif, edifFactory, instances);
			List<INet> lutNets = CreateLutNets(blif, edifFactory, instances);

			netResult.AddRange(ibufNets);
			netResult.AddRange(obufNets);
			netResult.AddRange(lutNets);

			return netResult;
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
			List<IInstance> instances)
		{
			List < INet > result = new List<INet>();

			foreach (Function netFunction in blif.Functions)
			{
				if(blif.Outputs.OutputList.Any(o => o.Name.Equals(netFunction.OutputPort.Name)))
					continue; //This <net> must be created in 'CreateObufNets(Blif blif, ITextViewElementsFactory edifFactory, List<IInstance> instances)' method
				string netName = netFunction.OutputPort.Name;
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

		private static IInterface CreateLibraryinterface(Blif blif, ITextViewElementsFactory edifFactory)
		{
			List<IPort> ports = blif.Inputs.InputList.Select(input => edifFactory.CreatePort(input.Name, PortDirection.INPUT)).ToList();
			ports.AddRange(blif.Outputs.OutputList.Select(output => edifFactory.CreatePort(output.Name, PortDirection.OUTPUT)));

			List<IProperty> properties = new List<IProperty>
			{
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.TYPE, DummyEdifConstants.PropertyOwner, blif.Model.Name),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.KEEP_HIERARCHY, DummyEdifConstants.PropertyOwner, "TRUE"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.SHREG_MIN_SIZE, DummyEdifConstants.PropertyOwner, "2"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.SHREG_EXTRACT_NGC, DummyEdifConstants.PropertyOwner, "YES"),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_UNIQUE_ID, DummyEdifConstants.PropertyOwner, 0),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_MACRO_TAG, DummyEdifConstants.PropertyOwner, 0),
				EdifHelper.CreateEdifProperty(edifFactory, PropertyType.NLW_MACRO_ALIAS, DummyEdifConstants.PropertyOwner,
					blif.Model.Name + "_" + blif.Model.Name)
			};

			IInterface @interfaceResult = edifFactory.CreateInterface(ports, DummyEdifConstants.LibraryInterfaceDesignator, properties);
			return @interfaceResult;
		}

		private static IList<ICell> GetExternalGenericCells(Blif blif, ITextViewElementsFactory edifFactory)
		{
			IEnumerable<int> genericLutSizes = blif.Functions.Select(f => f.Ports.Length - 1).Distinct();
			IEnumerable<ICell> genericLutCells = genericLutSizes.Select(size => CreateGenericLut(edifFactory, size));

			IList<ICell> result = genericLutCells.ToList();

			result.Add(CreateGenericInputCell(edifFactory));
			result.Add(CreateGenericOutputCell(edifFactory));
			return result;
		}

		private static ICell CreateGenericLut(ITextViewElementsFactory edifFactory, int size)
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
			IView view = edifFactory.CreateView(DummyEdifConstants.GenericViewName, ViewType.NETLIST, inInterface, null);
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

		private static ICell CreateGenericOutputCell(ITextViewElementsFactory edifFactory)
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
			IView view = edifFactory.CreateView(DummyEdifConstants.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(DummyEdifConstants.GenericObufName, CellType.GENERIC, view);
			return resultCell;
		}

		private static ICell CreateGenericInputCell(ITextViewElementsFactory edifFactory)
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
			IView view = edifFactory.CreateView(DummyEdifConstants.GenericViewName, ViewType.NETLIST, inInterface, null);
			ICell resultCell = edifFactory.CreateCell(DummyEdifConstants.GenericIbufName, CellType.GENERIC, view);
			return resultCell;
		}


		private static string GetLibraryName(Blif blif)
		{
			return blif.Model.Name + "_lib";
		}
	}
}
