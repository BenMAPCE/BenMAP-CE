using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ESIL.Kriging
{
	public struct CorParms
	{
		public double[] scale;

		public double[] smoothness;

		public int fittype;

		public int ctype;

		public double[] theta;

		public double[] power;

	}

	public static class Common
	{
		public static double eps = 2.2204e-016;

		public static CorParms corparms;

		private static string CRLF = "\r\n";
		public static void LogError(Exception ex)
		{
			try
			{
				Console.WriteLine(ex.ToString());
				string place = ex.StackTrace.ToString();
				int pos = place.LastIndexOf("\n");
				string msg = CRLF + DateTime.Now.ToString() + CRLF + ex.Message + CRLF + place.Substring(pos + 1);
				AppendErr(GetLogPath(ex), msg);
			}
			catch (Exception myEx)
			{
				string errMsg = string.Format("{0}{1}{0}{2} CommonClass.LogError,1", CRLF, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), myEx.Message);
				Console.WriteLine(errMsg);
			}
		}
		public static void LogError(Exception ex, string msg)
		{
			try
			{
				string place = ex.StackTrace.ToString();
				int pos = place.LastIndexOf("\n");

				string myMsg = CRLF + DateTime.Now.ToString() + CRLF + ex.Message + CRLF + msg + CRLF + place.Substring(pos + 1);
				AppendErr(GetLogPath(ex), myMsg);
			}
			catch (Exception myEx)
			{
				string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + "  " + "CommonClass.LogError,2";
				Console.WriteLine(errMsg);
			}
		}

		public static void LogError(string msg)
		{
			try
			{
				string myMsg = CRLF + DateTime.Now.ToString() + CRLF + msg;
				AppendErr(GetLogPath(null), myMsg);
			}
			catch (Exception myEx)
			{
				string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "CommonClass.LogError,3";
				Console.WriteLine(errMsg);
			}
		}

		private static string GetLogPath(Exception ex)
		{
			try
			{
				string logPath;





				logPath = AppDomain.CurrentDomain.BaseDirectory + "\\err_log.log";
				logPath = logPath.Replace("\\\\", "\\");
				CheckAndCreateDir(logPath);
				return logPath;
			}
			catch (Exception myEx)
			{
				string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "CommonClass.GetLogPath";
				Console.WriteLine(errMsg);
				return "err_log.log";
			}
		}

		private static void CheckAndCreateDir(string path)
		{
			try
			{
				string dir;
				int pos = path.LastIndexOf("\\");
				dir = path.Substring(0, pos);
				if (Directory.Exists(dir) == false)
				{
					Directory.CreateDirectory(dir);
				}
			}
			catch (Exception ex)
			{
				string errMsg = CRLF + DateTime.Now.ToString() + "\\rn" + ex.ToString() + "\r\npath=" + path + CRLF + "CommonClass.CheckAndCreateDir";
				Console.WriteLine(errMsg);
			}
		}

		private static void AppendErr(string path, string msg)
		{
			try
			{
				StreamWriter writer = new StreamWriter(path, true);
				writer.WriteLine(msg);
				writer.Close();
				Console.WriteLine(msg);
			}
			catch (Exception myEx)
			{
				string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "CommonClass.AppendErr";
				Console.WriteLine(errMsg);
			}
		}


		public static bool Init2Dimensions(ref double[,] x, int n, int p, double initValue)
		{
			try
			{
				x = new double[n, p];
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < p; j++)
					{
						x[i, j] = initValue;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool Init2Dimensions(ref double[,] x, int n, int p)
		{
			try
			{
				Random random = new Random();
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < p; j++)
					{
						x[i, j] = random.NextDouble();
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool Init2Dimensions(ref double[,] x, double initValue)
		{
			try
			{
				int rowCount = x.GetLength(0);
				int colCount = x.GetLength(1);
				for (int i = 0; i < rowCount; i++)
				{
					for (int j = 0; j < colCount; j++)
					{
						x[i, j] = initValue;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool Init2Dimensions(ref double[,] x, double[,] initValues)
		{
			try
			{
				int rowX = x.GetLength(0);
				int colX = x.GetLength(1);
				int rows = initValues.GetLength(0);
				int cols = initValues.GetLength(1);
				if ((rowX < rows) || (colX < cols))
				{ return false; }
				for (int i = 0; i < rows; i++)
				{
					for (int j = 0; j < cols; j++)
					{
						x[i, j] = initValues[i, j];
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static int[] Get2DimensionsSize(double[,] x)
		{
			int[] rowAndColCount = new int[2];
			try
			{
				rowAndColCount[0] = x.GetLength(0);
				rowAndColCount[1] = x.GetLength(1);
				return rowAndColCount;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return rowAndColCount;
			}
		}

		public static int Get2DimensionsSize(double[,] x, int rowOrCol)
		{
			int count = 0;
			try
			{
				if (rowOrCol != 0 && rowOrCol != 1)
				{ return count; }
				count = x.GetLength(rowOrCol);
				return count;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return count;
			}
		}

		public static bool Get2DimensionMaxMin(double[,] x, ref double[,] ranges)
		{
			try
			{
				int n = Get2DimensionsSize(x, 0);
				int p = Get2DimensionsSize(x, 1);
				for (int i = 0; i < p; i++)
				{
					double min, max;
					min = x[0, i];
					max = x[0, i];
					for (int j = 0; j < n; j++)
					{
						if (min > x[j, i])
						{ min = x[j, i]; }
						if (max < x[j, i])
						{ max = x[j, i]; }
					}
					ranges[i, 0] = min;
					ranges[i, 1] = max;
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool Get2DimensionMaxMin(double[,] x, ref double[] minValues, ref double[] maxValues)
		{
			try
			{
				int n = x.GetLength(0);
				int p = x.GetLength(1);
				minValues = new double[p];
				maxValues = new double[p];
				for (int i = 0; i < p; i++)
				{
					double min, max;
					min = x[0, i];
					max = x[0, i];
					for (int j = 0; j < n; j++)
					{
						if (min > x[j, i])
						{ min = x[j, i]; }
						if (max < x[j, i])
						{ max = x[j, i]; }
					}
					minValues[i] = min;
					maxValues[i] = max;
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool InitArray(ref double[] x, double value)
		{
			try
			{
				for (int i = 0; i < x.Length; i++)
				{
					x[i] = value;
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool InitArray(ref double[] x)
		{
			try
			{
				Random random = new Random();
				for (int i = 0; i < x.Length; i++)
				{
					x[i] = random.NextDouble();
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool InitArray(ref string[] x, string value)
		{
			try
			{
				for (int i = 0; i < x.Length; i++)
				{
					x[i] = value;
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool InitArray(ref int[] x, int value)
		{
			try
			{
				for (int i = 0; i < x.Length; i++)
				{
					x[i] = value;
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}


		public static bool GetArrayMaxMin(double[] x, ref double minValues, ref double maxValues)
		{
			try
			{
				int count = x.Length;
				minValues = x[0];
				maxValues = x[0];
				for (int i = 0; i < count; i++)
				{
					if (x[i] < minValues)
					{
						minValues = x[i];
					}
					if (x[i] > maxValues)
					{
						maxValues = x[i];
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool GetArrayMaxMin(double[] x, bool isMax, ref double minmax, ref int index)
		{
			try
			{
				int count = x.Length;
				if (count == 0)
				{ return false; }
				minmax = x[0];
				index = 0;
				if (isMax)
				{
					for (int i = 0; i < count; i++)
					{
						if (x[i] > minmax)
						{
							minmax = x[i];
							index = i;
						}
					}
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						if (x[i] < minmax)
						{
							minmax = x[i];
							index = i;
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}
		public static bool SortArray(double[] x, ref double[] newArray, ref int[] originIndexs)
		{
			bool ok = false;
			try
			{
				newArray = new double[x.Length];
				x.CopyTo(newArray, 0);
				ok = BubbleSort(ref newArray);
				if (!ok) { return ok; }
				for (int i = 0; i < newArray.Length; i++)
				{
					int tmpIndex = -1;
					double tmpValue = newArray[i];
					for (int j = 0; j < x.Length; j++)
					{
						if (x[j].CompareTo(tmpValue) == 0)
						{
							tmpIndex = j;
							break;
						}
					}
					originIndexs[i] = tmpIndex;
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool BubbleSort(ref double[] x)
		{
			try
			{
				int i, j;
				int m = x.Length - 1;
				int k = 0;
				double tmp = 0.0;
				while (k < m)
				{
					j = m - 1;
					m = 0;
					for (i = k; i <= j; i++)
					{
						if (x[i] > x[i + 1])
						{
							tmp = x[i];
							x[i] = x[i + 1];
							x[i + 1] = tmp;
							m = i;
						}
					}
					j = k + 1;
					k = 0;
					for (i = m; i >= j; i--)
					{
						if (x[i - 1] > x[i])
						{
							tmp = x[i];
							x[i] = x[i - 1];
							x[i - 1] = tmp;
							k = i;
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}

		public static bool InsertSort(double[] x, ref double[] newArray, ref int[] originIndexs)
		{
			try
			{
				bool ok = false;
				newArray = new double[x.Length];


				return ok;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return false;
			}
		}


		public static int[] Nonzerosign(double[] x)
		{
			bool ok = false;
			int count = x.Length;
			int[] y = new int[count];
			try
			{
				ok = MatrixCompute.Ones(ref y);
				if (!ok)
				{ return null; }
				for (int i = 0; i < count; i++)
				{
					if (x[i] >= 0)
					{ y[i] = 1; }
					else
					{ y[i] = -1; }
				}
				return y;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return null;
			}
		}

		public static double FwdFinDiffInsideBnds(double xC, double lb, double ub, double delta, int dim, double DiffMinChange)
		{
			double del = delta;
			try
			{

				if (lb != ub && xC >= lb && xC <= ub)
				{
					if (xC + delta > ub || xC + delta < lb)
					{
						delta = -delta;
						if (xC + delta > ub || xC + delta < lb)
						{
							double newDelta = 0;
							int indsign = 0;
							if ((xC - lb) < (ub - xC))
							{
								newDelta = ub - xC;
								indsign = 2;
							}
							else
							{
								newDelta = xC - lb;
								indsign = 1;
							}
							if (newDelta >= DiffMinChange)
							{
								del = Math.Pow(-1, indsign) * newDelta;

							}
							else
							{
								return 0.0;
							}
						}
					}
				}
				return del;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return del;
			}
		}


	}
}
