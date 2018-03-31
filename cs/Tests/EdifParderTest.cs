﻿using BLIFtoEDIF_Converter.Logic.Parser.Edif;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class EdifParderTest
	{
		[TestMethod]
		public void TestMethod888()
		{
			string edifSrc = @"(edif adder_as_mod 
	(edifVersion 2 0 0) 
	(edifLevel 0) 
	(keywordMap 
		(keywordLevel 0)
	) 
	(status 
		(written 
			(timestamp 2018 3 26 21 00 22) 
			(comment ""# RenameLog:  [C0 => C0_R0] [C1 => C1_R1]"")
				)
				) 
			(external UNISIMS
				(edifLevel 0)
			(technology
					(numberDefinition)
				)
				(cell LUT5
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
			(port I3
				(direction INPUT)
				) 
			(port I4
				(direction INPUT)
				) 
			(port O
				(direction OUTPUT)
				)
				)
				)
				) 
			(cell LUT3
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
				) 
			(cell LUT6
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
			(port I3
				(direction INPUT)
				) 
			(port I4
				(direction INPUT)
				) 
			(port I5
				(direction INPUT)
				) 
			(port O
				(direction OUTPUT)
				)
				)
				)
				) 
			(cell IBUF
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
				) 
			(cell OBUF
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
				)
				) 
			(library adder_as_mod_lib
				(edifLevel 0)
			(technology
					(numberDefinition)
				) 
				(cell adder_as_mod
				(cellType GENERIC)
			(view view_1
				(viewType NETLIST)
			(interface
			(port c0
				(direction INPUT)
				) 
			(port c1
				(direction INPUT)
				) 
			(port x20
				(direction INPUT)
				) 
			(port x21
				(direction INPUT)
				) 
			(port x10
				(direction INPUT)
				) 
			(port x11
				(direction INPUT)
				) 
			(port z0
				(direction OUTPUT)
				) 
			(port z1
				(direction OUTPUT)
				) 
			(port C0_R0
				(direction OUTPUT)
				) 
			(port C1_R1
				(direction OUTPUT)
				) 
			(designator ""xc6slx4 -3-tqg144"") 
			(property TYPE
				(string ""adder_as_mod"")
			(owner ""Xilinx"")
				) 
			(property KEEP_HIERARCHY
				(string ""TRUE"")
			(owner ""Xilinx"")
				) 
			(property SHREG_MIN_SIZE
				(string ""2"")
			(owner ""Xilinx"")
				) 
			(property SHREG_EXTRACT_NGC
				(string ""YES"")
			(owner ""Xilinx"")
				) 
			(property NLW_UNIQUE_ID
				(integer 0)
			(owner ""Xilinx"")
				) 
			(property NLW_MACRO_TAG
				(integer 0)
			(owner ""Xilinx"")
				) 
			(property NLW_MACRO_ALIAS
				(string ""adder_as_mod_adder_as_mod"")
			(owner ""Xilinx"")
				)
				) 
			(contents
				(instance LUT_f1 
				(viewRef view_1 
				(cellRef LUT5 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				) 
			(property INIT
				(string ""ABAAAAE8"")
			(owner ""Xilinx"")
				)
				) 
			(instance LUT_f2
				(viewRef view_1 
				(cellRef LUT3 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				) 
			(property INIT
				(string ""E8"")
			(owner ""Xilinx"")
				)
				) 
			(instance LUT_f3
				(viewRef view_1 
				(cellRef LUT3 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				) 
			(property INIT
				(string ""E8"")
			(owner ""Xilinx"")
				)
				) 
			(instance LUT_z1
				(viewRef view_1 
				(cellRef LUT6 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				) 
			(property INIT
				(string ""AAABAAAAAAAABEA8"")
			(owner ""Xilinx"")
				)
				) 
			(instance LUT_z0
				(viewRef view_1 
				(cellRef LUT6 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				) 
			(property INIT
				(string ""AAABABAAAAAAAAE8"")
			(owner ""Xilinx"")
				)
				) 
			(instance LUT_C1_R1
				(viewRef view_1 
				(cellRef LUT5 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				) 
			(property INIT
				(string ""ABBAAAE8"")
			(owner ""Xilinx"")
				)
				) 
			(instance LUT_C0_R0
				(viewRef view_1 
				(cellRef LUT5 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				) 
			(property INIT
				(string ""ABAABAE8"")
			(owner ""Xilinx"")
				)
				) 
			(instance
				(rename c0_IBUF_renamed_0 ""c0_IBUF"")
			(viewRef view_1
				(cellRef IBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename c1_IBUF_renamed_1 ""c1_IBUF"")
			(viewRef view_1
				(cellRef IBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename x20_IBUF_renamed_2 ""x20_IBUF"")
			(viewRef view_1
				(cellRef IBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename x21_IBUF_renamed_3 ""x21_IBUF"")
			(viewRef view_1
				(cellRef IBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename x10_IBUF_renamed_4 ""x10_IBUF"")
			(viewRef view_1
				(cellRef IBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename x11_IBUF_renamed_5 ""x11_IBUF"")
			(viewRef view_1
				(cellRef IBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename z0_OBUF_renamed_6 ""z0_OBUF"")
			(viewRef view_1
				(cellRef OBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename z1_OBUF_renamed_7 ""z1_OBUF"")
			(viewRef view_1
				(cellRef OBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename C0_R0_OBUF_renamed_8 ""C0_R0_OBUF"")
			(viewRef view_1
				(cellRef OBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(instance
				(rename C1_R1_OBUF_renamed_9 ""C1_R1_OBUF"")
			(viewRef view_1
				(cellRef OBUF 
				(libraryRef UNISIMS)
				)
				) 
			(property XSTLIB
				(boolean
					(true)
				) 
				(owner ""Xilinx"")
				)
				) 
			(net c0_IBUF
			(joined
				(portRef I5 
				(instanceRef LUT_z1)
				) 
			(portRef I5
				(instanceRef LUT_z0)
				) 
			(portRef I1
				(instanceRef LUT_C1_R1)
				) 
			(portRef I2
				(instanceRef LUT_C0_R0)
				) 
			(portRef O
				(instanceRef c0_IBUF_renamed_0)
				)
				)
				) 
			(net c1_IBUF
			(joined
				(portRef I3 
				(instanceRef LUT_z1)
				) 
			(portRef I2
				(instanceRef LUT_z0)
				) 
			(portRef I4
				(instanceRef LUT_C1_R1)
				) 
			(portRef I4
				(instanceRef LUT_C0_R0)
				) 
			(portRef O
				(instanceRef c1_IBUF_renamed_1)
				)
				)
				) 
			(net x20_IBUF
			(joined
				(portRef I3 
				(instanceRef LUT_f1)
				) 
			(portRef I1
				(instanceRef LUT_f3)
				) 
			(portRef O
				(instanceRef x20_IBUF_renamed_2)
				)
				)
				) 
			(net x21_IBUF
			(joined
				(portRef I1 
				(instanceRef LUT_f1)
				) 
			(portRef I1
				(instanceRef LUT_f2)
				) 
			(portRef O
				(instanceRef x21_IBUF_renamed_3)
				)
				)
				) 
			(net x10_IBUF
			(joined
				(portRef I2 
				(instanceRef LUT_f1)
				) 
			(portRef I2
				(instanceRef LUT_f3)
				) 
			(portRef O
				(instanceRef x10_IBUF_renamed_4)
				)
				)
				) 
			(net x11_IBUF
			(joined
				(portRef I4 
				(instanceRef LUT_f1)
				) 
			(portRef I2
				(instanceRef LUT_f2)
				) 
			(portRef O
				(instanceRef x11_IBUF_renamed_5)
				)
				)
				) 
			(net z0_OBUF
			(joined
				(portRef I0 
				(instanceRef LUT_z0)
				) 
			(portRef O
				(instanceRef LUT_z0)
				) 
			(portRef I
				(instanceRef z0_OBUF_renamed_6)
				)
				)
				) 
			(net z1_OBUF
			(joined
				(portRef I0 
				(instanceRef LUT_z1)
				) 
			(portRef O
				(instanceRef LUT_z1)
				) 
			(portRef I
				(instanceRef z1_OBUF_renamed_7)
				)
				)
				) 
			(net C0_R0_OBUF
			(joined
				(portRef I0 
				(instanceRef LUT_C0_R0)
				) 
			(portRef O
				(instanceRef LUT_C0_R0)
				) 
			(portRef I
				(instanceRef C0_R0_OBUF_renamed_8)
				)
				)
				) 
			(net C1_R1_OBUF
			(joined
				(portRef I0 
				(instanceRef LUT_C1_R1)
				) 
			(portRef O
				(instanceRef LUT_C1_R1)
				) 
			(portRef I
				(instanceRef C1_R1_OBUF_renamed_9)
				)
				)
				) 
			(net f1
			(joined
				(portRef I0 
				(instanceRef LUT_f1)
				) 
			(portRef O
				(instanceRef LUT_f1)
				) 
			(portRef I4
				(instanceRef LUT_z1)
				) 
			(portRef I1
				(instanceRef LUT_z0)
				) 
			(portRef I3
				(instanceRef LUT_C1_R1)
				) 
			(portRef I1
				(instanceRef LUT_C0_R0)
				)
				)
				) 
			(net f2
			(joined
				(portRef I0 
				(instanceRef LUT_f2)
				) 
			(portRef O
				(instanceRef LUT_f2)
				) 
			(portRef I2
				(instanceRef LUT_z1)
				) 
			(portRef I4
				(instanceRef LUT_z0)
				) 
			(portRef I2
				(instanceRef LUT_C1_R1)
				)
				)
				) 
			(net f3
			(joined
				(portRef I0 
				(instanceRef LUT_f3)
				) 
			(portRef O
				(instanceRef LUT_f3)
				) 
			(portRef I1
				(instanceRef LUT_z1)
				) 
			(portRef I3
				(instanceRef LUT_z0)
				) 
			(portRef I3
				(instanceRef LUT_C0_R0)
				)
				)
				) 
			(net c0
			(joined
				(portRef c0)
			(portRef I
				(instanceRef c0_IBUF_renamed_0)
				)
				)
				) 
			(net c1
			(joined
				(portRef c1)
			(portRef I
				(instanceRef c1_IBUF_renamed_1)
				)
				)
				) 
			(net x20
			(joined
				(portRef x20)
			(portRef I
				(instanceRef x20_IBUF_renamed_2)
				)
				)
				) 
			(net x21
			(joined
				(portRef x21)
			(portRef I
				(instanceRef x21_IBUF_renamed_3)
				)
				)
				) 
			(net x10
			(joined
				(portRef x10)
			(portRef I
				(instanceRef x10_IBUF_renamed_4)
				)
				)
				) 
			(net x11
			(joined
				(portRef x11)
			(portRef I
				(instanceRef x11_IBUF_renamed_5)
				)
				)
				) 
			(net z0
			(joined
				(portRef z0)
			(portRef O
				(instanceRef z0_OBUF_renamed_6)
				)
				)
				) 
			(net z1
			(joined
				(portRef z1)
			(portRef O
				(instanceRef z1_OBUF_renamed_7)
				)
				)
				) 
			(net C0_R0
			(joined
				(portRef C0_R0)
			(portRef O
				(instanceRef C0_R0_OBUF_renamed_8)
				)
				)
				) 
			(net C1_R1
			(joined
				(portRef C1_R1)
			(portRef O
				(instanceRef C1_R1_OBUF_renamed_9)
				)
				)
				)
				)
				)
				)
				) 
			(design adder_as_mod
				(cellRef adder_as_mod 
				(libraryRef adder_as_mod_lib)
				) 
			(property PART
				(string ""xc6slx4-3-tqg144"")
			(owner ""Xilinx"")
				)
				)
				)
";
			//var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			IEdif edif = EdifParser.GetEdif(edifSrc);

			Assert.IsNotNull(edif);

			Assert.IsNotNull(edif.Name);
			Assert.AreEqual("adder_as_mod", edif.Name);
			
		}
	}
}
