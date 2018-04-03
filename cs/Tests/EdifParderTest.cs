using System;
using System.Collections.Generic;
using BLIFtoEDIF_Converter.Logic.Parser.Edif;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl;

namespace Tests
{
	[TestClass]
	public class EdifParderTest
	{
		[TestMethod]
		public void ParseEdifTest()
		{
			string edifSrc = GetEmbeddedResouceSrc("Tests.DataFiles._02_Adder_7lut.adder_as_mod.edif");
			//var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			IEdif edif = new EdifParser().GetEdif(edifSrc);

			Assert.IsNotNull(edif);

			Assert.IsNotNull(edif.Name);
			Assert.AreEqual("adder_as_mod", edif.Name);
			
		}

		[TestMethod]
		public void ParseAdderAsEdifTest()
		{
			string edifSrc = GetEmbeddedResouceSrc("Tests.DataFiles.AdderAs.adder_as.edif");
			//var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			IEdif edif = new EdifParser()
			{
				RenameDictionary =
				{
					{"C00", "C0_R0"},
					{"C11", "C1_R1"},
					{"LUT_C00", "LUT_C0_R0" },
					{"LUT_C11", "LUT_C1_R1" },
					{"x10_IBUF_renamed_2", "x10_IBUF_renamed_4" },
					{"x11_IBUF_renamed_3", "x11_IBUF_renamed_5" },
					{"x20_IBUF_renamed_4", "x20_IBUF_renamed_2" },
					{"x21_IBUF_renamed_5", "x21_IBUF_renamed_3" },
					{"C00_OBUF", "C0_R0_OBUF" },
					{"C00_OBUF_renamed_6", "C0_R0_OBUF_renamed_8" },
					{"C11_OBUF", "C1_R1_OBUF" },
					{"C11_OBUF_renamed_7", "C1_R1_OBUF_renamed_9" },
					{"z0_OBUF_renamed_8", "z0_OBUF_renamed_6" },
					{"z1_OBUF_renamed_9", "z1_OBUF_renamed_7" },
				}
			}.GetEdif(edifSrc);

			//((Edif)edif).Status = new Status(new Written(DateTime.Now, new List<IComment>()
			//{
			//	new Comment("Do we need it in converter?")
			//}));
			BlifToEdifConverterBaseTest.IsValidAdderAsEdif(edif);

		}

		[TestMethod]
		public void TwoParserResultEqualsOfAdderAsEdifTest()
		{
			string edifSrc = GetEmbeddedResouceSrc("Tests.DataFiles.AdderAs.adder_as.edif");
			//var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			IEdif edif = new EdifParser().GetEdif(edifSrc);
			IEdif edif2 = new EdifParser().GetEdif(edifSrc);

			//((Edif)edif).Status = new Status(new Written(DateTime.Now, new List<IComment>()
			//{
			//	new Comment("Do we need it in converter?")
			//}));
			Assert.IsNotNull(edif);
			Assert.AreEqual(edif, edif2);
		}

		[TestMethod]
		public void DebugDiff01_Adder_12lutTest()
		{
			string edifSrc = GetEmbeddedResouceSrc("Tests.DataFiles._01_Adder_12lut.adder_as_mainX.edif");
			string edif2Src = GetEmbeddedResouceSrc("Tests.DataFiles._01_Adder_12lut.adder_as1.edif");
			//var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			IEdif edif = new EdifParser().GetEdif(edifSrc);
			IEdif edif2 = new EdifParser()
			{
				RenameDictionary =
				{
					{"C0_R0", "C00"},
					{"C1_R1", "C11"},
					{"adder_as1_lib", "adder_as_main_lib" },
					{"adder_as1", "adder_as_main" },
					{"adder_as1_adder_as1", "adder_as_main_adder_as_main" },
					{"C0_R0_OBUF", "C00_OBUF" },
					{"C1_R1_OBUF", "C11_OBUF" },
					{"C0_R0_OBUF_renamed_8", "C00_OBUF_renamed_6" },
					{"C1_R1_OBUF_renamed_9", "C11_OBUF_renamed_7" },
					{"LUT_C0_R0", "LUT_C00" },
					{"LUT_C1_R1", "LUT_C11" },
					{"x10_IBUF_renamed_4", "x10_IBUF_renamed_2" },
					{"x11_IBUF_renamed_5", "x11_IBUF_renamed_3" },
					{"x20_IBUF_renamed_2", "x20_IBUF_renamed_4" },
					{"x21_IBUF_renamed_3", "x21_IBUF_renamed_5" },
					{"z0_OBUF_renamed_6", "z0_OBUF_renamed_8" },
					{"z1_OBUF_renamed_7", "z1_OBUF_renamed_9" },
				}
				// [C0 => C0_R0] [C1 => C1_R1]
			}.GetEdif(edif2Src);

			//((Edif)edif).Status = new Status(new Written(DateTime.Now, new List<IComment>()
			//{
			//	new Comment("Do we need it in converter?")
			//}));
			Assert.IsNotNull(edif);
			Assert.AreEqual(edif, edif2);
		}

		private static string GetEmbeddedResouceSrc(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream, Encoding.Default))
			{
				string result = reader.ReadToEnd();
				return result;
			}
		}
	}
}
