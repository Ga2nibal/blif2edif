using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace combinations
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string folderPath;
				if (args.Length == 0)
				{
					Console.WriteLine("Enter outpuFolderPath: ");
					folderPath = Console.ReadLine();
				}
				else
					folderPath = args[0];

				if (!Directory.Exists(folderPath))
					throw new ArgumentException($"output directory '{folderPath}' does not exist");

				Console.WriteLine("Enter 'q' or 'exit' to exit");
				while (true)
				{
					Console.WriteLine("Enter n: ");
					string data = Console.ReadLine();
					if (data == null)
						data = string.Empty;
					if (data.StartsWith("q") || data.StartsWith("exit"))
						break;

					if (!int.TryParse(data, out var n))
					{
						Console.WriteLine("can not recognize 'n' as a number");
						continue;
					}

					int binaryCombinationCount = checked((int) Math.Pow(2, n));
					List<string> result = new List<string>((binaryCombinationCount));
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < binaryCombinationCount; i++)
					{
						sb.Clear();
						string binary = Convert.ToString(i, 2).PadLeft(n, '0');
						foreach (char c in binary)
						{
							if (c == '0')
								sb.Append("10");
							else if (c == '1')
								sb.Append("01");
							else
								throw new ArgumentOutOfRangeException(
									$"Invalid binary character: {c}. i: {i}. binary: {binary}");
						}

						result.Add(sb.ToString());
					}

					string filePath = Path.Combine(folderPath, $"binaryOutput-{n}.txt");
					if (File.Exists(filePath))
						File.Delete(filePath);
					File.WriteAllLines(filePath, result);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception occured: {ex}");
			}
			finally
			{
				Console.WriteLine($"Press any key to exit...");
				Console.ReadKey();
			}
		}
	}
}
