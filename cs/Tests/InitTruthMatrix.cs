using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLIFtoEDIF_Converter.InitCalculator;
using BLIFtoEDIF_Converter.Logic.Model.Blif.Function;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class InitTruthMatrix
	{
		[TestMethod]
		public void Truth2nTest()
		{
			int n = 2;
			int[][] initFunc = InitCalculator.CreateInitTruthMatrix(n);

			Assert.AreEqual(Math.Pow(2, n), initFunc.Length);

			foreach (int[] ints in initFunc)
			{
				Assert.AreEqual(n+1, ints.Length);
				Assert.AreEqual(0, ints.Last());
			}

			Assert.AreEqual(0, initFunc[0][0]);
			Assert.AreEqual(0, initFunc[0][1]);

			Assert.AreEqual(0, initFunc[1][0]);
			Assert.AreEqual(1, initFunc[1][1]);

			Assert.AreEqual(1, initFunc[2][0]);
			Assert.AreEqual(0, initFunc[2][1]);

			Assert.AreEqual(1, initFunc[3][0]);
			Assert.AreEqual(1, initFunc[3][1]);
		}

		[TestMethod]
		public void Truth3nTest()
		{
			int n = 3;
			int[][] initFunc = InitCalculator.CreateInitTruthMatrix(n);

			Assert.AreEqual(Math.Pow(2, n), initFunc.Length);

			foreach (int[] ints in initFunc)
			{
				Assert.AreEqual(n + 1, ints.Length);
				Assert.AreEqual(0, ints.Last());
			}

			int i = 0;
			Assert.AreEqual(0, initFunc[i][0]);
			Assert.AreEqual(0, initFunc[i][1]);
			Assert.AreEqual(0, initFunc[i][2]);

			i++;
			Assert.AreEqual(0, initFunc[i][0]);
			Assert.AreEqual(0, initFunc[i][1]);
			Assert.AreEqual(1, initFunc[i][2]);

			i++;
			Assert.AreEqual(0, initFunc[i][0]);
			Assert.AreEqual(1, initFunc[i][1]);
			Assert.AreEqual(0, initFunc[i][2]);

			i++;
			Assert.AreEqual(0, initFunc[i][0]);
			Assert.AreEqual(1, initFunc[i][1]);
			Assert.AreEqual(1, initFunc[i][2]);




			i++;
			Assert.AreEqual(1, initFunc[i][0]);
			Assert.AreEqual(0, initFunc[i][1]);
			Assert.AreEqual(0, initFunc[i][2]);

			i++;
			Assert.AreEqual(1, initFunc[i][0]);
			Assert.AreEqual(0, initFunc[i][1]);
			Assert.AreEqual(1, initFunc[i][2]);

			i++;
			Assert.AreEqual(1, initFunc[i][0]);
			Assert.AreEqual(1, initFunc[i][1]);
			Assert.AreEqual(0, initFunc[i][2]);

			i++;
			Assert.AreEqual(1, initFunc[i][0]);
			Assert.AreEqual(1, initFunc[i][1]);
			Assert.AreEqual(1, initFunc[i][2]);
		}
	}
}
