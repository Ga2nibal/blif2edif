using BLIFtoEDIF_Converter.Logic.Parser.Edif;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Text;

namespace Tests
{
	[TestClass]
	public class EdifParderTest
	{
		[TestMethod]
		public void ParseEdifTest()
		{
			string edifSrc = GetEmbeddedResouceSrc("Tests.DataFiles.02_Adder_7lut.adder_as_mod.edif");
			//var srcLines = blifSrc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			IEdif edif = EdifParser.GetEdif(edifSrc);

			Assert.IsNotNull(edif);

			Assert.IsNotNull(edif.Name);
			Assert.AreEqual("adder_as_mod", edif.Name);
			
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
