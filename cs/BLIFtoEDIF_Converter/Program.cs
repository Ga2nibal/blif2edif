using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BLIFtoEDIF_Converter.InitCalculator;
using BLIFtoEDIF_Converter.Logic.Model.Blif.Function;

namespace BLIFtoEDIF_Converter
{
	class Program
	{
		public static string InputArguments =
			@"Input arguments:
0: blif file path
1: output file path(rewrite mode). [Optional] default: [0path].init";

		static void Main(string[] args)
		{
			try
			{
				if (null == args || args.Length < 1)
					throw new ArgumentNullException(nameof(args),
						$"{nameof(args)} is incorrect. {Environment.NewLine} {InputArguments}");

				string inputFilePath = args[0];
				string outputFilePath;
				if (args.Length > 1)
					outputFilePath = args[1];
				else
					outputFilePath = Path.ChangeExtension(inputFilePath, ".init");

				if (!File.Exists(inputFilePath))
					throw new FileNotFoundException($"can not find file by requested path '{inputFilePath}'");

				if (!TryConvert(inputFilePath, outputFilePath, Encoding.Default))
					if (!TryConvert(inputFilePath, outputFilePath, Encoding.Unicode))
						throw new Exception("Can not convert file. Contact the developerю");
				Console.WriteLine("Completed");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex.Message}");
			}
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}

		private static bool TryConvert(string inputFilePath, string outputFilePath,
			Encoding encoding)
		{
			try
			{
				string[] lines = File.ReadAllLines(inputFilePath, encoding);

				List<Function> functions = FunctionParser.GetFunctions(lines);

				List<InitFuncValue> initValues = functions.Select(f => f.CalculateInit()).ToList();
				List<string> stringResults = initValues.Select(iv => iv.ToString()).ToList();

				File.WriteAllLines(outputFilePath, stringResults);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception. Encoding:{encoding}. Message: {ex.Message}");
				return false;
			}
			return true;
		}
	}
}
