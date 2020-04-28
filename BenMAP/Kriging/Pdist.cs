using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
	public static class Pdist
	{
		public static double[] GetPDist(double[,] X)
		{
			double[] y = new double[1];
			try
			{
				int nargin = 0;
				string dist = "";
				if (nargin < 2)
				{
					dist = "euc";
				}
				int[] tmpValues = Common.Get2DimensionsSize(X);
				int n = tmpValues[0]; int p = tmpValues[1]; double[,] tranpose = new double[p, n];
				MatrixCompute.Transpose(X, ref tranpose);
				switch (dist)
				{
					case "seu":
						break;
					case "mah":
						break;
					case "min":
						break;
					case "cos":
						break;
					case "cor":
						break;
					case "spe":
						break;
					default:
						break;
				}
				tmpValues = Common.Get2DimensionsSize(tranpose);
				n = tmpValues[0];
				p = tmpValues[1];
				y = new double[p * (p - 1) / 2];
				double tmp = 0.0;
				int count = 0;
				for (int i = 0; i < p - 1; i++)
				{
					for (int j = i + 1; j < p; j++)
					{
						tmp = 0.0;
						for (int row = 0; row < n; row++)
						{
							tmp = tmp + Math.Pow((tranpose[row, i] - tranpose[row, j]), 2.0);
						}
						y[count] = Math.Sqrt(tmp);
						count++;
					}
				}
				return y;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return y;
			}
		}



	}
}
