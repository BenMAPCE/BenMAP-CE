using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{

	public static class LHSDesign
	{
		public static double[,] LhsDesign(int n, int p)
		{
			try
			{
				bool ok = false;
				string eid = "";
				string emsg = "";
				string tmp = "";
				string crit = "";
				string dosmooth = "";
				int matchIndex = -1;
				string[] okargs = { "iterations", "criterion", "'smooth" };
				string[] defaults = { "NaN", "maximin", "on" };
				int maxiter = -1;

				tmp = Statgetargs.StatGetargs(okargs, defaults, ref eid, ref crit, ref dosmooth);
				if ((tmp == null) || (tmp.Length == 0))
				{ tmp = "NaN"; }
				else if (tmp == "NaN")
				{
					maxiter = 5;
				}
				else
				{
					ok = int.TryParse(tmp, out maxiter);
					if (!ok)
					{ return null; }
				}

				string[] okcrit = { "none", "maximin", "correlation" };
				matchIndex = CellStrMatch.GetMatchString(crit.ToLower(), okcrit);
				crit = okcrit[matchIndex];
				if (dosmooth.Length == 0) { dosmooth = "on"; }

				double[,] X = GetSample(n, p, dosmooth);

				if (crit == "none" || Common.Get2DimensionsSize(X, 0) < 2)
				{ maxiter = 0; }
				double bestScore, newscore;
				double[,] x = new double[n, p];
				switch (crit)
				{
					case "maximin":
						bestScore = GetScore(X, crit);

						for (int j = 2; j <= maxiter; j++)
						{
							x = GetSample(n, p, dosmooth);
							newscore = GetScore(x, crit);
							if (newscore > bestScore)
							{
								X = new double[n, p];
								ok = Common.Init2Dimensions(ref X, x);
								if (!ok)
								{ return null; }
								bestScore = newscore;
							}
						}
						break;
					case "correlation":
						break;
					case "":
						break;

				}
				return X;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return null;
			}
		}



		public static double[,] GetSample(int n, int p, string dosmooth)
		{
			double[,] x = new double[n, p];
			try
			{
				Common.Init2Dimensions(ref x, n, p);
				double[] r = new double[p];
				for (int i = 0; i < p; i++)
				{
					double[] tmpColValues = new double[n];
					for (int row = 0; row < n; row++)
					{
						tmpColValues[row] = x[row, i];
					}
					r = Rank(tmpColValues);
					for (int row = 0; row < n; row++)
					{
						x[row, i] = r[row];
					}

				}
				if (dosmooth == "on")
				{
					double[,] tmp2Dims = new double[n, p];
					Common.Init2Dimensions(ref tmp2Dims, n, p);
					for (int row = 0; row < n; row++)
					{
						for (int col = 0; col < p; col++)
						{
							x[row, col] = (x[row, col] - tmp2Dims[row, col]) / n;
						}
					}
				}
				else
				{
					for (int row = 0; row < n; row++)
					{
						for (int col = 0; col < p; col++)
						{
							x[row, col] = x[row, col] - 0.5;
						}
					}
				}

				return x;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return null;
			}
		}

		public static double GetScore(double[,] x, string crit)
		{
			double s = 0;
			try
			{
				int n = Common.Get2DimensionsSize(x, 0);
				if (n < 2)
				{
					s = 0;
					return s;
				}
				switch (crit)
				{
					case "correlation":

						break;
					case "maximin":
						double tmp = 0;
						Common.GetArrayMaxMin(Pdist.GetPDist(x), ref s, ref tmp);
						break;
				}
				return s;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return s;
			}
		}

		public static double[] Rank(double[] x)
		{
			double[] r = new double[x.Length];
			double[] sx = new double[x.Length];
			int[] rowidx = new int[x.Length];
			try
			{
				Common.SortArray(x, ref sx, ref rowidx);
				for (int i = 0; i < rowidx.Length; i++)
				{
					r[rowidx[i]] = i + 1;
				}
				return r;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return null;
			}
		}


	}
}
